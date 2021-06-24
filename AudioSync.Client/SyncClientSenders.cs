using AudioSync.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace AudioSync.Client
{
	public partial class SyncClient
	{
		public void Play()
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("Play").Wait();
		}

		public void Pause()
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("Pause").Wait();
		}

		public void Stop()
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("Stop").Wait();
		}

		public void Next()
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("Next").Wait();
		}

		public void Previous()
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("Previous").Wait();
		}

		public void SetQueue(Song[] songs)
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("SetQueue", songs).Wait();
		}

		public Song[] GetQueue() => _connection.InvokeAsync<Song[]>("GetQueue").GetAwaiter().GetResult();

		public void Enqueue(Song song)
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("Enqueue", song).Wait();
		}

		public void ClearQueue()
		{
			if (!IsMaster) return;
			_connection.InvokeAsync("ClearQueue").Wait();
		}
	}
}