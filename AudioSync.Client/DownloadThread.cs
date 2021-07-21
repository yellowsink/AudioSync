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
		
		public DownloadThread(DownloadManager manager) { _manager = manager; }

		public List<Song> Queue { get; } = new();

		public async Task Run()
		{
			if (_running)
				throw new InvalidOperationException("The download thread is still running");
			
			_running = true;

			while (!_stopQueued)
			{
				if (Queue.Count == 0)
				{
					await Task.Delay(100);
					continue;
				}

				var nextSong = Queue[0];
				Queue.RemoveAt(0);

				await _manager.DownloadSong(nextSong);
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