using System;
using AudioSync.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace AudioSync.Client.Backend
{
	public partial class ServerSyncAgent
	{
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

		private void SetupEvents()
		{
			// yuck repetitive code

			_connection.On<User>("UpdateUser", param => { UpdateUserEvent.Invoke(this, param); });
			_connection.On<string>("RemoveUser", param => { RemoveUserEvent.Invoke(this, param); });

			_connection.On<string>("Play",  param => { TransportPlayEvent.Invoke(this, param); });
			_connection.On<string>("Pause", param => { TransportPauseEvent.Invoke(this, param); });
			_connection.On<string>("Stop",  param => { TransportStopEvent.Invoke(this, param); });

			_connection.On<string>("Next",     param => { QueueNextEvent.Invoke(this, param); });
			_connection.On<string>("Previous", param => { QueuePreviousEvent.Invoke(this, param); });

			_connection.On<string>("ClearQueue", param => { QueueClearEvent.Invoke(this, param); });
			_connection.On<(string, Queue)>("SetQueue", param => { QueueSetEvent.Invoke(this, param); });
			_connection.On<(string, Song)>("Enqueue", param => { QueueAddEvent.Invoke(this, param); });
		}
	}
}