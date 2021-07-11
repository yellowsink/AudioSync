#nullable enable
using System.Threading.Tasks;
using AudioSync.Shared;
using NUnit.Framework;

namespace AudioSync.Tests
{
	public class AudioSyncTests
	{
		
		[TestCase("https://soundcloud.com/oneokrock/start-again", "https://soundcloud.com/oneokrock/start-again",
				  TestName = "Soundcloud -> Soundcloud (Start Again)")]
		[TestCase("https://open.spotify.com/track/46scODShYFATHbLfLE0dr1", "https://soundcloud.com/user-524915514/sparks-takanashi-kiara",
				  TestName = "Spotify -> Soundcloud (SPARKS)")]
		[TestCase("https://youtu.be/EjlMPu5sEgw", "https://soundcloud.com/soubread/getcha-calliope-suiseicover",
				  TestName = "YouTube -> Soundcloud (GETCHA!! cover)")]
		// WHY IS EVERYTHING ON SOUNDCLOUD
		[TestCase("https://youtu.be/P4-Khg04K78", "https://www.youtube.com/watch?v=P4-Khg04K78",
				  TestName = "Youtube -> Youtube (Bad Apple on delay lama (kill me))")]
		public async Task GetDownloadableUrlTest(string url, string? downloadableUrl)
		{
			var links  = (await Songlink.Get(url))?.LinksByPlatform;
			var result = links?.Soundcloud?.Url ?? links?.Youtube?.Url;
			
			Assert.AreEqual(downloadableUrl, result);
		}
	}
}