using System;
using System.Globalization;
using System.Linq;
using UnityEngine.UIElements;
using LearnEnglish.Core;

namespace LearnEnglish
{
	public sealed partial class EverydayView
	{
		private const int VocabularyPageSize = 8;
		private const string VocabularyRowPrefix = "vocabularyRow";
		private VocabularyRow[] _vocabulary;
		private VocabularyRow[] _visibleVocabulary;
		private int _vocabularyPage;
		private bool _vocabularyOpen;
		private bool _returnToVocabulary;
		private IWordSpeech _wordSpeech;
		private int _speechRevision;

		public void ConfigureWordSpeech(IWordSpeech speech)
		{
			_wordSpeech = speech ?? throw new ArgumentNullException(nameof(speech));
		}

		private sealed class VocabularyRow
		{
			public string Term { get; }
			public string Meaning { get; }
			public string Level { get; }
			public string Kind { get; }
			public string PartOfSpeech { get; }
			public int Rank { get; }
			public Unit Unit { get; }
			public Exercise Exercise { get; }

			public VocabularyRow(Unit unit, Exercise exercise)
			{
				Term = exercise.vocabularyTerm;
				Meaning = exercise.vocabularyMeaning;
				Level = unit.level;
				Kind = exercise.vocabularyKind;
				Unit = unit;
				Exercise = exercise;
			}

			public VocabularyRow(VocabularyReferenceEntry entry)
			{
				Term = entry.term;
				Meaning = entry.meaning;
				Level = entry.level;
				Kind = "words";
				PartOfSpeech = entry.pos;
				Rank = entry.rank;
			}
		}

		private void BindVocabulary()
		{
			var practice = _curriculum.units.SelectMany(unit => unit.exercises
				.Where(exercise => !string.IsNullOrEmpty(exercise.vocabularyTerm))
				.Select(exercise => new VocabularyRow(unit, exercise))
			);
			var reference = _curriculum.vocabularyReference.entries.Select(entry => new VocabularyRow(entry));
			_vocabulary = practice.Concat(reference)
				.OrderBy(x => x.Level)
				.ThenBy(x => x.Exercise == null)
				.ThenBy(x => x.Rank == 0 ? int.MaxValue : x.Rank)
				.ThenBy(x => x.Term, StringComparer.OrdinalIgnoreCase).ToArray();

			foreach (Ui id in new[]
				{ Ui.vocabularyButton, Ui.vocabularyBack, Ui.vocabularyPrevious, Ui.vocabularyNext })
			{
				B(id).text = T(id.ToString());
			}
			foreach (Ui id in new[] { Ui.vocabularyTitle, Ui.vocabularyEmpty, Ui.vocabularySources })
			{
				L(id).text = T(id.ToString());
			}
			B(Ui.vocabularyButton).clicked += OpenVocabulary;
			B(Ui.vocabularyBack).clicked += ShowHome;
			B(Ui.vocabularyPrevious).clicked += () => MoveVocabularyPage(-1);
			B(Ui.vocabularyNext).clicked += () => MoveVocabularyPage(1);
			B(Ui.speakAnswer).text = T("wordSpeech");
			B(Ui.speakAnswer).clicked += () => SpeakWord(_session.Current.vocabularyTerm, Ui.speechAnswerStatus);
			var search = _root.Q<TextField>(nameof(Ui.vocabularySearch));
			search.label = T("vocabularySearch");
			search.RegisterValueChangedCallback(_ => RefreshVocabulary());
			BindVocabularyFilter(Ui.vocabularyLevel, T("vocabularyAllLevels"), "A1", "A2", "B1", "B2", "C1");
			BindVocabularyFilter(
				Ui.vocabularyKind,
				T("vocabularyAllKinds"),
				T("vocabularyKind_words"),
				T("vocabularyKind_expressions"),
				T("vocabularyKind_phrasal")
			);
			BindVocabularyFilter(
				Ui.vocabularyScope,
				T("vocabularyWithPractice"),
				T("vocabularyWholeCatalog"),
				T("vocabularyDue")
			);

			for (int i = 0; i < VocabularyPageSize; i++)
			{
				int index = i;
				var row = _root.Q(VocabularyRowPrefix + i);
				var action = row.Q<Button>();
				action.text = T("vocabularyPractice");
				action.clicked += () =>
					OpenVocabularyExercise(_visibleVocabulary[_vocabularyPage * VocabularyPageSize + index]);
				var speaker = row.Q<Button>(className: "vocabulary-speak");
				speaker.text = T("wordSpeech");
				speaker.clicked += () => SpeakWord(
					_visibleVocabulary[_vocabularyPage * VocabularyPageSize + index].Term,
					Ui.vocabularySpeechStatus
				);
			}
		}

