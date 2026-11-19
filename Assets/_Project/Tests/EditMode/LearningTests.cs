using System;
using System.Collections;
using System.IO;
using System.Linq;
using LearnEnglish.Core;
using NUnit.Framework;
using RD.Save;
using UnityEngine;
using UnityEngine.TestTools;

namespace LearnEnglish.Tests
{
	public sealed class LearningTests
	{
		private Curriculum _curriculum;
		private DateTime _now;
		private Unit First => _curriculum.units[0];
		private Exercise Card => First.exercises[0];

		[SetUp]
		public void SetUp()
		{
			_curriculum =
				JsonUtility.FromJson<Curriculum>(Resources.Load<TextAsset>(EnglishBootstrap.CurriculumResource).text);
			_now = new DateTime(2026, 11, 18, 12, 0, 0, DateTimeKind.Utc);
		}

		[Test]
		public void EntireCurriculumHasValidChoicesAndExplanations()
		{
			_curriculum.Validate();
			Assert.That(_curriculum.units.Length, Is.EqualTo(72));
			Assert.That(_curriculum.Exercises.Count(), Is.EqualTo(344));
			Assert.That(_curriculum.Exercises.Count(x => !string.IsNullOrEmpty(x.vocabularyTerm)), Is.EqualTo(121));
			Assert.That(_curriculum.Exercises.Count(x => x.HasTokens), Is.EqualTo(26));
			Assert.That(_curriculum.Exercises.Count(x => x.IsDialogue), Is.EqualTo(28));
			Assert.That(
				_curriculum.units.Select(x => x.level).Distinct(),
				Is.EquivalentTo(new[] { "A1", "A2", "B1", "B2", "C1" })
			);
			Assert.That(_curriculum.units.All(x => x.rule.Contains("\n\n")), Is.True);
		}

		[TestCase("  WE   ARE ready!  ", "We are ready", true)]
		[TestCase("were ready", "we're ready", false)]
		[TestCase("I work yesterday", "I worked yesterday", false)]
		[TestCase("turn off it", "turn it off", false)]
		[TestCase("", "am", false)]
		public void MatchingNormalizesOnlyPermittedDifferences(string answer, string expected, bool matches)
		{
			Assert.That(AnswerMatcher.Matches(answer, new[] { expected }), Is.EqualTo(matches));
		}

		[Test]
		public void DuplicateCorrectChoicesAreRejected()
		{
			Card.options = new[] { Card.answers[0], Card.answers[0] + "." };
			Assert.Throws<ArgumentException>(() => _curriculum.Validate());
		}

		[Test]
		public void AnswerMutationIsDetachedAndAssistanceDoesNotEstablishMastery()
		{
			var original = new Profile();
			var assisted = LearningPolicy.Record(original, First.id, Card, true, true, _now);
			Assert.That(original.attempts, Is.Zero);
			Assert.That(original.items, Is.Empty);
			Assert.That(assisted.attempts, Is.EqualTo(1));
			Assert.That(assisted.items[0].successes, Is.Zero);
			Assert.That(assisted.items[0].recalls, Is.Zero);
		}

		[Test]
		public void ChoiceRecognitionCannotCountAsRecallOrRetention()
		{
			var profile = LearningPolicy.Record(new Profile(), First.id, Card, true, false, _now, false);
			profile = LearningPolicy.Record(profile, First.id, Card, true, false, _now.AddDays(1), false);
			Assert.That(profile.items[0].successes, Is.EqualTo(2));
			Assert.That(profile.items[0].recalls, Is.Zero);
			Assert.That(profile.items[0].delayedRecalls, Is.Zero);
		}

		[Test]
		public void DelayedTypedRecallRequiresADueDayAndIndependentAnswer()
		{
			var profile = LearningPolicy.Record(new Profile(), First.id, Card, true, false, _now);
			long originalDue = profile.items[0].dueTicks;
			profile = LearningPolicy.Record(profile, First.id, Card, true, false, _now.AddHours(1));
			Assert.That(profile.items[0].dueTicks, Is.EqualTo(originalDue));
			Assert.That(profile.items[0].stage, Is.EqualTo(1));
			Assert.That(profile.items[0].delayedRecalls, Is.Zero);
			profile = LearningPolicy.Record(profile, First.id, Card, true, true, _now.AddDays(1));
			Assert.That(profile.items[0].delayedRecalls, Is.Zero);
			profile = LearningPolicy.Record(profile, First.id, Card, true, false, _now.AddDays(1));
			Assert.That(profile.items[0].delayedRecalls, Is.EqualTo(1));
			Assert.That(profile.items[0].dueTicks, Is.EqualTo(_now.AddDays(4).Ticks));
		}

