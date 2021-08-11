using System.Linq;
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
			_syncAgent!.TransportPlayEvent += (_, _) => Play();
			_syncAgent.TransportPauseEvent += (_, _) => Pause();
			_syncAgent.TransportStopEvent  += (_, _) => Stop();

			_syncAgent.QueueNextEvent     += (_, _) => Next();
			_syncAgent.QueuePreviousEvent += (_, _) => Previous();

			_syncAgent.UpdateUserEvent += (_, u) => UpdateUser(u);
			_syncAgent.RemoveUserEvent += (_, u) => RemoveUser(u);

			_syncAgent.QueueSetEvent   += (_, p) => SetQueue(p.Item2);
			_syncAgent.QueueAddEvent   += (_, p) => AddSong(p.Item2);
			_syncAgent.QueueClearEvent += (_, _) => SetQueue(new Queue());
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

			if (dialog.SyncAgent == null) Close(); // dialog was closed early

			_syncAgent = dialog.SyncAgent;

			if (_syncAgent is { IsMaster: true }) ShowMediaControls(true);
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
			// register event handlers
			RegisterSyncEventHandlers();
			RegisterDownloadEventHandlers();
			// it no workey
			/*_audioManager.FinishedPlaying += async (_, _) =>
			{
				if (_queue.Songs.Length - 1 == _queue.CurrentIndex)
					Stop(); // no next song
				else
				{
					await _syncAgent!.Next();
					var users = await _syncAgent.GetUsers();
					if (users.All(u => u.Status != UserStatus.DownloadingCurrentSong))
						await _syncAgent.Play();
				}
			};*/

			((MainWindowViewModel) DataContext!).Users.Clear();
			foreach (var user in await _syncAgent!.GetUsers())
				((MainWindowViewModel) DataContext!).Users.AddOrUpdate(user);

#pragma warning disable 4014
			Task.Factory.StartNew(_downloadThread!.Run().Wait);
#pragma warning restore 4014

			UpdateCacheView();
			UpdateHistoryView();

			// show rich presence
			var serverAgent = _syncAgent as ServerSyncAgent ?? null;
			_presenceManager.UpdateStatus(serverAgent != null, serverAgent?.Url, serverAgent?.Name);
		}
	}
}