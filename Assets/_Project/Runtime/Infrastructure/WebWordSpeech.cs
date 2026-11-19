using System;
using UnityEngine;
using UnityEngine.Networking;

namespace LearnEnglish
{
	public sealed class WebWordSpeech : IWordSpeech, IDisposable
	{
		private const string Endpoint = "https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q=";
		private readonly AudioSource _player;
		private UnityWebRequest _request;
		private AudioClip _clip;
		private string _term;

		public WebWordSpeech(Transform owner)
		{
			var host = new GameObject("Vocabulary pronunciation");
			host.transform.SetParent(owner, false);
			_player = host.AddComponent<AudioSource>();
			_player.playOnAwake = false;
		}

		public async Awaitable<bool> SpeakAsync(string term)
		{
			Stop();
			if (!_player || string.IsNullOrWhiteSpace(term) || term.Length > 80)
			{
				return false;
			}
			if (_clip && term == _term)
			{
				_player.PlayOneShot(_clip);
				return true;
			}

			using var request = UnityWebRequestMultimedia.GetAudioClip(Endpoint + Uri.EscapeDataString(term), AudioType.MPEG);
			_request = request;
			request.timeout = 7;
			try
			{
				var operation = request.SendWebRequest();
				while (!operation.isDone && _request == request)
				{
					await Awaitable.NextFrameAsync();
				}
				if (_request != request || !_player || request.result != UnityWebRequest.Result.Success)
				{
					return false;
				}

				var clip = DownloadHandlerAudioClip.GetContent(request);
				if (!clip)
				{
					return false;
				}
				if (_clip)
				{
					UnityEngine.Object.Destroy(_clip);
				}
				_clip = clip;
				_term = term;
				_player.PlayOneShot(clip);
				return true;
			}
			finally
			{
				if (_request == request)
				{
					_request = null;
				}
			}
		}

		public void Stop()
		{
			var request = _request;
			_request = null;
			request?.Abort();
			if (_player)
			{
				_player.Stop();
			}
		}

		public void Dispose()
		{
			Stop();
			if (_clip)
			{
				UnityEngine.Object.Destroy(_clip);
				_clip = null;
			}
			if (_player)
			{
				UnityEngine.Object.Destroy(_player.gameObject);
			}
		}
	}
}
