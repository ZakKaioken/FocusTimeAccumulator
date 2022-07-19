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

		public static System.Diagnostics.Process? GetForegroundProcess( )
		{
			try
			{
				if ( GetWindowThreadProcessId( GetForegroundWindow( ), out uint pid ) != 0 )
					return System.Diagnostics.Process.GetProcessById( (int)pid );
			}
			catch { }
			return null;
		}

	}
}