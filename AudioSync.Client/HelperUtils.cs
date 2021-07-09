using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AudioSync.Client
{
	public static class HelperUtils
	{
		public static Task RunOnNewThread(this Task task) => Task.Factory.StartNew(task.Wait);

		public static ILogger<T> CreateLogger<T>() => new LoggerFactory().CreateLogger<T>();
	}
}