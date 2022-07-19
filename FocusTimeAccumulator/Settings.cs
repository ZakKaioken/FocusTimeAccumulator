namespace FocusTimeAccumulator
{
	[Serializable]
	public class Settings
	{
		public TimeSpan idleTime = TimeSpan.FromMinutes( 3 );
		public TimeSpan tickTime = TimeSpan.FromSeconds( 1 );
		public int maxTitleLength = 128;
		public bool doPoolSimilarityChecking = true;
		public bool idleModeEnabled = true;
		public string timeStampFormat = "d";
		public string poolFileStructure = @"Apps/Pools/{t}/{p}.json";
		public string bucketFileStructure = @"Apps/Buckets/{t}/{p}.json";
		public FocusSetting focusSetting = FocusSetting.pool;
		public List<AppSetting> appSettings = new List<AppSetting>( );
		public List<SimilarTitles> similarTitles = new List<SimilarTitles>( );
		
		[Serializable]
		public class SimilarTitles
		{
			public string proc = "";
			public List<string> titles = new( );
			public bool combineSimilarTitles = true;
		}

		[Serializable]
		public class AppSetting
		{
			public string proc = "";
			public bool shared = false;
		}

		[Flags]
		[Serializable]
		public enum FocusSetting
		{
			pool = 1,
			bucket = 1 << 1
		}
	}
}
