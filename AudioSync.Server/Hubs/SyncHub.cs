using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioSync.Shared;
using Microsoft.AspNetCore.SignalR;

namespace AudioSync.Server.Hubs
{
	public class SyncHub : Hub
	{
		private readonly IDataService _store;
		
		public SyncHub(IDataService store) => _store = store;

		#region Auth
		
		public bool ConnectMaster(string name)
		{
			if (!_store.Exists("master")) return false;
			
			Console.WriteLine($"{name} is the new master");
			
			_store.Set("master", Context.ConnectionId);
			SetName(name);
			
			return true;
		}

		public void DisconnectMaster()
		{
			Console.WriteLine($"Master ({GetName()} left");
			
			_store.Remove("master");
			RemoveName();
		}

		public async Task ConnectClient(string name)
		{
			if (IsMaster()) return;
			
			Console.WriteLine($"{name} joined");
			
			await Groups.AddToGroupAsync(Context.ConnectionId, "clients");
			SetName(name);
		}

		public async Task DisconnectClient()
		{
			if (IsMaster()) return;
			
			Console.WriteLine($"{GetName()} left");

			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "clients");
			RemoveName();
		}
		
		#endregion
		
		#region Transport
		
		public async Task Play()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Play", GetName());
			Console.WriteLine("Transport play");
		}

		public async Task Pause()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Pause", GetName());
			Console.WriteLine("Transport pause");
		}

		public async Task Stop()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Stop", GetName());
			Console.WriteLine("Transport stop");
		}
		
		#endregion

		#region Queue

		public async Task Next()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Next", GetName());
			Console.WriteLine("Queue next");
		}

		public async Task Previous()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Previous", GetName());
			Console.WriteLine("Queue previous");
		}

		public async Task SetQueue(Song[] songs)
		{
			if (!IsMaster()) return;
			
			_store.Set("songs", songs.ToList());

			await Clients.Group("clients").SendAsync("SetQueue", GetName(), songs);
			
			Console.WriteLine("Set queue");
		}

		public Song[] GetQueue()
		{
			Console.WriteLine($"{GetName()} queried the queue");
			return _store.TryGet("songs", out var stored)
				? ((IList<Song>) stored)!.ToArray()
				: Array.Empty<Song>();
		}

		public async Task Enqueue(Song song)
		{
			if (!IsMaster()) return;
			var songs = _store.TryGet("songs", out var stored)
				? (IList<Song>) stored
				: new List<Song>();
				
			songs?.Add(song);
			
			_store.Set("songs", songs);

			await Clients.Group("clients").SendAsync("Enqueue", GetName(), song);
			
			Console.WriteLine("Song added to the queue");
		}

		public async Task ClearQueue()
		{
			if (!IsMaster()) return;
			
			_store.Remove("songs");

			await Clients.Group("clients").SendAsync("ClearQueue", GetName());
			
			Console.WriteLine("Queue cleared");
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