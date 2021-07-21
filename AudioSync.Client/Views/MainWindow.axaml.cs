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
		private readonly AudioManager     _audioManager = new();
		private readonly CacheManager     _cacheManager = new();
		private readonly Config           _config;
		private          Queue            _queue = new();
		private          DownloadManager? _downloadManager;
		private          SyncClient?      _syncClient;

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
			_config = Config.Load();

			// don't leave hanging connections to the server
			// ReSharper disable once AsyncVoidLambda
			Closing += async (_, _) =>
			{
				if (_syncClient != null) await _syncClient.Disconnect();
				Stop();
				_cacheManager.Dispose(_config?.CacheDaysThreshold);
				_config?.Save();
			};

#pragma warning disable 4014
			StartupTasks();
#pragma warning restore 4014
		}

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		private async Task RunConnectDialog()
		{
			var dialog = new ConnectDialog();
			await dialog.ShowDialog(this);

			if (dialog.SyncClient == null) Close(); // dialog was closed early

			_syncClient = dialog.SyncClient;

			if (_syncClient is { IsMaster: true }) ShowMediaControls(true);
		}

		private async Task RunToolDialog()
		{
			var dialog = new ToolDialog();
			await dialog.ShowDialog(this);

			_toolManager = dialog.ToolManager;
			if (_toolManager.Versions.Ytdl == null) Close(); // no ytdl
			_downloadManager = new DownloadManager(_cacheManager);
		}

		private async Task StartupTasks()
		{
			await RunToolDialog();
			await RunConnectDialog();
			// register event handlers from server
			RegisterHandlers();
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

			_audioManager.Play();
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
		}
		
		private void Previous()
		{
			Stop();
			_queue.Previous();
			Play();
		}

#endregion

#region Lists Management

		private void AddSong(Song song)
		{
			_queue.Add(song);
			((MainWindowViewModel) DataContext!).Songs.Add(song);
		}

		private void RemoveSong(int index)
		{
			_queue.Remove(index);
			((MainWindowViewModel) DataContext!).Songs.RemoveAt(index);
		}

		private void SetQueue(Queue queue)
		{
			_queue = queue;
			
			((MainWindowViewModel) DataContext!).Songs.Clear();
			((MainWindowViewModel) DataContext!).Songs.AddRange(_queue.Songs);
		}

		private void UpdateUser(User user) => ((MainWindowViewModel) DataContext!).Users.AddOrUpdate(user);

		private void RemoveUser(string name) => ((MainWindowViewModel) DataContext!).Users.RemoveKey(name);

#endregion

		private void RegisterHandlers()
		{
			_syncClient!.TransportPlayEvent  += (_, _) => Play();
			_syncClient!.TransportPauseEvent += (_, _) => Pause();
			_syncClient!.TransportStopEvent  += (_, _) => Stop();

			_syncClient!.QueueNextEvent     += (_, _) => Next();
			_syncClient!.QueuePreviousEvent += (_, _) => Previous();
			
			_syncClient!.UpdateUserEvent += (_, u) => UpdateUser(u);
			_syncClient!.RemoveUserEvent += (_, u) => RemoveUser(u);

			_syncClient!.QueueSetEvent += (_, p) => SetQueue(p.Item2);
			_syncClient!.QueueAddEvent += (_, p) => AddSong(p.Item2);
			_syncClient!.QueueClearEvent += (_, _) => SetQueue(new Queue());
		}
	}
}