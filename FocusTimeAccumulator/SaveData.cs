using Newtonsoft.Json;

namespace FocusTimeAccumulator
{
	internal class SaveData
	{
		public static void WriteToBinaryFile<T>( string filePath, T objectToWrite, bool append = false )
		{
			using ( StreamWriter file = File.CreateText( filePath ) )
			{
				JsonSerializer ser = new JsonSerializer( );
				ser.Serialize( file, objectToWrite );
			}
		}
		public static T ReadFromBinaryFile<T>( string filePath )
		{
			using ( StreamReader file = File.OpenText( filePath ) )
			{
				JsonSerializer serializer = new JsonSerializer( );
				return (T)serializer.Deserialize( file, typeof( T ) );
			}
		}
	}
}
