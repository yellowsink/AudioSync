using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using AudioSync.Shared;
using Avalonia.Markup.Xaml;
using DynamicData;

namespace AudioSync.Client.Views
{
	public partial class MainWindow
	{
		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		private void RegisterSyncEventHandlers()
		{
			_syncClient!.TransportPlayEvent  += (_, _) => Play();
			_syncClient.TransportPauseEvent += (_, _) => Pause();
			_syncClient.TransportStopEvent  += (_, _) => Stop();

			_syncClient.QueueNextEvent     += (_, _) => Next();
			_syncClient.QueuePreviousEvent += (_, _) => Previous();

			_syncClient.UpdateUserEvent += (_, u) => UpdateUser(u);
			_syncClient.RemoveUserEvent += (_, u) => RemoveUser(u);

			_syncClient.QueueSetEvent   += (_, p) => SetQueue(p.Item2);
			_syncClient.QueueAddEvent   += (_, p) => AddSong(p.Item2);
			_syncClient.QueueClearEvent += (_, _) => SetQueue(new Queue());
		}
		
		private void RegisterDownloadEventHandlers()
		{
			_downloadThread!.StartDownloadEvent += (_, s) =>
			{
				UpdateUserStatus(s);
				UpdateDownloadQueue();
			};
			_downloadThread.FinishDownloadEvent     += (_, _) => UpdateDownloadQueue();
			_downloadThread.FinishAllDownloadsEvent += (_, _) => UpdateUserStatus(null);
		}

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
			_downloadThread = new DownloadThread(new DownloadManager(_cacheManager));
		}

		private async Task StartupTasks()
		{
			await RunToolDialog();
			await RunConnectDialog();
			// register event handlers from server
			RegisterSyncEventHandlers();

			((MainWindowViewModel) DataContext!).Users.Clear();
			foreach (var user in await _syncClient!.GetUsers())
				((MainWindowViewModel) DataContext!).Users.AddOrUpdate(user);

			RegisterDownloadEventHandlers();

#pragma warning disable 4014
			Task.Factory.StartNew(_downloadThread!.Run().Wait);
#pragma warning restore 4014
			
			UpdateCacheView();
		}
	}
}