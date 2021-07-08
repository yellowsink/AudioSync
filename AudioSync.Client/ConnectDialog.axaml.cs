using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.X11;

namespace AudioSync.Client
{
	public class ConnectDialog : Window
	{
		public SyncClient? SyncClient;

		public ConnectDialog()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		private void ButtonConnect_OnClick(object? sender, RoutedEventArgs e)
		{
			var statusBar      = this.FindControl<TextBlock>("StatusBar");
			var urlBox         = this.FindControl<TextBox>("TextBoxServerUrl");
			var nameBox        = this.FindControl<TextBox>("TextBoxName");
			var masterBox      = this.FindControl<CheckBox>("CheckBoxMaster");
			var connectButton = this.FindControl<Button>("ButtonConnect");

			try
			{
				SyncClient = new SyncClient(urlBox.Text, nameBox.Text, masterBox.IsChecked ?? false);
				
				urlBox.IsEnabled        = false;
				nameBox.IsEnabled       = false;
				masterBox.IsEnabled     = false;
				connectButton.IsEnabled = false;
				

				// I have to do it like this so that it runs on another thread, and it doesnt hang
				Task.Factory.StartNew(() => SyncClient.Connect().Wait()).Wait();
				Close();
			}
			catch (AggregateException)
			{
				statusBar.Text          = "Error connecting. Check URL is correct";
				urlBox.IsEnabled        = true;
				nameBox.IsEnabled       = true;
				masterBox.IsEnabled     = true;
				connectButton.IsEnabled = true;
				SyncClient              = null;
			}
		}
	}
}