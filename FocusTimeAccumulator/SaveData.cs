using FocusTimeAccumulator.Features.Pool;
using System.Text;
using Newtonsoft.Json;

namespace FocusTimeAccumulator
{
	internal class SaveData
	{
		
		public static void SerializeJson<T>( string filePath, T objectToWrite )
		{
			//make the folder structure for the file in the case it's missing
			CreateMissingPath( filePath );
			using ( StreamWriter file = File.CreateText( filePath ) )
			{
				JsonSerializer ser = new JsonSerializer( );
				ser.Serialize( file, objectToWrite );
			}
		}
		public static T DeserializeJson<T>( string filePath )
		{
			using ( StreamReader file = File.OpenText( filePath ) )
			{
				JsonSerializer serializer = new JsonSerializer( );
				return (T)serializer.Deserialize( file, typeof( T ) );
			}
		}

		static void CreateMissingPath( string path )
		{
			var directory = Path.GetFullPath( path ).Replace( Path.GetFileName( path ), "" );
			if ( !Directory.Exists( directory ) )
				Directory.CreateDirectory( directory );
		}
		public static string GetFilePath( string appName, PathMode mode )
		{
			var now = DateTime.Now;
			StringBuilder stringbuilder = new( $"{appName} " );
			if ( mode.HasFlag( PathMode.Daily ) )
				stringbuilder.Append( now.ToString( "d" ) ).Replace( $"/", " " );
			else if ( mode.HasFlag( PathMode.Monthly ) )
				stringbuilder.Append( now.ToString( "mm yyyy" ) );

			stringbuilder.Append( ".json" );
			return stringbuilder.ToString( );
		}
	}
}
