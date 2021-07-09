using NodaTime;
using NodaTime.Extensions;

namespace AudioSync.Client.Backend.Cache
{
	public class YoutubeItem : ICacheItem
	{
		public string    FileExtension          { get; set; }
		public string    SongName               { get; set; }
		public string    ArtistName             { get; set; }
		public string    CachePrefix            => "youtube";
		public LocalDate ObjectCreationDateTime { get; set; } = SystemClock.Instance.InUtc().GetCurrentDate();

		public YoutubeItem(string song, string artist, string extension)
		{
			FileExtension = extension;
			SongName      = song;
			ArtistName    = artist;
		}
	}
}