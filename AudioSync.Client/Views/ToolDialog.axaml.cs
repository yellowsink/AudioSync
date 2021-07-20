using System.Threading.Tasks;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AudioSync.Client.Views
{
	public class ToolDialog : Window
	{
		public ToolManager ToolManager = new();

		public ToolDialog()
		{
			DataContext = new ToolDialogViewModel();

			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif

#pragma warning disable 4014
			CheckForUpdates();
#pragma warning restore 4014
		}

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);


		private async Task CheckForUpdates()
		{
			((ToolDialogViewModel) DataContext!).Status = "Checking for updates...";	
			
			var ytdlInstalled = ToolManager.Versions.Ytdl != null;
			if (ytdlInstalled)
			{
				var ytdlUpToDate = await ToolManager.YtdlUpToDate();
				((ToolDialogViewModel) DataContext!).Ytdl = ytdlUpToDate
																? $"Up to date ({ToolManager.Versions.Ytdl})"
																: $"New version available ({ToolManager.Versions.Ytdl})";
			}
			else
			{
				((ToolDialogViewModel) DataContext!).Ytdl = "Not installed";
			}
			
			((ToolDialogViewModel) DataContext!).Status = string.Empty;
		}
	}
}