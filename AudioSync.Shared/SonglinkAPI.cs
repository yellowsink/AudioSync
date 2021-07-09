#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioSync.Shared
{
	// ReSharper disable once InconsistentNaming
	public static class SonglinkAPI
	{
		public static async Task<PlatformLinks?> Get(string url)
		{
			var client      = new HttpClient();
			var response = await client.GetStringAsync($"https://api.song.link/v1-alpha.1/links?url={url}");
			return JsonSerializer.Deserialize<SonglinkResponse>(response)?.LinksByPlatform;
		}

		public static async Task<string?> Soundcloud(string url) => (await Get(url)).Soundcloud?.Url;

		public static async Task<string?> Youtube(string url) => (await Get(url)).Youtube?.Url;

		public class SonglinkResponse
		{
			public string        EntityUniqueId  { get; set; } = null!;
			public string        UserCountry     { get; set; } = null!;
			public string        PageUrl         { get; set; } = null!;
			public PlatformLinks LinksByPlatform { get; set; } = null!;
			// not bothering with entitiesByUniqueId
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public class PlatformLinks
		{
			public PlatformLink? AmazonMusic { get; set; }
			public PlatformLink? AmazonStore { get; set; }
			public PlatformLink? Deezer      { get; set; }
			public PlatformLink? AppleMusic  { get; set; }
			// ReSharper disable once InconsistentNaming
			public PlatformLink? iTunes       { get; set; }
			public PlatformLink? Napster      { get; set; }
			public PlatformLink? Pandora      { get; set; }
			public PlatformLink? Soundcloud   { get; set; }
			public PlatformLink? Spotify      { get; set; }
			public PlatformLink? Tidal        { get; set; }
			public PlatformLink? Yandex       { get; set; }
			public PlatformLink? Youtube      { get; set; }
			public PlatformLink? YoutubeMusic { get; set; }
		}

		public class PlatformLink
		{
			public string Country             { get; set; }
			public string Url                 { get; set; }
			public string NativeAppUrlMobile  { get; set; }
			public string NativeAppUrlDesktop { get; set; }
			public string EntityUniqueId      { get; set; }
		}
	}
}