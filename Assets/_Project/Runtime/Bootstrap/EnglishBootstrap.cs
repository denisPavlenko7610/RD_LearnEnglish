using System;
using System.IO;
using System.Linq;
using LearnEnglish.Core;
using RD.DependencyInjection;
using RD.Localization;
using RD.Save;
using RD.SceneFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LearnEnglish
{
	[RequireComponent(typeof(UIDocument))]
	public sealed class EnglishBootstrap : MonoBehaviour
	{
		public const string AppSceneId = "app";
		public const string CurriculumResource = "Curriculum";
		private const string VocabularyResource = "VocabularyReference";
		public const string StringsResource = "UIStrings";
		public const string SaveFolder = "EnglishEveryday";
		[SerializeField] private SceneCatalog _scenes;
		[SerializeField] private string _saveDirectoryOverride;
		private Container _container;
		private LocalizationService _strings;
		private bool _initializing;
		private UIDocument _document;
		private Button _retry;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			_document = GetComponent<UIDocument>();
			_retry = _document.rootVisualElement.Q<Button>("bootRetry");
			_retry.clicked += Initialize;
		}

		private void Start() => Initialize();

		private async void Initialize()
		{
			if (_initializing)
				return;

			_initializing = true;
			_retry.SetEnabled(false);
			try
			{
				_container?.Dispose();
				var curriculum = JsonUtility.FromJson<Curriculum>(Resources.Load<TextAsset>(CurriculumResource).text);
				curriculum.vocabularyReference =
					JsonUtility.FromJson<VocabularyReference>(Resources.Load<TextAsset>(VocabularyResource).text);
				curriculum.Validate();
				var table = JsonUtility.FromJson<LocalizedTable>(Resources.Load<TextAsset>(StringsResource).text);
				var builder = new ContainerBuilder();
				builder.RegisterInstance(curriculum);
				builder.RegisterFactory(_ => new JsonLocalizationProvider(table), Lifetime.Singleton)
					.As<ILocalizationProvider>();
				builder.RegisterFactory(
					r => new LocalizationService(r.Resolve<ILocalizationProvider>()),
					Lifetime.Singleton
				);
				string directory = string.IsNullOrWhiteSpace(_saveDirectoryOverride)
					? Path.Combine(UnityEngine.Application.persistentDataPath, SaveFolder)
					: _saveDirectoryOverride;
				builder.RegisterFactory(_ => new SaveStore(directory), Lifetime.Singleton);
				builder.RegisterFactory(
					r => new ProfileRepository(r.Resolve<SaveStore>(), curriculum),
					Lifetime.Singleton
				).As<IProfileRepository>();
				builder.RegisterFactory(r => new SettingsRepository(r.Resolve<SaveStore>()), Lifetime.Singleton)
					.As<ISettingsRepository>();
				builder.RegisterFactory(_ => new RD.SceneFlow.SceneFlow(_scenes), Lifetime.Singleton).As<ISceneFlow>();
				if (Debug.isDebugBuild)
				{
					builder.RegisterFactory(_ => new WebWordSpeech(transform), Lifetime.Singleton).As<IWordSpeech>();
				}
				_container = builder.Build();
				_strings = _container.Resolve<LocalizationService>();
				await _strings.SetLocaleAsync("ru");
				if (!this)
					return;

				var settingsRepository = _container.Resolve<ISettingsRepository>();
				var settings = await settingsRepository.LoadAsync();
				if (!this)
					return;

				_document.rootVisualElement.Q(className: "app")?.AddToClassList(AppThemes.ClassName(settings.theme));
				_document.rootVisualElement.Q<Label>("bootLoading").text = _strings.Get("bootLoading");
				var repository = _container.Resolve<IProfileRepository>();
				var profile = await repository.LoadAsync();
				if (!this)
					return;

				await _container.Resolve<ISceneFlow>().ChangeSceneAsync(AppSceneId, destroyCancellationToken);
				if (!this)
					return;

				var scene = SceneManager.GetSceneByPath(_scenes.GetPath(AppSceneId));
				var view = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<EverydayView>(true))
					.Single();
				view.Construct(
					curriculum,
					repository,
					profile,
					_strings,
					new SettingsSession(settingsRepository, settings)
				);
				if (Debug.isDebugBuild)
				{
					view.ConfigureWordSpeech(_container.Resolve<IWordSpeech>());
				}
				_document.enabled = false;
			}
			catch (Exception error)
			{
				Debug.LogException(error);
				if (!this)
					return;

				var root = _document.rootVisualElement;
				var failure = root.Q<Label>("bootError");
				failure.text = _strings?.Get("bootError") ?? error.Message;
				failure.RemoveFromClassList("hidden");
				_retry.text = _strings?.Get("bootRetry") ?? "Retry";
				_retry.RemoveFromClassList("hidden");
				_retry.SetEnabled(true);
			}
			finally
			{
				_initializing = false;
			}
		}

		private void OnDestroy()
		{
			if (_retry != null)
				_retry.clicked -= Initialize;
			_container?.Dispose();
		}
	}
}