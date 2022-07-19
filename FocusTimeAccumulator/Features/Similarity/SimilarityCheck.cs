using System.IO;
using FocusTimeAccumulator.Features.Pool;
using static Program;
namespace FocusTimeAccumulator.Features.Similarity
{
	public class SimilarityCheck
	{
		public static int similarCount = 0;
		public static string GetSimilarTitle( ref PoolApp app, string path, string appName, string appTitle )
		{
			var similarPackets = app.poolPackets.Where( p => LevenshteinDistance.Calculate( p.pageTitle, appTitle ) < 5 );

			//get a profile
			Settings.SimilarTitles profile = null;
			var profiles = settings.similarTitles.Where( t => t.proc == appName );
			if ( !profiles.Any( ) )
			{
				profile = new( ) { proc = appName };
			}
			else
			{
				var p = profiles.First( );
				if ( p.combineSimilarTitles )
					profile = p;
			}
			//if there are titles that are similar inside the profile
			var titles = profile.titles.Where( ss => { var x = LevenshteinDistance.Calculate( ss, appTitle ); return x is < 10 and > 0; } );

			if ( similarPackets.Count() > 2 && !titles.Any() )
			{
				//only add the similar titles profile to settings if we add titles to it
				if ( !profiles.Any( ) )
					settings.similarTitles.Add( profile );
				//add the similar title to the settings and then save it
				profile.titles.Add( appTitle );

				var main = similarPackets.First( );
				//ideally this should not run twice on the same item
				Console.WriteLine( $"duplicate \"{main.pageTitle}\" found. Merging all duplicates of this app from now on. you can disable this feature or rename a merged app name in the {appSettingfile} file" );
				
				
				var dead = similarPackets.Where( s => s != main );
				foreach ( var packet in dead.ToList() )
				{
					main.span += packet.span;
					main.focusCount += packet.focusCount;
					app.poolPackets.Remove( packet );
				}
				SaveData.SerializeJson( appSettingfile, settings ); //save new suggestion
			}
			var tt = titles.Any( ) ? titles.First( ) : appTitle;
			appTitle = tt;
			return appTitle;
		}

	}
}