		[Test]
		public void MistakeWaitsForTwoOtherAttemptsAndSurvivesCopies()
		{
			var profile = LearningPolicy.Record(new Profile(), First.id, Card, false, false, _now);
			Assert.That(LearningPolicy.SelectReview(_curriculum, profile.Copy(), _now), Is.Null);
			profile = LearningPolicy.Record(profile, First.id, First.exercises[1], true, false, _now);
			Assert.That(LearningPolicy.SelectReview(_curriculum, profile, _now), Is.Null);
			profile = LearningPolicy.Record(profile, First.id, First.exercises[2], true, false, _now);
			Assert.That(LearningPolicy.SelectReview(_curriculum, profile, _now).id, Is.EqualTo(Card.id));
			profile = LearningPolicy.Record(profile, First.id, Card, true, true, _now);
			Assert.That(profile.items.Single(x => x.id == Card.id).failed, Is.True);
		}

		[Test]
		public void UnknownIdsUnsupportedVersionsAndInvalidEvidenceAreRejected()
		{
			var profile = LearningPolicy.Record(new Profile(), First.id, Card, true, false, _now);
			profile.version++;
			Assert.Throws<ArgumentException>(() => profile.Validate(_curriculum));
			profile.version = Profile.CurrentVersion;
			profile.items[0].recalls = 100;
			Assert.Throws<ArgumentException>(() => profile.Validate(_curriculum));
			profile.items[0].recalls = 1;
			profile.items[0].id = "unknown";
			Assert.Throws<ArgumentException>(() => profile.Validate(_curriculum));
		}

		[UnityTest]
		public IEnumerator SaveFailureRetriesSnapshotWithoutReplayingAnswer()
		{
			var repository = new MemoryRepository { Fail = true };
			var session = new LessonSession(_curriculum, repository, new Profile());
			session.Start(First, _now);
			Assert.That(session.Submit(" ", _now), Is.False);
			Assert.That(session.Submit(session.Current.answers[0], _now), Is.True);
			Assert.That(session.Submit(session.Current.answers[0], _now), Is.False);
			var failed = session.PersistAsync().GetAwaiter();
			while (!failed.IsCompleted)
				yield return null;

			Assert.Throws<IOException>(() => failed.GetResult());
			Assert.That(session.HasPendingSave, Is.True);
			Assert.That(session.Profile.attempts, Is.Zero);
			Assert.Throws<InvalidOperationException>(() => session.Next(_now));
			repository.Fail = false;
			var retry = session.PersistAsync().GetAwaiter();
			while (!retry.IsCompleted)
				yield return null;

			retry.GetResult();
			Assert.That(session.Profile.attempts, Is.EqualTo(1));
			Assert.That(repository.Snapshot.attempts, Is.EqualTo(1));
			Assert.That(repository.Writes, Is.EqualTo(2));
			Assert.That(session.HasPendingSave, Is.False);
		}

		[UnityTest]
		public IEnumerator ProgressRoundTripsAndOptionsBecomeRecallAfterRestart()
		{
			string directory = Path.Combine("TestResults", "saves", Guid.NewGuid().ToString("N"));
			var repository = new ProfileRepository(new SaveStore(directory), _curriculum);
			var profile = LearningPolicy.Record(new Profile(), First.id, Card, true, false, _now, false);
			var save = repository.SaveAsync(profile).GetAwaiter();
			while (!save.IsCompleted)
				yield return null;

			save.GetResult();
			var load = repository.LoadAsync().GetAwaiter();
			while (!load.IsCompleted)
				yield return null;

			var loaded = load.GetResult();
			Assert.That(loaded.items[0].dueTicks, Is.EqualTo(profile.items[0].dueTicks));
			var session = new LessonSession(_curriculum, repository, loaded);
			session.Start(First, _now.AddDays(1));
			Assert.That(session.Current.id, Is.EqualTo(Card.id));
			Assert.That(session.UsesChoices, Is.False);
		}

