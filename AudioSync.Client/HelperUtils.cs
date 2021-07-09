using System.Threading.Tasks;

namespace AudioSync.Client
{
	public static class HelperUtils
	{
		public static Task RunOnNewThread(this Task task) => Task.Factory.StartNew(task.Wait);
	}
}