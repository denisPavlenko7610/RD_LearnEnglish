using System;
using System.IO;
using LearnEnglish.Core;
using RD.SceneFlow;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace LearnEnglish.Editor
{
	public static class EnglishProjectSetup
	{
		public const string BootScene = "Assets/_Project/Scenes/Boot.unity";
		public const string AppScene = "Assets/_Project/Scenes/App.unity";
		private const string UiRoot = "Assets/_Project/Runtime/UI/";
		private const string PanelPath = "Assets/_Project/Settings/EverydayPanel.asset";
		private const string CatalogPath = "Assets/_Project/Config/SceneCatalog.asset";

		public static void BuildAndroidPilot()
		{
			string[] arguments = Environment.GetCommandLineArgs();
			int outputIndex = Array.IndexOf(arguments, "-buildOutput") + 1;
			if (outputIndex == 0 ||
				outputIndex >= arguments.Length ||
				!string.Equals(Path.GetExtension(arguments[outputIndex]), ".apk", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Pass an APK path with -buildOutput.");
			}

			string output = Path.GetFullPath(arguments[outputIndex]);
			Directory.CreateDirectory(Path.GetDirectoryName(output));
			string previousIdentifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
			bool previousBundle = EditorUserBuildSettings.buildAppBundle;
			try
			{
				PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.rd.learnenglish.pilot");
				EditorUserBuildSettings.buildAppBundle = false;
				var report = BuildPipeline.BuildPlayer(
					new BuildPlayerOptions
					{
						scenes = new[] { BootScene, AppScene },
						target = BuildTarget.Android,
						locationPathName = output,
						options = BuildOptions.Development
					}
				);
				if (report.summary.result != BuildResult.Succeeded)
				{
					throw new BuildFailedException($"Android pilot build {report.summary.result}.");
				}
			}
			finally
			{
				PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, previousIdentifier);
				EditorUserBuildSettings.buildAppBundle = previousBundle;
			}
		}

		[MenuItem("Learn English/Prepare Boot and App")]
		public static void Prepare()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play mode before setup.");
			if (EditorSceneManager.GetActiveScene().isDirty)
				throw new InvalidOperationException("Save the current scene before setup.");

			foreach (string folder in new[] { "Scenes", "Settings", "Config" })
				Directory.CreateDirectory("Assets/_Project/" + folder);
			AssetDatabase.Refresh();
			var curriculum =
				JsonUtility.FromJson<Curriculum>(Resources.Load<TextAsset>(EnglishBootstrap.CurriculumResource).text);
			curriculum.Validate();
			PlayerSettings.productName = "English Everyday";
			PlayerSettings.defaultScreenWidth = 1280;
			PlayerSettings.defaultScreenHeight = 720;
			PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
			PlayerSettings.resizableWindow = true;
			PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
			PlayerSettings.allowedAutorotateToPortrait = false;
			PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
			PlayerSettings.allowedAutorotateToLandscapeLeft = true;
			PlayerSettings.allowedAutorotateToLandscapeRight = true;
			PlayerSettings.Android.renderOutsideSafeArea = false;
			var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
			if (!panel)
			{
				panel = ScriptableObject.CreateInstance<PanelSettings>();
				AssetDatabase.CreateAsset(panel, PanelPath);
			}
			panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
			panel.referenceResolution = new Vector2Int(1280, 720);
			panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
			panel.match = .5f;
			panel.themeStyleSheet = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(UiRoot + "Everyday.tss");
			EditorUtility.SetDirty(panel);
			var catalog = AssetDatabase.LoadAssetAtPath<SceneCatalog>(CatalogPath);
			if (!catalog)
			{
				catalog = ScriptableObject.CreateInstance<SceneCatalog>();
				AssetDatabase.CreateAsset(catalog, CatalogPath);
			}
			var serializedCatalog = new SerializedObject(catalog);
			var entries = serializedCatalog.FindProperty("_scenes");
			entries.arraySize = 2;
			for (int index = 0; index < 2; index++)
			{
				entries.GetArrayElementAtIndex(index).FindPropertyRelative("_id").stringValue =
					index == 0 ? "boot" : EnglishBootstrap.AppSceneId;
				entries.GetArrayElementAtIndex(index).FindPropertyRelative("_path").stringValue =
					index == 0 ? BootScene : AppScene;
			}
			serializedCatalog.ApplyModifiedPropertiesWithoutUndo();
			catalog.ThrowIfInvalid();
			EditorUtility.SetDirty(catalog);
			AssetDatabase.SaveAssets();
			PrepareScene(AppScene, "EnglishWorkspace", "Everyday.uxml", panel, catalog, false);
			PrepareScene(BootScene, "EnglishLifetime", "Boot.uxml", panel, catalog, true);
			EditorBuildSettings.scenes = new[]
				{ new EditorBuildSettingsScene(BootScene, true), new EditorBuildSettingsScene(AppScene, true) };
			EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScene);
			AssetDatabase.SaveAssets();
			Debug.Log("English Everyday Boot/App prepared.");
		}

		private static void PrepareScene(
			string path,
			string ownerName,
			string uxml,
			PanelSettings panel,
			SceneCatalog catalog,
			bool boot
		)
		{
			var scene = File.Exists(path)
				? EditorSceneManager.OpenScene(path)
				: EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
			catalog = AssetDatabase.LoadAssetAtPath<SceneCatalog>(CatalogPath);
			GameObject owner = null;
			foreach (var candidate in scene.GetRootGameObjects())
				if (candidate.name == ownerName)
					owner = candidate;
			owner = owner ? owner : new GameObject(ownerName);
			var document = owner.GetComponent<UIDocument>() ?? owner.AddComponent<UIDocument>();
			document.panelSettings = panel;
			document.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UiRoot + uxml);
			if (boot)
			{
				var bootstrap = owner.GetComponent<EnglishBootstrap>() ?? owner.AddComponent<EnglishBootstrap>();
				var serialized = new SerializedObject(bootstrap);
				serialized.FindProperty("_scenes").objectReferenceValue = catalog;
				serialized.ApplyModifiedPropertiesWithoutUndo();
			}
			else if (!owner.GetComponent<EverydayView>())
				owner.AddComponent<EverydayView>();
			var camera = UnityEngine.Object.FindAnyObjectByType<Camera>();
			if (!camera)
				camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = new Color32(245, 242, 233, 255);
			camera.orthographic = true;
			camera.transform.position = new Vector3(0, 0, -10);
			EditorSceneManager.SaveScene(scene, path);
		}
	}
}