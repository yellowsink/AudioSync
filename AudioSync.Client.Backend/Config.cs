using System.IO;
using System.Text.Json;

namespace AudioSync.Client.Backend
{
	/// <summary>
	///     Keeps track of the app's settings and saves them to / loads them from the disk
	/// </summary>
	public class Config
	{
		public void Save(string? configPath = null)
		{
			configPath ??= OSDefaults.DefaultConfigLocation;

			File.WriteAllText(configPath, JsonSerializer.Serialize(this));
		}

		public static Config Load(string? configPath = null)
		{
			configPath ??= OSDefaults.DefaultConfigLocation;

			if (File.Exists(configPath))
				return JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath)) ?? new Config();
			return new Config();
		}

		/// <summary>
		///		How many days to keep items in cache. Set null to keep forever.
		/// </summary>
		public int? CacheDaysThreshold { get; set; } = 30;
	}
}