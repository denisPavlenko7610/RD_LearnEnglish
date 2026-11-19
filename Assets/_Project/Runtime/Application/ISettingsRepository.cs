using LearnEnglish.Core;
using UnityEngine;

namespace LearnEnglish
{
	public interface ISettingsRepository
	{
		Awaitable<AppSettings> LoadAsync();
		Awaitable SaveAsync(AppSettings snapshot);
	}
}
