using System.Diagnostics;

namespace AudioSync.Shared
{
	[DebuggerDisplay("{Name} ({Status})")]
	public class User
	{
		public User(string name) { Name = name; }

		public string     Name     { get; set; }
		public UserStatus Status   { get; set; } = UserStatus.Ready;
		public bool       IsMaster { get; set; }
	}

	public enum UserStatus
	{
		Ready,
		DownloadingCurrentSong,
		DownloadingSongs,
		UpdatingTools
	}
}