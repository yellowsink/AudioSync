using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using AudioSync.Shared;
using NodaTime;
using NodaTime.Extensions;

namespace AudioSync.Client.Backend
{
	[DebuggerDisplay("{SongName} by {ArtistName} ({AlbumName}) [{CachePrefix}]")]
	public class CacheItem
	{
#pragma warning disable 8618
		// ReSharper disable once UnusedMember.Global
		public CacheItem() { }
#pragma warning restore 8618

		public CacheItem(string song, string artist, string album, string extension, string cachePrefix)
		{
			FileExtension = extension;
			CachePrefix   = cachePrefix;
			SongName      = song;
			ArtistName    = artist;
			AlbumName     = album;
		}

		public CacheItem(Song song, string extension, string cachePrefix)
		{
			FileExtension = extension;
			CachePrefix   = cachePrefix;
			SongName      = song.Name;
			ArtistName    = song.Artist;
			AlbumName     = song.Album;
		}

		public string    FileExtension          { get; set; }
		public string    SongName               { get; set; }
		public string    ArtistName             { get; set; }
		public string?   AlbumName              { get; set; }
		public string    CachePrefix            { get; set; }
		
		/// <summary>
		///		Only intended for use by Utf8Json please ignore
		/// </summary>
		[DataMember(Name = "ObjectCreationDate")]
		public string ObjectCreationDateStr
		{
			get => ObjectCreationDate.ToString("R", CultureInfo.InvariantCulture);
			set => ObjectCreationDate = LocalDate.FromDateTime(DateTime.Parse(value));
		}
		
		[IgnoreDataMember]
		public LocalDate ObjectCreationDate { get; set; } = SystemClock.Instance.InUtc().GetCurrentDate();

		public FileInfo File(string cacheroot)
			=> new(Path.Combine(cacheroot, CachePrefix, ArtistName, SongName + FileExtension));


		public override bool Equals(object? obj) => obj is CacheItem c && Equals(c);

		// ReSharper disable once MemberCanBePrivate.Global
		protected bool Equals(CacheItem other)
			=> FileExtension == other.FileExtension && SongName == other.SongName && ArtistName == other.ArtistName
			&& CachePrefix   == other.CachePrefix;

		// ReSharper disable NonReadonlyMemberInGetHashCode
		public override int GetHashCode() => HashCode.Combine(FileExtension, SongName, ArtistName, CachePrefix);
		// ReSharper restore NonReadonlyMemberInGetHashCode
	}
}