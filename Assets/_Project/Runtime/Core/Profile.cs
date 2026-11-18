using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnEnglish.Core
{
	[Serializable]
	public sealed class Profile
	{
		public const int CurrentVersion = 1;
		public int version = CurrentVersion;
		public int attempts;
		public string lastUnit = "";
		public List<ExerciseProgress> items = new();
		public List<bool> recentReviews = new();

		public void Validate(Curriculum curriculum)
		{
			if (version != CurrentVersion ||
				attempts < 0 ||
				items == null ||
				recentReviews == null ||
				recentReviews.Count > Math.Min(attempts, LearningPolicy.ReviewWindow - 1) ||
				lastUnit == null ||
				(lastUnit.Length > 0 && !curriculum.units.Any(x => x.id == lastUnit)))
				throw new ArgumentException("Unsupported or invalid profile.");

			var known = new HashSet<string>(curriculum.Exercises.Select(x => x.id));
			var ids = new HashSet<string>();
			foreach (var item in items)
			{
				if (item == null ||
					!known.Contains(item.id ?? "") ||
					!ids.Add(item.id) ||
					item.successes < 0 ||
					item.successes > attempts ||
					item.stage < 0 ||
					item.stage > LearningPolicy.ReviewStageCount ||
					item.recalls < 0 ||
					item.recalls > item.successes ||
					item.delayedRecalls < 0 ||
					item.delayedRecalls > item.recalls ||
					item.lastAttempt < 1 ||
					item.lastAttempt > attempts ||
					item.dueTicks < 0 ||
					item.dueTicks > DateTime.MaxValue.Ticks)
					throw new ArgumentException("Invalid exercise progress.");
			}
			if (items.Count > attempts)
				throw new ArgumentException("Invalid attempt count.");
		}

		public Profile Copy() => new()
		{
			version = version,
			attempts = attempts,
			lastUnit = lastUnit,
			recentReviews = new List<bool>(recentReviews),
			items = items.Select(x => x.Copy()).ToList()
		};
	}

	[Serializable]
	public sealed class ExerciseProgress
	{
		public string id;
		public int successes;
		public int recalls;
		public int delayedRecalls;
		public int stage;
		public bool failed;
		public long dueTicks;
		public int lastAttempt;
		public ExerciseProgress Copy() => (ExerciseProgress)MemberwiseClone();
	}
}