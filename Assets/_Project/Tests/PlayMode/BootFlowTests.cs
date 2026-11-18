using System;
using System.Collections;
using System.IO;
using System.Linq;
using LearnEnglish.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace LearnEnglish.PlayTests
{
	public sealed class BootFlowTests
	{
		private string _directory;
		private VisualElement Root => UnityEngine.Object.FindAnyObjectByType<EverydayView>()
			?.GetComponent<UIDocument>().rootVisualElement;

		[SetUp]
		public void SetUp()
		{
			Assert.That(
				UnityEngine.Object.FindAnyObjectByType<EnglishBootstrap>(),
				Is.Null,
				"Disable Play-from-Boot for test execution; refusing to touch an existing profile."
			);
			_directory = Path.GetFullPath(Path.Combine("TestResults", "play-saves", Guid.NewGuid().ToString("N")));
			SceneManager.sceneLoaded += IsolateSave;
		}

		private void IsolateSave(Scene scene, LoadSceneMode mode)
		{
			if (scene.name != "Boot")
				return;

			var boot = UnityEngine.Object.FindAnyObjectByType<EnglishBootstrap>();
			Assert.That(boot, Is.Not.Null);
			JsonUtility.FromJsonOverwrite(
				JsonUtility.ToJson(new SaveOverride { _saveDirectoryOverride = _directory }),
				boot
			);
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			SceneManager.sceneLoaded -= IsolateSave;
			yield return DestroyOwners();
		}

		private static IEnumerator WaitFor(Func<bool> condition)
		{
			float deadline = Time.realtimeSinceStartup + 25;
			while (!condition())
			{
				Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline), "Boot/UI operation timed out.");
				yield return null;
			}
		}

		private static IEnumerator DestroyOwners()
		{
			foreach (var view in UnityEngine.Object.FindObjectsByType<EverydayView>())
				UnityEngine.Object.Destroy(view.gameObject);
			yield return null;

			foreach (var boot in UnityEngine.Object.FindObjectsByType<EnglishBootstrap>())
				UnityEngine.Object.Destroy(boot.gameObject);
			yield return null;
		}

		private bool Shown(string name) => Root?.Q(name) != null && !Root.Q(name).ClassListContains("hidden");

		private void Press(string name)
		{
			var button = Root.Q<Button>(name);
			Assert.That(button, Is.Not.Null);
			Assert.That(button.enabledInHierarchy, Is.True);
			using var submit = NavigationSubmitEvent.GetPooled();
			submit.target = button;
			button.SendEvent(submit);
		}

		[UnityTest]
		public IEnumerator BootExplanationAnswerHintPersistenceAndRestart()
		{
			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && !string.IsNullOrWhiteSpace(Root.Q<Label>("headline").text)
			);

			Assert.That(Root.Q<Label>("topicRule").text.Length, Is.GreaterThan(50));
			Press("startButton");
			Assert.That(Shown("lessonIntro"), Is.True);
			Assert.That(Shown("exerciseBody"), Is.False);
			Press("beginButton");
			Assert.That(Shown("choices"), Is.True);
			var wrong = Enumerable.Range(0, 4).Select(i => Root.Q<Button>("option" + i)).First(x => x.text != "am");
			Press(wrong.name);
			yield return WaitFor(() => Shown("advanceButton"));

			Assert.That(Root.Q("answerControls").enabledSelf, Is.False);
			Assert.That(Root.Q<Label>("explanation").text.Length, Is.GreaterThan(10));
			Assert.That(File.Exists(Path.Combine(_directory, "adult-english-v1.json")), Is.True);
			Press("advanceButton");
			Press("hintButton");
			Assert.That(Shown("reference"), Is.True);
			Assert.That(Root.Q<Label>("assistedStatus").text, Is.Not.Empty);
			Root.Q<TextField>("answerInput").value = "is";
			Press("checkButton");
			yield return WaitFor(() => Shown("advanceButton"));

			var saved = JsonUtility.FromJson<Profile>(
				File.ReadAllText(Path.Combine(_directory, "adult-english-v1.json"))
			);
			Assert.That(saved.attempts, Is.EqualTo(2));
			Assert.That(saved.items.Single(x => x.id.EndsWith(".01")).failed, Is.True);
			Assert.That(saved.items.Single(x => x.id.EndsWith(".02")).successes, Is.Zero);
			yield return DestroyOwners();

			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && Root.Q<Label>("evidenceSummary").text.Contains("2"));

			Assert.That(
				UnityEngine.Object.FindObjectsByType<EnglishBootstrap>().Length,
				Is.EqualTo(1)
			);
			Press("continueButton");
			Assert.That(Shown("lessonIntro"), Is.True);
		}

		[Serializable]
		private sealed class SaveOverride
		{
			public string _saveDirectoryOverride;
		}
	}
}
