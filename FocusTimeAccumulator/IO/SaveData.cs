using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace FocusTimeAccumulator
{
	internal class SaveData
	{
		//save any text to a file
		public static void Save( string filePath, string text )
		{
			//make the folder structure for the file in the case it's missing
			CreateMissingPath( filePath );

			using ( FileStream file = new( filePath, FileMode.Append, FileAccess.Write, FileShare.Read ) )
			using ( StreamWriter writer = new( file, Encoding.UTF8 ) )
			{
				writer.Write( text.ToString( ) );
			}

		}
		public static void SerializeJson<T>( string filePath, T objectToWrite )
		{
			//make the folder structure for the file in the case it's missing
			CreateMissingPath( filePath );
			using ( FileStream file = new( filePath, FileMode.Append, FileAccess.Write, FileShare.Read ) )
			using ( StreamWriter writer = new( file, Encoding.UTF8 ) )
			{
				JsonSerializer ser = new( )
				{
					Formatting = Formatting.Indented
				};
				ser.Serialize( writer, objectToWrite );
			}
		}
		public static T? DeserializeJson<T>( string filePath )
		{
			using ( FileStream file = new( filePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
			using ( StreamReader reader = new( file, Encoding.UTF8 ) ) { 
				JsonSerializer serializer = new JsonSerializer( );
				var dc = serializer.Deserialize( reader, typeof( T ) );
				return dc is T t ? t : default;
			}
		}

		public static void CreateMissingPath( string path )
		{
			var filename = Path.GetFileName( path );
			var directory = Path.GetFullPath( path );
			if ( filename.Length > 0 )
				directory = directory.Replace( filename ?? "", "" );
			if ( !Directory.Exists( directory ) )
				Directory.CreateDirectory( directory );
		}
		public static string CreatePath( string appName, string feature, string stucture, string timeFormat )
		{
			StringBuilder sb = new( stucture );
			sb.Replace( "/", "\\" ); //support forward slashes in directory as backslashes
			sb.Replace( @"{time}", DateTime.Now.ToString( timeFormat ).Replace("/", " ") );
			sb.Replace( @"{process}", appName );
			sb.Replace( @"{feature}", feature );
			var path = sb.ToString( );
			return path;
		}

		internal static void CopyFile( string path, string newPath ) {
			var filename = Path.GetFileName( path );
			var directory = Path.GetFullPath( path );
			if ( filename.Length > 0 )
				directory = directory.Replace( filename ?? "", "" );


			var vp = Path.Combine( directory, newPath, filename );
			CreateMissingPath( vp );
			Console.WriteLine( $"attempting to save file to {vp}" );
			File.Copy( path, vp );
		}
	}
}
