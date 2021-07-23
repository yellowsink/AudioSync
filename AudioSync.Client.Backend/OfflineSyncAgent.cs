using System;
using System.Threading.Tasks;
using AudioSync.Shared;
#pragma warning disable 1998

namespace AudioSync.Client.Backend
{
	/// <summary>
	///		Pretends to be a server to facilitate offline use
	/// </summary>
	public class OfflineSyncAgent : ISyncAgent
	{
		public bool   IsMaster => true;
		public string Name     => "Offline User";

		private          User? _user;
		private readonly Queue _queue = new();
		
		public async Task Connect() => _user = new User(Name) { IsMaster = true };

		public async Task Disconnect() => _user = null;

		public event EventHandler<User>            UpdateUserEvent     = (_, _) => { };
		public event EventHandler<string>          RemoveUserEvent     = (_, _) => { };
		public event EventHandler<string>          TransportPlayEvent  = (_, _) => { };
		public event EventHandler<string>          TransportPauseEvent = (_, _) => { };
		public event EventHandler<string>          TransportStopEvent  = (_, _) => { };
		public event EventHandler<string>          QueueNextEvent      = (_, _) => { };
		public event EventHandler<string>          QueuePreviousEvent  = (_, _) => { };
		public event EventHandler<(string, Queue)> QueueSetEvent       = (_, _) => { };
		public event EventHandler<(string, Song)>  QueueAddEvent       = (_, _) => { };
		public event EventHandler<string>          QueueClearEvent     = (_, _) => { };

		public async Task SetStatus(UserStatus status)
		{
			_user!.Status = status;
			UpdateUserEvent.Invoke(this, _user);
		}

		public async Task<bool> TrySetName(string name)
		{
			_user!.Name = name;
			UpdateUserEvent.Invoke(this, _user);
			return true;
		}

		public async Task<User[]> GetUsers() => new[] { _user! };

		public async Task Play() => TransportPlayEvent.Invoke(this, Name);

		public async Task Pause() => TransportPauseEvent.Invoke(this, Name);

		public async Task Stop() => TransportStopEvent.Invoke(this, Name);

		public async Task Next() => QueueNextEvent.Invoke(this, Name);

		public async Task Previous() => QueuePreviousEvent.Invoke(this, Name);

		public async Task SetQueue(Song[] songs)
		{
			_queue.Clear();
			foreach (var song in songs) _queue.Add(song);
			QueueSetEvent.Invoke(this, (Name, _queue));
		}

		public async Task<Queue> GetQueue() => _queue;

		public async Task Enqueue(Song song) => _queue.Add(song);

		public async Task ClearQueue() => _queue.Clear();

		public void Dispose() { }
	}
}