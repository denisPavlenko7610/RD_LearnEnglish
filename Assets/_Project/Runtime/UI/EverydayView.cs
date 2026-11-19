using System;
using System.Collections.Generic;
using System.Linq;
using LearnEnglish.Core;
using RD.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace LearnEnglish
{
	[RequireComponent(typeof(UIDocument))]
	public sealed partial class EverydayView : MonoBehaviour
	{
		private const string Hidden = "hidden";
		private const string Selected = "selected";
		private const string CafeUnit = "a1.services";
		private const float CompactWidth = 900;
		private const string TokenPrefix = "token";
		private const string WordTheme = "topic-words";
		private const string PhrasalTheme = "topic-phrasal";
		private const string ChoiceCorrect = "choice-correct";
		private const string ChoiceWrong = "choice-wrong";
		private const string ThemeCardPrefix = "theme";
		private Curriculum _curriculum;
		private LocalizationService _strings;
		private LessonSession _session;
		private SettingsSession _settings;
		private bool _settingsOpen;
		private VisualElement _root;
		private int _selected;
		private bool _lesson;
		private bool _busy;
		private bool _intro;
		private string[] _options;
		private string _track;
		private string _previewUnitId;
		private bool _rulePreviewOpen;
		private string[] _tokens;
		private Button[] _tokenButtons;
		private readonly List<int> _pickedTokens = new();

		private enum Ui
		{
			brand,
			tagline,
			eyebrow,
			headline,
			subtitle,
			continueButton,
			courseLabel,
			previousUnit,
			nextUnit,
			startButton,
			homeButton,
			hintButton,
			ruleButton,
			checkButton,
			advanceButton,
			retryButton,
			dismissButton,
			pilotNotice,
			homeScreen,
			lessonScreen,
			statSeen,
			statRecall,
			statDelayed,
			statSeenLabel,
			statRecallLabel,
			statDelayedLabel,
			appRoot,
			settingsButton,
			settingsScreen,
			settingsTitle,
			settingsSubtitle,
			settingsDone,
			settingsStatus,
			reviewCount,
			topicTitle,
			topicGoal,
			topicProgress,
			topicRule,
			unitCounter,
			lessonLevel,
			lessonTopic,
			lessonEvidence,
			prompt,
			context,
			answerControls,
			choices,
			answerInput,
			assistedStatus,
			reference,
			referenceTitle,
			referenceText,
			feedback,
			feedbackTitle,
			expectedAnswer,
			explanation,
			saveStatus,
			topicImage,
			levelA1,
			levelA2,
			levelB1,
			levelB2,
			levelC1,
			option0,
			option1,
			option2,
			option3,
			lessonIntro,
			introTitle,
			introRule,
			beginButton,
			exerciseBody,
			progressTitle,
			topicDialogue,
			dialogueText,
			filterAll,
			filterWords,
			filterPhrasal,
			levelTabs,
			assembly,
			assemblyAnswer,
			assemblyUndo,
			topicPreview,
			topicLevel,
			topicMeter,
			previewRule,
			vocabularyButton,
			vocabularyScreen,
			vocabularyBack,
			vocabularyTitle,
			vocabularySearch,
			vocabularyLevel,
			vocabularyKind,
			vocabularyScope,
			vocabularyEmpty,
			vocabularyPrevious,
			vocabularyNext,
			vocabularyPage,
			vocabularySources,
			vocabularySpeechStatus,
			speakAnswer,
			speechAnswerStatus,
			modelPractice,
			modelInstruction,
			modelPhrase,
			speakModel,
			modelStatus,
			sourcePractice,
			speakSource,
			showSource,
			sourceStatus,
			sourceTranscript,
			recordModel,
			playRecording,
			clearRecording,
			recordingStatus
		}

		public void Construct(
			Curriculum curriculum,
			IProfileRepository repository,
			Profile profile,
			LocalizationService strings,
			SettingsSession settings
		)
		{
			if (_session != null)
				throw new InvalidOperationException("View already constructed.");

			_curriculum = curriculum;
			_strings = strings;
			_settings = settings ?? throw new ArgumentNullException(nameof(settings));
			_session = new LessonSession(curriculum, repository, profile);
			_root = GetComponent<UIDocument>().rootVisualElement;
			foreach (Ui id in new[]
				{
					Ui.brand, Ui.tagline, Ui.eyebrow, Ui.headline, Ui.subtitle, Ui.courseLabel, Ui.pilotNotice,
					Ui.progressTitle, Ui.topicDialogue, Ui.statSeenLabel, Ui.statRecallLabel, Ui.statDelayedLabel,
					Ui.settingsTitle, Ui.settingsSubtitle
				})
				L(id).text = T(id.ToString());
			foreach (Ui id in new[]
				{
					Ui.continueButton, Ui.previousUnit, Ui.nextUnit, Ui.startButton, Ui.homeButton,
					Ui.hintButton, Ui.ruleButton, Ui.checkButton, Ui.advanceButton, Ui.retryButton, Ui.dismissButton,
					Ui.settingsButton, Ui.settingsDone
				})
				B(id).text = T(id.ToString());
			B(Ui.settingsButton).clicked += ToggleSettings;
			B(Ui.settingsDone).clicked += CloseSettings;
			foreach (string theme in AppThemes.All)
			{
				string captured = theme;
				string card = ThemeCardName(theme);
				L(card + "Name").text = T(card + "Name");
				L(card + "Note").text = T(card + "Note");
				L(card + "Tag").text = T("themeSelected");
				_root.Q<Button>(card).clicked += () => SelectTheme(captured);
			}
			ApplyTheme();
			Input.label = T("answerLabel");
			B(Ui.beginButton).text = T("beginButton");
			B(Ui.beginButton).clicked += BeginPractice;
			L(Ui.modelInstruction).text = T("modelPractice");
			B(Ui.speakModel).text = T("speakModel");
			B(Ui.speakModel).clicked += PlayRehearsalModel;
			BindRehearsal();
			B(Ui.speakSource).text = T("speakSource");
			B(Ui.speakSource).clicked += () =>
				SpeakWord(_session.Current.speechText, Ui.sourceStatus, "sourceSpeechUnavailable");
			B(Ui.showSource).clicked += ToggleSourceTranscript;
			B(Ui.continueButton).clicked += Continue;
			B(Ui.startButton).clicked += StartTopic;
			B(Ui.previousUnit).clicked += () => MoveTopic(-1);
			B(Ui.nextUnit).clicked += () => MoveTopic(1);
			B(Ui.homeButton).clicked += BackFromLesson;
			B(Ui.hintButton).clicked += () => ShowReference(false);
			B(Ui.ruleButton).clicked += () => ShowReference(true);
			B(Ui.dismissButton).clicked += () => Show(Ui.reference, false);
			B(Ui.checkButton).clicked += Check;
			B(Ui.advanceButton).clicked += Advance;
			B(Ui.retryButton).clicked += Save;
			B(Ui.previewRule).clicked += ToggleRulePreview;
			B(Ui.filterAll).text = T("filterAll");
			B(Ui.filterWords).text = T("filterWords");
			B(Ui.filterPhrasal).text = T("filterPhrasal");
			B(Ui.filterAll).clicked += () => SelectTrack(null);
			B(Ui.filterWords).clicked += () => SelectTrack(Unit.WordTrack);
			B(Ui.filterPhrasal).clicked += () => SelectTrack(Unit.PhrasalTrack);
			B(Ui.assemblyUndo).text = T("assemblyUndo");
			B(Ui.assemblyUndo).clicked += UndoToken;
			_tokenButtons = Enumerable.Range(0, Exercise.MaxTokens).Select(i => _root.Q<Button>(TokenPrefix + i))
				.ToArray();
			for (int i = 0; i < _tokenButtons.Length; i++)
			{
				int index = i;
				_tokenButtons[i].clicked += () => PickToken(index);
			}
			foreach (Ui id in new[] { Ui.levelA1, Ui.levelA2, Ui.levelB1, Ui.levelB2, Ui.levelC1 })
			{
				Ui captured = id;
				B(id).clicked += () => SelectLevel(captured.ToString().Substring(5));
			}
			foreach (Ui id in new[] { Ui.option0, Ui.option1, Ui.option2, Ui.option3 })
			{
				int index = (int)id - (int)Ui.option0;
				B(id).clicked += () => Submit(_options[index]);
			}
			_root.RegisterCallback<KeyDownEvent>(OnKey, TrickleDown.TrickleDown);
			_root.RegisterCallback<GeometryChangedEvent>(OnGeometry);
			_selected = Math.Max(0, Array.FindIndex(curriculum.units, x => x.id == profile.lastUnit));
			BindVocabulary();
			ShowHome();
		}

		private VisualElement E(Ui id) => _root.Q(id.ToString());
		private Label L(Ui id) => _root.Q<Label>(id.ToString());
		private Label L(string name) => _root.Q<Label>(name);
		private Button B(Ui id) => _root.Q<Button>(id.ToString());
		private TextField Input => _root.Q<TextField>(Ui.answerInput.ToString());
		private string T(string key, params object[] arguments) => _strings.Get(key, arguments);
		private void Show(Ui id, bool visible) => E(id).EnableInClassList(Hidden, !visible);

		private void OnGeometry(GeometryChangedEvent evt) =>
			_root.EnableInClassList("compact", evt.newRect.width < CompactWidth);

		private void ShowHome()
		{
			if (_busy || _session.HasPendingSave)
				return;

			_lesson = false;
			ClearRehearsal();
			StopWordSpeech();
			_settingsOpen = false;
			_vocabularyOpen = false;
			_returnToVocabulary = false;
			Show(Ui.vocabularyScreen, false);
			Show(Ui.homeScreen, true);
			Show(Ui.lessonScreen, false);
			Show(Ui.settingsScreen, false);
			B(Ui.settingsButton).EnableInClassList(Selected, false);
			var items = _session.Profile.items;
			L(Ui.statSeen).text = items.Count.ToString();
			L(Ui.statRecall).text = items.Count(x => x.recalls > 0).ToString();
			L(Ui.statDelayed).text = items.Count(x => x.delayedRecalls > 0).ToString();
			L(Ui.reviewCount).text = T("reviewCount", _session.PendingCount(DateTime.UtcNow));
			RenderTopic();
		}

		private static string ThemeCardName(string theme) =>
			ThemeCardPrefix + char.ToUpperInvariant(theme[0]) + theme.Substring(1);

		private void ToggleSettings()
		{
			if (_settingsOpen)
				CloseSettings();
			else
				OpenSettings();
		}

		private void OpenSettings()
		{
			ClearRehearsal();
			StopWordSpeech();
			_settingsOpen = true;
			Show(Ui.homeScreen, false);
			Show(Ui.lessonScreen, false);
			Show(Ui.settingsScreen, true);
			Show(Ui.vocabularyScreen, false);
			B(Ui.settingsButton).EnableInClassList(Selected, true);
			L(Ui.settingsStatus).text = string.Empty;
			_root.Q<ScrollView>(nameof(Ui.settingsScreen)).scrollOffset = Vector2.zero;
		}

		private void CloseSettings()
		{
			_settingsOpen = false;
			Show(Ui.settingsScreen, false);
			Show(Ui.homeScreen, !_lesson && !_vocabularyOpen);
			Show(Ui.lessonScreen, _lesson);
			Show(Ui.vocabularyScreen, _vocabularyOpen);
			B(Ui.settingsButton).EnableInClassList(Selected, false);
		}

		private void SelectTheme(string theme)
		{
			if (_settings.SelectTheme(theme))
				ApplyTheme();

			SaveSettings();
		}

		private void ApplyTheme()
		{
			foreach (string theme in AppThemes.All)
			{
				bool active = theme == _settings.Theme;
				E(Ui.appRoot).EnableInClassList(AppThemes.ClassName(theme), active);
				string card = ThemeCardName(theme);
				_root.Q<Button>(card).EnableInClassList(Selected, active);
				_root.Q(card + "Tag").EnableInClassList(Hidden, !active);
			}
		}

		private async void SaveSettings()
		{
			try
			{
				await _settings.PersistAsync();
				if (!this || _settings.HasPendingSave)
					return;

				L(Ui.settingsStatus).text = T("themeSaved");
			}
			catch (Exception error)
			{
				Debug.LogWarning("English settings save failed: " + error.Message);
				if (!this)
					return;

				L(Ui.settingsStatus).text = T("themeSaveError");
			}
		}

		private void RenderTopic()
		{
			var unit = _curriculum.units[_selected];
			if (_previewUnitId != unit.id)
			{
				_previewUnitId = unit.id;
				SetRulePreview(false);
			}
			bool wordTopic = unit.track == Unit.WordTrack;
			bool phrasalTopic = unit.track == Unit.PhrasalTrack;
			E(Ui.topicPreview).EnableInClassList(WordTheme, wordTopic);
			E(Ui.topicPreview).EnableInClassList(PhrasalTheme, phrasalTopic);
			L(Ui.topicLevel).text = T(
				wordTopic ? "wordTopicLabel" : phrasalTopic ? "phrasalTopicLabel" : "generalTopicLabel",
				unit.level
			);
			L(Ui.topicTitle).text = unit.title;
			L(Ui.topicGoal).text = unit.goal;
			L(Ui.topicRule).text = unit.rule;
			int recalled = unit.exercises.Count(e => _session.Profile.items.Any(p => p.id == e.id && p.recalls > 0));
			var meter = _root.Q<ProgressBar>(nameof(Ui.topicMeter));
			meter.highValue = unit.exercises.Length;
			meter.value = recalled;
			L(Ui.topicProgress).text = T(
				"topicEvidence",
				recalled,
				unit.exercises.Count(e => _session.Profile.items.Any(p => p.id == e.id && p.delayedRecalls > 0)),
				unit.exercises.Length
			);
			var siblings = VisibleTopics();
			string position = T("unitCounter", Array.IndexOf(siblings, unit) + 1, siblings.Length);
			L(Ui.unitCounter).text = _track == null ? position : unit.level + " · " + position;
			Show(Ui.levelTabs, _track == null);
			B(Ui.filterAll).EnableInClassList(Selected, _track == null);
			B(Ui.filterWords).EnableInClassList(Selected, _track == Unit.WordTrack);
			B(Ui.filterPhrasal).EnableInClassList(Selected, _track == Unit.PhrasalTrack);
			foreach (Ui id in new[] { Ui.levelA1, Ui.levelA2, Ui.levelB1, Ui.levelB2, Ui.levelC1 })
				B(id).EnableInClassList(Selected, id.ToString() == "level" + unit.level);
			Show(Ui.topicImage, unit.id == CafeUnit);
			Show(Ui.topicDialogue, unit.exercises.Any(x => x.IsDialogue));
		}

		private void ToggleRulePreview()
		{
			SetRulePreview(!_rulePreviewOpen);
			if (_rulePreviewOpen)
			{
				var scroll = _root.Q<ScrollView>(nameof(Ui.homeScreen));
				scroll.schedule.Execute(() => scroll.ScrollTo(E(Ui.topicRule)));
			}
		}

		private void SetRulePreview(bool open)
		{
			_rulePreviewOpen = open;
			Show(Ui.topicRule, open);
			B(Ui.previewRule).text = T(open ? "closePreviewRule" : "openPreviewRule");
		}

		private void SelectLevel(string level)
		{
			_selected = Array.FindIndex(_curriculum.units, x => x.level == level);
			RenderTopic();
		}

		private Unit[] VisibleTopics()
		{
			string level = _curriculum.units[_selected].level;
			return _curriculum.units.Where(x => _track == null ? x.level == level : x.track == _track).ToArray();
		}

		private void SelectTrack(string track)
		{
			if (track != null)
			{
				int first = Array.FindIndex(_curriculum.units, x => x.track == track);
				if (first < 0)
					return;

				if (_curriculum.units[_selected].track != track)
					_selected = first;
			}
			_track = track;
			RenderTopic();
		}

		private void MoveTopic(int direction)
		{
			var siblings = VisibleTopics();
			int index = (Array.IndexOf(siblings, _curriculum.units[_selected]) + direction + siblings.Length) %
				siblings.Length;
			_selected = Array.IndexOf(_curriculum.units, siblings[index]);
			RenderTopic();
		}

		private void Continue()
		{
			int saved = Array.FindIndex(_curriculum.units, x => x.id == _session.Profile.lastUnit);
			if (saved >= 0)
			{
				_selected = saved;
				if (_track != _curriculum.units[saved].track)
					_track = null;
			}
			StartTopic();
		}

		private void StartTopic()
		{
			_session.Start(_curriculum.units[_selected], DateTime.UtcNow);
			_lesson = true;
			Show(Ui.homeScreen, false);
			Show(Ui.lessonScreen, true);
			RenderExercise();
			ShowIntroduction();
		}

		private void ShowIntroduction()
		{
			_intro = true;
			L(Ui.introTitle).text = T("ruleTitle");
			var unit = _curriculum.UnitFor(_session.Current.id);
			L(Ui.introRule).text = unit.rule;
			bool hasModel = !string.IsNullOrEmpty(unit.modelPhrase);
			Show(Ui.modelPractice, hasModel);
			Show(Ui.speakModel, hasModel && _wordSpeech != null);
			L(Ui.modelPhrase).text = unit.modelPhrase;
			Show(Ui.lessonIntro, true);
			Show(Ui.exerciseBody, false);
		}

		private void BeginPractice()
		{
			if (_intro)
			{
				ClearRehearsal();
				StopWordSpeech();
			}
			_intro = false;
			Show(Ui.lessonIntro, false);
			Show(Ui.exerciseBody, true);
		}

		private void RenderExercise()
		{
			StopWordSpeech();
			Show(Ui.speakAnswer, false);
			BeginPractice();
			var exercise = _session.Current;
			bool hasSource = !string.IsNullOrEmpty(exercise.speechText);
			Show(Ui.sourcePractice, hasSource);
			Show(Ui.speakSource, hasSource && _wordSpeech != null);
			Show(Ui.showSource, hasSource && _wordSpeech != null);
			L(Ui.sourceTranscript).text = exercise.speechText;
			SetSourceTranscript(hasSource && _wordSpeech == null);
			var unit = _curriculum.UnitFor(exercise.id);
			L(Ui.lessonLevel).text = unit.level == "C1" ? T("levelPreview") : unit.level;
			L(Ui.lessonTopic).text = unit.title;
			var evidence = _session.Profile.items.SingleOrDefault(x => x.id == exercise.id);
			L(Ui.lessonEvidence).text = T(
				evidence == null ? "evidenceNew" :
				evidence.failed ? "evidenceMistake" :
				evidence.delayedRecalls > 0 ? "evidenceRetained" :
				evidence.recalls > 0 ? "evidenceRecall" : "evidenceSeen"
			);
			L(Ui.prompt).text = _session.AlternateContext != null ? T("contextReviewPrompt") :
				exercise.HasTokens && !_session.UsesAssembly ? T("assemblyRecall") :
				exercise.options.Length > 0 && !_session.UsesChoices ? T("recallPrompt") : exercise.prompt;
			L(Ui.context).text = _session.AlternateContext?.context ?? exercise.context;
			L(Ui.dialogueText).text = exercise.dialogue ?? string.Empty;
			Show(Ui.dialogueText, exercise.IsDialogue);
			Show(Ui.reference, false);
			Show(Ui.feedback, false);
			Show(Ui.retryButton, false);
			Show(Ui.advanceButton, false);
			Show(Ui.choices, _session.UsesChoices);
			Show(Ui.answerInput, !_session.UsesChoices && !_session.UsesAssembly);
			Show(Ui.assembly, _session.UsesAssembly);
			Show(Ui.checkButton, !_session.UsesChoices);
			E(Ui.answerControls).SetEnabled(true);
			Input.SetValueWithoutNotify(string.Empty);
			ConfigureAssembly(exercise);
			L(Ui.assistedStatus).text = string.Empty;
			L(Ui.saveStatus).text = string.Empty;
			_options = exercise.options.OrderBy(_ => UnityEngine.Random.value).ToArray();
			foreach (Ui id in new[] { Ui.option0, Ui.option1, Ui.option2, Ui.option3 })
			{
				int index = (int)id - (int)Ui.option0;
				Show(id, index < _options.Length);
				B(id).EnableInClassList(ChoiceCorrect, false);
				B(id).EnableInClassList(ChoiceWrong, false);
				if (index < _options.Length)
					B(id).text = _options[index];
			}
			_root.Q<ScrollView>(Ui.lessonScreen.ToString()).scrollOffset = Vector2.zero;
		}

		private void ShowReference(bool rule)
		{
			_session.UseHint();
			L(Ui.referenceTitle).text = T(rule ? "ruleTitle" : "hintTitle");
			L(Ui.referenceText).text =
				rule
					? _curriculum.UnitFor(_session.Current.id).rule
					: _session.AlternateContext?.hint ?? _session.Current.hint;
			L(Ui.assistedStatus).text = _session.Assisted ? T("assisted") : string.Empty;
			Show(Ui.reference, true);
		}

		private void ToggleSourceTranscript() => SetSourceTranscript(E(Ui.sourceTranscript).ClassListContains(Hidden));

		private void SetSourceTranscript(bool visible)
		{
			Show(Ui.sourceTranscript, visible);
			B(Ui.showSource).text = T(visible ? "hideSource" : "showSource");
		}

		private void ConfigureAssembly(Exercise exercise)
		{
			_pickedTokens.Clear();
			B(Ui.checkButton).SetEnabled(!_session.UsesAssembly);
			if (!_session.UsesAssembly)
				return;

			_tokens = exercise.tokens.OrderBy(_ => UnityEngine.Random.value).ToArray();
			if (_tokens.SequenceEqual(exercise.tokens))
				Array.Reverse(_tokens);
			for (int i = 0; i < _tokenButtons.Length; i++)
			{
				var button = _tokenButtons[i];
				button.EnableInClassList(Hidden, i >= _tokens.Length);
				button.SetEnabled(true);
				if (i < _tokens.Length)
					button.text = _tokens[i];
			}
			RenderAssembly();
		}

		private string AssembledAnswer => string.Join(" ", _pickedTokens.Select(i => _tokens[i]));

		private void PickToken(int index)
		{
			if (_session.Answered || _pickedTokens.Contains(index))
				return;

			_pickedTokens.Add(index);
			_tokenButtons[index].SetEnabled(false);
			RenderAssembly();
		}

		private void UndoToken()
		{
			if (_pickedTokens.Count == 0 || _session.Answered)
				return;

			int last = _pickedTokens[^1];
			_pickedTokens.RemoveAt(_pickedTokens.Count - 1);
			_tokenButtons[last].SetEnabled(true);
			RenderAssembly();
		}

		private void RenderAssembly()
		{
			L(Ui.assemblyAnswer).text = _pickedTokens.Count == 0 ? T("assemblyEmpty") : AssembledAnswer;
			B(Ui.assemblyUndo).SetEnabled(_pickedTokens.Count > 0);
			B(Ui.checkButton).SetEnabled(_pickedTokens.Count == _tokens.Length);
		}

		private void Check()
		{
			if (_session.UsesAssembly && _pickedTokens.Count != _tokens.Length)
				return;

			Submit(_session.UsesAssembly ? AssembledAnswer : Input.value);
		}

		private void Submit(string answer)
		{
			if (_busy || _session.Answered)
				return;

			if (!_session.Submit(answer, DateTime.UtcNow))
			{
				L(Ui.saveStatus).text = T("emptyAnswer");
				return;
			}
			MarkChoices(answer);
			E(Ui.answerControls).SetEnabled(false);
			Show(Ui.reference, false);
			Show(Ui.feedback, true);
			E(Ui.feedback).EnableInClassList("incorrect", !_session.Correct);
			L(Ui.feedbackTitle).text = T(_session.Correct ? "correct" : "incorrect");
			L(Ui.expectedAnswer).text = T("expected", _session.Current.answers[0]);
			if (!string.IsNullOrEmpty(_session.Current.speechText))
			{
				SetSourceTranscript(true);
			}
			Show(Ui.speakAnswer, _wordSpeech != null && !string.IsNullOrEmpty(_session.Current.vocabularyTerm));
			L(Ui.explanation).text = _session.AlternateContext?.explanation ?? _session.Current.explanation;
			Save();
		}

		private void MarkChoices(string picked)
		{
			if (!_session.UsesChoices)
				return;

			foreach (Ui id in new[] { Ui.option0, Ui.option1, Ui.option2, Ui.option3 })
			{
				int index = (int)id - (int)Ui.option0;
				if (index >= _options.Length)
					continue;

				bool correct = AnswerMatcher.Matches(_options[index], _session.Current.answers);
				B(id).EnableInClassList(ChoiceCorrect, correct);
				B(id).EnableInClassList(ChoiceWrong, !correct && _options[index] == picked);
			}
		}

		private async void Save()
		{
			if (_busy)
				return;

			_busy = true;
			B(Ui.homeButton).SetEnabled(false);
			Show(Ui.retryButton, false);
			L(Ui.saveStatus).text = T("saving");
			try
			{
				await _session.PersistAsync();
				if (!this)
					return;

				L(Ui.saveStatus).text = T("saved");
				Show(Ui.advanceButton, true);
				var scroll = _root.Q<ScrollView>(Ui.lessonScreen.ToString());
				scroll.schedule.Execute(() => scroll.ScrollTo(E(Ui.feedback)));
			}
			catch (Exception error)
			{
				Debug.LogWarning("English profile save failed: " + error.Message);
				if (!this)
					return;

				L(Ui.saveStatus).text = T("saveError");
				Show(Ui.retryButton, true);
			}
			finally
			{
				_busy = false;
				if (this)
					B(Ui.homeButton).SetEnabled(!_session.HasPendingSave);
			}
		}

		private void Advance()
		{
			if (_busy || !_session.Answered || _session.HasPendingSave)
				return;

			var previousUnit = _curriculum.UnitFor(_session.Current.id);
			_session.Next(DateTime.UtcNow);
			RenderExercise();
			var nextUnit = _curriculum.UnitFor(_session.Current.id);
			if (previousUnit != nextUnit && !_session.Profile.items.Any(x => x.id == _session.Current.id))
				ShowIntroduction();
		}

		private void OnKey(KeyDownEvent evt)
		{
			if (_settingsOpen)
			{
				if (evt.keyCode == KeyCode.Escape)
					CloseSettings();

				return;
			}
			if (_vocabularyOpen && evt.keyCode == KeyCode.Escape)
			{
				ShowHome();
				return;
			}
			if (!_lesson)
				return;

			if (evt.keyCode == KeyCode.Escape)
				BackFromLesson();
			if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter)
				return;

			if (evt.target is Button)
			{
				return;
			}
			evt.StopImmediatePropagation();
			if (_intro)
			{
				BeginPractice();
				return;
			}
			if (_session.Answered)
				Advance();
			else if (!_session.UsesChoices)
				Check();
		}

		private void OnDestroy()
		{
			ReleaseRecording();
			_wordSpeech?.Stop();
			_root?.UnregisterCallback<KeyDownEvent>(OnKey, TrickleDown.TrickleDown);
			_root?.UnregisterCallback<GeometryChangedEvent>(OnGeometry);
		}
	}
}