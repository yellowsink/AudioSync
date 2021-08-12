using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using Avalonia.Interactivity;
using JetBrains.Annotations;

namespace AudioSync.Client.Views
{
	public partial class MainWindow
	{
		private void ShowMediaControls(bool show) => ((MainWindowViewModel) DataContext!).ShowMediaControls = show;

		private async Task Play()
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

			_barThread.Reset((int) await GetSecondsInSong(_queue.Songs[_queue.CurrentIndex]));

			Task.Factory.StartNew(_audioManager.Play).Wait();

			UpdateNowPlayingMetadata();
		}

		private void Pause()
		{
			if (_audioManager.IsPlaying) _audioManager.Pause();
			_barThread.Pause();
		}

		private void Stop()
		{
			if (_audioManager.Status != AudioManagerStatus.Idle) _audioManager.Stop();
			_audioManager.File = null;
			UpdateNowPlayingMetadata();
			_barThread.PauseToStart();
		}

		private async Task Next()
		{
			Stop();
			_queue.Next();
			await Play();
			UpdateUserStatus(_downloadThread?.CurrentlyDownloading);
		}

		private async Task Previous()
		{
			Stop();
			_queue.Previous();
			await Play();
			UpdateUserStatus(_downloadThread?.CurrentlyDownloading);
		}

		[UsedImplicitly]
		private async void ButtonPlay(object? sender = null, RoutedEventArgs e = null!) => await _syncAgent!.Play();

		[UsedImplicitly]
		private async void ButtonPause(object? sender = null, RoutedEventArgs e = null!) => await _syncAgent!.Pause();

		[UsedImplicitly]
		private async void ButtonStop(object? sender = null, RoutedEventArgs e = null!) => await _syncAgent!.Stop();
	}
}