using System;
using ReactiveUI;

namespace AudioSync.Client.ViewModels
{
	public class ToolDialogViewModel : ViewModelBase
	{
		private string _status = string.Empty;

		public string Status
		{
			get => _status;
			set => this.RaiseAndSetIfChanged(ref _status, value);
		}
		
		private string _ytdl = string.Empty;

		public string Ytdl
		{
			get => _ytdl;
			set => this.RaiseAndSetIfChanged(ref _ytdl, value);
		}
	}
}