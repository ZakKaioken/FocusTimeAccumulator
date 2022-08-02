using System;
using System.Net.Sockets;
using FocusTimeAccumulator.Features.Bucket;
using FocusTimeAccumulator.Features.Similarity;
using FocusTimeAccumulator.IO;
using static Program;

namespace FocusTimeAccumulator.Features.Pool
{
    public class FocusPool
    {
        public DateTime prev = DateTime.Now;
        public DateTime now = DateTime.Now;

        public Dictionary<string, PoolApp> cachedApps = new Dictionary<string, PoolApp>( );
        public string path;
		//this function name could go for an improvement
		public void DoFocusPool( string appName, string appTitle )
        {
			path = SaveData.CreatePath( appName, "Pools", settings.fileStructure, settings.timeStampFormat );
            var app = GetApp( appName );
			var setting = settings.appSettings.Where( a => a.proc == appName ).ToList( ).FirstOrDefault( );

            //if similar checking is enabled in the settings
            //get and replace the current title with the saved title in settings
            //this also will add any similar title that appears immediately after itself to the settings
            //i want to check and merge the similar items in the pool that are marked as merge in settings
            if ( settings.doPoolSimilarityChecking )
            {
                appTitle = SimilarityCheck.GetSimilarPoolTitle( app, path, appName, appTitle );
            }

			(prev, now) = (now, DateTime.Now);
			if ( app.poolPackets.Any( ) )
            {
                //check if the profile belongs to a shared app
                if ( setting != null && setting.shared )
                    UpdateSharedApp( app );
                else//if it's not a shared app
                    UpdateApp( app, appTitle );
            }

            if ( setting != null && setting.shared )
                CreateSharedApp( app );
            else
                CreateApp( app, appTitle );


            app.poolPackets = app.poolPackets.OrderBy( d => d.time ).ToList( );//sort based on time
            SaveData.SerializeJson( path, app ); //save json
        }

		public PoolApp GetApp( string appName )
		{
            //if an app is cached, return that app
            if ( cachedApps.ContainsKey( appName ) )
                return cachedApps[ appName ];

            PoolApp app;

            //if no app is cached we attempt to deserialize it from the disk
            if (File.Exists( path ))
                app = SaveData.DeserializeJson<PoolApp>( path );
            else
                //if there is no file to Deserialize we create a new empty app
                app = new( appName, ProcessCache.GetProductName(appName), ProcessCache.GetProductDescription(appName));

            // Some times the Deserialization fails due to empty or corrupt json files
            // So we make sure the app is initialized
            // !!! This will overwrite corrupt files !!!
            if (app == null)
                app = new( appName, ProcessCache.GetProductName(appName), ProcessCache.GetProductDescription(appName));
            
            //then cache the app
            cachedApps.Add( appName, app );
            return app;
		}

		void UpdateApp( PoolApp app, string title )
        {
            //if there is a packet
            var packets = app.poolPackets.Where( p => p.pageTitle == title ).ToList( );
            if ( packets.Any( ) )
            {
                var packet = packets[ 0 ];
                var span = now - prev;

				plugins?.ForEach( x => x?.ModifyPoolAppUpdate( packet ) );
				packet.span += now - prev; //update span
                packet.focusCount++;


                if ( settings.focusConsoleSetting.HasFlag( Settings.FocusSetting.pool ) )
                {
                    var message = MessageBuilder.BuildMessage( settings.poolAppUpdated, now, app.name, title, span, packet.span );
                    Console.WriteLine( message );
                }
            }
        }

        void CreateApp( PoolApp app, string title )
        {
            //if there is no profile for this title
            var aps = app.poolPackets.Where( a => a.pageTitle == title );
            if ( !aps.Any( ) )
            {
                //add a new app
                var span = now - prev;

                if ( settings.focusConsoleSetting.HasFlag( Settings.FocusSetting.pool ) )
                {
                    var message = MessageBuilder.BuildMessage( settings.poolAppCreated, now, app.name, title, span );
                    Console.WriteLine( message );
                }
                PoolApp.AppSpan packet = new( )
                {
                    focusCount = 1,
                    pageTitle = title,
                    time = DateTime.Now - span, //subtract span from now to get start time
                    span = span //set the span
                };
				plugins?.ForEach( x => x?.ModifyPoolAppCreation( packet ) );
				app.poolPackets.Add( packet );
            }
        }

        void UpdateSharedApp( PoolApp app )
        {
            //if there is a packet
            if ( app.poolPackets.Any( ) )
            {
                var packet = app.poolPackets[ 0 ]; //increase it's span
                var span = now - prev;
                packet.span += span;
				packet.focusCount++;

				plugins?.ForEach( x => x?.ModifyPoolAppUpdate( packet ) );
				if ( settings.focusConsoleSetting.HasFlag( Settings.FocusSetting.pool ) )
                {
                    var message = MessageBuilder.BuildMessage( settings.poolSharedAppUpdated, now, app.name, "", span, packet.span );
                    Console.WriteLine( message );
                }
            }
        }

        void CreateSharedApp( PoolApp app )
        {
            //if there are no profiles for this process, make a new one
            if ( !app.poolPackets.Any( ) )
            {

				var span = now - prev;
                if ( settings.focusConsoleSetting.HasFlag( Settings.FocusSetting.pool ) )
                {
                    var message = MessageBuilder.BuildMessage( settings.poolAppCreated, now, app.name, "", span );
                    Console.WriteLine( message );
                }
                PoolApp.AppSpan packet = new( )
                {
                    focusCount = 1,
                    pageTitle = app.name, // no need for lookup table if there's only one possible title for this page
                    time = DateTime.Now - span, //subtract span from now to get start time
                    span = span //set the span
                };

				plugins?.ForEach( x => x?.ModifyPoolAppCreation( packet ) );

				app.poolPackets.Add(packet);
            }
        }
    }
}
