using System;
using System.Collections.Generic;
using System.Linq;
using LearnEnglish.Core;
using UnityEngine;

namespace LearnEnglish
{
	public sealed class LessonSession
	{
		private readonly Curriculum _curriculum;
		private readonly IProfileRepository _repository;
		private Profile _pending;
		private bool _saving;
		private bool _currentIsReview;
		public Profile Profile { get; private set; }
		public Unit Unit { get; private set; }
		public Exercise Current { get; private set; }
		public ReviewContext AlternateContext { get; private set; }
		public bool Assisted { get; private set; }
		public bool Answered { get; private set; }
		public bool Correct { get; private set; }
		public bool HasPendingSave => _pending != null;
		private bool IsFirstAttempt => Current != null && Profile.items.All(x => x.id != Current.id);
		public bool UsesChoices => IsFirstAttempt && Current.options.Length > 0;
		public bool UsesAssembly => IsFirstAttempt && Current.HasTokens;
		public int PendingCount(DateTime utcNow) => Profile.items.Count(x => LearningPolicy.NeedsReview(x, utcNow));

		public LessonSession(Curriculum curriculum, IProfileRepository repository, Profile profile)
		{
			curriculum.Validate();
			profile.Validate(curriculum);
			_curriculum = curriculum;
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			Profile = profile.Copy();
		}

		public void Start(Unit unit, DateTime utcNow)
		{
			if (HasPendingSave)
				throw new InvalidOperationException("Save the pending answer first.");
			if (!_curriculum.units.Contains(unit))
				throw new ArgumentException("Unknown unit.");

			Unit = unit;
			Current = null;
			Answered = false;
			Next(utcNow);
		}

		public void UseHint()
		{
			if (Current != null && !Answered)
				Assisted = true;
		}

		public void StartVocabulary(string exerciseId, DateTime utcNow)
		{
			if (HasPendingSave)
				throw new InvalidOperationException("Save the pending answer first.");

			var unit = _curriculum.UnitFor(exerciseId);
			var exercise = unit.exercises.Single(x => x.id == exerciseId);
			if (string.IsNullOrEmpty(exercise.vocabularyTerm))
				throw new ArgumentException("Unknown vocabulary exercise.", nameof(exerciseId));

			Unit = unit;
			Current = exercise;
			Answered = false;
			Assisted = false;
			var evidence = Profile.items.SingleOrDefault(x => x.id == exerciseId);
			_currentIsReview = evidence != null && LearningPolicy.NeedsReview(evidence, utcNow);
			SetReviewContext(evidence);
		}

		public bool Submit(string answer, DateTime utcNow)
		{
			if (Current == null || Answered || HasPendingSave || string.IsNullOrWhiteSpace(answer))
				return false;

			Correct = AnswerMatcher.Matches(answer, Current.answers);
			_pending = LearningPolicy.Record(
				Profile,
				Unit.id,
				Current,
				Correct,
				Assisted,
				utcNow,
				!UsesChoices && !UsesAssembly,
				_currentIsReview
			);
			_pending.Validate(_curriculum);
			Answered = true;
			return true;
		}

		public async Awaitable PersistAsync()
		{
			if (_pending == null)
				return;

			if (_saving)
				throw new InvalidOperationException("A save is already running.");

			_saving = true;
			try
			{
				await _repository.SaveAsync(_pending.Copy());
				Profile = _pending;
				_pending = null;
			}
			finally
			{
				_saving = false;
			}
		}

		public void Next(DateTime utcNow)
		{
			if (HasPendingSave)
				throw new InvalidOperationException("Save the pending answer first.");
			if (Unit == null)
				throw new InvalidOperationException("No unit selected.");

			if (Answered && Unit.exercises.All(e => Profile.items.Any(p => p.id == e.id && p.recalls > 0)))
			{
				int index = Array.IndexOf(_curriculum.units, Unit);
				if (index + 1 < _curriculum.units.Length &&
					(_curriculum.units[index + 1].level != "C1" || Unit.level == "C1"))
					Unit = _curriculum.units[index + 1];
			}
			string previousId = Current?.id;
			Assisted = false;
			Answered = false;
			var review = LearningPolicy.SelectReview(_curriculum, Profile, utcNow);
			bool canReview = Profile.recentReviews.Count(x => x) < LearningPolicy.ReviewsPerWindow;
			_currentIsReview = canReview && review != null;
			if (_currentIsReview)
			{
				Current = review;
			}
			else
			{
				var eligible = Unit.exercises.Where(x =>
					{
						var progress = Profile.items.SingleOrDefault(p => p.id == x.id);
						return progress == null ||
							!progress.failed ||
							Profile.attempts - progress.lastAttempt >= LearningPolicy.MistakeSpacing;
					}
				).ToArray();
				if (eligible.Length == 0)
					eligible = Unit.exercises;
				Current = eligible.OrderBy(x => x.id == previousId)
					.ThenBy(x => Profile.items.Any(p => p.id == x.id && p.successes > 0))
					.ThenBy(x => Profile.items.SingleOrDefault(p => p.id == x.id)?.lastAttempt ?? 0).First();
			}
			SetReviewContext(Profile.items.SingleOrDefault(p => p.id == Current.id));
		}

		private void SetReviewContext(ExerciseProgress evidence)
		{
			AlternateContext = _currentIsReview &&
				evidence != null &&
				(evidence.failed
					? !string.IsNullOrEmpty(Current.vocabularyTerm)
					: evidence.stage % 2 == 1)
					? Current.AlternateReview
					: null;
		}
	}
}