using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AudioSync.Client
{
	public partial class MainWindow : Window
	{
		private SyncClient _syncClient;
		private bool       _syncClientReady;
		
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()                                    { AvaloniaXamlLoader.Load(this); }

		private void ButtonConnect_OnClick(object? sender, RoutedEventArgs e)
		{
			var urlBox         = this.FindControl<TextBox>("TextBoxServerUrl");
			var nameBox        = this.FindControl<TextBox>("TextBoxName");
			var masterBox      = this.FindControl<CheckBox>("CheckBoxMaster");
			var connnectButton = this.FindControl<Button>("ButtonConnect");

			_syncClient      = new(urlBox.Text, nameBox.Text, masterBox.IsChecked ?? false);
			_syncClientReady = true;
			
			urlBox.IsEnabled         = false;
			nameBox.IsEnabled        = false;
			masterBox.IsEnabled      = false;
			connnectButton.IsEnabled = false;
			
			// I have to do it like this so that it runs on another thread, and it doesnt hang
			Task.Factory.StartNew(async () => await _syncClient.Connect()).Wait();
		}
	}
}