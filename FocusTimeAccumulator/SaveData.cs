using Newtonsoft.Json;

namespace FocusTimeAccumulator
{
	internal class SaveData
	{
		public static void SerializeJson<T>( string filePath, T objectToWrite )
		{
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
	}
}
