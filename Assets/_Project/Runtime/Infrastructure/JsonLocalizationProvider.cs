using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using RD.Localization;
using UnityEngine;

namespace LearnEnglish
{
	[Serializable] public sealed class LocalizedEntry { public string key; public string text; }
	[Serializable] public sealed class LocalizedTable { public LocalizedEntry[] entries; }

	public sealed class JsonLocalizationProvider : ILocalizationProvider
	{
		private readonly Dictionary<string, string> _entries;
		public string CurrentLocale => "ru";
		public IReadOnlyCollection<string> AvailableLocales { get; } = Array.AsReadOnly(new[] { "ru" });
		public event Action<LocaleChanged> LocaleChanged { add { } remove { } }

		public JsonLocalizationProvider(LocalizedTable table)
		{
			if (table?.entries == null || table.entries.Any(x => x == null || string.IsNullOrWhiteSpace(x.key) || string.IsNullOrWhiteSpace(x.text)))
				throw new ArgumentException("Invalid localization table.");
			_entries = table.entries.ToDictionary(x => x.key, x => x.text);
		}

		public bool TryGet(string key, object[] arguments, out string text)
		{
			if (!_entries.TryGetValue(key, out text)) return false;
			if (arguments.Length > 0) text = string.Format(CultureInfo.InvariantCulture, text, arguments);
			return true;
		}

		public async Awaitable SetLocaleAsync(string locale, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (locale != CurrentLocale) throw new ArgumentException("Unsupported locale.");
			await Awaitable.MainThreadAsync();
		}
	}
}