		private void BindVocabularyFilter(Ui id, params string[] choices)
		{
			var field = VocabularyFilter(id);
			field.label = T(id.ToString());
			field.choices = choices.ToList();
			field.index = 0;
			field.RegisterValueChangedCallback(_ => RefreshVocabulary());
		}

		private DropdownField VocabularyFilter(Ui id) => _root.Q<DropdownField>(id.ToString());

		private void OpenVocabulary()
		{
			ClearRehearsal();
			_vocabularyOpen = true;
			_lesson = false;
			_returnToVocabulary = false;
			Show(Ui.homeScreen, false);
			Show(Ui.lessonScreen, false);
			Show(Ui.vocabularyScreen, true);
			B(Ui.homeButton).text = T("homeButton");
			RenderVocabulary();
		}

		private void RefreshVocabulary()
		{
			_vocabularyPage = 0;
			if (_vocabularyOpen)
			{
				RenderVocabulary();
			}
		}

		private void MoveVocabularyPage(int offset)
		{
			_vocabularyPage += offset;
			RenderVocabulary();
		}

		private void RenderVocabulary()
		{
			StopWordSpeech();
			var evidence = _session.Profile.items.ToDictionary(x => x.id);
			var now = DateTime.UtcNow;
			var level = VocabularyFilter(Ui.vocabularyLevel);
			string kind = VocabularyFilter(Ui.vocabularyKind).index switch
			{
				1 => "words",
				2 => "expressions",
				3 => "phrasal",
				_ => null
			};
			int scope = VocabularyFilter(Ui.vocabularyScope).index;
			string query = _root.Q<TextField>(nameof(Ui.vocabularySearch)).value?.Trim() ?? string.Empty;
			_visibleVocabulary = _vocabulary.Where(row =>
					(level.index == 0 || row.Level == level.value) &&
					(kind == null || row.Kind == kind) &&
					(scope == 1 || row.Exercise != null) &&
					(scope != 2 ||
						(evidence.TryGetValue(row.Exercise.id, out var progress) &&
							LearningPolicy.NeedsReview(progress, now))) &&
					(query.Length == 0 || VocabularyMatches(row.Term, query) || VocabularyMatches(row.Meaning, query))
				)
				.OrderByDescending(row => string.Equals(row.Term, query, StringComparison.OrdinalIgnoreCase))
				.ToArray();

			int pages = Math.Max(1, (_visibleVocabulary.Length + VocabularyPageSize - 1) / VocabularyPageSize);
			_vocabularyPage = Math.Clamp(_vocabularyPage, 0, pages - 1);
			Show(Ui.vocabularyEmpty, _visibleVocabulary.Length == 0);
			Show(Ui.vocabularySources, scope == 1);
			B(Ui.vocabularyPrevious).SetEnabled(_vocabularyPage > 0);
			B(Ui.vocabularyNext).SetEnabled(_vocabularyPage + 1 < pages);
			L(Ui.vocabularyPage).text = T("vocabularyPage", _vocabularyPage + 1, pages, _visibleVocabulary.Length);

			for (int i = 0; i < VocabularyPageSize; i++)
			{
				var element = _root.Q(VocabularyRowPrefix + i);
				int index = _vocabularyPage * VocabularyPageSize + i;
				element.EnableInClassList(Hidden, index >= _visibleVocabulary.Length);
				if (index >= _visibleVocabulary.Length)
				{
					continue;
				}
				var row = _visibleVocabulary[index];
				var progress = row.Exercise != null && evidence.TryGetValue(row.Exercise.id, out var saved)
					? saved
					: null;
				string type = T(
					row.Exercise != null ? "vocabularyKind_" + row.Kind : "vocabularyPos_" + row.PartOfSpeech
				);
				element.Q<Label>(className: "vocabulary-term").text = T("vocabularyEntry", row.Term, row.Level, type);
				string meaning = string.IsNullOrWhiteSpace(row.Meaning) ? T("vocabularyNoGloss") : row.Meaning;
				element.Q<Label>(className: "vocabulary-meaning").text =
					T("vocabularyEntryDetail", meaning, VocabularyStatus(row, progress, now));
				element.Q<Button>().EnableInClassList(Hidden, row.Exercise == null);
				element.Q<Button>(className: "vocabulary-speak").EnableInClassList(Hidden, _wordSpeech == null);
			}
			_root.Q<ScrollView>(nameof(Ui.vocabularyScreen)).scrollOffset = UnityEngine.Vector2.zero;
		}

