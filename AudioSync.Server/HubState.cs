#nullable enable
using System.Collections.Generic;
using AudioSync.Shared;

namespace AudioSync.Server
{
	public class HubState
	{
		public string? MasterId = null;
		public bool    MasterExists => MasterId != null;
		
		public Dictionary<string, User> Users = new();
		public HashSet<string>          Names = new();

		public Queue Queue = new();
	}
}