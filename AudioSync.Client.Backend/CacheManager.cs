using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AudioSync.Shared;
using JsonLines;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;

namespace AudioSync.Client.Backend
{
	/// <summary>
	///     Keeps track of the cache and is responsible for downloading music
	/// </summary>
	public class CacheManager : IDisposable
	{
		private readonly ILogger<CacheManager> _logger = HelperUtils.CreateLogger<CacheManager>();

		public readonly string CacheRoot;

		private List<CacheItem> _cacheItems;
		private bool            _disposed;

#pragma warning disable 8618
		public CacheManager(string? cacheLocation = null)
#pragma warning restore 8618
		{
			CacheRoot = cacheLocation ?? OSDefaults.DefaultCacheLocation;
			Directory.CreateDirectory(CacheRoot);

			try
			{
				LoadCache();
			}
			catch (FileNotFoundException)
			{
				_cacheItems = new List<CacheItem>();
			}
		}

		public CacheItem[] CacheItems
		{
			get => _cacheItems.ToArray();
			set => _cacheItems = value.ToList();
		}

		public void Dispose() => Dispose(null);

		public void LoadCache()
		{
			CheckDisposed();

			_logger.LogInformation("Loading cache index from disk");
			var cacheIndexPath = Path.Combine(CacheRoot, "index.json");
			CacheItems = JsonLinesSerializer.Deserialize<CacheItem>(File.ReadAllText(cacheIndexPath)) ??
						 Array.Empty<CacheItem>();
		}

		public void SaveCache()
		{
			CheckDisposed();

			_logger.LogInformation("Saving cache index to disk");
			var cacheIndexPath = Path.Combine(CacheRoot, "index.json");
			File.WriteAllText(cacheIndexPath, JsonLinesSerializer.Serialize(CacheItems));
		}

		/// <summary>
		///     Clean the cache of any item not in the index. If daysThreshold supplied will remove old items
		/// </summary>
		public void CleanCache(int? daysThreshold = null)
		{
			CheckDisposed();

			_logger.LogInformation("Cleaning Cache");
			// Get all actual files
			var songs = GetItemsFromDisk();

			_logger.LogDebug("Removing cache items not in index");
			// Not in index
			foreach (var song in songs)
				if (_cacheItems.All(cacheItem => !song.Equals(cacheItem)))
					song.File(CacheRoot).Delete();

			// Old files
			if (daysThreshold != null)
			{
				_logger.LogDebug($"Removing cache items older than {daysThreshold} days");
				var currentDateTime   = SystemClock.Instance.InUtc().GetCurrentDate();
				var thresholdDateTime = currentDateTime.PlusDays(-daysThreshold.Value);
				foreach (var cacheItem in _cacheItems)
				{
					var comparison = thresholdDateTime.CompareTo(cacheItem.ObjectCreationDateTime);
					if (comparison <= 0) // on the threshold date or before it
						File.Delete(Path.Combine(CacheRoot, cacheItem.CachePrefix, cacheItem.ArtistName,
												 cacheItem.FileExtension + cacheItem.FileExtension));
				}
			}

			_logger.LogDebug("Removing empty directories");
			// Empty directories
			foreach (var prefix in new DirectoryInfo(CacheRoot).GetDirectories())
			{
				foreach (var artist in prefix.GetDirectories())
					if (!artist.GetFileSystemInfos().Any())
						artist.Delete();

				if (!prefix.GetFileSystemInfos().Any()) prefix.Delete();
			}
		}

		public void MoveIntoCache(CacheItem item, FileInfo oldFileLocation)
		{
			CheckDisposed();

			_logger.LogInformation($"Moving {item.SongName} by {item.ArtistName} into the cache");

			item.FileExtension = oldFileLocation.Extension;
			_cacheItems.Add(item);

			var artistDir = Path.Combine(CacheRoot, item.CachePrefix, item.ArtistName);
			Directory.CreateDirectory(artistDir);
			oldFileLocation.MoveTo(Path.Combine(artistDir, item.SongName + oldFileLocation.Extension));
			SaveCache();
		}

		public void RebuildIndexFromDisk()
		{
			CheckDisposed();

			_logger.LogInformation("Rebuilding cache index from disk");
			_cacheItems = GetItemsFromDisk().ToList();
			SaveCache();
		}

		public void EmptyCache()
		{
			CheckDisposed();

			_logger.LogInformation("Emptying the entire cache");
			foreach (var prefix in new DirectoryInfo(CacheRoot).GetDirectories()) prefix.Delete(true);
			_cacheItems.Clear();
			SaveCache();
		}

		public void CompletelyRemoveCache()
		{
			CheckDisposed();

			_logger.LogInformation("Removing the cache entirely");
			Directory.Delete(CacheRoot, true);
			Dispose();
		}

		public (CacheItem, FileInfo)? GetFromCache(Song song)
		{
			CheckDisposed();

			var cacheItem
				= _cacheItems.FirstOrDefault(item => item.SongName == song.Name && item.ArtistName == song.Artist);

			if (cacheItem == null) return null;

			var file = new FileInfo(Path.Combine(CacheRoot, cacheItem.CachePrefix, cacheItem.ArtistName,
												 cacheItem.SongName + cacheItem.FileExtension));

			return (cacheItem, file);
		}

		private CacheItem[] GetItemsFromDisk()
		{
			var prefixes = new DirectoryInfo(CacheRoot).GetDirectories();

			return prefixes.SelectMany(prefix => SongsInPrefix(prefix.GetDirectories(), prefix.Name)).ToArray();


			static IEnumerable<CacheItem> SongsInPrefix(IEnumerable<DirectoryInfo> artists, string prefixName)
				=> artists.SelectMany(artist => SongsInArtist(artist.GetFiles(), prefixName, artist.Name));

			static IEnumerable<CacheItem> SongsInArtist(IEnumerable<FileInfo> songs, string prefixName, string artist)
				=> songs.Select(song => new CacheItem(song.Name, artist, "", song.Extension, prefixName));
		}

		public void Dispose(int? daysThreshold)
		{
			CleanCache(daysThreshold);
			SaveCache();
			_disposed = true;
		}

		/// <summary>
		///     It's messy but oh well.
		/// </summary>
		private void CheckDisposed()
		{
			if (_disposed)
				throw new InvalidOperationException("This CacheManager has been disposed and cannot be used");
		}
	}
}