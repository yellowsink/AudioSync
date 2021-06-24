using System.Linq;
using System.Threading.Tasks;
using AudioSync.Shared;
using Microsoft.AspNetCore.SignalR;

namespace AudioSync.Server.Hubs
{
	public class SyncHub : Hub
	{
		private IDataService _store;
		
		public SyncHub(IDataService store) { _store = store; }

		#region Auth
		
		public bool ConnectMaster(string name)
		{
			if (!_store.Exists("master")) return false;
			
			_store.Set("master", Context.ConnectionId);
			SetName(name);
			
			return true;
		}

		public void DisconnectMaster()
		{
			_store.Remove("master");
			RemoveName();
		}

		public async Task ConnectClient(string name)
		{
			if (IsMaster()) return;
			
			await Groups.AddToGroupAsync(Context.ConnectionId, "clients");
			SetName(name);
		}

		public async Task DisconnectClient()
		{
			if (IsMaster()) return;

			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "clients");
			RemoveName();
		}
		
		#endregion
		
		#region Transport
		
		public async Task Play()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Play", GetName());
		}

		public async Task Pause()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Pause", GetName());
		}

		public async Task Stop()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Stop", GetName());
		}
		
		#endregion

		#region Queue

		public async Task SetSongs(Song[] songs)
		{
			if (!IsMaster()) return;
			
			_store.Set("songs", songs.ToList());

			await Clients.Group("clients").SendAsync("SetSongs", GetName(), songs);
		}

		public async Task Enqueue(Song song)
		{
			
		}

		#endregion
		
		#region Misc
		
		private bool IsMaster() => _store.TryGet("master", out var id) && (string)id == Context.ConnectionId;

		private void SetName(string name) => SetName(Context.ConnectionId, name);
		private string GetName()            => GetName(Context.ConnectionId);
		private void RemoveName()         => RemoveName(Context.ConnectionId);
		
		private void   SetName(string    connectionId, string name) => _store.Set("name_" + connectionId, name);
		private string GetName(string    connectionId) => (string)_store.Get("name_" + connectionId);
		private void   RemoveName(string connectionId) => _store.Remove("name_" + connectionId);

		#endregion
	}
}