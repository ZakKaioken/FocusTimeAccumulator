﻿using System.Collections.Generic;
using System.IO;
using FocusTimeAccumulator.Features.Pool;
using FocusTimeAccumulator.Features.Similarity;
using FocusTimeAccumulator.IO;
using static Program;
namespace FocusTimeAccumulator.Features.Bucket
{
	public class FocusBucket
	{

		public DateTime prev = DateTime.Now;
		public DateTime now = DateTime.Now;


		public Dictionary<string, BucketApp> cachedApps = new Dictionary<string, BucketApp>( );
		public void DoFocusBucket( string appName, string appTitle )
		{
			(prev, now) = (now, DateTime.Now);

			string path = SaveData.CreatePath( appName, "Buckets", settings.fileStructure, settings.timeStampFormat );
			var app = GetApp( appName, appTitle, path );
			var setting = settings.appSettings.Where( a => a.proc == appName ).ToList( ).FirstOrDefault( );


			if ( settings.doPoolSimilarityChecking )
			{
				appTitle = SimilarityCheck.GetSimilarBucketTitle( app, path, appName, appTitle );
			}

			//make an app for this item, without profile checking (the file seperates buckets based on process and path mode)
			CreateApp( app, appTitle );


			app.poolPackets = app.poolPackets.OrderBy( d => d.time ).ToList( );//sort based on time
			SaveData.SerializeJson( path, app ); //save json
		}

		BucketApp GetApp(string appName, string appTitle, string path) {

			//if an app is cached, return that app
			if ( cachedApps.ContainsKey( appName ) )
				return cachedApps[ appName ];

			BucketApp app = null;

			//if no app is cached we attempt to deserialize it from the disk
			if ( File.Exists( path ) )
			{
				app = SaveData.DeserializeJson<BucketApp>( path );
				if (app==null)
					SaveData.CopyFile( path, @"PossiblyCorrupt\" );
			}
			// Some times the Deserialization fails due to empty or corrupt json files
			// So we make sure the app is initialized
			// !!! This will overwrite corrupt files !!!
			app ??= new( appName );
			

			//then cache the app
			cachedApps.Add( appName, app );
			return app;
		}

		void CreateApp( BucketApp app, string title )
		{
			//check the dictionary for an id with the title,
			int id;
			if ( !app.titles.ContainsKey( title ) )
			{ //if there are no ids, set the id equal to the current size of the dictionary then add it as the key
				id = app.titles.Count;
				app.titles.Add(title, id );
			}
			else //if there is an id just get the id back
				id = app.titles[title];


			//calculate timespan
			var span = now - prev;
			if ( settings.focusConsoleSetting.HasFlag( Settings.FocusSetting.bucket ) )
			{
				var message = MessageBuilder.BuildMessage( settings.bucketAdd, now, app.name, title, span );
				Console.WriteLine( message );
			}
			BucketApp.AppSpan packet = new( )
			{
				pageTitle = id,
				time = DateTime.Now - span, //subtract span from now to get start time
				span = span //set the span
			};

			plugins?.ForEach( x => x?.ModifyBucketPacketCreation( packet ) );
			app.poolPackets.Add( packet );
		}

	}
}
