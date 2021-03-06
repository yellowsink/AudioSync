using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AudioSync.Shared
{
	[DebuggerDisplay("{Name} by {Artist}")]
	public class Song
	{
		// Used to prevent unneeded Songlink API requests
		private string _cachedSoundcloudUrl;
		private string _cachedYoutubeUrl;

		/// <summary>
		///     The raw URL given for the song
		/// </summary>
		public string RawUrl { get; }

		/// <summary>
		///     If using youtube is preferred over soundcloud. Set automatically if a song is not on soundcloud
		/// </summary>
		public bool UseYoutube;

		public Song(string name, string artist, string album, string rawUrl)
		{
			Name   = name;
			Artist = artist;
			Album  = album;
			RawUrl = rawUrl;
		}

		/// <summary>
		///     The name of the song
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     The artist of the song
		/// </summary>
		public string Artist { get; set; }

		/// <summary>
		///     The album the song was on
		/// </summary>
		public string Album { get; set; }

		/// <summary>
		///     Gets the URL to download the song from either soundcloud, or if unavailable youtube
		/// </summary>
		/// <returns>A URL supported by YTDL to download the song</returns>
		/// <exception cref="SongUnavailableException">Neither soundcloud nor youtube have the song</exception>
		public string DownloadableUrl => DownloadableUrlAsync().GetAwaiter().GetResult();

		/// <summary>
		///     The Soundcloud URL of the song - uses song.link asynchronously and caches seamlessly
		/// </summary>
		private async Task<string> SoundcloudUrlAsync() => _cachedSoundcloudUrl ??= await Songlink.Soundcloud(RawUrl);

		/// <summary>
		///     The Youtube URL of the song - uses song.link asynchronously and caches seamlessly
		/// </summary>
		private async Task<string> YoutubeUrlAsync() => _cachedYoutubeUrl ??= await Songlink.Youtube(RawUrl);

		/// <summary>
		///     Asynchronously gets the URL to download the song from either soundcloud, or if unavailable youtube
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


		public override bool Equals(object? obj) => obj is Song s && Equals(s);

		protected bool Equals(Song other) => RawUrl == other.RawUrl && Name == other.Name && Artist == other.Artist;

		public override int GetHashCode() => HashCode.Combine(RawUrl, Name, Artist);
	}
}