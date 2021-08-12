using System;
using System.Threading.Tasks;

namespace AudioSync.Client
{
	public class SongProgressBarThread
	{
		public SongProgressBarThread(Action<int, int> updateProgress)
		{
			_updateProgress = updateProgress;
		}
		
		private readonly Action<int, int> _updateProgress;
		private          bool             _running;
		private          bool             _stop;
		private          bool             _paused;
		private          bool             _restartSong;
		private          int              _songSeconds;

		public async Task Run()
		{
			if (_running)
				throw new InvalidOperationException("The progress bar thread is still running");
			
			_running = true;

			var count               = 0;
			var songNeedsRestarting = false;
			while (!_stop)
			{
				if (_restartSong)
				{
					count               = 0;
					songNeedsRestarting = false;
					_restartSong        = false;
				}
				if (count == _songSeconds)
					songNeedsRestarting = true;
				
				if (!_paused && !songNeedsRestarting)
					_updateProgress(count++, _songSeconds);
				
				await Task.Delay(1000);
			}

			_running = false;
			_stop    = false;
		}

		public void Stop()   => _stop = true;
		public void Pause()  => _paused = true;
		public void Resume() => _paused = false;
		public void PauseToStart()
		{
			Pause();
			_updateProgress(0, _songSeconds);
		}

		public void Reset(int songSeconds)
		{
			_songSeconds    = songSeconds;
			_restartSong = true;
		}
	}
}