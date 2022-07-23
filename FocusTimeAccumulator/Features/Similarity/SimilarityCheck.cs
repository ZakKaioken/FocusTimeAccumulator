using System;
using System.IO;
using FocusTimeAccumulator.Features.Bucket;
using FocusTimeAccumulator.Features.Pool;
using FocusTimeAccumulator.IO;
using static Program;
namespace FocusTimeAccumulator.Features.Similarity
{
	public class SimilarityCheck
	{
		//this code is going to practically be a duplicate itself
		public static string GetSimilarPoolTitle( PoolApp app, string path, string appName, string appTitle )
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

				var main = similarPackets.First( );

				profile.titles.Add( main.pageTitle );
				//ideally this should not run twice on the same item
				if ( settings.focusConsoleSetting.HasFlag( Settings.FocusSetting.pool ) )
				{
					var message = MessageBuilder.BuildMessage( settings.poolMerge, DateTime.Now, app.name, appTitle, TimeSpan.Zero );
					Console.WriteLine( message );
				}

				var dead = similarPackets.Where( s => s != main );
				foreach ( var packet in dead.ToList( ) )
				{
					main.span += packet.span;
					main.focusCount += packet.focusCount;
					app.poolPackets.Remove( packet );
				}
				SaveData.SerializeJson( appSettingfile, settings ); //save new suggestion
				return main.pageTitle;
			}
			return titles.Any( ) ? titles.First( ) : appTitle;
		}
		
		public static string GetSimilarBucketTitle( BucketApp app, string path, string appName, string appTitle )
		{
			var similarTitles = app.titles.Where( p => LevenshteinDistance.Calculate( p.Key, appTitle ) < 5 );

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

			if ( similarTitles.Count( ) > 2 && !titles.Any( ) )
			{
				//only add the similar titles profile to settings if we add titles to it
				if ( !profiles.Any( ) )
					settings.similarTitles.Add( profile );
				//add the similar title to the settings and then save it

				var main = similarTitles.First( );

				profile.titles.Add( main.Key );
				if ( settings.focusConsoleSetting.HasFlag( Settings.FocusSetting.bucket ) )
				{
					var message = MessageBuilder.BuildMessage( settings.bucketMerge, DateTime.Now, app.name, appTitle, TimeSpan.Zero );
					Console.WriteLine( message );
				}

				var dead = similarTitles.Where( s => s.Value != main.Value );
				foreach ( var packet in dead.ToList( ) )
				{
					var tb = app.poolPackets.Where( o => o.pageTitle == packet.Value );
					foreach (var t in tb) {
						t.pageTitle = main.Value;
					}
					app.titles.Remove( packet.Key );
				}
				SaveData.SerializeJson( appSettingfile, settings ); //save new suggestion
				return main.Key;
			}
			return titles.Any( ) ? titles.First( ) : appTitle;
		}

	}
}
