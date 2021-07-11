using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;
using FileMode = System.IO.FileMode;

namespace AudioSync.Client.Backend
{
	/// <summary>
	///     Keeps track of required tools and makes sure they're up to date
	/// </summary>
	public class ToolManager
	{
		public readonly string ToolDirectory;

		private readonly ILogger<ToolManager> _logger = HelperUtils.CreateLogger<ToolManager>();

		public ToolVersions Versions = new();

		public string? YtdlCustomLocation;


		public ToolManager(string? toolDirectory = null, string? ytdlCustomLocation = null)
		{
			ToolDirectory = toolDirectory ?? OSDefaults.DefaultToolLocation;
			Directory.CreateDirectory(ToolDirectory);
			YtdlCustomLocation = ytdlCustomLocation;
			LoadToolVersions();
		}

		public string YtdlExecutableLocation
			=> YtdlCustomLocation ?? Path.Combine(ToolDirectory, OSDefaults.DefaultYtdlFileName);


		public async Task<RepositoryTag> GetLatestTagVersion(string user, string name)
		{
			_logger.LogDebug($"Fetching latest tag for {user}/{name}");

			var gh = new GitHubClient(new ProductHeaderValue("AudioSync"));

			var tags = await gh.Repository.GetAllTags(user, name);

			return tags.Aggregate(tags[0],
								  (current, next) => next.Name.VersionIsNewerThan(current.Name) ? next : current);
		}

		public async Task UpdateYtdl(string? latestTagVersion = null)
		{
			// Don't try update YTDL if we're using a custom one
			if (YtdlCustomLocation != null) return;

			_logger.LogInformation("Checking for YTDL updates");
			latestTagVersion ??= (await GetLatestTagVersion("ytdl-org", "youtube-dl")).Name;

			// oh boy that's messy
			if (Versions.Ytdl?.VersionIsNewerThan(latestTagVersion) == false) return;

			_logger.LogInformation($"Updating YTDL from {Versions.Ytdl} to {latestTagVersion}");
			var client   = new HttpClient();
			var response = await client.GetAsync(ToolDownloadLocations.Ytdl);
			var stream   = new FileStream(YtdlExecutableLocation, FileMode.Create);
			await response.Content.CopyToAsync(stream);
			SaveToolVersions();
		}

		public void DestroyTools()
		{
			_logger.LogInformation("Removing all downloaded tools");
			Versions = new ToolVersions();

			new DirectoryInfo(ToolDirectory).Delete(true);
		}

		private void SaveToolVersions()
		{
			_logger.LogDebug("Saving tool versions");
			var toolVersionsPath = Path.Combine(ToolDirectory, "versions.json");
			File.WriteAllText(toolVersionsPath, JsonSerializer.Serialize(Versions));
		}

		private void LoadToolVersions()
		{
			_logger.LogDebug("Loading tool versions");
			var toolVersionsPath = Path.Combine(ToolDirectory, "versions.json");
			Versions = JsonSerializer.Deserialize<ToolVersions>(File.ReadAllText(toolVersionsPath)) ??
					   new ToolVersions();
		}
	}

	public class ToolVersions
	{
		public string? Ytdl { get; set; }
	}

	public static class ToolDownloadLocations
	{
		public const  string YtdlUnix    = "https://yt-dl.org/downloads/latest/youtube-dl";
		public const  string YtdlWindows = "https://yt-dl.org/downloads/latest/youtube-dl.exe";
		public static string Ytdl => OSDefaults.IsOnWindows ? YtdlWindows : YtdlUnix;
	}
}