		[UnityTest]
		public IEnumerator DamagedSaveIsPreserved()
		{
			string directory = Path.Combine("TestResults", "saves", Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(directory);
			string path = Path.Combine(directory, ProfileRepository.Slot + ".json");
			const string damaged = "{ definitely not valid JSON";
			File.WriteAllText(path, damaged);
			var repository = new ProfileRepository(new SaveStore(directory), _curriculum);
			var load = repository.LoadAsync().GetAwaiter();
			while (!load.IsCompleted)
				yield return null;

			Assert.Throws<InvalidSaveDataException>(() => load.GetResult());
			Assert.That(File.ReadAllText(path), Is.EqualTo(damaged));
		}

		[UnityTest]
		public IEnumerator ReviewCapPersistsAcrossSessionRestarts()
		{
			var profile = new Profile();
			foreach (var exercise in First.exercises)
				profile = LearningPolicy.Record(profile, First.id, exercise, true, false, _now);
			profile.recentReviews = new() { true, false, true, false };
			var repository = new MemoryRepository();
			var session = new LessonSession(_curriculum, repository, profile);
			session.Start(_curriculum.units[1], _now.AddDays(2));
			Assert.That(_curriculum.units[1].exercises.Contains(session.Current), Is.True);
			session.Submit(session.Current.answers[0], _now.AddDays(2));
			var save = session.PersistAsync().GetAwaiter();
			while (!save.IsCompleted)
				yield return null;

			save.GetResult();
			var restarted = new LessonSession(_curriculum, repository, repository.Snapshot);
			Assert.That(restarted.Profile.recentReviews.Count, Is.EqualTo(4));
			Assert.That(restarted.Profile.lastUnit, Is.EqualTo(_curriculum.units[1].id));
		}

		[UnityTest]
		public IEnumerator MissingSchemaCannotBecomeAnEmptyProfile()
		{
			string directory = Path.Combine("TestResults", "saves", Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(directory);
			string path = Path.Combine(directory, ProfileRepository.Slot + ".json");
			File.WriteAllText(path, "{}");
			var repository = new ProfileRepository(new SaveStore(directory), _curriculum);
			var load = repository.LoadAsync().GetAwaiter();
			while (!load.IsCompleted)
				yield return null;

			Assert.Throws<InvalidSaveDataException>(() => load.GetResult());
			Assert.That(File.ReadAllText(path), Is.EqualTo("{}"));
		}

		[UnityTest]
		public IEnumerator TopicPracticeRespectsMistakeSpacingWhenReviewsAreCapped()
		{
			var profile = new Profile();
			foreach (var card in First.exercises.Skip(1))
				profile = LearningPolicy.Record(profile, First.id, card, true, false, _now);
			profile = LearningPolicy.Record(profile, First.id, Card, false, false, _now);
			profile.recentReviews = new() { true, false, true, false };
			var session = new LessonSession(_curriculum, new MemoryRepository(), profile);
			session.Start(First, _now);
			Assert.That(session.Current.id, Is.Not.EqualTo(Card.id));
			session.Submit(session.Current.answers[0], _now);
			var save = session.PersistAsync().GetAwaiter();
			while (!save.IsCompleted)
				yield return null;

			save.GetResult();
			session.Next(_now);
			Assert.That(session.Current.id, Is.Not.EqualTo(Card.id));
			session.Submit(session.Current.answers[0], _now);
			save = session.PersistAsync().GetAwaiter();
			while (!save.IsCompleted)
				yield return null;

			save.GetResult();
			session.Next(_now);
			Assert.That(session.Current.id, Is.EqualTo(Card.id));
		}

		[UnityTest]
		public IEnumerator DueReviewUsesNewContextAndRecordsRecallUnderOriginalId()
		{
			var unit = _curriculum.units.Single(x => x.id == "b1.phrasal-actions");
			var card = unit.exercises[0];
			var repository = new MemoryRepository();
			var fresh = new LessonSession(_curriculum, repository, new Profile());
			fresh.Start(unit, _now);
			Assert.That(fresh.UsesChoices, Is.True);
			Assert.That(fresh.AlternateContext, Is.Null);
			var recognized = LearningPolicy.Record(new Profile(), unit.id, card, true, false, _now, false);
			var session = new LessonSession(_curriculum, repository, recognized);
			session.Start(unit, _now);
			Assert.That(session.AlternateContext, Is.Null);
			var due = _now.AddDays(1);
			session.Start(unit, due);
			Assert.That(session.AlternateContext.context, Does.Contain("radio"));
			Assert.That(session.UsesChoices, Is.False);
			Assert.That(session.Submit("turn it off", due), Is.True);
			var save = session.PersistAsync().GetAwaiter();
			while (!save.IsCompleted)
			{
				yield return null;
			}
			save.GetResult();
			var saved = repository.Snapshot.items.Single();
			Assert.That(saved.id, Is.EqualTo(card.id));
			Assert.That(saved.delayedRecalls, Is.EqualTo(1));
			session.Start(unit, due.AddDays(3));
			Assert.That(session.Current.id, Is.EqualTo(card.id));
			Assert.That(session.AlternateContext, Is.Null);
		}

		[Test]
		public void VocabularyMistakeReturnsInAnotherContextAfterTwoAnswers()
		{
			var unit = _curriculum.units.Single(x => x.id == "a1.public-transport-actions");
			var card = unit.exercises.Single(x => x.vocabularyTerm == "get off");
			var profile = LearningPolicy.Record(new Profile(), unit.id, card, false, false, _now);
			var session = new LessonSession(_curriculum, new MemoryRepository(), profile);
			session.Start(unit, _now);
			Assert.That(session.Current, Is.Not.SameAs(card));

			foreach (var other in unit.exercises.Where(x => x != card).Take(LearningPolicy.MistakeSpacing))
			{
				profile = LearningPolicy.Record(profile, unit.id, other, true, false, _now);
			}
			var restarted = new LessonSession(_curriculum, new MemoryRepository(), profile);
			restarted.Start(unit, _now);
			Assert.That(restarted.Current, Is.SameAs(card));
			Assert.That(restarted.AlternateContext.context, Does.Contain("tram"));
		}

		private sealed class MemoryRepository : IProfileRepository
		{
			public bool Fail;
			public int Writes;
			public Profile Snapshot;

			public async Awaitable<Profile> LoadAsync()
			{
				await Awaitable.MainThreadAsync();
				return Snapshot?.Copy() ?? new Profile();
			}

			public async Awaitable SaveAsync(Profile snapshot)
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