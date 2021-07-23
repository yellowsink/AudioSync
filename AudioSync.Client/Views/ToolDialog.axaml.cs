using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AudioSync.Client.Views
{
	public class ToolDialog : Window
	{
		public ToolManager ToolManager = new();

		public ToolDialog()
		{
			DataContext = new ToolDialogViewModel
			{
				EnableDone = ToolManager.Versions.Ytdl != null
			};

			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

#pragma warning disable 4014
			CheckForUpdates();
#pragma warning restore 4014
		}

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);


		private async void CheckForUpdates(object? sender = null, RoutedEventArgs e = null!)
		{
			((ToolDialogViewModel) DataContext!).Status        = "Checking for updates...";
			((ToolDialogViewModel) DataContext!).EnableCheck   = false;
			((ToolDialogViewModel) DataContext!).EnableInstall = false;

			var ytdlInstalled = ToolManager.Versions.Ytdl != null;
			((ToolDialogViewModel) DataContext!).Ytdl = ytdlInstalled
															? await ToolManager.YtdlUpToDate()
																  ? $"Up to date ({ToolManager.Versions.Ytdl})"
																  : $"New version available ({ToolManager.Versions.Ytdl})"
															: "Not installed";

			((ToolDialogViewModel) DataContext!).Status        = "Idle";
			((ToolDialogViewModel) DataContext!).EnableCheck   = true;
			((ToolDialogViewModel) DataContext!).EnableInstall = true;
		}

		private async void UpdateTools(object? sender = null, RoutedEventArgs e = null!)
		{
			((ToolDialogViewModel) DataContext!).Status        = "Installing YTDL...";
			((ToolDialogViewModel) DataContext!).EnableCheck   = false;
			((ToolDialogViewModel) DataContext!).EnableInstall = false;

			await ToolManager.UpdateYtdl();

			((ToolDialogViewModel) DataContext!).Status        = "Idle";
			((ToolDialogViewModel) DataContext!).EnableCheck   = true;
			((ToolDialogViewModel) DataContext!).EnableInstall = true;
			((ToolDialogViewModel) DataContext!).EnableDone    = ToolManager.Versions.Ytdl != null;

			CheckForUpdates();
		}

		private void CloseButton(object? sender, RoutedEventArgs e) => Close();
	}
}