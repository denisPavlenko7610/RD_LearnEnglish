using UnityEngine;

namespace LearnEnglish
{
	public interface IWordSpeech
	{
		Awaitable<bool> SpeakAsync(string term);
		void Stop();
	}
}
