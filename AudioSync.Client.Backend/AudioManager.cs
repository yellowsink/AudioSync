using System;
using System.IO;
using System.Threading.Tasks;
using NetCoreAudio;

namespace AudioSync.Client.Backend
{
	/// <summary>
	///     Manages the music thread and keeps everything in check
	/// </summary>
	public class AudioManager
	{
		private readonly Player _player = new();

		public FileInfo? File;

		public bool IsPlaying => _player.Playing;
		public bool IsPaused  => _player.Paused;

		public AudioManagerStatus Status
			=> IsPlaying
				   ? AudioManagerStatus.Playing
				   : IsPaused
					   ? AudioManagerStatus.Paused
					   : AudioManagerStatus.Idle;

		public Task Play()
		{
			if (File == null) throw new InvalidOperationException("You must set a file before playing audio");
			if (IsPlaying) throw new InvalidOperationException("Cannot play audio when audio is already playing");

			return (IsPaused ? _player.Resume() : _player.Play(File.FullName)).RunOnNewThread();
		}

		public Task Pause()
		{
			if (!IsPlaying) throw new InvalidOperationException("Cannot pause unless audio is playing");

			return _player.Pause().RunOnNewThread();
		}

		public Task Stop()
		{
			if (Status == AudioManagerStatus.Idle) throw new InvalidOperationException("Cannot stop when idle");

			return _player.Stop().RunOnNewThread();
		}

		public void OnFinishedPlaying(Action<object, EventArgs> action)
			=> _player.PlaybackFinished += (_, args) => action(this, args);
	}

	public enum AudioManagerStatus
	{
		Playing,
		Paused,
		Idle
	}
}