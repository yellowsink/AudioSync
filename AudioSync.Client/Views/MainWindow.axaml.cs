using System;
using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using AudioSync.Shared;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Xabe.FFmpeg;

namespace AudioSync.Client.Views
{
	public partial class MainWindow : Window
	{
		private readonly AudioManager           _audioManager   = new();
		private readonly CacheManager           _cacheManager   = new();
		private readonly HistoryManager         _historyManager = new();
		private readonly Config                 _config;
		private readonly DiscordPresenceManager _presenceManager;
		private          SongProgressBarThread  _barThread = null!;
		private          DownloadThread        _downloadThread = null!;
		private          Queue                  _queue = new();
		private          ISyncAgent?            _syncAgent;

		private ToolManager? _toolManager;

		public MainWindow()
		{
			// Avalonia stuff
			DataContext = new MainWindowViewModel();
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

			// Init
			_config          = Config.Load();
			_presenceManager = new DiscordPresenceManager(_config.DiscordPresenceAppId);

			// don't leave hanging connections to the server
			// ReSharper disable once AsyncVoidLambda
			Closing += async (_, _) =>
			{
				if (_syncAgent != null) await _syncAgent.Disconnect();
				Stop();
				_cacheManager.Dispose(_config.CacheDaysThreshold);
				_config.Save();
			};

#pragma warning disable 4014
			StartupTasks();
#pragma warning restore 4014
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

			if (string.IsNullOrWhiteSpace(song) || string.IsNullOrWhiteSpace(artist) || string.IsNullOrWhiteSpace(url))
				return;

			if (string.IsNullOrWhiteSpace(album))
				album = null;

			AddSong(new Song(song, artist, album, url));
		}

		private void UpdateUserStatus(Song? currentlyDownloading)
		{
			if (currentlyDownloading == null)
				Task.Factory.StartNew(_syncAgent!.SetStatus(UserStatus.Ready).Wait);
			else if (currentlyDownloading.Equals(_queue.Songs[_queue.CurrentIndex]))
				Task.Factory.StartNew(_syncAgent!.SetStatus(UserStatus.DownloadingCurrentSong).Wait);
			else
				Task.Factory.StartNew(_syncAgent!.SetStatus(UserStatus.DownloadingSongs).Wait);
		}

		public async Task<double> GetSecondsInSong(Song song)
		{
			var cacheItem = _cacheManager.GetFromCache(song);
			if (cacheItem == null)
				throw new Exception("Song is not in cache");
			var file   = cacheItem.Value.Item2;
			var length = (await FFmpeg.GetMediaInfo(file.FullName)).Duration;

			return length.TotalSeconds;
		}
	}
}