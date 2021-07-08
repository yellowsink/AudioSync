using System.Threading.Tasks;

namespace AudioSync.Shared
{
	public class Song
	{
		/// <summary>
		/// The name of the song
		/// </summary>
		public string Name;
		/// <summary>
		/// The artist of the song
		/// </summary>
		public string Artist;
		/// <summary>
		/// The raw URL given for the song
		/// </summary>
		public string RawUrl;

		/// <summary>
		/// Used internally to prevent unneeded Songlink API requests
		/// </summary>
		private string _cachedSoundCloudUrl;

		/// <summary>
		/// The Soundcloud URL of the song - uses song.link and caches seamlessly 
		/// </summary>
		public string SoundCloudUrl => _cachedSoundCloudUrl ??= SonglinkAPI.Soundcloud(RawUrl).GetAwaiter().GetResult();
		
		/// <summary>
		/// The Soundcloud URL of the song - uses song.link asynchronously and caches seamlessly
		/// </summary>
		public async Task<string> SoundCloudUrlAsync() => _cachedSoundCloudUrl ??= await SonglinkAPI.Soundcloud(RawUrl);
	}
}