namespace AudioSync.Shared
{
	public class User
	{
		public string     Name;
		public UserStatus Status = UserStatus.Ready;
		public bool       IsMaster = false;

		public User(string name) => Name = name;
	}

	public enum UserStatus
	{
		Ready,
		DownloadingCurrentSong,
		DownloadingSongs,
		UpdatingTools
	}
}