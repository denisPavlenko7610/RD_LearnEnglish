using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnEnglish.Core
{
	public static class LearningPolicy
	{
		public const int ReviewStageCount = 5;

		public static int ReviewInterval(int stage) => stage switch
		{
			0 => 1,
			1 => 3,
			2 => 7,
			3 => 14,
			_ => 30
		};

		public const int MistakeSpacing = 2;
		public const int ReviewWindow = 5;
		public const int ReviewsPerWindow = 2;

		public static Profile Record(
			Profile profile,
			string unitId,
			Exercise exercise,
			bool correct,
			bool assisted,
			DateTime utcNow,
			bool typedRecall = true,
			bool review = false
		)
		{
			var next = profile.Copy();
			next.attempts++;
			next.lastUnit = unitId;
			next.recentReviews.Add(review);
			while (next.recentReviews.Count > ReviewWindow - 1)
				next.recentReviews.RemoveAt(0);
			var item = next.items.SingleOrDefault(x => x.id == exercise.id);
			if (item == null)
			{
				item = new ExerciseProgress { id = exercise.id };
				next.items.Add(item);
			}
			item.lastAttempt = next.attempts;
			if (!correct)
			{
				item.failed = true;
				item.stage = 0;
				item.dueTicks = utcNow.Ticks;
			}
			else if (!assisted)
			{
				item.successes++;
				if (typedRecall)
				{
					item.recalls++;
					if (item.stage > 0 && item.dueTicks <= utcNow.Ticks && !item.failed)
						item.delayedRecalls++;
				}
				item.failed = false;
				if (item.dueTicks <= utcNow.Ticks)
				{
					int interval = ReviewInterval(item.stage);
					item.stage = Math.Min(item.stage + 1, ReviewStageCount);
					item.dueTicks = utcNow.AddDays(interval).Ticks;
				}
			}
			return next;
		}

		public static bool NeedsReview(ExerciseProgress item, DateTime utcNow) =>
			item.failed || item.dueTicks <= utcNow.Ticks;

		public static Exercise SelectReview(Curriculum curriculum, Profile profile, DateTime utcNow)
		{
			var pending = profile.items.Where(x => NeedsReview(x, utcNow)).ToArray();
			var eligible = pending.Where(x => !x.failed || profile.attempts - x.lastAttempt >= MistakeSpacing)
				.OrderByDescending(x => x.failed).ThenBy(x => x.dueTicks).ThenBy(x => x.lastAttempt).FirstOrDefault();
			if (eligible == null)
				return null;

			return curriculum.Exercises.Single(x => x.id == eligible.id);
		}
	}
}