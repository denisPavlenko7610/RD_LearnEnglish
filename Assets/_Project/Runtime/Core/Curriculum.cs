using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnEnglish.Core
{
	[Serializable]
	public sealed class Curriculum
	{
		public Unit[] units;
		[NonSerialized] public VocabularyReference vocabularyReference;
		public IEnumerable<Exercise> Exercises => units.SelectMany(x => x.exercises);

		public void Validate()
		{
			if (units == null || units.Length == 0)
				throw new ArgumentException("Curriculum has no units.");

			var unitIds = new HashSet<string>();
			var exerciseIds = new HashSet<string>();
			foreach (var unit in units)
			{
				if (unit == null ||
					string.IsNullOrWhiteSpace(unit.id) ||
					!unitIds.Add(unit.id) ||
					!new[] { "A1", "A2", "B1", "B2", "C1" }.Contains(unit.level) ||
					(!string.IsNullOrEmpty(unit.track) &&
						unit.track != Unit.WordTrack &&
						unit.track != Unit.PhrasalTrack) ||
					string.IsNullOrWhiteSpace(unit.title) ||
					string.IsNullOrWhiteSpace(unit.goal) ||
					string.IsNullOrWhiteSpace(unit.rule) ||
					unit.exercises == null ||
					unit.exercises.Length == 0)
					throw new ArgumentException("Invalid or duplicate curriculum unit.");

				foreach (var exercise in unit.exercises)
				{
					if (exercise == null ||
						string.IsNullOrWhiteSpace(exercise.id) ||
						!exerciseIds.Add(exercise.id) ||
						string.IsNullOrWhiteSpace(exercise.prompt) ||
						string.IsNullOrWhiteSpace(exercise.context) ||
						string.IsNullOrWhiteSpace(exercise.hint) ||
						string.IsNullOrWhiteSpace(exercise.explanation) ||
						exercise.answers == null ||
						exercise.answers.Length == 0 ||
						exercise.answers.Any(string.IsNullOrWhiteSpace) ||
						exercise.options == null)
						throw new ArgumentException("Invalid or duplicate curriculum exercise.");

					if (exercise.AlternateReview is { } review &&
						(string.IsNullOrWhiteSpace(review.context) ||
							review.context == exercise.context ||
							string.IsNullOrWhiteSpace(review.hint) ||
							string.IsNullOrWhiteSpace(review.explanation)))
					{
						throw new ArgumentException("Invalid alternate review context for " + exercise.id);
					}
					bool hasVocabulary = !string.IsNullOrEmpty(exercise.vocabularyTerm) ||
						!string.IsNullOrEmpty(exercise.vocabularyKind) ||
						!string.IsNullOrEmpty(exercise.vocabularyMeaning);
					if (hasVocabulary &&
						(string.IsNullOrWhiteSpace(exercise.vocabularyTerm) ||
							string.IsNullOrWhiteSpace(exercise.vocabularyMeaning) ||
							exercise.vocabularyKind is not ("words" or "expressions" or "phrasal") ||
							exercise.options.Length > 0 ||
							exercise.HasTokens ||
							exercise.AlternateReview == null ||
							!AnswerMatcher.Matches(exercise.vocabularyTerm, exercise.answers)))
						throw new ArgumentException("Invalid vocabulary exercise for " + exercise.id);

					if (exercise.HasTokens &&
						(exercise.tokens.Length < 2 ||
							exercise.tokens.Length > Exercise.MaxTokens ||
							exercise.tokens.Any(x => string.IsNullOrWhiteSpace(x) || x.Any(char.IsWhiteSpace)) ||
							exercise.options.Length > 0 ||
							exercise.IsDialogue ||
							!AnswerMatcher.Matches(string.Join(" ", exercise.tokens), exercise.answers)))
					{
						throw new ArgumentException("Invalid sentence assembly for " + exercise.id);
					}
					if (exercise.options.Length > 0 &&
						(exercise.options.Length < 2 ||
							exercise.options.Length > 4 ||
							exercise.options.Any(string.IsNullOrWhiteSpace) ||
							exercise.options.Select(AnswerMatcher.Normalize).Distinct().Count() !=
							exercise.options.Length ||
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
		public const string WordTrack = "words";
		public const string PhrasalTrack = "phrasal";
		public string track;
		public string id;
		public string level;
		public string title;
		public string goal;
		public string rule;
		public string modelPhrase;
		public Exercise[] exercises;
	}

	[Serializable]
	public sealed class Exercise
	{
		public const int MaxTokens = 8;
		public string id;
		public string prompt;
		public string context;
		public string[] answers;
		public string[] options;
		public string[] tokens;
		public bool HasTokens => tokens is { Length: > 0 };
		public string dialogue;
		public string speechText;
		public ReviewContext reviewContext;
		public string hint;
		public string explanation;
		public string vocabularyTerm;
		public string vocabularyMeaning;
		public string vocabularyKind;
		public ReviewContext AlternateReview => reviewContext is { HasContent: true } ? reviewContext : null;
		public bool IsDialogue => !string.IsNullOrWhiteSpace(dialogue);
	}

	[Serializable]
	public sealed class ReviewContext
	{
		public string context;
		public string hint;
		public string explanation;
		public bool HasContent => !string.IsNullOrEmpty(context) ||
			!string.IsNullOrEmpty(hint) ||
			!string.IsNullOrEmpty(explanation);
	}
}