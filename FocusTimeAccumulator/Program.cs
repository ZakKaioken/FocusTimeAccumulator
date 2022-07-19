using System;
using System.IO;
using System.Text;
using Algorithms;
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
	public static int similarCount = 0;

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

		
		//checking if a program's title matches the similar titles in the settings
		//with an added process verification to make sure we're not
		//merging two similar titles from different programs
		var matchingSimilarTitles = settings.appSettings.Where( o => o.proc == appName && o.combineSimilarTitles && appTitle.Contains( o.similarTitles ) );
		if ( matchingSimilarTitles.Any( ) )
			appTitle = matchingSimilarTitles.First( ).similarTitles; //all similar titles will use the similar title instead of the original title

		//"Reloading Assemblies (56%)" will become "Reloading Assemlies"
		//i want to build in a system automatically adding titles that spawn similar too often
		if ( currentActivePage != appTitle || currentActiveApp != appName )
		if ( SimilarCheck( appTitle ) ) return;

		//if we did not get an input device ping in more than (settings.idleTime), add [Idle] tag to process
		if ( settings.idleModeEnabled && ( DateTime.Now - lastInput ) > settings.idleTime )
			appTitle = appTitle.Insert( 0, "[Idle] " );

		//limit app titles based on character length
		if ( appTitle.Length > settings.maxTitleLength )
			appTitle = appTitle[..settings.maxTitleLength ];

		//if the process name or title changes
		if ( currentActivePage != appTitle || currentActiveApp != appName)
		{

			//do a global swap, swap prev with what now was last tick
			(prev, now) = (now, DateTime.Now);

			//check the flags to see if the user wants to run both pool and or buckets at the same time
			if (settings.focusSetting.HasFlag(Settings.FocusSetting.pool))
				pool.DoFocusPool( currentActiveApp, currentActivePage );
			if ( settings.focusSetting.HasFlag( Settings.FocusSetting.bucket ) )
				bucket.DoFocusBucket( currentActiveApp, currentActivePage );

			//set old name and title
			currentActiveApp = appName;
			currentActivePage = appTitle;
		}
	}
	public static bool SimilarCheck(string appTitle) { 


		var distance = LevenshteinDistance.Calculate( appTitle, currentActivePage );
		if ( distance <= 5 )
			similarCount++;
		else
			similarCount = 0;

		if ( similarCount > 2 )
		{
			var c = settings.similarSuggestions.Where( ss => LevenshteinDistance.Calculate( ss, appTitle ) < 10 );
			if ( !c.Any( ) )
			{
				//found a new similar suggestion that is not similar to one 
				settings.similarSuggestions.Add( appTitle );
				Console.WriteLine( $"adding a new suggestion to the similar suggestions part of the {appSettingfile} file" );
				SaveData.SerializeJson( appSettingfile, settings ); //save new suggestion
			}
		}

		//i want to implement automatic grouping and merging for similar items
		if ( similarCount > 2 )
		{
			Console.WriteLine( "refusing add more similar apps" );
			return true;
		}
		return false;
	}

}