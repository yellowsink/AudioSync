using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Shared;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
		
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

#pragma warning disable 4014
			RunConnectDialog();
#pragma warning restore 4014
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
			var header = this.FindControl<StackPanel>("StackPanelHeader");

			_mediaControlPlay = new Button
			{
				Content                    = "⯈",
				FontSize                   = 20,
				Width                      = 50,
				Height                     = 50,
				VerticalContentAlignment   = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center
			};
			_mediaControlPlay.Click += Play;

			_mediaControlPause = new Button
			{
				Content                    = "┃┃",
				FontSize                   = 20,
				Width                      = 50,
				Height                     = 50,
				VerticalContentAlignment   = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center
			};
			_mediaControlPause.Click += Pause;
			
			_mediaControlStop = new Button
			{
				Content                    = "⯀",
				FontSize                   = 20,
				Width                      = 50,
				Height                     = 50,
				VerticalContentAlignment   = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center
			};
			_mediaControlStop.Click += Stop;
			
			header.Children.Insert(0, _mediaControlPlay);
			header.Children.Insert(1, _mediaControlPause);
			header.Children.Insert(2, _mediaControlStop);
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
	}
}