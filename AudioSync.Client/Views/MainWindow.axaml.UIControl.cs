using System;
using System.Linq;
using AudioSync.Client.Backend;
using AudioSync.Client.ViewModels;
using AudioSync.Shared;
using Avalonia.Input;
using DynamicData;
using JetBrains.Annotations;

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
			var cacheItems = _cacheManager.CacheItems.OrderBy(c => c.ArtistName)
										  .ThenBy(c => c.AlbumName)
										  .ThenBy(c => c.SongName);

			((MainWindowViewModel) DataContext!).Cache.Clear();
			((MainWindowViewModel) DataContext!).Cache.AddRange(cacheItems);
		}

		private void UpdateNowPlayingMetadata()
		{
			if (_audioManager.Status == AudioManagerStatus.Idle)
			{
				_presenceManager.NotPlaying();
				((MainWindowViewModel) DataContext!).SongName   = string.Empty;
				((MainWindowViewModel) DataContext!).ArtistName = string.Empty;
				((MainWindowViewModel) DataContext!).AlbumName  = string.Empty;
				((MainWindowViewModel) DataContext!).Format     = string.Empty;
			}
			else
			{
				var song = _queue.Songs[_queue.CurrentIndex];
				_presenceManager.UpdateCurrentSong(song);
				((MainWindowViewModel) DataContext!).SongName   = song.Name;
				((MainWindowViewModel) DataContext!).ArtistName = song.Artist;
				((MainWindowViewModel) DataContext!).AlbumName  = song.Album;
				((MainWindowViewModel) DataContext!).Format
					= _cacheManager.GetFromCache(song)?.Item1.FileExtension.ToUpper() ?? string.Empty;
			}
		}

		[UsedImplicitly]
		private void TextBoxAddSong_OnKeyUp(object? sender, KeyEventArgs keyEventArgs)
		{
			if (!((MainWindowViewModel) DataContext!).InputAddSong.Contains("\t")) return;

			var songParts = ((MainWindowViewModel) DataContext!).InputAddSong.Split("\t");
			switch (songParts.Length)
			{
				case 1:
					break;
				case 2:
					((MainWindowViewModel) DataContext!).InputAddSong   = songParts[0].Trim();
					((MainWindowViewModel) DataContext!).InputAddArtist = songParts[1].Trim();
					break;
				case 3:
					((MainWindowViewModel) DataContext!).InputAddSong   = songParts[0].Trim();
					((MainWindowViewModel) DataContext!).InputAddArtist = songParts[1].Trim();
					((MainWindowViewModel) DataContext!).InputAddAlbum  = songParts[2].Trim();
					break;

				default:
					((MainWindowViewModel) DataContext!).InputAddSong   = songParts[0].Trim();
					((MainWindowViewModel) DataContext!).InputAddArtist = songParts[1].Trim();

					var remainder = songParts[new Range(2, Index.End)]
					   .Aggregate(string.Empty, (current, next) => current + next);
					((MainWindowViewModel) DataContext!).InputAddAlbum = remainder.Trim();
					break;
			}
		}
	}
}