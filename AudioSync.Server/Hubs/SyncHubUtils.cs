using System.Collections.Generic;
using AudioSync.Shared;

namespace AudioSync.Server.Hubs
{
	public partial class SyncHub
	{
		private bool IsMaster() => _state.MasterId == Context.ConnectionId;

		private void SetOrAddUser(User user, string connectionId = null)
			=> _state.Users[connectionId ?? Context.ConnectionId] = user;

		private User GetUser(string connectionId = null)
			=> _state.Users.GetValueOrDefault(connectionId ?? Context.ConnectionId);

		private void RemoveUser(string connectionId = null)
		{
			if (!_state.Users.ContainsKey(connectionId ?? Context.ConnectionId)) return;
			_state.Users.Remove(connectionId ?? Context.ConnectionId);
		}

		private bool RegisterName(string name)
		{
			if (IsNameRegistered(name)) return false;

			_state.Names.Add(name);
			return true;
		}

		private void RemoveNameIfRegistered(string name)
		{
			if (!IsNameRegistered(name)) return;
			_state.Names.Remove(name);
		}

		private bool IsNameRegistered(string name) => _state.Names.Contains(name);
	}
}