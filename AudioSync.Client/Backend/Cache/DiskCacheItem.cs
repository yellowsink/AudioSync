using System;
using System.IO;
using NodaTime;
using NodaTime.Extensions;

namespace AudioSync.Client.Backend.Cache
{
	public class DiskCacheItem : ICacheItem
	{
		
		public FileInfo File { get; set; }

		public string  FileExtension
		{
			get => File.Extension;
			set => throw new NotImplementedException();
		}

		public string SongName
		{
			get => File.Extension;
			set => throw new NotImplementedException();
		}

		public string ArtistName
		{
			get => File.DirectoryName!;
			set => throw new NotImplementedException();
		}

		public string  CachePrefix           { get; }
		public LocalDate ObjectCreationDateTime { get; set; } = SystemClock.Instance.InUtc().GetCurrentDate();
		
		public DiskCacheItem(FileInfo file, string prefix)
		{
			File        = file;
			CachePrefix = prefix;
		}

		public override bool Equals(object? obj)
			=> obj is ICacheItem cacheItem
			&& cacheItem.SongName == SongName
			&& cacheItem.ArtistName == ArtistName
			&& cacheItem.CachePrefix == CachePrefix
			&& cacheItem.FileExtension == FileExtension;

		protected bool Equals(DiskCacheItem other) => File.Equals(other.File) && CachePrefix == other.CachePrefix && ObjectCreationDateTime.Equals(other.ObjectCreationDateTime);

		public override int GetHashCode() => HashCode.Combine(File, CachePrefix, ObjectCreationDateTime);
	}
}