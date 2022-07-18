using System;
using System.IO;
using System.Text;
using FocusTimeAccumulator;
using FocusTimeAccumulator.Features.Bucket;
using FocusTimeAccumulator.Features.Pool;

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
	static FocusBucket bucket = new FocusBucket( );

	static System.Timers.Timer timer = new System.Timers.Timer( );
	public static async Task Main( )
	{
		//if the settings exist
		if ( File.Exists( appSettingfile ) )
			settings = SaveData.DeserializeJson<Settings>( appSettingfile );
		else
		{
			settings = new( ); //this was exposed, but i don't need it to be anymore
			SaveData.SerializeJson( appSettingfile, settings ); //save settings immediately
		}
		//set up timer to tick at the timespan set in the settings
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

		//if we did not get an input device ping in more than (settings.idleTime), add [Idle] tag to process
		if ( settings.idleModeEnabled && ( DateTime.Now - lastInput ) > settings.idleTime )
			appTitle = appTitle.Insert( 0, "[Idle] " );

		//limit app titles based on character length
		if ( appTitle.Length > settings.maxTitleLength )
			appTitle = appTitle[..settings.maxTitleLength ];

		//if the process name or title changes
		if ( currentActivePage != appTitle || currentActiveApp != appName)
		{	//do a global swap, swap prev with what now was last tick
			(prev, now) = (now, DateTime.Now);

			//check the flags to see if the user wants to run both pool and or buckets at the same time
			if (settings.focusSetting.HasFlag(Settings.FocusSetting.pool))
				pool.DoFocusPool( currentActiveApp, currentActivePage, PathMode.Daily );
			if ( settings.focusSetting.HasFlag( Settings.FocusSetting.bucket ) )
				bucket.DoFocusBucket( currentActiveApp, currentActivePage, PathMode.Daily );

			//set old name and title
			currentActiveApp = appName;
			currentActivePage = appTitle;
		}
	}
	

}