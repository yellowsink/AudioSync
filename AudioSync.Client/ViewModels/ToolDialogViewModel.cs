using ReactiveUI;

namespace AudioSync.Client.ViewModels
{
	public class ToolDialogViewModel : ViewModelBase
	{
		private bool   _enableCheck = true;
		private bool   _enableDone;
		private bool   _enableInstall = true;
		private string _status        = string.Empty;

		private string _ytdl = string.Empty;

		public string Status
		{
			get => _status;
			set => this.RaiseAndSetIfChanged(ref _status, value);
		}

		public string Ytdl
		{
			get => _ytdl;
			set => this.RaiseAndSetIfChanged(ref _ytdl, value);
		}

		public bool EnableCheck
		{
			get => _enableCheck;
			set => this.RaiseAndSetIfChanged(ref _enableCheck, value);
		}

		public bool EnableInstall
		{
			get => _enableInstall;
			set => this.RaiseAndSetIfChanged(ref _enableInstall, value);
		}

		public bool EnableDone
		{
			get => _enableDone;
			set => this.RaiseAndSetIfChanged(ref _enableDone, value);
		}
	}
}