using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Shared;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AudioSync.Client.Frontend
{
	public class MainWindow : Window
	{
		private Queue _queue = new();
		
		private SyncClient?  _syncClient;
		private AudioManager _audioManager = new();
		private CacheManager _cacheManager = new();

		private Button? _mediaControlPlay;
		private Button? _mediaControlPause;
		private Button? _mediaControlStop;

		private List<Song>               Songs = new();
		private Dictionary<string, User> Users = new();

		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

#pragma warning disable 4014
			RunConnectDialog();
#pragma warning restore 4014


			// don't leave hanging connections to the server
			Closing += (_, _)
				=> Task.Factory.StartNew(() => _syncClient?.Disconnect().Wait()).Wait();
			
			
			// TODO: REMOVE TEST DATA!!!!
			AddSong(new Song("Start Again", "ONE OK ROCK",     "https://soundcloud.com/oneokrock/start-again"));
			AddSong(new Song("SPARKS",      "Takanashi Kiara", "https://open.spotify.com/track/46scODShYFATHbLfLE0dr1"));
			UpdateUser(new User("Test user 1"));
			UpdateUser(new User("Test user 2"));
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
			
			_syncClient      = dialog.SyncClient;

			if (_syncClient is { IsMaster: true }) ShowMediaControls();
		}

		#region Media Controls

		private void ShowMediaControls()
		{
			var container = this.FindControl<StackPanel>("StackPanelMediaControls");

			_mediaControlPlay = new Button
			{
				Content = "⯈",
				Classes = { "MediaControl" }
			};
			_mediaControlPlay.Click += Play;

			_mediaControlPause = new Button
			{
				Content = "┃┃",
				Classes = { "MediaControl" }
			};
			_mediaControlPause.Click += Pause;
			
			_mediaControlStop = new Button
			{
				Content = "⯀",
				Classes = { "MediaControl" }
			};
			_mediaControlStop.Click += Stop;
			
			container.Children.Add(_mediaControlPlay);
			container.Children.Add(_mediaControlPause);
			container.Children.Add(_mediaControlStop);
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

		private void AddSong(Song song)
		{
			Songs.Add(song);
			var songList = this.FindControl<DataGrid>("DataGridSongList");
			songList.Items = Songs;
		}

		private void RemoveSong(int index)
		{
			Songs.RemoveAt(index);
			var songList = this.FindControl<DataGrid>("DataGridSongList");
			songList.Items = Songs;
		}

		private void UpdateUser(User user)
		{
			Users[user.Name] = user;
			var userList = this.FindControl<DataGrid>("DataGridUserList");
			userList.Items = Users.Keys;
		}

		private void RemoveUser(string name)
		{
			Users.Remove(name);
			var userList = this.FindControl<DataGrid>("DataGridUserList");
			userList.Items = Users.Keys;
		}

		#endregion
	}
}