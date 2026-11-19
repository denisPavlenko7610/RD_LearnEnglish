using System;

namespace LearnEnglish.Core
{
	public static class AppThemes
	{
		public const string Midnight = "midnight";
		public const string Ocean = "ocean";
		public const string Forest = "forest";
		public const string Ember = "ember";
		public const string Daylight = "daylight";
		public const string Default = Midnight;
		public const string ClassPrefix = "theme-";

		public static string[] All => new[] { Midnight, Ocean, Forest, Ember, Daylight };

		public static bool IsKnown(string id) => id != null && Array.IndexOf(All, id) >= 0;

		public static string ClassName(string id) => ClassPrefix + id;
	}

	[Serializable]
	public sealed class AppSettings
	{
		public const int CurrentVersion = 1;
		public int version = CurrentVersion;
		public string theme = AppThemes.Default;

		public void Validate()
		{
			if (version != CurrentVersion || !AppThemes.IsKnown(theme))
				throw new ArgumentException("Unsupported or invalid settings.");
		}

		public AppSettings Copy() => (AppSettings)MemberwiseClone();
	}
}
