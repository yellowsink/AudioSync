using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace AudioSync.Client.Backend
{
	/// <summary>
	///     Syncs multiple instances via an AudioSync.Server connection
	/// </summary>
	public partial class ServerSyncAgent : ISyncAgent
	{
		private readonly HubConnection            _connection;
		private readonly ILogger<ServerSyncAgent> _logger = HelperUtils.CreateLogger<ServerSyncAgent>();

		public string Url;

#pragma warning disable 8618
		public ServerSyncAgent(string url, string name, bool isMaster = false)
#pragma warning restore 8618
		{
			Url =  url.TrimEnd('/');
			Url += "/synchub";

			_connection = new HubConnectionBuilder().WithUrl(Url).WithAutomaticReconnect().Build();

			IsMaster = isMaster;
			Name     = name;

			SetupEvents();
		}

		public bool   IsMaster { get; private set; }
		public string Name     { get; private set; }

		public void Dispose() => DisposeAsync().Wait();

		public async Task Connect()
		{
			await _connection.StartAsync();
			if (IsMaster)
			{
				// Will fail if there is already a master.
				var result            = await _connection.InvokeAsync<bool>("ConnectMaster", Name);
				if (!result) IsMaster = false;
			}

			// Does nothing if you are the master, else connects as a normal client
			await _connection.InvokeAsync("ConnectClient", Name);

			_logger.LogInformation($"Connected as a {(IsMaster ? "master" : "client")}");
		}

		public async Task Disconnect()
		{
			if (IsMaster)
				await _connection.InvokeAsync("DisconnectMaster");
			else
				await _connection.InvokeAsync("DisconnectClient");

			await _connection.StopAsync();

			_logger.LogInformation("Disconnected");
		}

		private async Task DisposeAsync()
		{
			if (_connection.State != HubConnectionState.Disconnected) await Disconnect();
			await _connection.DisposeAsync();
		}
	}
}