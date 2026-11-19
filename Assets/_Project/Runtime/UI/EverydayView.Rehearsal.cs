using UnityEngine;
using UnityEngine.UIElements;

namespace LearnEnglish
{
	public sealed partial class EverydayView
	{
		private const int RecordingSeconds = 10;
		private const int RecordingSampleRate = 16000;
		private AudioClip _rehearsalClip;
		private AudioSource _rehearsalPlayer;
		private IVisualElementScheduledItem _recordingMonitor;
		private bool _recording;
		private int _recordedSamples;
		private float _recordingDeadline;

		private void BindRehearsal()
		{
			B(Ui.playRecording).text = T("playRecording");
			B(Ui.clearRecording).text = T("clearRecording");
			B(Ui.recordModel).clicked += ToggleRecording;
			B(Ui.playRecording).clicked += PlayRecording;
			B(Ui.clearRecording).clicked += ClearRehearsal;
			UpdateRecordingControls();
		}

		private void ToggleRecording()
		{
			if (_recording)
			{
				FinishRecording();
				return;
			}
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
			{
				L(Ui.recordingStatus).text = T("microphonePermission");
				UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
				return;
			}
			#endif
			ClearRehearsal();
			StopWordSpeech();
			if (Microphone.devices.Length == 0)
			{
				L(Ui.recordingStatus).text = T("microphoneUnavailable");
				return;
			}
			_rehearsalClip = Microphone.Start(null, false, RecordingSeconds, RecordingSampleRate);
			if (!_rehearsalClip)
			{
				L(Ui.recordingStatus).text = T("microphoneUnavailable");
				return;
			}
			_recording = true;
			_recordingDeadline = Time.realtimeSinceStartup + RecordingSeconds + 0.5f;
			L(Ui.recordingStatus).text = T("recordingActive");
			_recordingMonitor = _root.schedule.Execute(UpdateRecordingPosition).Every(100);
			UpdateRecordingControls();
		}

		private void UpdateRecordingPosition()
		{
			_recordedSamples = Mathf.Max(_recordedSamples, Microphone.GetPosition(null));
			if ((_recordedSamples > 0 && !Microphone.IsRecording(null)) ||
				Time.realtimeSinceStartup >= _recordingDeadline)
			{
				FinishRecording();
			}
		}

		private void FinishRecording()
		{
			_recordedSamples = Mathf.Max(_recordedSamples, Microphone.GetPosition(null));
			Microphone.End(null);
			_recordingMonitor?.Pause();
			_recording = false;
			L(Ui.recordingStatus).text = _recordedSamples > 0 ? string.Empty : T("microphoneUnavailable");
			UpdateRecordingControls();
		}

		private void PlayRehearsalModel()
		{
			if (_rehearsalPlayer)
			{
				_rehearsalPlayer.Stop();
			}
			SpeakWord(L(Ui.modelPhrase).text, Ui.modelStatus);
		}

		private void PlayRecording()
		{
			if (!_rehearsalClip || _recording || _recordedSamples <= 0)
			{
				return;
			}
			StopWordSpeech();
			if (!_rehearsalPlayer)
			{
				_rehearsalPlayer = gameObject.AddComponent<AudioSource>();
				_rehearsalPlayer.playOnAwake = false;
			}
			_rehearsalPlayer.Stop();
			_rehearsalPlayer.clip = _rehearsalClip;
			double start = AudioSettings.dspTime + 0.05;
			_rehearsalPlayer.PlayScheduled(start);
			_rehearsalPlayer.SetScheduledEndTime(start + (double)_recordedSamples / _rehearsalClip.frequency);
		}

		private void ClearRehearsal()
		{
			ReleaseRecording();
			L(Ui.recordingStatus).text = string.Empty;
			UpdateRecordingControls();
		}

		private void ReleaseRecording()
		{
			_recordingMonitor?.Pause();
			_recordingMonitor = null;
			if (_recording)
			{
				Microphone.End(null);
				_recording = false;
			}
			if (_rehearsalPlayer)
			{
				_rehearsalPlayer.Stop();
				_rehearsalPlayer.clip = null;
			}
			if (_rehearsalClip)
			{
				Destroy(_rehearsalClip);
				_rehearsalClip = null;
			}
			_recordedSamples = 0;
		}

		private void UpdateRecordingControls()
		{
			B(Ui.recordModel).text = T(_recording ? "stopRecording" : "recordModel");
			B(Ui.speakModel).SetEnabled(!_recording);
			B(Ui.playRecording).SetEnabled(!_recording && _recordedSamples > 0);
			B(Ui.clearRecording).SetEnabled(_rehearsalClip != null);
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused && _root != null)
			{
				ClearRehearsal();
				StopWordSpeech();
			}
		}
	}
}