using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AudioSync.Client
{
	public class MainWindow : Window
	{
		private SyncClient? _syncClient;
		private bool        _syncClientReady;
		
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

			RunConnectDialog();
		}

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		private async Task RunConnectDialog()
		{
			var dialog = new ConnectDialog();
			await dialog.ShowDialog(this);
			
			if (dialog.SyncClient == null) Close(); // dialog was closed early
			
			_syncClient      = dialog.SyncClient;
			_syncClientReady = true;
		}
	}
}