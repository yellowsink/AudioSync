using System;
using AudioSync.Shared;
using Tmds.DBus;

namespace AudioSync.Client
{
	public partial class SyncClient
	{
		private void SetupEvents()
		{
			AddOneParamEvent("Play",       TransportPlayEvent);
			AddOneParamEvent("Pause",      TransportPauseEvent);
			AddOneParamEvent("Stop",       TransportStopEvent);
			AddOneParamEvent("Next",       QueueNextEvent);
			AddOneParamEvent("Previous",   QueuePreviousEvent);
			AddOneParamEvent("ClearQueue", QueueClearEvent);
			AddTwoParamEvent("SetQueue", QueueSetEvent);
			AddTwoParamEvent("Enqueue",  QueueAddEvent);
		}

		// Add an event with just a string, for the name of the user who performed the action
		private void AddOneParamEvent<T>(string signalREvent, EventHandler<T> @event)
		{
			_connection.On(signalREvent, new []{typeof(string)},
						   async (args, o) =>
						   { 
							   @event.Invoke(o, (T) args[0]);
						   },
						   this);
		}

		private void AddTwoParamEvent<T1, T2>(string signalREvent, EventHandler<(T1, T2)> @event)
		{
			_connection.On(signalREvent, new []{typeof(T1), typeof(T2)},
						   async (args, o) =>
						   { 
							   @event.Invoke(o, ((T1) args[0], (T2) args[1]));
						   },
						   this);
		}

		public event EventHandler<string>           TransportPlayEvent;
		public event EventHandler<string>           TransportPauseEvent;
		public event EventHandler<string>           TransportStopEvent;
		public event EventHandler<string>           QueueNextEvent;
		public event EventHandler<string>           QueuePreviousEvent;
		public event EventHandler<(string, Song[])> QueueSetEvent;
		public event EventHandler<(string, Song)>   QueueAddEvent;
		public event EventHandler<string>           QueueClearEvent;
	}
}