using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusTimeAccumulator.Features.Pool;
using static Program;
namespace FocusTimeAccumulator.Features.Bucket
{
    public class FocusBucket
    {
		public void DoFocusBucket( string appName, string appTitle )
        {
			string path = SaveData.CreatePath( appName, settings.poolFileStructure, settings.timeStampFormat );
			var app = File.Exists( path ) ? SaveData.DeserializeJson<BucketApp>( path ) : new BucketApp( appName );
			var setting = settings.appSettings.Where( a => a.proc == appName ).ToList( ).FirstOrDefault( );
			
			//make an app for this item, without profile checking (the file seperates buckets based on process and path mode)
			CreateApp( app, appTitle );

			app.poolPackets = app.poolPackets.OrderBy( d => d.time ).ToList( );//sort based on time
			SaveData.SerializeJson( path, app ); //save json
		}

		void CreateApp( BucketApp app, string pageTitle )
		{
			//check the dictionary for an id with the title,
			int id;
			if ( !app.titles.ContainsKey( pageTitle ) )
			{ //if there are no ids, set the id equal to the current size of the dictionary then add it as the key
				id = app.titles.Count;
				app.titles.Add( pageTitle, id );
			}
			else //if there is an id just get the id back
				id = app.titles[ pageTitle ];
			//calculate timespan
			var span = now - prev;
			app.poolPackets.Add( new( )
			{
				pageTitle = id, 
				time = DateTime.Now-span, //subtract span from now to get start time
				span = span //set the span
			} );
		}

	}
}