		private async void SpeakWord(string term, Ui status, string unavailableKey = "wordSpeechUnavailable")
		{
			if (_wordSpeech == null)
			{
				return;
			}
			int revision = ++_speechRevision;
			L(status).text = T("wordSpeechLoading");
			bool played = false;
			try
			{
				played = await _wordSpeech.SpeakAsync(term);
			}
			catch (Exception error)
			{
				UnityEngine.Debug.LogWarning("Pronunciation playback failed: " + error.Message);
			}
			if (this && revision == _speechRevision)
			{
				L(status).text = played ? string.Empty : T(unavailableKey);
			}
		}

		private void StopWordSpeech()
		{
			_speechRevision++;
			_wordSpeech?.Stop();
			L(Ui.vocabularySpeechStatus).text = string.Empty;
			L(Ui.speechAnswerStatus).text = string.Empty;
			L(Ui.modelStatus).text = string.Empty;
			L(Ui.sourceStatus).text = string.Empty;
		}

		private static bool VocabularyMatches(string text, string query) =>
			CultureInfo.InvariantCulture.CompareInfo.IndexOf(
				text ?? string.Empty,
				query,
				CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
			) >=
			0;

		private string VocabularyStatus(VocabularyRow row, ExerciseProgress progress, DateTime now)
		{
			if (row.Exercise == null)
			{
				return T("vocabularyReferenceOnly");
			}
			if (progress == null)
			{
				return T("vocabularyNew");
			}
			if (progress.failed)
			{
				return T("vocabularyNeedsPractice");
			}
			if (progress.recalls == 0)
			{
				return T("vocabularyAssisted");
			}
			string evidence = T(progress.delayedRecalls > 0 ? "vocabularyRetained" : "vocabularyRecalled");
			return LearningPolicy.NeedsReview(progress, now) ? T("vocabularyReviewDue", evidence) : evidence;
		}

		private void OpenVocabularyExercise(VocabularyRow row)
		{
			_session.StartVocabulary(row.Exercise.id, DateTime.UtcNow);
			_track = null;
			_selected = Array.IndexOf(_curriculum.units, row.Unit);
			_returnToVocabulary = true;
			_vocabularyOpen = false;
			_lesson = true;
			Show(Ui.vocabularyScreen, false);
			Show(Ui.lessonScreen, true);
			B(Ui.homeButton).text = T("vocabularyReturn");
			RenderExercise();
		}

		private void BackFromLesson()
		{
			if (_busy || _session.HasPendingSave)
			{
				return;
			}
			if (_returnToVocabulary)
			{
				OpenVocabulary();
			}
			else
			{
				ShowHome();
			}
		}
	}
}