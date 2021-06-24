using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace AudioSync.Client
{
	public partial class SyncClient : IDisposable
	{
		private HubConnection _connection;

		public bool   IsMaster { get; private set; }
		public string Name     { get; }

		public SyncClient(string url, string name, bool isMaster = false)
		{
			url =  url.TrimEnd('/');
			url += "/synchub";
			
			_connection = new HubConnectionBuilder()
						 .AddJsonProtocol()
						 .WithUrl(url)
						 .WithAutomaticReconnect()
						 .Build();

			IsMaster = isMaster;
			Name     = name;
			
			SetupEvents();
		}

		public async Task Connect()
		{
			await _connection.StartAsync();
			if (IsMaster)
			{
				var result           = await _connection.InvokeAsync<bool>("ConnectMaster", Name);
				if (result) IsMaster = false;
				await _connection.InvokeAsync("ConnectClient", Name);
			}
			else
				await _connection.InvokeAsync("ConnectClient", Name);
		}

		public async Task Disconnect()
		{
			if (IsMaster)
				await _connection.InvokeAsync("DisconnectMaster");
			else
				await _connection.InvokeAsync("DisconnectClient");
			
			await _connection.StopAsync();
		}

		public       void Dispose()      => DisposeAsync().Wait();

		private async Task DisposeAsync()
		{
			if (_connection.State != HubConnectionState.Disconnected) await Disconnect();
			await _connection.DisposeAsync();
		}
	}
}