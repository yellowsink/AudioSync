using NodaTime;
using NodaTime.Extensions;

namespace AudioSync.Client.Backend.Cache
{
	public class SoundCloudItem : ICacheItem
	{
		public string    FileExtension          { get; set; }
		public string    SongName               { get; set; }
		public string    ArtistName             { get; set; }
		public string    CachePrefix            => "soundcloud";
		public LocalDate ObjectCreationDateTime { get; set; } = SystemClock.Instance.InUtc().GetCurrentDate();

		public SoundCloudItem(string song, string artist, string extension)
		{
			FileExtension = extension;
			SongName      = song;
			ArtistName    = artist;
		}
		
		
	}
}