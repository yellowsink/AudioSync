using System.IO;
using AudioSync.Shared;
using NodaTime;
using NodaTime.Extensions;

namespace AudioSync.Client.Backend
{
	public class CacheItem
	{
#pragma warning disable 8618
		// ReSharper disable once UnusedMember.Global
		public CacheItem(){}
#pragma warning restore 8618
		
		public CacheItem(string song, string artist, string extension, string cachePrefix)
		{
			FileExtension = extension;
			CachePrefix   = cachePrefix;
			SongName      = song;
			ArtistName    = artist;
		}

		public CacheItem(Song song, string extension, string cachePrefix)
		{
			FileExtension = extension;
			CachePrefix   = cachePrefix;
			SongName      = song.Name;
			ArtistName    = song.Album;
		}

		public FileInfo File(string cacheroot)
			=> new(Path.Combine(cacheroot, CachePrefix, ArtistName, SongName + FileExtension));

		public string    FileExtension          { get; set; }
		public string    SongName               { get; set; }
		public string    ArtistName             { get; set; }
		public string    CachePrefix            { get; set; }
		public LocalDate ObjectCreationDateTime { get; set; } = SystemClock.Instance.InUtc().GetCurrentDate();
	}
}