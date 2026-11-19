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
			yield return WaitFor(() => Shown("homeScreen") && Root.Q<Label>("statSeen").text == "2");

			Assert.That(
				UnityEngine.Object.FindObjectsByType<EnglishBootstrap>().Length,
				Is.EqualTo(1)
			);
			Press("continueButton");
			Assert.That(Shown("lessonIntro"), Is.True);
		}

		[UnityTest]
		public IEnumerator DialogueIsReadableBeforeAnswerAndSavesEnglishRecall()
		{
			var curriculum =
				JsonUtility.FromJson<Curriculum>(Resources.Load<TextAsset>(EnglishBootstrap.CurriculumResource).text);
			var unit = curriculum.units[0];
			var profile = new Profile();
			foreach (var card in unit.exercises.Where(x => !x.IsDialogue))
			{
				profile = LearningPolicy.Record(profile, unit.id, card, true, false, DateTime.UtcNow);
			}
			Directory.CreateDirectory(_directory);
			string path = Path.Combine(_directory, "adult-english-v1.json");
			File.WriteAllText(path, JsonUtility.ToJson(profile));
			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && !string.IsNullOrWhiteSpace(Root.Q<Label>("headline").text)
			);

			Press("startButton");
			Press("beginButton");
			Assert.That(Shown("dialogueText"), Is.True);
			Assert.That(Root.Q<Label>("dialogueText").text, Does.Contain("Maya:"));
			Assert.That(Shown("feedback"), Is.False);
			Root.Q<TextField>("answerInput").value = "support";
			Press("checkButton");
			yield return WaitFor(() => Shown("advanceButton"));

			Assert.That(Root.Q<Label>("explanation").text, Does.Contain("службе поддержки"));
			var saved = JsonUtility.FromJson<Profile>(File.ReadAllText(path));
			Assert.That(saved.items.Single(x => x.id.Contains(".dialogue.")).recalls, Is.EqualTo(1));
		}

		[UnityTest]
		public IEnumerator WordFilterAndAssemblyUndoSaveGuidedAttempt()
		{
			var curriculum =
				JsonUtility.FromJson<Curriculum>(Resources.Load<TextAsset>(EnglishBootstrap.CurriculumResource).text);
			var unit = curriculum.units.Single(x => x.id == "a2.word-choice");
			var profile = new Profile();
			foreach (var card in unit.exercises.Where(x => !x.HasTokens))
			{
				profile = LearningPolicy.Record(profile, unit.id, card, true, false, DateTime.UtcNow);
			}
			Directory.CreateDirectory(_directory);
			string path = Path.Combine(_directory, "adult-english-v1.json");
			File.WriteAllText(path, JsonUtility.ToJson(profile));
			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && !string.IsNullOrWhiteSpace(Root.Q<Label>("headline").text)
			);

			Press("filterPhrasal");
			Assert.That(
				Root.Q<Label>("topicTitle").text,
				Is.EqualTo(curriculum.units.First(x => x.track == Unit.PhrasalTrack).title)
			);
			Assert.That(Shown("levelTabs"), Is.False);
			Assert.That(Root.Q("topicPreview").ClassListContains("topic-phrasal"), Is.True);
			Assert.That(Shown("topicRule"), Is.False);
			Press("previewRule");
			Assert.That(Shown("topicRule"), Is.True);
			Press("filterWords");
			int wordIndex = Array.IndexOf(curriculum.units.Where(x => x.track == Unit.WordTrack).ToArray(), unit);
			for (int i = 0; i < wordIndex; i++)
			{
				Press("nextUnit");
			}
			Assert.That(Root.Q<Label>("topicTitle").text, Is.EqualTo(unit.title));
			Assert.That(Root.Q("topicPreview").ClassListContains("topic-words"), Is.True);
			Assert.That(Shown("topicRule"), Is.False);
			Assert.That(Root.Q<ProgressBar>("topicMeter").value, Is.EqualTo(3));
			Press("startButton");
			Press("beginButton");
			Assert.That(Shown("assembly"), Is.True);
			Assert.That(Shown("answerInput"), Is.False);
			Assert.That(Root.Q<Button>("checkButton").enabledSelf, Is.False);
			var cardToAssemble = unit.exercises.Single(x => x.HasTokens);

			Button Word(string value) => Enumerable.Range(0, cardToAssemble.tokens.Length)
				.Select(i => Root.Q<Button>("token" + i)).Single(x => x.text == value);

			Press(Word("trying").name);
			Assert.That(Word("trying").enabledSelf, Is.False);
			Press("assemblyUndo");
			Assert.That(Word("trying").enabledSelf, Is.True);
			foreach (string token in cardToAssemble.tokens)
			{
				Press(Word(token).name);
			}
			Press("checkButton");
			yield return WaitFor(() => Shown("advanceButton"));

			var saved = JsonUtility.FromJson<Profile>(File.ReadAllText(path)).items
				.Single(x => x.id == cardToAssemble.id);
			Assert.That(saved.successes, Is.EqualTo(1));
			Assert.That(saved.recalls, Is.Zero);
		}

		[UnityTest]
		public IEnumerator BusScenarioMovesFromRecognitionToIndependentRecall()
		{
			var curriculum =
				JsonUtility.FromJson<Curriculum>(Resources.Load<TextAsset>(EnglishBootstrap.CurriculumResource).text);
			var unit = curriculum.units.Single(x => x.id == "a1.bus-stop");
			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && !string.IsNullOrWhiteSpace(Root.Q<Label>("headline").text)
			);

			int topicIndex = Array.IndexOf(curriculum.units.Where(x => x.level == unit.level).ToArray(), unit);
			for (int i = 0; i < topicIndex; i++)
				Press("nextUnit");
			Assert.That(Root.Q<Label>("topicTitle").text, Is.EqualTo(unit.title));
			var speaker = new UnavailableWordSpeech();
			UnityEngine.Object.FindAnyObjectByType<EverydayView>().ConfigureWordSpeech(speaker);
			Press("startButton");
			Assert.That(Root.Q<Label>("introRule").text, Does.Contain("Get off"));
			Assert.That(Shown("modelPractice"), Is.True);
			Assert.That(Root.Q<Button>("playRecording").enabledSelf, Is.False);
			Press("speakModel");
			Assert.That(speaker.Term, Is.EqualTo(unit.modelPhrase));
			Press("beginButton");

			Assert.That(Shown("choices"), Is.True);
			var option = Enumerable.Range(0, 4).Select(i => Root.Q<Button>("option" + i)).Single(x => x.text == "go");
			Press(option.name);
			yield return WaitFor(() => Shown("advanceButton"));

			Press("advanceButton");

			Assert.That(Shown("assembly"), Is.True);
			var assembly = unit.exercises.Single(x => x.HasTokens);
			Button[] buttons = Enumerable.Range(0, assembly.tokens.Length).Select(i => Root.Q<Button>("token" + i))
				.ToArray();
			foreach (string token in assembly.tokens)
				Press(buttons.Single(x => x.text == token).name);
			Press("checkButton");
			yield return WaitFor(() => Shown("advanceButton"));

			Press("advanceButton");

			foreach (string answer in new[]
				{ "off", "next", "6", "I will get off at the next stop.", "8", "I'll take bus 7" })
			{
				Assert.That(Shown("answerInput"), Is.True);
				Assert.That(Shown("dialogueText"), Is.EqualTo(answer == "next"));
				if (answer == "8")
				{
					Assert.That(Shown("sourceTranscript"), Is.False);
					Assert.That(Root.Q<Button>("speakSource").text, Is.EqualTo("Послушать онлайн"));
					Press("speakSource");
					yield return WaitFor(() => Root.Q<Label>("sourceStatus").text.Contains("Попробуйте ещё раз"));

					Assert.That(speaker.Term, Is.EqualTo(unit.exercises[6].speechText));
					Assert.That(Root.Q<Button>("speakSource").enabledSelf, Is.True);
					Assert.That(Shown("sourceTranscript"), Is.False);
					Press("showSource");
					Assert.That(Shown("sourceTranscript"), Is.True);
					Assert.That(Root.Q<Label>("sourceTranscript").text, Does.Contain("Bus eight"));
				}
				Root.Q<TextField>("answerInput").value = answer;
				Press("checkButton");
				yield return WaitFor(() => Shown("advanceButton"));

				Press("advanceButton");
			}

			string path = Path.Combine(_directory, "adult-english-v1.json");
			var saved = JsonUtility.FromJson<Profile>(File.ReadAllText(path));
			Assert.That(saved.items.Count(x => x.successes == 1), Is.EqualTo(8));
			Assert.That(saved.items.Count(x => x.recalls == 1), Is.EqualTo(6));
			Assert.That(saved.items.Single(x => x.id == "a1.bus-stop.06").recalls, Is.EqualTo(1));
			Assert.That(Shown("choices"), Is.False);
			Assert.That(Shown("assembly"), Is.False);
			Assert.That(Shown("answerInput"), Is.True);
		}

		[UnityTest]
		public IEnumerator VocabularySearchShowsSavedRecallAfterMistakeAndRestart()
		{
			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && !string.IsNullOrWhiteSpace(Root.Q<Label>("headline").text)
			);

			var speaker = new UnavailableWordSpeech();
			UnityEngine.Object.FindAnyObjectByType<EverydayView>().ConfigureWordSpeech(speaker);
			Press("vocabularyButton");
			Assert.That(Shown("vocabularyScreen"), Is.True);
			Root.Q<DropdownField>("vocabularyScope").index = 1;
			var search = Root.Q<TextField>("vocabularySearch");
			search.value = "oak";
			Assert.That(Root.Q("vocabularyRow0").Q<Label>(className: "vocabulary-term").text, Does.StartWith("oak"));
			Assert.That(Root.Q("vocabularyRow0").Q<Button>().ClassListContains("hidden"), Is.True);
			var pronunciation = Root.Q("vocabularyRow0").Q<Button>(className: "vocabulary-speak");
			Assert.That(pronunciation.ClassListContains("hidden"), Is.False);
			using (var submit = NavigationSubmitEvent.GetPooled())
			{
				submit.target = pronunciation;
				pronunciation.SendEvent(submit);
			}
			yield return WaitFor(() => Root.Q<Label>("vocabularySpeechStatus").text.Contains("недоступна"));

			Assert.That(speaker.Term, Is.EqualTo("oak"));

			Root.Q<DropdownField>("vocabularyScope").index = 0;
			Root.Q<DropdownField>("vocabularyLevel").index = 5;
			Root.Q<DropdownField>("vocabularyKind").index = 3;
			search.value = "вывести";
			Assert.That(
				Root.Q("vocabularyRow0").Q<Label>(className: "vocabulary-term").text,
				Does.StartWith("phase out")
			);
			PressVocabularyExercise();
			Assert.That(Root.Q<Label>("context").text, Does.Contain("old system"));
			Root.Q<TextField>("answerInput").value = "phase in";
			Press("checkButton");
			yield return WaitFor(() => Shown("advanceButton"));

			Press("homeButton");
			Root.Q<DropdownField>("vocabularyScope").index = 2;
			Assert.That(VocabularyEntryStatus(), Does.Contain("Нужно повторить"));
			PressVocabularyExercise();
			Assert.That(Root.Q<Label>("context").text, Does.Contain("printed invoices"));
			Root.Q<TextField>("answerInput").value = "phase out";
			Press("checkButton");
			yield return WaitFor(() => Shown("advanceButton"));

			Press("homeButton");
			Assert.That(Shown("vocabularyEmpty"), Is.True);

			yield return DestroyOwners();

			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && Root.Q<Label>("statSeen").text == "1");

			Press("vocabularyButton");
			Root.Q<TextField>("vocabularySearch").value = "phase out";
			Assert.That(VocabularyEntryStatus(), Does.Contain("Вспомнили самостоятельно"));
		}

		[UnityTest]
		public IEnumerator DarkThemeIsDefaultSwitchesLiveAndSurvivesRestart()
		{
			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && !string.IsNullOrWhiteSpace(Root.Q<Label>("headline").text)
			);

			var app = Root.Q("appRoot");
			Assert.That(app.ClassListContains("theme-midnight"), Is.True);
			Press("settingsButton");
			Assert.That(Shown("settingsScreen"), Is.True);
			Assert.That(Shown("homeScreen"), Is.False);
			Assert.That(Root.Q<Button>("themeMidnight").ClassListContains("selected"), Is.True);
			Press("themeDaylight");
			Assert.That(app.ClassListContains("theme-daylight"), Is.True);
			Assert.That(app.ClassListContains("theme-midnight"), Is.False);
			Assert.That(Root.Q<Button>("themeDaylight").ClassListContains("selected"), Is.True);
			Assert.That(Root.Q<Button>("themeMidnight").ClassListContains("selected"), Is.False);
			yield return WaitFor(() => !string.IsNullOrEmpty(Root.Q<Label>("settingsStatus").text));

			string path = Path.Combine(_directory, "settings-v1.json");
			Assert.That(File.ReadAllText(path), Does.Contain("daylight"));
			Press("settingsDone");
			Assert.That(Shown("homeScreen"), Is.True);
			Assert.That(Shown("settingsScreen"), Is.False);
			yield return DestroyOwners();

			SceneManager.LoadScene("Boot");
			yield return WaitFor(() => Shown("homeScreen") && !string.IsNullOrWhiteSpace(Root.Q<Label>("headline").text)
			);

			Assert.That(Root.Q("appRoot").ClassListContains("theme-daylight"), Is.True);
			Assert.That(Root.Q<Button>("themeDaylight").ClassListContains("selected"), Is.True);
		}

		private string VocabularyEntryStatus() =>
			Root.Q("vocabularyRow0").Q<Label>(className: "vocabulary-meaning").text;

		private void PressVocabularyExercise()
		{
			var button = Root.Q("vocabularyRow0").Q<Button>();
			Assert.That(button.ClassListContains("hidden"), Is.False);
			using var submit = NavigationSubmitEvent.GetPooled();
			submit.target = button;
			button.SendEvent(submit);
		}

		private sealed class UnavailableWordSpeech : IWordSpeech
		{
			public string Term { get; private set; }

			public async Awaitable<bool> SpeakAsync(string term)
			{
				Term = term;
				await Awaitable.MainThreadAsync();
				return false;
			}

			public void Stop() { }
		}

		[Serializable]
		private sealed class SaveOverride
		{
			public string _saveDirectoryOverride;
		}
	}
}