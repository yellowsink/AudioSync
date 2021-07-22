using System.Linq;
using AudioSync.Client.ViewModels;
using AudioSync.Shared;
using DynamicData;

namespace AudioSync.Client.Views
{
	public partial class MainWindow
	{
		private void AddSong(Song song)
		{
			_queue.Add(song);
			((MainWindowViewModel) DataContext!).Songs.Add(song);
			
			DownloadSongIfNeeded(song);
		}

		private void SetQueue(Queue queue)
		{
			_queue = queue;
			((MainWindowViewModel) DataContext!).Songs.Clear();
			((MainWindowViewModel) DataContext!).Songs.AddRange(_queue.Songs);

			foreach (var song in queue.Songs)
				DownloadSongIfNeeded(song);
		}

		private void UpdateUser(User user) => ((MainWindowViewModel) DataContext!).Users.AddOrUpdate(user);

		private void RemoveUser(string name) => ((MainWindowViewModel) DataContext!).Users.RemoveKey(name);
		
		private void DownloadSongIfNeeded(Song song)
		{
			if (_cacheManager.GetFromCache(song) == null) _downloadThread!.Enqueue(song);
		}

		private void UpdateDownloadQueue()
		{
			((MainWindowViewModel) DataContext!).Downloads.Clear();
			((MainWindowViewModel) DataContext!).Downloads.AddRange(_downloadThread!.Queue);
			UpdateCacheView();
		}

		private void UpdateCacheView()
		{
			var cacheItems = _cacheManager.CacheItems
										  .OrderBy(c => c.ArtistName)
										  .ThenBy(c => c.AlbumName)
										  .ThenBy(c => c.SongName);
			
			((MainWindowViewModel) DataContext!).Cache.Clear();
			((MainWindowViewModel) DataContext!).Cache.AddRange(cacheItems);
		}
	}
}