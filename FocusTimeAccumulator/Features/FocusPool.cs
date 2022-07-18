using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTimeAccumulator.Features
{
	public class FocusPool
	{
		App currentApp;
		DateTime prev => Program.prev;
		DateTime now => Program.now;

		//this function name could go for an improvement
		public void DoFocusPool( string appName, string appTitle, PathMode pathMode )
		{
			var file = GetFilePath( appName, pathMode );
			var path = Path.Combine( "Apps", file );
			var app = File.Exists( path ) ? SaveData.DeserializeJson<App>( path ) : new App( appName );
			var setting = Program.settings.appSettings.Where( a => a.proc == appName ).ToList( ).FirstOrDefault();

			//if the current page has a profile
			if ( currentApp != null )
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

			app.packets = app.packets.OrderBy( d => d.time ).ToList();//sort based on time
			SaveData.SerializeJson( path, app ); //save json
			currentApp = app;
		}

		string GetFilePath(string appName, PathMode mode) {
			StringBuilder stringbuilder = new($"{appName} " );
			if ( mode.HasFlag( PathMode.Daily ) )
				stringbuilder.Append( now.ToString( "d" ) ).Replace( $"/", " " );
			else if (mode.HasFlag (PathMode.Monthly))
				stringbuilder.Append( now.ToString( "mm yyyy" ) );

			stringbuilder.Append( ".json" );
			return stringbuilder.ToString( );
		}
		void UpdateApp( App app, string title )
		{
			var packets = app.packets.Where( p => p.pageTitle == title ).ToList();
			if ( packets.Any( ) )
			{
				var packet = packets[ 0 ];
				packet.span += now - prev;
				Console.WriteLine( $"exiting {title} after {now - prev}. total time: {packet.span}" );
			}
		}

		void CreateApp( App app, string pageTitle )
		{
			var aps = app.packets.Where( a => a.pageTitle == pageTitle );
			if ( !aps.Any( ) )
			{
				Console.WriteLine( $"adding new app: {pageTitle}" );
				app.packets.Add( new( )
				{
					pageTitle = pageTitle,
					time = DateTime.Now,
					span = TimeSpan.Zero
				} );
			}
		}

		void UpdateSharedApp( App app )
		{
			if ( app.packets.Any( ) )
			{
				var packet = app.packets[ 0 ];
				packet.span += now - prev;
				Console.WriteLine( $"exiting shared app {app.name} after {now - prev}. total time: {packet.span}" );
			}
		}

		void CreateSharedApp( App app )
		{
			if ( !app.packets.Any( ) )
			{
				Console.WriteLine( $"adding new shared app: {app.name}" );
				app.packets.Add( new( )
				{
					pageTitle = app.name,
					time = DateTime.Now,
					span = TimeSpan.Zero
				} );
			}
		}
	}
	[Flags]
	public enum PathMode
	{
		JustAppName = 1,
		Daily = 1 << 1,
		Monthly = 1 << 2
	}
}
