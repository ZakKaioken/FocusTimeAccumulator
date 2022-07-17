using FocusTimeAccumulator;

class Program
{
	//possibly bad way to hold data
	static string file = "apps.json";
	static string appSettingfile = "sharedApps.json";
	static float idleTime=3;
	static bool idleModeEnabled = true;
	//app is the process without a title, page is the process title
	//in the json edit the shared bool to true on apps
	//which you don't want to save individual page titles on
	//(such as spotify or microsoft edge/chrome)
	static string currentActiveApp = "";
	static string currentActivePage = "";

	static DateTime prev = DateTime.Now;
	static DateTime now = DateTime.Now;

	static List<App> apps;
	static List<AppSetting> appSettings;
	static System.Timers.Timer timer = new System.Timers.Timer( );
	public static async Task Main( )
	{
		//make a new list for storing saved app data
		apps = File.Exists( file ) ? SaveData.DeserializeJson<List<App>>( file ) : new( );
		if ( File.Exists( appSettingfile ) )
		{
			appSettings = SaveData.DeserializeJson<List<AppSetting>>( appSettingfile );
		} else
		{
			appSettings = new( );
			appSettings.Add( new( ) { proc = "", shared = true } );
			SaveData.SerializeJson( appSettingfile, appSettings );
		}
		//set up timer to tick every second
		timer.Elapsed += ( _, _ ) => Tick( );
		timer.Interval = 1000;
		//start timer and then hold the program open indefinitely
		timer.Start( );
		await Task.Delay( -1 );
	}
	static void Tick( )
	{
		var appName = FocusFinder.WindowsProcessFocusApi.GetForegroundProcessName( );
		var appTitle = FocusFinder.WindowsProcessFocusApi.GetForegroundProcessTitle( );
		var lastInput = FocusFinder.WindowsProcessFocusApi.GetLastInputTime( );

		if ( idleModeEnabled && ( DateTime.Now - lastInput ).TotalMinutes > idleTime )
			appTitle = appTitle.Insert( 0, "[Idle] " );

		if ( appTitle.Length > 128 )
			appTitle = appTitle.Substring( 0, 128 );

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