#nullable enable
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#pragma warning disable 8618

namespace AudioSync.Shared
{
	// ReSharper disable once InconsistentNaming
	public static class Songlink
	{
		private static readonly HttpClient _client = new();

		public static async Task<SonglinkResponse?> Get(string url)
		{
			var response = await _client.GetStringAsync($"https://api.song.link/v1-alpha.1/links?url={url}");
			return JsonSerializer.Deserialize<SonglinkResponse>(response);
		}

		public static async Task<string?> Soundcloud(string url) => (await Get(url))?.LinksByPlatform?.Soundcloud?.Url;

		public static async Task<string?> Youtube(string url) => (await Get(url))?.LinksByPlatform?.Youtube?.Url;

		public class SonglinkResponse
		{
			[JsonPropertyName("entityUniqueId")]
			public string? EntityUniqueId { get; set; }

			[JsonPropertyName("userCountry")]
			public string? UserCountry { get; set; }

			[JsonPropertyName("pageUrl")]
			public string? PageUrl { get; set; }

			[JsonPropertyName("linksByPlatform")]
			public PlatformLinks? LinksByPlatform { get; set; }
			// not bothering with entitiesByUniqueId
		}

		public class PlatformLinks
		{
			[JsonPropertyName("soundcloud")]
			public PlatformLink? Soundcloud { get; set; }

			[JsonPropertyName("youtube")]
			public PlatformLink? Youtube { get; set; }
		}

		public class PlatformLink
		{
			[JsonPropertyName("country")]
			public string? Country { get; set; }

			[JsonPropertyName("url")]
			public string? Url { get; set; }
		}
	}
}