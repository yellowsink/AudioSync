using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using Avalonia.Interactivity;

namespace AudioSync.Client.Views
{
	public partial class MainWindow
	{
		private void ShowMediaControls(bool show) => ((MainWindowViewModel) DataContext!).ShowMediaControls = show;

		private void Play()
		{
			if (_audioManager.IsPlaying) return; // We're already playing, so do nothing

			// A file is not already loaded into the audioManager
			if (_audioManager.File == null)
			{
				if (_queue.Songs.Length == 0) return; // There's nothing in the queue to use

				var cachedSong = _cacheManager.GetFromCache(_queue.Songs[_queue.CurrentIndex]);
				if (!cachedSong.HasValue) return; // song not in cache, so do nothing

				_audioManager.File = cachedSong.Value.Item2;
			}

			Task.Factory.StartNew(_audioManager.Play);
		}

		private void Pause()
		{
			if (_audioManager.IsPlaying) _audioManager.Pause();
		}

		private void Stop()
		{
			if (_audioManager.Status != AudioManagerStatus.Idle) _audioManager.Stop();
			_audioManager.File = null;
		}

		private void Next()
		{
			Stop();
			_queue.Next();
			Play();
			UpdateUserStatus(_downloadThread?.CurrentlyDownloading);
		}

		private void Previous()
		{
			Stop();
			_queue.Previous();
			Play();
			UpdateUserStatus(_downloadThread?.CurrentlyDownloading);
		}

		private async void ButtonPlay(object?  sender = null, RoutedEventArgs e = null!) => await _syncClient!.Play();
		private async void ButtonPause(object? sender = null, RoutedEventArgs e = null!) => await _syncClient!.Pause();
		private async void ButtonStop(object?  sender = null, RoutedEventArgs e = null!) => await _syncClient!.Stop();
	}
}