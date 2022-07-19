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
				JsonSerializer ser = new JsonSerializer();
				ser.Formatting = Formatting.Indented;
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
		public static string CreatePath( string appName, string stucture, string timeFormat )
		{
			StringBuilder sb = new StringBuilder( stucture );
			sb.Replace( "/", "\\" ); //support forward slashes in directory as backslashes
			sb.Replace( @"{t}", DateTime.Now.ToString( timeFormat ).Replace("/", " ") );
			sb.Replace( @"{p}", appName );
			var path = sb.ToString( );
			return path;
		}
	}
}
