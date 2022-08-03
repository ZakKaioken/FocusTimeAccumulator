﻿using System.Diagnostics;
using System.Threading;
using FocusTimeAccumulator;
using FocusTimeAccumulator.Features.Bucket;
using FocusTimeAccumulator.Features.Plugins;
using FocusTimeAccumulator.Features.Pool;
using FocusTimeAccumulator.Features.Similarity;
using FocusTimeAccumulator.IO;
using Newtonsoft.Json;

public class Program
{
	public static string appSettingfile = "settings.json";
	public static Settings settings;
	public static bool exiting = false;
	public static string prevName = "";
	public static string prevTitle = "";

	public static FocusPool pool = new FocusPool( );
	public static FocusBucket bucket = new FocusBucket( );

	static System.Timers.Timer timer = new System.Timers.Timer( );
	public static List<Plugin?>? plugins = new List<Plugin?>( );
	public static async Task Main( )
	{
		//if the settings exist
		if ( File.Exists( appSettingfile ) )
			settings = SaveData.DeserializeJson<Settings>( appSettingfile );
		else
		{
			settings = new( ); //this was exposed, but i don't need it to be anymore
			settings.appSettings.Add( new( ) );
		}

		//allow any updates to the settings file to fill the important missing json data in the file
		SaveData.SerializeJson( appSettingfile, settings ); //save settings immediately


		try
		{
			plugins = PluginLoader.LoadPlugins<Plugin?>( settings.pluginPath );
		}
		catch ( Exception e )
		{
			Console.WriteLine( e );
			CrashDump.Dump( e );
		}

		plugins?.ForEach( x => x?.OnStart( ) );

		//set up timer to tick at the timespan set in the settings
		timer.Elapsed += ( _, _ ) => Tick( );
		timer.Interval = settings.tickTime.TotalMilliseconds;

		Console.CancelKeyPress += ( _, e ) =>
		{
			if ( exiting ) //if were already exiting, don't print again
				return;

			e.Cancel = true;
			exiting = true;
			Console.WriteLine( "ctrl+c was pressed, saving..." );
			Tick( );
		};

		SetInitialNames();
		
		//start timer and then hold the program open indefinitely
		timer.Start( );
		await Task.Delay( -1 );
		timer.Stop( );
		//timer.Dispose( );
	}

	static List<Thread> threads = new List<Thread>();

	[STAThread]
	static void AddTickThread(ChangeData cd) {
		var thread = new Thread( TickThread );
		threads.Add( thread );
		thread.IsBackground = false;
		thread.Start( cd );
	}



	public static void SetInitialNames() {
		var appProcess = FocusFinder.WindowsProcessFocusApi.GetForegroundProcess( );
		var appName = appProcess?.ProcessName ?? "Unknown";
		var appTitle = appProcess?.MainWindowTitle ?? "Unknown";
		var lastInput = FocusFinder.WindowsProcessFocusApi.GetLastInputTime( );

		// Cache process info for later use
		ProcessCache.Cache(appName, appProcess);

		//if we did not get an input device ping in more than (settings.idleTime), add [Idle] tag to process
		if ( settings.idleModeEnabled && ( DateTime.Now - lastInput ) > settings.idleTime )
			appTitle = appTitle.Insert( 0, "[Idle] " );
		//limit app titles based on character length
		if ( appTitle.Length > settings.maxTitleLength )
			appTitle = appTitle[ ..settings.maxTitleLength ];
			prevName = appName;
			prevTitle = appTitle;
	}

	[STAThread]
	public static void Tick( ) {
		try
		{
			plugins?.ForEach( x => x?.OnTick( ) );
			//better foreground process finding suggestion from issue #2
			var appProcess = FocusFinder.WindowsProcessFocusApi.GetForegroundProcess( );
			var appName = appProcess?.ProcessName ?? "Unknown";
			var appTitle = appProcess?.MainWindowTitle ?? "Unknown";
			var lastInput = FocusFinder.WindowsProcessFocusApi.GetLastInputTime( );

			// Cache process info for later use
			ProcessCache.Cache(appName, appProcess);

			//if we did not get an input device ping in more than (settings.idleTime), add [Idle] tag to process
			if ( settings.idleModeEnabled && ( DateTime.Now - lastInput ) > settings.idleTime )
				appTitle = appTitle.Insert( 0, "[Idle] " );

			//limit app titles based on character length
			if ( appTitle.Length > settings.maxTitleLength )
				appTitle = appTitle[ ..settings.maxTitleLength ];

			//if the process name or title changes
			if ( appProcess != null && (exiting || prevTitle != appTitle || prevName != appName ) )
			{
					var cd = new ChangeData( )
					{
						appTitle = appTitle,
						appName = appName,
						prvName = prevName,
						prvTitle = prevTitle,
						appProcess = appProcess
					};
					AddTickThread( cd );
				}
				
			}

		catch ( Exception e )
		{
			Console.WriteLine( e );

			// If console closes, we can look at a crash dump.
			CrashDump.Dump( e );
		}
		
		
	}
	class ChangeData {
		public Process appProcess;
		public string prvName, prvTitle, appName, appTitle;

		internal void Deconstruct( out Process appProcess, out string prvName, out string prvTitle, out string appName, out string appTitle )
		{
			appProcess = this.appProcess;
			prvName = this.prvName;
			prvTitle = this.prvTitle;
			appName = this.appName;
			appTitle = this.appTitle;
		}	
	}
	static void TickThread( object data) {

		var (appProcess, prvName, prvTitle, appName, appTitle) = (ChangeData)data;
		
		try
		{
			plugins?.ForEach( x => x?.OnProcessChanged( appProcess, prvName, prvTitle, appName, appTitle ) );


			//check the flags to see if the user wants to run both pool and or buckets at the same time
			if ( settings.focusSetting.HasFlag( Settings.FocusSetting.pool ) )
				pool.DoFocusPool( prevName, prevTitle );

			if ( settings.focusSetting.HasFlag( Settings.FocusSetting.bucket ) )
				bucket.DoFocusBucket( prevName, prevTitle );

			//set old name and title
			prevName = appName;
			prevTitle = appTitle;
			//
			plugins?.ForEach( x => x?.OnProcessChanged( appProcess, prvName, prvTitle, appName, appTitle ) );
			Thread.Sleep( 3000 );
			if ( exiting )
			{
				Console.WriteLine( $"saved you can now safely close the terminal" );
				Environment.Exit( 0 );
			}
		} catch (Exception e) {
			Console.WriteLine( e );

			// If console closes, we can look at a crash dump.
			CrashDump.Dump( e );
		}

		//remove this thread given it is no longer in use
		threads.Remove( Thread.CurrentThread );
	}

}