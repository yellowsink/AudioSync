using System;
using System.Net.Http;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AudioSync.Client.Views
{
	public class ConnectDialog : Window
	{
		public ISyncAgent? SyncAgent;

		public ConnectDialog()
		{
			DataContext = new ConnectDialogViewModel
			{
				Status          = "Idle",
				ControlsEnabled = true
			};

			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		private async void ButtonConnect_OnClick(object? sender, RoutedEventArgs e)
		{
			var vm = (ConnectDialogViewModel) DataContext!;

			vm.ControlsEnabled = false;

			try
			{
				SyncAgent = new ServerSyncAgent(vm.InputServerUrl, vm.InputUserName, vm.InputTryBeMaster);

				await SyncAgent.Connect();
				Close();
			}
			catch (HttpRequestException)
			{ // Could not connect
				vm.Status = "Error connecting. Check URL is correct";
			}
			catch (UriFormatException)
			{ // The URL wasn't valid
				vm.Status = "Please enter a valid URL";
			}
			catch (Exception)
			{
				// ignored
			}

			// if we're here, something went wrong
			vm.ControlsEnabled = true;
			SyncAgent          = null;
		}

		private void ButtonOffline_OnClick(object? sender, RoutedEventArgs e)
		{
			SyncAgent = new OfflineSyncAgent();
			Close();
		}
	}
}