using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AudioSync.Shared;
using JsonLines;

namespace AudioSync.Client.Backend
{
	public class HistoryManager : IDisposable
	{
		public readonly string     HistoryFileLocation;
		private         List<Song> _historyItems;

#pragma warning disable 8618
		public HistoryManager(string? historyLocation = null)
#pragma warning restore 8618
		{
			HistoryFileLocation = historyLocation ?? OSDefaults.DefaultHistoryLocation;
			Directory.CreateDirectory(new FileInfo(HistoryFileLocation).DirectoryName!);

			try { LoadHistory(); }
			catch (UnauthorizedAccessException) { _historyItems = new List<Song>(); }
		}

		public Song[] History
		{
			get => _historyItems.ToArray();
			set => _historyItems = value.ToList();
		}

		public void LoadHistory()
			=> _historyItems = JsonLinesSerializer.Deserialize<HistorySaveItem>(File.ReadAllText(HistoryFileLocation))
												  .Select(si => (Song) si)
												  .ToList();

		public void SaveHistory()
			=> File.WriteAllText(HistoryFileLocation,
								 JsonLinesSerializer.Serialize(History.Select(s => (HistorySaveItem) s)));

		public void Add(Song song)
		{
			_historyItems.RemoveAll(s => Equals(s, song));
			_historyItems.Insert(0, song);
			SaveHistory();
		}

		public void Dispose() => SaveHistory();
	}

	public class HistorySaveItem
	{
		public HistorySaveItem(Song s)
			: this(s.Name, s.Artist, s.Album, s.RawUrl)
		{
		}

		public HistorySaveItem(string name, string artist, string album, string url)
		{
			Name   = name;
			Artist = artist;
			Album  = album;
			Url    = url;
		}

		public string Name;
		public string Artist;
		public string Album;
		public string Url;

		public static implicit operator Song(HistorySaveItem si) => new(si.Name, si.Artist, si.Album, si.Url);
		public static implicit operator HistorySaveItem(Song s)  => new(s);
	}
}