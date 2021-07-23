using System;
using System.Threading.Tasks;
using AudioSync.Shared;

namespace AudioSync.Client.Backend
{
	public interface ISyncClient : IDisposable
	{
		bool                                IsMaster { get; }
		string                              Name     { get; }
		Task                                Connect();
		Task                                Disconnect();
		
		event EventHandler<User>            UpdateUserEvent;
		event EventHandler<string>          RemoveUserEvent;
		event EventHandler<string>          TransportPlayEvent;
		event EventHandler<string>          TransportPauseEvent;
		event EventHandler<string>          TransportStopEvent;
		event EventHandler<string>          QueueNextEvent;
		event EventHandler<string>          QueuePreviousEvent;
		event EventHandler<(string, Queue)> QueueSetEvent;
		event EventHandler<(string, Song)>  QueueAddEvent;
		event EventHandler<string>          QueueClearEvent;
		
		Task                                SetStatus(UserStatus status);
		Task<bool>                          TrySetName(string    name);
		Task<User[]>                        GetUsers();
		Task                                Play();
		Task                                Pause();
		Task                                Stop();
		Task                                Next();
		Task                                Previous();
		Task                                SetQueue(Song[] songs);
		Task<Queue>                         GetQueue();
		Task                                Enqueue(Song song);
		Task                                ClearQueue();
	}
}