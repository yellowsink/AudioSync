using System.Threading.Tasks;
using AudioSync.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace AudioSync.Client.Backend
{
	public partial class SyncClient
	{
		public void SetStatus(UserStatus status) => _connection.InvokeAsync("SetStatus", status).Wait();

		public bool TrySetName(string name)
		{
			var success       = _connection.InvokeAsync<bool>("SetName", name).GetAwaiter().GetResult();
			if (success) Name = name;
			return success;
		}

		public async Task<User[]> GetUsers() => await _connection.InvokeAsync<User[]>("GetUsers");

		public async Task Play()
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("Play");
		}

		public async Task Pause()
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("Pause");
		}

		public async Task Stop()
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("Stop");
		}

		public async Task Next()
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("Next");
		}

		public async Task Previous()
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("Previous");
		}

		public async Task SetQueue(Song[] songs)
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("SetQueue", songs);
		}

		public async Task<Queue> GetQueue() => await _connection.InvokeAsync<Queue>("GetQueue");

		public async Task Enqueue(Song song)
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("Enqueue", song);
		}

		public async Task ClearQueue()
		{
			if (!IsMaster) return;
			await _connection.InvokeAsync("ClearQueue");
		}
	}
}