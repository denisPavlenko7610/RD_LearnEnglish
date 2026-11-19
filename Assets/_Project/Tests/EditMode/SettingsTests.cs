using System;
using System.Collections;
using System.IO;
using LearnEnglish.Core;
using NUnit.Framework;
using RD.Save;
using UnityEngine;
using UnityEngine.TestTools;

namespace LearnEnglish.Tests
{
	public sealed class SettingsTests
	{
		private static string NewDirectory() =>
			Path.Combine("TestResults", "saves", Guid.NewGuid().ToString("N"));

		[Test]
		public void DefaultThemeIsDarkAndOnlyKnownThemesAreValid()
		{
			var settings = new AppSettings();
			Assert.That(settings.theme, Is.EqualTo(AppThemes.Midnight));
			Assert.DoesNotThrow(() => settings.Validate());
			Assert.That(AppThemes.All, Has.Length.EqualTo(5));
			Assert.That(AppThemes.ClassName(AppThemes.Ocean), Is.EqualTo("theme-ocean"));
			foreach (string theme in AppThemes.All)
				Assert.DoesNotThrow(() => new AppSettings { theme = theme }.Validate());

			Assert.Throws<ArgumentException>(() => new AppSettings { theme = "neon" }.Validate());
			Assert.Throws<ArgumentException>(() => new AppSettings { theme = null }.Validate());
			Assert.Throws<ArgumentException>(() => new AppSettings { version = 2 }.Validate());
		}

		[Test]
		public void SessionAppliesThemeAtOnceAndRejectsUnknownIds()
		{
			var session = new SettingsSession(new MemorySettings(), new AppSettings());
			Assert.That(session.SelectTheme(AppThemes.Forest), Is.True);
			Assert.That(session.Theme, Is.EqualTo(AppThemes.Forest));
			Assert.That(session.HasPendingSave, Is.True);
			Assert.That(session.SelectTheme(AppThemes.Forest), Is.False);
			Assert.Throws<ArgumentException>(() => session.SelectTheme("neon"));
			Assert.That(session.Theme, Is.EqualTo(AppThemes.Forest));
		}

		[UnityTest]
		public IEnumerator FailedWriteKeepsThemeAndRetriesPersistenceOnly()
		{
			var repository = new MemorySettings { Fail = true };
			var session = new SettingsSession(repository, new AppSettings());
			session.SelectTheme(AppThemes.Ember);
			var failed = session.PersistAsync().GetAwaiter();
			while (!failed.IsCompleted)
				yield return null;

			Assert.Throws<IOException>(() => failed.GetResult());
			Assert.That(session.Theme, Is.EqualTo(AppThemes.Ember));
			Assert.That(session.HasPendingSave, Is.True);
			Assert.That(repository.Snapshot, Is.Null);

			repository.Fail = false;
			var retry = session.PersistAsync().GetAwaiter();
			while (!retry.IsCompleted)
				yield return null;

			retry.GetResult();
			Assert.That(session.HasPendingSave, Is.False);
			Assert.That(repository.Snapshot.theme, Is.EqualTo(AppThemes.Ember));
			Assert.That(repository.Writes, Is.EqualTo(2));
		}

		[UnityTest]
		public IEnumerator ThemeRoundTripsAndMissingFileStartsWithDarkDefault()
		{
			string directory = NewDirectory();
			var first = new SettingsRepository(new SaveStore(directory));
			var load = first.LoadAsync().GetAwaiter();
			while (!load.IsCompleted)
				yield return null;

			Assert.That(load.GetResult().theme, Is.EqualTo(AppThemes.Midnight));
			var save = first.SaveAsync(new AppSettings { theme = AppThemes.Daylight }).GetAwaiter();
			while (!save.IsCompleted)
				yield return null;

			save.GetResult();
			Assert.That(File.Exists(Path.Combine(directory, SettingsRepository.Slot + ".json")), Is.True);
			var restarted = new SettingsRepository(new SaveStore(directory));
			load = restarted.LoadAsync().GetAwaiter();
			while (!load.IsCompleted)
				yield return null;

			Assert.That(load.GetResult().theme, Is.EqualTo(AppThemes.Daylight));
		}

		[UnityTest]
		public IEnumerator UnreadableSettingsFallBackToDefaultsAndStayPreserved()
		{
			string directory = NewDirectory();
			Directory.CreateDirectory(directory);
			string path = Path.Combine(directory, SettingsRepository.Slot + ".json");
			const string unsupported = "{\"version\":1,\"theme\":\"neon\"}";
			File.WriteAllText(path, unsupported);
			LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("English settings were unreadable"));
			var repository = new SettingsRepository(new SaveStore(directory));
			var load = repository.LoadAsync().GetAwaiter();
			while (!load.IsCompleted)
				yield return null;

			Assert.That(load.GetResult().theme, Is.EqualTo(AppThemes.Midnight));
			var save = repository.SaveAsync(new AppSettings { theme = AppThemes.Ocean }).GetAwaiter();
			while (!save.IsCompleted)
				yield return null;

			Assert.Throws<InvalidOperationException>(() => save.GetResult());
			Assert.That(File.ReadAllText(path), Is.EqualTo(unsupported));
		}

		private sealed class MemorySettings : ISettingsRepository
		{
			public bool Fail;
			public int Writes;
			public AppSettings Snapshot;

			public async Awaitable<AppSettings> LoadAsync()
			{
				await Awaitable.MainThreadAsync();
				return Snapshot?.Copy() ?? new AppSettings();
			}

			public async Awaitable SaveAsync(AppSettings snapshot)
			{
				Writes++;
				await Awaitable.MainThreadAsync();
				if (Fail)
					throw new IOException("Simulated unavailable disk.");

				Snapshot = snapshot.Copy();
			}
		}
	}
}
