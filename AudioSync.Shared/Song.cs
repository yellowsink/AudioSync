namespace AudioSync.Shared
{
	public class Song
	{
		public string Name;
		public string Artist;
		public string RawUrl;

		public string SoundCloudUrl => SonglinkAPI.Soundcloud(RawUrl).GetAwaiter().GetResult();
	}
}