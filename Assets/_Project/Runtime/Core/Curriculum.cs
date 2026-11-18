using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnEnglish.Core
{
	[Serializable]
	public sealed class Curriculum
	{
		public Unit[] units;
		public IEnumerable<Exercise> Exercises => units.SelectMany(x => x.exercises);

		public void Validate()
		{
			if (units == null || units.Length == 0) throw new ArgumentException("Curriculum has no units.");
			var unitIds = new HashSet<string>();
			var exerciseIds = new HashSet<string>();
			foreach (var unit in units)
			{
				if (unit == null || string.IsNullOrWhiteSpace(unit.id) || !unitIds.Add(unit.id) ||
					!new[] { "A1", "A2", "B1", "B2", "C1" }.Contains(unit.level) ||
					string.IsNullOrWhiteSpace(unit.title) || string.IsNullOrWhiteSpace(unit.goal) ||
					string.IsNullOrWhiteSpace(unit.rule) || unit.exercises == null || unit.exercises.Length == 0)
					throw new ArgumentException("Invalid or duplicate curriculum unit.");
				foreach (var exercise in unit.exercises)
				{
					if (exercise == null || string.IsNullOrWhiteSpace(exercise.id) || !exerciseIds.Add(exercise.id) ||
						string.IsNullOrWhiteSpace(exercise.prompt) || string.IsNullOrWhiteSpace(exercise.context) ||
						string.IsNullOrWhiteSpace(exercise.hint) || string.IsNullOrWhiteSpace(exercise.explanation) ||
						exercise.answers == null || exercise.answers.Length == 0 || exercise.answers.Any(string.IsNullOrWhiteSpace) ||
						exercise.options == null)
						throw new ArgumentException("Invalid or duplicate curriculum exercise.");
					if (exercise.options.Length > 0 && (exercise.options.Length < 2 || exercise.options.Length > 4 ||
						exercise.options.Any(string.IsNullOrWhiteSpace) ||
						exercise.options.Select(AnswerMatcher.Normalize).Distinct().Count() != exercise.options.Length ||
						exercise.options.Count(x => AnswerMatcher.Matches(x, exercise.answers)) != 1))
						throw new ArgumentException("Choice exercise must have exactly one valid option.");
				}
			}
		}

		public Unit UnitFor(string exerciseId) => units.Single(x => x.exercises.Any(e => e.id == exerciseId));
	}

	[Serializable]
	public sealed class Unit
	{
		public string id;
		public string level;
		public string title;
		public string goal;
		public string rule;
		public Exercise[] exercises;
	}

	[Serializable]
	public sealed class Exercise
	{
		public string id;
		public string prompt;
		public string context;
		public string[] answers;
		public string[] options;
		public string hint;
		public string explanation;
	}
}
