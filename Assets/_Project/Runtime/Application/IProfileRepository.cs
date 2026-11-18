using LearnEnglish.Core;
using UnityEngine;

namespace LearnEnglish
{
	public interface IProfileRepository
	{
		Awaitable<Profile> LoadAsync();
		Awaitable SaveAsync(Profile snapshot);
	}
}
