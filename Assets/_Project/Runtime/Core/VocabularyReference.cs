using System;

namespace LearnEnglish.Core
{
	[Serializable]
	public sealed class VocabularyReference
	{
		public VocabularyReferenceEntry[] entries;
	}

	[Serializable]
	public sealed class VocabularyReferenceEntry
	{
		public string term;
		public string pos;
		public string level;
		public int rank;
		public string meaning;
	}
}
