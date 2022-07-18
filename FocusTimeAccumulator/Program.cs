using System;
using System.IO;
using FocusTimeAccumulator;

class Program
{
	//possibly bad way to hold data
	static string file = "apps.json";
	static string appSettingfile = "settings.json";
	static Settings settings; 

	//app is the process without a title, page is the process title
	//in the json edit the shared bool to true on apps
	//which you don't want to save individual page titles on
	//(such as spotify or microsoft edge/chrome)
	static string currentActiveApp = "";
	static string currentActivePage = "";

	static DateTime prev = DateTime.Now;
	static DateTime now = DateTime.Now;

	static List<App> apps;
	static System.Timers.Timer timer = new System.Timers.Timer( );
	public static async Task Main( )
	{
		//make a new list for storing saved app data
		apps = File.Exists( file ) ? SaveData.DeserializeJson<List<App>>( file ) : new( );
		if ( File.Exists( appSettingfile ) )
		{
			settings = SaveData.DeserializeJson<Settings>( appSettingfile );
		} else
		{
			settings = new( );
			settings.idleModeEnabled = true;
			settings.tickTime = TimeSpan.FromSeconds(1);
			settings.idleTime = TimeSpan.FromMinutes(3);
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
		if ( currentActivePage != appTitle )
		{
			(prev, now) = (now, DateTime.Now);
			//get the current page if it's different
			var activeApp = apps.Where( a => a.name == currentActivePage );
			var activeAppSetting = appSettings.Where( s => s.proc == currentActiveApp );
			//if the current page has a profile
			if ( activeApp.Any( ) )
			{
				//get the profile
				var appProfile = activeApp.ToList( )[ 0 ];
				var isu = appSettings.Where( s => s.proc == currentActiveApp );
				//check if the profile belongs to a shared app
				if ( isu.Any( ) && isu.ToList( )[ 0 ].shared )
				{
					//calculate time span
					appProfile.span += now - prev; //add time span
					Console.WriteLine( $"exiting shared app {currentActiveApp} after {now - prev}. total time: {appProfile.span}" );
				}
				else//if it's not a shared app
				{
					var currentApp = apps.Where( a => a.name == currentActivePage );
					if ( currentApp.Any( ) )
					{ //get the non shared profile
						var appProfile2 = activeApp.ToList( )[ 0 ];
						appProfile2.span += now - prev; //add time span
						Console.WriteLine( $"exiting {currentActivePage} after {now - prev}. total time: {appProfile.span}" );
					}
				}
			}

			var f = appSettings.Where( a => a.proc == appName ).ToList( );
			if ( f.Any( ) && f.Any( x => x.shared ) )
			{
				var aps = apps.Where( a => a.proc == appName );
				if ( !aps.Any( ) )
				{
					Console.WriteLine( $"adding new shared app: {appName}" );
					apps.Add( new( )
					{
						name = appTitle,
						proc = appName,
						span = TimeSpan.Zero //new apps have no span
					} );
				}
			}
			else
			{

				var aps = apps.Where( a => a.name == appTitle );
				if ( !aps.Any( ) )
				{
					Console.WriteLine( $"adding new app: {appTitle}" );
					apps.Add( new( )
					{
						name = appTitle,
						proc = appName,
						span = TimeSpan.Zero //new apps have no span
					} );
				}
			}
			currentActiveApp = appName;
			currentActivePage = appTitle;
			SaveData.SerializeJson( file, apps ); //save json
		}
	}
}