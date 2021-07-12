using ReactiveUI;

namespace AudioSync.Client.ViewModels
{
	public class ConnectDialogViewModel : ViewModelBase
	{
		private bool   _controlsEnabled;
		private string _status = null!;

		public string Status
		{
			get => _status;
			set => this.RaiseAndSetIfChanged(ref _status, value);
		}

		public string InputServerUrl   { get; set; } = null!;
		public string InputUserName    { get; set; } = null!;
		public bool   InputTryBeMaster { get; set; }

		public bool ControlsEnabled
		{
			get => _controlsEnabled;
			set => this.RaiseAndSetIfChanged(ref _controlsEnabled, value);
		}
	}
}