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
		/// If using youtube is preferred over soundcloud. Set automatically if a song is not on soundcloud
		/// </summary>
		public bool UseYoutube;
		
		// Used to prevent unneeded Songlink API requests
		private string _cachedSoundcloudUrl;
		private string _cachedYoutubeUrl;

		/// <summary>
		/// The Soundcloud URL of the song - uses song.link asynchronously and caches seamlessly
		/// </summary>
		private async Task<string> SoundcloudUrlAsync() => _cachedSoundcloudUrl ??= await SonglinkAPI.Soundcloud(RawUrl);
		
		/// <summary>
		/// The Youtube URL of the song - uses song.link asynchronously and caches seamlessly
		/// </summary>
		private async Task<string> YoutubeUrlAsync() => _cachedYoutubeUrl ??= await SonglinkAPI.Youtube(RawUrl);

		/// <summary>
		/// Asynchronously gets the URL to download the song from either soundcloud, or if unavailable youtube
		/// </summary>
		/// <returns>A URL supported by YTDL to download the song</returns>
		/// <exception cref="SongUnavailableException">Neither soundcloud nor youtube have the song</exception>
		public async Task<string> DownloadableUrlAsync()
		{
			var scUrl = await SoundcloudUrlAsync();
			if (!UseYoutube && scUrl != null) return scUrl;

			UseYoutube = true;
			var ytUrl = await YoutubeUrlAsync();
			if (ytUrl == null) throw new SongUnavailableException();
			return ytUrl;
		}
		
		/// <summary>
		/// Gets the URL to download the song from either soundcloud, or if unavailable youtube
		/// </summary>
		/// <returns>A URL supported by YTDL to download the song</returns>
		/// <exception cref="SongUnavailableException">Neither soundcloud nor youtube have the song</exception>
		public string DownloadableUrl => DownloadableUrlAsync().GetAwaiter().GetResult();
	}
}