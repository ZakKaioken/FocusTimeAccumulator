using System.Collections.Generic;
using FocusTimeAccumulator.Features.Similarity;
using FocusTimeAccumulator.IO;
using static Program;
namespace FocusTimeAccumulator.Features.Bucket
{
	public class FocusBucket
	{

		public DateTime prev = DateTime.Now;
		public DateTime now = DateTime.Now;

		public string previousTitle="oil";

		public void DoFocusBucket( string appName, string appTitle )
		{
			(prev, now) = (now, DateTime.Now);
			
			string path = SaveData.CreatePath( appName, "Buckets", settings.fileStructure, settings.timeStampFormat );
			var app = File.Exists( path ) ? SaveData.DeserializeJson<BucketApp>( path ) : new BucketApp( appName );
			var setting = settings.appSettings.Where( a => a.proc == appName ).ToList( ).FirstOrDefault( );


			if ( settings.doPoolSimilarityChecking )
			{// this is really bad :(
				appTitle = SimilarityCheck.GetSimilarBucketTitle( app, path, appName, appTitle );

				//wierd redo of the tech in the program.cs
				//this does change detection but with similar titles in mind
				//this was made here because we don't want the similar tech in buckets

				if ( !Program.exiting && appTitle == previousTitle )
					return;
				previousTitle = appTitle;
			}

			//make an app for this item, without profile checking (the file seperates buckets based on process and path mode)
			CreateApp( app, appTitle );

			app.poolPackets = app.poolPackets.OrderBy( d => d.time ).ToList( );//sort based on time
			SaveData.SerializeJson( path, app ); //save json
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
			app.poolPackets.Add( new( )
			{
				pageTitle = id,
				time = DateTime.Now - span, //subtract span from now to get start time
				span = span //set the span
			} );
		}

	}
}
