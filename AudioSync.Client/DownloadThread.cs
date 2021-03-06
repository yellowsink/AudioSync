using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Shared;

namespace AudioSync.Client
{
	public class DownloadThread : IDisposable
	{
		private readonly DownloadManager _manager;
		private          bool            _running;
		private          bool            _stopQueued;

		public DownloadThread(DownloadManager manager) { _manager = manager; }
		public Song? CurrentlyDownloading { get; private set; }

		public List<Song> Queue { get; } = new();

		public void Dispose() { StopRun(); }

		public event EventHandler<Song> StartDownloadEvent      = (_, _) => { };
		public event EventHandler<Song> FinishDownloadEvent     = (_, _) => { };
		public event EventHandler       FinishAllDownloadsEvent = (_, _) => { };

		public async Task Run()
		{
			if (_running)
				throw new InvalidOperationException("The download thread is still running");

			_running = true;

			var sentAllFinishEvent = true;

			while (!_stopQueued)
			{
				if (Queue.Count == 0)
				{
					if (!sentAllFinishEvent)
						FinishAllDownloadsEvent.Invoke(this, EventArgs.Empty);
					sentAllFinishEvent = true;
					await Task.Delay(100);
					continue;
				}

				sentAllFinishEvent = false;

				var nextSong = CurrentlyDownloading = Queue[0];
				StartDownloadEvent.Invoke(this, nextSong);

				await _manager.DownloadSong(nextSong);

				CurrentlyDownloading = null;
				Queue.RemoveAt(0);
				FinishDownloadEvent.Invoke(this, nextSong);
			}

			_running = false;
		}

		public void StopRun() => _stopQueued = true;

		public void Enqueue(Song song) => Queue.Add(song);
	}
}