using System;
using System.IO;

namespace AudioSync.Client.Backend
{
	// ReSharper disable once InconsistentNaming
	public static class OSDefaults
	{
		public static readonly bool IsOnWindows
			= Environment.OSVersion.Platform is PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT or
				  PlatformID.WinCE;

#region CacheLocation

		private static string DefaultWindowsCacheLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
							@"AudioSyncCache");

		private static string DefaultUnixCacheLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache/AudioSync");

		private static string DefaultMacCacheLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
							"Library/Caches/AudioSync");

		// ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
		public static string DefaultCacheLocation => Environment.OSVersion.Platform switch
		{
			PlatformID.Win32S       => DefaultWindowsCacheLocation,
			PlatformID.Win32Windows => DefaultWindowsCacheLocation,
			PlatformID.Win32NT      => DefaultWindowsCacheLocation,
			PlatformID.WinCE        => DefaultWindowsCacheLocation,
			PlatformID.Unix         => DefaultUnixCacheLocation,
			PlatformID.MacOSX       => DefaultMacCacheLocation,
			_                       => throw new ArgumentOutOfRangeException()
		};

#endregion

#region DownloadLocation

		private static string DefaultWindowsDownloadLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
							@"Temp\AudioSyncDownloads");

		private const string DefaultUnixDownloadLocation = "/tmp/audiosync_downloads";

		private const string DefaultMacDownloadLocation = DefaultUnixDownloadLocation;

		// ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
		public static string DefaultDownloadLocation => Environment.OSVersion.Platform switch
		{
			PlatformID.Win32S       => DefaultWindowsDownloadLocation,
			PlatformID.Win32Windows => DefaultWindowsDownloadLocation,
			PlatformID.Win32NT      => DefaultWindowsDownloadLocation,
			PlatformID.WinCE        => DefaultWindowsDownloadLocation,
			PlatformID.Unix         => DefaultUnixDownloadLocation,
			PlatformID.MacOSX       => DefaultMacDownloadLocation,
			_                       => throw new ArgumentOutOfRangeException()
		};

#endregion

#region ToolLocation

		private static string DefaultWindowsToolLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
							@"AudioSyncTools");

		private static string DefaultUnixToolLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".audiosync_tools");

		private static string DefaultMacToolLocation => DefaultUnixToolLocation;

		// ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
		public static string DefaultToolLocation => Environment.OSVersion.Platform switch
		{
			PlatformID.Win32S       => DefaultWindowsToolLocation,
			PlatformID.Win32Windows => DefaultWindowsToolLocation,
			PlatformID.Win32NT      => DefaultWindowsToolLocation,
			PlatformID.WinCE        => DefaultWindowsToolLocation,
			PlatformID.Unix         => DefaultUnixToolLocation,
			PlatformID.MacOSX       => DefaultMacToolLocation,
			_                       => throw new ArgumentOutOfRangeException()
		};

#endregion

#region ToolFileNames

		private const string DefaultWindowsYtdlFileName = "ytdl.exe";

		private const string DefaultUnixYtdlFileName = "ytdl";

		private const string DefaultMacYtdlFileName = DefaultUnixYtdlFileName;

		// ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
		public static string DefaultYtdlFileName => Environment.OSVersion.Platform switch
		{
			PlatformID.Win32S       => DefaultWindowsYtdlFileName,
			PlatformID.Win32Windows => DefaultWindowsYtdlFileName,
			PlatformID.Win32NT      => DefaultWindowsYtdlFileName,
			PlatformID.WinCE        => DefaultWindowsYtdlFileName,
			PlatformID.Unix         => DefaultUnixYtdlFileName,
			PlatformID.MacOSX       => DefaultMacYtdlFileName,
			_                       => throw new ArgumentOutOfRangeException()
		};

#endregion

#region ConfigLocation

		private static string DefaultWindowsConfigLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
							@"AudioSync\config.json");

		private static string DefaultUnixConfigLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
							".config/audiosync_config.json");

		private static string DefaultMacConfigLocation
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
							"Library/AudioSync/config.json");

		// ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
		public static string DefaultConfigLocation => Environment.OSVersion.Platform switch
		{
			PlatformID.Win32S       => DefaultWindowsConfigLocation,
			PlatformID.Win32Windows => DefaultWindowsConfigLocation,
			PlatformID.Win32NT      => DefaultWindowsConfigLocation,
			PlatformID.WinCE        => DefaultWindowsConfigLocation,
			PlatformID.Unix         => DefaultUnixConfigLocation,
			PlatformID.MacOSX       => DefaultMacConfigLocation,
			_                       => throw new ArgumentOutOfRangeException()
		};

#endregion
	}
}