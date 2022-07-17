using System.Runtime.InteropServices;

namespace FocusFinder
{
	class WindowsProcessFocusApi
	{
		// The GetForegroundWindow function returns a handle to the foreground window
		// (the window  with which the user is currently working).
		[System.Runtime.InteropServices.DllImport( "user32.dll" )]
		public static extern IntPtr GetForegroundWindow( );


		[DllImport( "user32.dll" )]
		static extern bool GetLastInputInfo( ref LASTINPUTINFO plii );
		public static DateTime GetLastInputTime( )
		{
			var lastInputInfo = new LASTINPUTINFO( );
			lastInputInfo.cbSize = (uint)Marshal.SizeOf( lastInputInfo );
			GetLastInputInfo( ref lastInputInfo );
			return DateTime.Now.AddMilliseconds( -( Environment.TickCount - lastInputInfo.dwTime ) );
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		}

		// The GetWindowThreadProcessId function retrieves the identifier of the thread
		// that created the specified window and, optionally, the identifier of the
		// process that created the window.
		[System.Runtime.InteropServices.DllImport( "user32.dll" )]
		public static extern Int32 GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId );

		// Returns the name of the process owning the foreground window.
		public static string GetForegroundProcessName( )
		{
			IntPtr hwnd = GetForegroundWindow( );

			// The foreground window can be NULL in certain circumstances,
			// such as when a window is losing activation.
			if ( hwnd == null )
				return "Unknown";

			uint pid;
			GetWindowThreadProcessId( hwnd, out pid );

			foreach ( System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses( ) )
			{
				if ( p.Id == pid )
					return p.ProcessName;
			}

			return "Unknown";
		}
		public static string GetForegroundProcessTitle( )
		{
			IntPtr hwnd = GetForegroundWindow( );

			// The foreground window can be NULL in certain circumstances,
			// such as when a window is losing activation.
			if ( hwnd == null )
				return "Unknown";

			uint pid;
			GetWindowThreadProcessId( hwnd, out pid );

			foreach ( System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses( ) )
			{
				if ( p.Id == pid )
					return p.MainWindowTitle;
			}

			return "Unknown";
		}
	}
}