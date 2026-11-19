using System;
using LearnEnglish.Core;
using UnityEngine;

namespace LearnEnglish
{
	public sealed class SettingsSession
	{
		private readonly ISettingsRepository _repository;
		private readonly AppSettings _settings;
		private bool _dirty;
		private bool _saving;

		public string Theme => _settings.theme;
		public bool HasPendingSave => _dirty || _saving;

		public SettingsSession(ISettingsRepository repository, AppSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			settings.Validate();
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_settings = settings.Copy();
		}

		public bool SelectTheme(string theme)
		{
			if (!AppThemes.IsKnown(theme))
				throw new ArgumentException("Unknown theme.", nameof(theme));

			if (_settings.theme == theme)
				return false;

			_settings.theme = theme;
			_dirty = true;
			return true;
		}

		public async Awaitable PersistAsync()
		{
			if (_saving)
				return;

			_saving = true;
			try
			{
				while (_dirty)
				{
					_dirty = false;
					try
					{
						await _repository.SaveAsync(_settings.Copy());
					}
					catch
					{
						_dirty = true;
						throw;
					}
				}
			}
			finally
			{
				_saving = false;
			}
		}
	}
}
