using System;
using System.Threading.Tasks;
using AudioSync.Client.Backend;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AudioSync.Client.Frontend
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
			var statusBar     = this.FindControl<TextBlock>("StatusBar");
			var urlBox        = this.FindControl<TextBox>("TextBoxServerUrl");
			var nameBox       = this.FindControl<TextBox>("TextBoxName");
			var masterBox     = this.FindControl<CheckBox>("CheckBoxMaster");
			var connectButton = this.FindControl<Button>("ButtonConnect");

			try
			{
				SyncClient = new SyncClient(urlBox.Text, nameBox.Text, masterBox.IsChecked ?? false);

				urlBox.IsEnabled        = false;
				nameBox.IsEnabled       = false;
				masterBox.IsEnabled     = false;
				connectButton.IsEnabled = false;


				// I have to run it on another thread, and the HTTP connection doesnt hang
				// SyncClient.Connect().RunOnNewThread().Wait(); // why the hell doesnt this work
				Task.Factory.StartNew(() => SyncClient.Connect().Wait()).Wait();
				Close();
			}
			catch (AggregateException)
			{ // Something went wrong in the Connect() thread
				statusBar.Text = "Error connecting. Check URL is correct";
			}
			catch (UriFormatException)
			{ // The URL wasn't valid
				statusBar.Text = "Please enter a valid URL";
			}
			catch (Exception)
			{
				// ignored
			}

			// if we're here, something went wrong
			urlBox.IsEnabled        = true;
			nameBox.IsEnabled       = true;
			masterBox.IsEnabled     = true;
			connectButton.IsEnabled = true;
			SyncClient              = null;
		}
	}
}