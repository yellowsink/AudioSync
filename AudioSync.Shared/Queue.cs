using System.Collections.Generic;
using System.Linq;

namespace AudioSync.Shared
{
	public class Queue
	{
		private List<Song> _songs = new();
		public  Song[]     Songs        => _songs.ToArray();
		public  int        CurrentIndex { get; private set; }

		public Queue(Song[] songs = null, int index = 0)
		{
			_songs       = songs?.ToList() ?? new();
			CurrentIndex = index;
		}

		public void Add(Song   song)    => _songs.Add(song);
		public void Remove(int index)   => _songs.RemoveAt(index);
		public void Clear()             => _songs.Clear();
		public void Next()              => CurrentIndex++;
		public void Previous()          => CurrentIndex--;
		public void SetIndex(int index) => CurrentIndex = index;
	}
}