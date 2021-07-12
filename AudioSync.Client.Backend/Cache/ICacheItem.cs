using NodaTime;

namespace AudioSync.Client.Backend.Cache
{
	public interface ICacheItem
	{
		public string    FileExtension          { get; set; }
		public string    SongName               { get; set; }
		public string    ArtistName             { get; set; }
		public string    CachePrefix            { get; }
		public LocalDate ObjectCreationDateTime { get; set; }
	}
}