using UnityEditor;
using UnityEditor.SceneManagement;

namespace LearnEnglish.Editor
{
	[InitializeOnLoad]
	public static class PlayFromBoot
	{
		static PlayFromBoot()
		{
			EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EnglishProjectSetup.BootScene);
		}
	}
}
