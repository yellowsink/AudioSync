using System.Collections.Generic;
using AudioSync.Shared;
using ReactiveUI;

namespace AudioSync.Client
{
	public class MainWindowViewModel : ReactiveObject
	{
		private readonly Backing _backing = new();

		public List<Song> Songs
		{
			get => _backing.Songs;
			set => this.RaiseAndSetIfChanged(ref _backing.Songs, value);
		}


		public Dictionary<string, User> Users
		{
			get => _backing.Users;
			set => this.RaiseAndSetIfChanged(ref _backing.Users, value);
		}


		public string SongName
		{
			get => _backing.SongName;
			set => this.RaiseAndSetIfChanged(ref _backing.SongName, value);
		}

		public string ArtistName
		{
			get => _backing.ArtistName;
			set => this.RaiseAndSetIfChanged(ref _backing.ArtistName, value);
		}

		public string AlbumName
		{
			get => _backing.AlbumName;
			set => this.RaiseAndSetIfChanged(ref _backing.AlbumName, value);
		}

		public string Format
		{
			get => _backing.Format;
			set => this.RaiseAndSetIfChanged(ref _backing.Format, value);
		}

		public bool ShowMediaControls
		{
			get => _backing.ShowMediaControls;
			set => this.RaiseAndSetIfChanged(ref _backing.ShowMediaControls, value);
		}

		private class Backing
		{
			internal string AlbumName  = string.Empty;
			internal string ArtistName = string.Empty;
			internal string Format     = string.Empty;

			internal string     SongName = string.Empty;
			internal List<Song> Songs    = new();

			internal Dictionary<string, User> Users = new();

			internal bool ShowMediaControls;
		}
	}
}