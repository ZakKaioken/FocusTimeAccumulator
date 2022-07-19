using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace FocusTimeAccumulator.Features.Pool
{
    public class FocusPool
    {

        //this function name could go for an improvement
        public void DoFocusPool( string appName, string appTitle)
        {
;

            string path = SaveData.CreatePath( appName, settings.poolFileStructure, settings.timeStampFormat );
            var app = File.Exists( path ) ? SaveData.DeserializeJson<PoolApp>( path ) : new PoolApp( appName );
            var setting = settings.appSettings.Where( a => a.proc == appName ).ToList( ).FirstOrDefault( );
            //I debated on whether or not this if statement should go in this order
            //i'm still hung up, but this looks elegant.
            //if the current page has a profile
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

        

        void UpdateApp( PoolApp app, string title)
        {
            //if there is a packet
            var packets = app.poolPackets.Where(p => p.pageTitle == title).ToList();
            if (packets.Any())
            {   
                var packet = packets[0];
                packet.span += now - prev; //update span
                Console.WriteLine($"exiting {title} after {now - prev}. total time: {packet.span}");
            }
        }

        void CreateApp( PoolApp app, string pageTitle)
        {
            //if there is no profile for this title
            var aps = app.poolPackets.Where(a => a.pageTitle == pageTitle);
            if (!aps.Any())
            {
                Console.WriteLine($"adding new app: {pageTitle}");
                //add a new aoo
				var span = now - prev;
				app.poolPackets.Add(new()
                {
                    pageTitle = pageTitle,
                    time = DateTime.Now-span, //subtract span from now to get start time
					span = span //set the span
				} );
            }
        }

        void UpdateSharedApp( PoolApp app )
        {
            //if there is a packet
            if (app.poolPackets.Any())
            {
                var packet = app.poolPackets[0]; //increase it's span
                packet.span += now - prev;
                Console.WriteLine($"exiting shared app {app.name} after {now - prev}. total time: {packet.span}");
            }
        }

        void CreateSharedApp( PoolApp app )
        {
            //if there are no profiles for this process, make a new one
            if (!app.poolPackets.Any())
            {
                Console.WriteLine($"adding new shared app: {app.name}");

				var span = now - prev;
				app.poolPackets.Add(new()
                {
                    pageTitle = app.name, // no need for lookup table if there's only one possible title for this page
					time = DateTime.Now - span, //subtract span from now to get start time
					span = span //set the span
				} );
            }
        }
    }
}
