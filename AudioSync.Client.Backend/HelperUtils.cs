using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AudioSync.Client.Backend
{
	public static class HelperUtils
	{
		public static Task RunOnNewThread(this Task task) => Task.Factory.StartNew(task.Wait);

		public static ILogger<T> CreateLogger<T>() => new LoggerFactory().CreateLogger<T>();

		public static bool VersionIsNewerThan(this string current, string next)
			=> string.Compare(current, next, StringComparison.Ordinal) > 0;
	}
}