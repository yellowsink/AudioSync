using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Shared;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicData;

namespace AudioSync.Client.Frontend
{
	public class MainWindow : Window
	{
		private Queue _queue = new();

		private SyncClient?  _syncClient;
		private AudioManager _audioManager = new();
		private CacheManager _cacheManager = new();

		public MainWindow()
		{
			DataContext = new MainWindowViewModel();

			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

#pragma warning disable 4014
			RunConnectDialog();
#pragma warning restore 4014


			// don't leave hanging connections to the server
			Closing += (_, _) => Task.Factory.StartNew(() => _syncClient?.Disconnect().Wait()).Wait();


			// TODO: REMOVE TEST DATA!!!!

#region Test Data - REMOVE ME!!!!

			AddSong(new Song("Start Again", "ONE OK ROCK", "https://soundcloud.com/oneokrock/start-again"));
			AddSong(new Song("SPARKS", "Takanashi Kiara", "https://open.spotify.com/track/46scODShYFATHbLfLE0dr1"));
			UpdateUser(new User("Test user 1"));
			UpdateUser(new User("Test user 2"));
			((MainWindowViewModel) DataContext).SongName   = "Start Again";
			((MainWindowViewModel) DataContext).ArtistName = "ONE OK ROCK";
			((MainWindowViewModel) DataContext).AlbumName  = "Ambitions";
			((MainWindowViewModel) DataContext).Format     = "MP3";

#endregion
		}

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		/// <summary>
		/// Shows the connect to server dialog and performs actions based on result
		/// </summary>
		private async Task RunConnectDialog()
		{
			var dialog = new ConnectDialog();
			await dialog.ShowDialog(this);

			if (dialog.SyncClient == null) Close(); // dialog was closed early

			_syncClient = dialog.SyncClient;

			if (_syncClient is { IsMaster: true }) ShowMediaControls();
		}

#region Media Controls

		private void ShowMediaControls()
		{
			var container = this.FindControl<StackPanel>("StackPanelMediaControls");

			var mediaControlPlay = new Button
			{
				Content = "⯈",
				Classes = { "MediaControl" }
			};
			mediaControlPlay.Click += Play;

			var mediaControlPause = new Button
			{
				Content = "┃┃",
				Classes = { "MediaControl" }
			};
			mediaControlPause.Click += Pause;

			var mediaControlStop = new Button
			{
				Content = "⯀",
				Classes = { "MediaControl" }
			};
			mediaControlStop.Click += Stop;

			container.Children.Add(mediaControlPlay);
			container.Children.Add(mediaControlPause);
			container.Children.Add(mediaControlStop);
		}

		private void Play(object? sender, RoutedEventArgs routedEventArgs)
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

			_audioManager.Play();
		}

		private void Pause(object? sender, RoutedEventArgs routedEventArgs)
		{
			if (_audioManager.IsPlaying) _audioManager.Pause();
		}

		private void Stop(object? sender, RoutedEventArgs routedEventArgs)
		{
			if (_audioManager.Status != AudioManagerStatus.Idle) _audioManager.Stop();
		}

#endregion

		// TODO: This is the crash button™️
		private void ButtonSettings_OnClick(object? sender, RoutedEventArgs e)
		{
			throw new System.NotImplementedException();
		}

#region Lists Management

		private void AddSong(Song song) => ((MainWindowViewModel) DataContext!).Songs.Add(song);

		private void RemoveSong(int index) => ((MainWindowViewModel) DataContext!).Songs.RemoveAt(index);

		private void UpdateUser(User user) => ((MainWindowViewModel) DataContext!).Users.AddOrUpdate(user);

		private void RemoveUser(string name) => ((MainWindowViewModel) DataContext!).Users.RemoveKey(name);

#endregion
	}
}