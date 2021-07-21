using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AudioSync.Shared;

namespace AudioSync.Client.Backend
{
	public class DownloadManager
	{
		private readonly Action<CacheItem, FileInfo> _moveToCacheAction;
		private readonly string                       _ytdlPath;

		/// <param name="moveToCacheAction">An action that moves a file into the cache</param>
		/// <param name="ytdlPath">Where YTDL is: leave blank for OS default</param>
		public DownloadManager(Action<CacheItem, FileInfo> moveToCacheAction, string? ytdlPath = null)
		{
			_ytdlPath = ytdlPath ?? Path.Combine(OSDefaults.DefaultToolLocation, OSDefaults.DefaultYtdlFileName);
			if (!File.Exists(_ytdlPath))
				throw new
					ArgumentException("Either given path does not contain a file, or default path is empty if no path supplied",
									  nameof(ytdlPath));

			_moveToCacheAction = moveToCacheAction;
		}

		public DownloadManager(CacheManager cacheManager, string? ytdlPath = null) : this(cacheManager.MoveIntoCache,
			ytdlPath)
		{
		}

		/// <summary>
		///     Downloads a song then moves it to the cache using the action given in the constructor
		/// </summary>
		public async Task DownloadSong(Song song, string? downloadFolder = null)
		{
			downloadFolder ??= OSDefaults.DefaultDownloadLocation;
			Directory.CreateDirectory(downloadFolder);
			var (file, ext) = await RunYtdl(await song.DownloadableUrlAsync(), $"{song.Artist} - {song.Name}",
											downloadFolder);

			_moveToCacheAction(CreateCacheItem(song, ext), file);
		}

		/// <summary>
		///     Runs YTDL to download the given url to the given folder with the given filename
		/// </summary>
		private async Task<(FileInfo, string)> RunYtdl(string url, string filename, string downloadFolder)
		{
			// %(ext)s is a pattern that tells YTDL to insert the correct file extension
			var startOptions = new ProcessStartInfo(_ytdlPath, $"{url} --print-json -o \"{filename}.%(ext)s\"")
			{
				WorkingDirectory = downloadFolder,
				RedirectStandardOutput = true
			};
			var process = Process.Start(startOptions);
			await process!.WaitForExitAsync();
			var result = await process.StandardOutput.ReadToEndAsync();

			var ytdlOutputObject = JsonSerializer.Deserialize<YtdlOutputObject>(result)!;

			return (new FileInfo(Path.Combine(downloadFolder, ytdlOutputObject.Filename)), ytdlOutputObject.Extension);
		}

		/// <summary>
		///     Creates a cache item for the given song with today's date
		/// </summary>
		private static CacheItem CreateCacheItem(Song song, string fileExtension)
			=> new(song.Name, song.Artist, fileExtension, song.UseYoutube ? "youtube" : "soundcloud");
	}

	/// <summary>
	///     Please ignore outside of DownloadManager: exists to read the output of YTDL
	/// </summary>
	public class YtdlOutputObject
	{
		[JsonPropertyName("_filename")]
		public string Filename { get; set; } = null!;

		[JsonPropertyName("ext")]
		public string Extension { get; set; } = null!;
	}
}