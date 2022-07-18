using System;
using System.IO;
using FocusTimeAccumulator;
using FocusTimeAccumulator.Features;

class Program
{
	//possibly bad way to hold data
	static string file = "apps.json";
	static string appSettingfile = "settings.json";
	public static Settings settings; 

	//app is the process without a title, page is the process title
	//in the json edit the shared bool to true on apps
	//which you don't want to save individual page titles on
	//(such as spotify or microsoft edge/chrome)
	static string currentActiveApp = "";
	static string currentActivePage = "";

	public static DateTime prev = DateTime.Now;
	public static DateTime now = DateTime.Now;

	static FocusPool pool = new FocusPool( );

	static List<App> apps;
	static System.Timers.Timer timer = new System.Timers.Timer( );
	public static async Task Main( )
	{
		//make a new list for storing saved app data
		
		if ( File.Exists( appSettingfile ) )
		{
			settings = SaveData.DeserializeJson<Settings>( appSettingfile );
		} else
		{
			settings = new( )
			{
				idleModeEnabled = true,
				tickTime = TimeSpan.FromSeconds( 1 ),
				idleTime = TimeSpan.FromMinutes( 3 )
			};
			settings.appSettings.Add( new( ) { proc = "", shared = true } );
			SaveData.SerializeJson( appSettingfile, settings );
		}
		//set up timer to tick every second
		timer.Elapsed += ( _, _ ) => Tick( );
		timer.Interval = settings.tickTime.TotalMilliseconds;
		//start timer and then hold the program open indefinitely
		timer.Start( );
		await Task.Delay( -1 );
	}
	static void Tick( )
	{
		//better foreground process finding suggestion from issue #2
		var appProcess = FocusFinder.WindowsProcessFocusApi.GetForegroundProcess( ); 
		var appName = appProcess?.ProcessName ?? "Unknown";
		var appTitle = appProcess?.MainWindowTitle ?? "Unknown";
		var lastInput = FocusFinder.WindowsProcessFocusApi.GetLastInputTime( );

		if ( settings.idleModeEnabled && ( DateTime.Now - lastInput ) > settings.idleTime )
			appTitle = appTitle.Insert( 0, "[Idle] " );

		if ( appTitle.Length > 128 )
			appTitle = appTitle.Substring( 0, 128 );

		var appSettings = settings.appSettings;
		if ( currentActivePage != appTitle || currentActiveApp != appName)
		{
			(prev, now) = (now, DateTime.Now);

			pool.DoFocusPool(appName, appTitle, PathMode.Daily );

			currentActiveApp = appName;
			currentActivePage = appTitle;
		}
	}

}