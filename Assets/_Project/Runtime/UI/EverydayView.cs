using System;
using System.Linq;
using LearnEnglish.Core;
using RD.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace LearnEnglish
{
	[RequireComponent(typeof(UIDocument))]
	public sealed class EverydayView : MonoBehaviour
	{
		private const string Hidden = "hidden";
		private const string Selected = "selected";
		private const string CafeUnit = "a1.services";
		private const float CompactWidth = 900;
		private Curriculum _curriculum;
		private LocalizationService _strings;
		private LessonSession _session;
		private VisualElement _root;
		private int _selected;
		private bool _lesson;
		private bool _busy;
		private bool _intro;
		private string[] _options;

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
			evidenceSummary,
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
			exerciseBody
		}

		public void Construct(
			Curriculum curriculum,
			IProfileRepository repository,
			Profile profile,
			LocalizationService strings
		)
		{
			if (_session != null)
				throw new InvalidOperationException("View already constructed.");

			_curriculum = curriculum;
			_strings = strings;
			_session = new LessonSession(curriculum, repository, profile);
			_root = GetComponent<UIDocument>().rootVisualElement;
			foreach (Ui id in new[]
				{ Ui.brand, Ui.tagline, Ui.eyebrow, Ui.headline, Ui.subtitle, Ui.courseLabel, Ui.pilotNotice })
				L(id).text = T(id.ToString());
			foreach (Ui id in new[]
				{
					Ui.continueButton, Ui.previousUnit, Ui.nextUnit, Ui.startButton, Ui.homeButton,
					Ui.hintButton, Ui.ruleButton, Ui.checkButton, Ui.advanceButton, Ui.retryButton, Ui.dismissButton
				})
				B(id).text = T(id.ToString());
			Input.label = T("answerLabel");
			B(Ui.beginButton).text = T("beginButton");
			B(Ui.beginButton).clicked += BeginPractice;
			B(Ui.continueButton).clicked += Continue;
			B(Ui.startButton).clicked += StartTopic;
			B(Ui.previousUnit).clicked += () => MoveTopic(-1);
			B(Ui.nextUnit).clicked += () => MoveTopic(1);
			B(Ui.homeButton).clicked += ShowHome;
			B(Ui.hintButton).clicked += () => ShowReference(false);
			B(Ui.ruleButton).clicked += () => ShowReference(true);
			B(Ui.dismissButton).clicked += () => Show(Ui.reference, false);
			B(Ui.checkButton).clicked += Check;
			B(Ui.advanceButton).clicked += Advance;
			B(Ui.retryButton).clicked += Save;
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
			ShowHome();
		}

		private VisualElement E(Ui id) => _root.Q(id.ToString());
		private Label L(Ui id) => _root.Q<Label>(id.ToString());
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
			Show(Ui.homeScreen, true);
			Show(Ui.lessonScreen, false);
			var items = _session.Profile.items;
			L(Ui.evidenceSummary).text = T(
				"evidenceSummary",
				items.Count,
				items.Count(x => x.recalls > 0),
				items.Count(x => x.delayedRecalls > 0)
			);
			L(Ui.reviewCount).text = T("reviewCount", _session.PendingCount(DateTime.UtcNow));
			RenderTopic();
		}

		private void RenderTopic()
		{
			var unit = _curriculum.units[_selected];
			L(Ui.topicTitle).text = unit.title;
			L(Ui.topicGoal).text = unit.goal;
			L(Ui.topicRule).text = unit.rule;
			L(Ui.topicProgress).text = T(
				"topicEvidence",
				unit.exercises.Count(e => _session.Profile.items.Any(p => p.id == e.id && p.recalls > 0)),
				unit.exercises.Count(e => _session.Profile.items.Any(p => p.id == e.id && p.delayedRecalls > 0)),
				unit.exercises.Length
			);
			var siblings = _curriculum.units.Where(x => x.level == unit.level).ToArray();
			L(Ui.unitCounter).text = T("unitCounter", Array.IndexOf(siblings, unit) + 1, siblings.Length);
			foreach (Ui id in new[] { Ui.levelA1, Ui.levelA2, Ui.levelB1, Ui.levelB2, Ui.levelC1 })
				B(id).EnableInClassList(Selected, id.ToString() == "level" + unit.level);
			Show(Ui.topicImage, unit.id == CafeUnit);
		}

		private void SelectLevel(string level)
		{
			_selected = Array.FindIndex(_curriculum.units, x => x.level == level);
			RenderTopic();
		}

		private void MoveTopic(int direction)
		{
			var siblings = _curriculum.units.Where(x => x.level == _curriculum.units[_selected].level).ToArray();
			int index = (Array.IndexOf(siblings, _curriculum.units[_selected]) + direction + siblings.Length) %
				siblings.Length;
			_selected = Array.IndexOf(_curriculum.units, siblings[index]);
			RenderTopic();
		}

		private void Continue()
		{
			int saved = Array.FindIndex(_curriculum.units, x => x.id == _session.Profile.lastUnit);
			if (saved >= 0)
				_selected = saved;
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
			L(Ui.introRule).text = _curriculum.UnitFor(_session.Current.id).rule;
			Show(Ui.lessonIntro, true);
			Show(Ui.exerciseBody, false);
		}

		private void BeginPractice()
		{
			_intro = false;
			Show(Ui.lessonIntro, false);
			Show(Ui.exerciseBody, true);
		}

		private void RenderExercise()
		{
			BeginPractice();
			var exercise = _session.Current;
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
			L(Ui.prompt).text = exercise.options.Length > 0 && !_session.UsesChoices
				? T("recallPrompt")
				: exercise.prompt;
			L(Ui.context).text = exercise.context;
			Show(Ui.reference, false);
			Show(Ui.feedback, false);
			Show(Ui.retryButton, false);
			Show(Ui.advanceButton, false);
			Show(Ui.choices, _session.UsesChoices);
			Show(Ui.answerInput, !_session.UsesChoices);
			Show(Ui.checkButton, !_session.UsesChoices);
			E(Ui.answerControls).SetEnabled(true);
			Input.SetValueWithoutNotify(string.Empty);
			L(Ui.assistedStatus).text = string.Empty;
			L(Ui.saveStatus).text = string.Empty;
			_options = exercise.options.OrderBy(_ => UnityEngine.Random.value).ToArray();
			foreach (Ui id in new[] { Ui.option0, Ui.option1, Ui.option2, Ui.option3 })
			{
				int index = (int)id - (int)Ui.option0;
				Show(id, index < _options.Length);
				if (index < _options.Length)
					B(id).text = _options[index];
			}
			_root.Q<ScrollView>(Ui.lessonScreen.ToString()).scrollOffset = Vector2.zero;
		}

		private void ShowReference(bool rule)
		{
			_session.UseHint();
			L(Ui.referenceTitle).text = T(rule ? "ruleTitle" : "hintTitle");
			L(Ui.referenceText).text = rule ? _curriculum.UnitFor(_session.Current.id).rule : _session.Current.hint;
			L(Ui.assistedStatus).text = _session.Assisted ? T("assisted") : string.Empty;
			Show(Ui.reference, true);
		}

		private void Check() => Submit(Input.value);

		private void Submit(string answer)
		{
			if (_busy || _session.Answered)
				return;

			if (!_session.Submit(answer, DateTime.UtcNow))
			{
				L(Ui.saveStatus).text = T("emptyAnswer");
				return;
			}
			E(Ui.answerControls).SetEnabled(false);
			Show(Ui.reference, false);
			Show(Ui.feedback, true);
			E(Ui.feedback).EnableInClassList("incorrect", !_session.Correct);
			L(Ui.feedbackTitle).text = T(_session.Correct ? "correct" : "incorrect");
			L(Ui.expectedAnswer).text = T("expected", _session.Current.answers[0]);
			L(Ui.explanation).text = _session.Current.explanation;
			Save();
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
			if (!_lesson)
				return;

			if (evt.keyCode == KeyCode.Escape)
				ShowHome();
			if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter)
				return;

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
			_root?.UnregisterCallback<KeyDownEvent>(OnKey, TrickleDown.TrickleDown);
			_root?.UnregisterCallback<GeometryChangedEvent>(OnGeometry);
		}
	}
}