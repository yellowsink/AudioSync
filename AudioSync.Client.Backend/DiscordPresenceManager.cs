using System;
using AudioSync.Shared;
using DiscordRPC;

namespace AudioSync.Client.Backend
{
	public class DiscordPresenceManager : IDisposable
	{
		private readonly DiscordRpcClient _client;
		private readonly RichPresence     _presence;

		public DiscordPresenceManager(string appId)
		{
			_client = new DiscordRpcClient(appId);
			_client.Initialize();

			_presence = new RichPresence
			{
				Details = "Hello, World!",
				State   = "Connecting to a server"
			};

			_client.SetPresence(_presence);
		}

		public void Dispose()
		{
			_client.ClearPresence();
			_client.Invoke();
			_client.Dispose();
		}

		public void UpdateCurrentSong(Song song)
		{
			_presence.Details = $"{song.Name} - {song.Artist}";
			_client.SetPresence(_presence);
		}

		public void UpdateStatus(bool isOnline, string? ip, string? username)
		{
			_presence.State = !isOnline ? "Listening offline" : $"listening on {ip} as {username}";
			_client.SetPresence(_presence);
		}

		public void NotPlaying()
		{
			_presence.Details = "Not playing";
			_client.SetPresence(_presence);
		}
	}
}