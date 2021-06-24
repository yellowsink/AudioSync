using System.Collections.Generic;

namespace AudioSync.Server
{
	public class DataService : IDataService
	{
		private readonly Dictionary<string, object> _store = new();

		public void Set(string name, object value) => _store[name] = value;

		public object Get(string name) => _store[name];

		public bool TryGet(string name, out object? value) => _store.TryGetValue(name, out value);

		public bool Exists(string name) => _store.ContainsKey(name);

		public void Remove(string name) => _store.Remove(name);
	}

	public interface IDataService
	{
		public void Set(string name, object value);

		public object Get(string name);

		public bool TryGet(string name, out object? value);

		public bool Exists(string name);
		public void Remove(string name);
	}
}