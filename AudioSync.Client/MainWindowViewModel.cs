using AudioSync.Shared;
using DynamicData;
using ReactiveUI;

namespace AudioSync.Client
{
	public class MainWindowViewModel : ReactiveObject
	{
		public SourceList<Song> Songs
		{
			get => _backing.Songs;
			set => this.RaiseAndSetIfChanged(ref _backing.Songs, value);
		}
		
		
		public SourceCache<User, string> Users
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

		
		
		private Backing _backing = new();

		private class Backing
		{
			internal SourceList<Song> Songs = new();

			internal SourceCache<User, string> Users = new(user => user.Name);

			internal string SongName   = string.Empty;
			internal string ArtistName = string.Empty;
			internal string AlbumName  = string.Empty;
			internal string Format     = string.Empty;
		}
	}
}