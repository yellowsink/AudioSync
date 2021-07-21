using System;
using System.Linq;
using System.Threading.Tasks;
using AudioSync.Shared;
using Microsoft.AspNetCore.SignalR;

namespace AudioSync.Server.Hubs
{
	public partial class SyncHub : Hub
	{
		private readonly HubState _state;

		public SyncHub(HubState state) { _state = state; }

#region Auth

		public async Task<bool> ConnectMaster(string name)
		{
			if (_state.MasterExists || !RegisterName(name)) return false;

			Console.WriteLine($"{name} is the new master");

			_state.MasterId = Context.ConnectionId;
			SetOrAddUser(new User(name)
			{
				IsMaster = true
			});

			await Clients.Group("clients").SendAsync("UpdateUser", GetUser());

			return true;
		}

		public async Task DisconnectMaster()
		{
			var name = GetUser().Name;
			Console.WriteLine($"Master ({name}) left");

			_state.MasterId = null;
			RemoveNameIfRegistered(name);
			RemoveUser();

			await Clients.Group("clients").SendAsync("RemoveUser", name);
		}

		public async Task<bool> ConnectClient(string name)
		{
			if (IsMaster() || !RegisterName(name)) return false;

			Console.WriteLine($"{name} joined");

			await Groups.AddToGroupAsync(Context.ConnectionId, "clients");
			SetOrAddUser(new User(name));

			await Clients.Group("clients").SendAsync("UpdateUser", GetUser());
			return true;
		}

		public async Task DisconnectClient()
		{
			if (IsMaster()) return;

			var name = GetUser().Name;
			Console.WriteLine($"{name} left");

			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "clients");
			RemoveNameIfRegistered(name);
			RemoveUser();

			await Clients.Group("clients").SendAsync("RemoveUser", name);
		}

		public async Task<User[]> GetUsers() => _state.Users.Values.ToArray();

		public async Task SetStatus(UserStatus status)
		{
			var user = GetUser();
			user.Status = status;
			SetOrAddUser(user);
			await Clients.Group("clients").SendAsync("UpdateUser", GetUser());
		}

		public async Task<bool> SetName(string name)
		{
			if (!RegisterName(name)) return false;

			var user = GetUser();
			RemoveNameIfRegistered(user.Name);

			user.Name = name;
			SetOrAddUser(user);

			await Clients.Group("clients").SendAsync("UpdateUser", GetUser());
			return true;
		}

#endregion

#region Transport

		public async Task Play()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Play", GetUser().Name);
			Console.WriteLine("Transport play");
		}

		public async Task Pause()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Pause", GetUser().Name);
			Console.WriteLine("Transport pause");
		}

		public async Task Stop()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Stop", GetUser().Name);
			Console.WriteLine("Transport stop");
		}

#endregion

#region Queue

		public async Task Next()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Next", GetUser().Name);
			Console.WriteLine("Queue next");
		}

		public async Task Previous()
		{
			if (IsMaster()) await Clients.Group("clients").SendAsync("Previous", GetUser().Name);
			Console.WriteLine("Queue previous");
		}

		public async Task SetQueue(Queue queue)
		{
			if (!IsMaster()) return;

			_state.Queue = queue;

			await Clients.Group("clients").SendAsync("SetQueue", GetUser().Name, queue);

			Console.WriteLine("Set queue");
		}

		public Queue GetQueue()
		{
			Console.WriteLine($"{GetUser().Name} queried the queue");
			return _state.Queue;
		}

		public async Task Enqueue(Song song)
		{
			if (!IsMaster()) return;

			_state.Queue.Add(song);

			await Clients.Group("clients").SendAsync("Enqueue", GetUser().Name, song);

			Console.WriteLine("Song added to the queue");
		}

		public async Task ClearQueue()
		{
			if (!IsMaster()) return;

			_state.Queue.Clear();

			await Clients.Group("clients").SendAsync("ClearQueue", GetUser().Name);

			Console.WriteLine("Queue cleared");
		}

#endregion
	}
}