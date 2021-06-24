using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace AudioSync.Client
{
	public class SyncClient : IDisposable
	{
		private HubConnection _connection;
		
		public SyncClient(string url)
		{
			url = url.TrimEnd('/');
			
			_connection = new HubConnectionBuilder()
						 .WithUrl(url + "/chathub")
						 .WithAutomaticReconnect()
						 .Build();
		}

		public async Task Connect() => await _connection.StartAsync();

		public async Task Disconnect() => await _connection.StopAsync();

		public async Task Send(string endpoint, params object[] args) => await _connection.InvokeCoreAsync(endpoint, args);

		public async Task<T> Send<T>(string endpoint, params object[] args) => (T) await _connection.InvokeCoreAsync(endpoint, typeof(T), args);

		public void AddEventHandler(string endpoint, Type[] types, Func<object[], Task> handler) => _connection.On(endpoint, types, handler);

		public       void Dispose()      => DisposeAsync().GetAwaiter().GetResult();

		private async Task DisposeAsync() => await _connection.DisposeAsync();
	}
}