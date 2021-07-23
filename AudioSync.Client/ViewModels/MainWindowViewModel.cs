using System;
using System.Collections.Generic;
using AudioSync.Client.Backend;
using AudioSync.Shared;
using DynamicData;
using ReactiveUI;

namespace AudioSync.Client.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly Backing _backing = new();

		public SourceList<Song> Songs
		{
			get => _backing.Songs;
			set => this.RaiseAndSetIfChanged(ref _backing.Songs, value);
		}

		public IObservable<IReadOnlyCollection<Song>> SongsBindable => Songs.Connect().ToCollection();


		public SourceCache<User, string> Users
		{
			get => _backing.Users;
			set => this.RaiseAndSetIfChanged(ref _backing.Users, value);
		}

		public IObservable<IReadOnlyCollection<User>> UsersBindable => Users.Connect().ToCollection();

		public SourceList<Song> Downloads
		{
			get => _backing.Downloads;
			set => this.RaiseAndSetIfChanged(ref _backing.Downloads, value);
		}

		public IObservable<IReadOnlyCollection<Song>> DownloadsBindable => Downloads.Connect().ToCollection();

		public SourceList<CacheItem> Cache
		{
			get => _backing.Cache;
			set => this.RaiseAndSetIfChanged(ref _backing.Cache, value);
		}

		public IObservable<IReadOnlyCollection<CacheItem>> CacheBindable => Cache.Connect().ToCollection();


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


		public string InputAddSong
		{
			get => _backing.InputAddSong;
			set => this.RaiseAndSetIfChanged(ref _backing.InputAddSong, value);
		}

		public string InputAddArtist
		{
			get => _backing.InputAddArtist;
			set => this.RaiseAndSetIfChanged(ref _backing.InputAddArtist, value);
		}

		public string InputAddAlbum
		{
			get => _backing.InputAddAlbum;
			set => this.RaiseAndSetIfChanged(ref _backing.InputAddAlbum, value);
		}

		public string InputAddUrl
		{
			get => _backing.InputAddUrl;
			set => this.RaiseAndSetIfChanged(ref _backing.InputAddUrl, value);
		}

		private class Backing
		{
			internal string AlbumName  = string.Empty;
			internal string ArtistName = string.Empty;

			internal SourceList<CacheItem> Cache = new();

			internal SourceList<Song> Downloads      = new();
			internal string           Format         = string.Empty;
			internal string           InputAddAlbum  = string.Empty;
			internal string           InputAddArtist = string.Empty;

			internal string InputAddSong = string.Empty;
			internal string InputAddUrl  = string.Empty;

			internal bool ShowMediaControls;

			internal string           SongName = string.Empty;
			internal SourceList<Song> Songs    = new();

			internal SourceCache<User, string> Users = new(u => u.Name);
		}
	}
}