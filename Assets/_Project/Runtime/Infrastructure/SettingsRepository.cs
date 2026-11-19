using System;
using System.IO;
using LearnEnglish.Core;
using RD.Save;
using UnityEngine;

namespace LearnEnglish
{
	public sealed class SettingsRepository : ISettingsRepository
	{
		public const string Slot = "settings-v1";
		private readonly SaveStore _store;
		private bool _preserveStoredFile;

		public SettingsRepository(SaveStore store)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}

		public async Awaitable<AppSettings> LoadAsync()
		{
			try
			{
				var stored = await _store.LoadAsync<AppSettings>(
					Slot,
					data =>
					{
						try
						{
							if (data == null)
								throw new ArgumentException("Empty settings.");

							data.Validate();
						}
						catch (ArgumentException error)
						{
							throw new InvalidSaveDataException(
								"Invalid settings; original files preserved.",
								error
							);
						}
					}
				);
				return stored.Copy();
			}
			catch (FileNotFoundException)
			{
				return new AppSettings();
			}
			catch (InvalidSaveDataException error)
			{
				Debug.LogWarning("English settings were unreadable and are preserved unchanged: " + error.Message);
				_preserveStoredFile = true;
				return new AppSettings();
			}
		}

		public async Awaitable SaveAsync(AppSettings snapshot)
		{
			if (_preserveStoredFile)
				throw new InvalidOperationException("Stored settings are unreadable and are preserved unchanged.");

			snapshot.Validate();
			await _store.SaveAsync(Slot, snapshot.Copy());
		}
	}
}
