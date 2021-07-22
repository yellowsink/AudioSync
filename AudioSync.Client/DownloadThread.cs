using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Shared;

namespace AudioSync.Client
{
	public class DownloadThread : IDisposable
	{
		private bool            _stopQueued;
		private bool            _running;
		private DownloadManager _manager;

		public event EventHandler<Song> StartDownloadEvent      = (_, _) => { };
		public event EventHandler<Song> FinishDownloadEvent     = (_, _) => { };
		public event EventHandler       FinishAllDownloadsEvent = (_, _) => { };

		public DownloadThread(DownloadManager manager) { _manager = manager; }

		public List<Song> Queue { get; } = new();

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
				
				var nextSong = Queue[0];
				StartDownloadEvent.Invoke(this, nextSong);
				
				await _manager.DownloadSong(nextSong);
				
				Queue.RemoveAt(0);
				FinishDownloadEvent.Invoke(this, nextSong);
			}

			_running = false;
		}

		public void StopRun() => _stopQueued = true;

		public void Enqueue(Song song) => Queue.Add(song);

		public void Dispose()
		{
			StopRun();
		}
	}
}