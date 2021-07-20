using System;
using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using AudioSync.Shared;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicData;

namespace AudioSync.Client.Views
{
	public class MainWindow : Window
	{
		private readonly AudioManager _audioManager = new();
		private readonly CacheManager _cacheManager = new();
		private readonly Queue        _queue        = new();
		private readonly Config       _config;

		private SyncClient? _syncClient;

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

			_config = Config.Load();


			// don't leave hanging connections to the server
			Closing += async (_, _) =>
			{
				if (_syncClient != null) await _syncClient.Disconnect();
				Stop();
				_cacheManager.Dispose(_config.CacheDaysThreshold);
				_config.Save();
			};


			// TODO: REMOVE TEST DATA!!!!

#region Test Data - REMOVE ME!!!!

			AddSong(new Song("Start Again", "ONE OK ROCK", "Ambitions",
							 "https://soundcloud.com/oneokrock/start-again"));
			AddSong(new Song("SPARKS", "Takanashi Kiara", "SPARKS - Single",
							 "https://open.spotify.com/track/46scODShYFATHbLfLE0dr1"));
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
		///     Shows the connect to server dialog and performs actions based on result
		/// </summary>
		private async Task RunConnectDialog()
		{
			var dialog = new ConnectDialog();
			await dialog.ShowDialog(this);

			if (dialog.SyncClient == null) Close(); // dialog was closed early

			_syncClient = dialog.SyncClient;

			if (_syncClient is { IsMaster: true }) ShowMediaControls(true);
		}

		// TODO: This is the crash button™️
		private void ButtonSettings_OnClick(object? sender, RoutedEventArgs e) { throw new NotImplementedException(); }

		private void ButtonAddSong_OnClick(object? sender, RoutedEventArgs e)
		{
			var vm     = (MainWindowViewModel) DataContext!;
			var song   = vm.InputAddSong;
			var artist = vm.InputAddArtist;
			var album  = vm.InputAddAlbum;
			var url    = vm.InputAddUrl;

			if (string.IsNullOrWhiteSpace(song)  || string.IsNullOrWhiteSpace(artist) ||
				string.IsNullOrWhiteSpace(album) || string.IsNullOrWhiteSpace(url))
				return;

			AddSong(new Song(song, artist, album, url));
		}

#region Media Controls

		private void ShowMediaControls(bool show) => ((MainWindowViewModel) DataContext!).ShowMediaControls = show;

		private void Play(object? sender = null, RoutedEventArgs routedEventArgs = null!)
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

		private void Pause(object? sender = null, RoutedEventArgs routedEventArgs = null!)
		{
			if (_audioManager.IsPlaying) _audioManager.Pause();
		}

		private void Stop(object? sender = null, RoutedEventArgs routedEventArgs = null!)
		{
			if (_audioManager.Status != AudioManagerStatus.Idle) _audioManager.Stop();
		}

#endregion

#region Lists Management

		private void AddSong(Song song) => ((MainWindowViewModel) DataContext!).Songs.Add(song);

		private void RemoveSong(int index) => ((MainWindowViewModel) DataContext!).Songs.RemoveAt(index);

		private void UpdateUser(User user) => ((MainWindowViewModel) DataContext!).Users.AddOrUpdate(user);

		private void RemoveUser(string name) => ((MainWindowViewModel) DataContext!).Users.RemoveKey(name);

#endregion
	}
}