using System;
using System.Collections.Generic;
using System.IO;
using LearnEnglish.Core;
using RD.Save;
using UnityEngine;

namespace LearnEnglish
{
	public sealed class ProfileRepository : IProfileRepository
	{
		public const string Slot = "adult-english-v1";
		private readonly SaveStore _store;
		private readonly Curriculum _curriculum;

		public ProfileRepository(SaveStore store, Curriculum curriculum)
		{
			_store = store;
			_curriculum = curriculum;
		}

		public async Awaitable<Profile> LoadAsync()
		{
			try
			{
				var document = await _store.LoadAsync<ProfileDocument>(
					Slot,
					data =>
					{
						try
						{
							if (data == null)
								throw new ArgumentException("Empty profile.");

							data.ToProfile().Validate(_curriculum);
						}
						catch (ArgumentException error)
						{
							throw new InvalidSaveDataException(
								"Invalid English profile; original files preserved.",
								error
							);
						}
					}
				);
				return document.ToProfile();
			}
			catch (FileNotFoundException)
			{
				return new Profile();
			}
		}

		public async Awaitable SaveAsync(Profile snapshot)
		{
			snapshot.Validate(_curriculum);
			var detached = snapshot.Copy();
			await _store.SaveAsync(
				Slot,
				new ProfileDocument
				{
					version = detached.version, attempts = detached.attempts, lastUnit = detached.lastUnit,
					items = detached.items, recentReviews = detached.recentReviews
				}
			);
		}
	}

	[Serializable]
	public sealed class ProfileDocument
	{
		public int version;
		public int attempts;
		public string lastUnit;
		public List<ExerciseProgress> items;
		public List<bool> recentReviews;

		public Profile ToProfile() => new()
		{
			version = version, attempts = attempts, lastUnit = lastUnit, items = items, recentReviews = recentReviews
		};
	}
}