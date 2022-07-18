using Newtonsoft.Json;

namespace FocusTimeAccumulator
{
	internal class SaveData
	{
		public static void SerializeJson<T>( string filePath, T objectToWrite )
		{
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
	}
}
