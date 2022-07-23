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
		public string fileStructure = @"Apps/{feature}/{time}/{process}.json";
		public string errorLogPath = @"Logs/CrashDump-{time}.txt";
		public string poolAppCreated = "[Pool Add]: {title} added after being focused for {span}";
		public string poolAppUpdated = "[Pool Update]: {title} was focused for {span}. Total Time {totalTime}";
		public string poolSharedAppCreated = "[Shared Pool Add]: {process} added after being focused for {span}";
		public string poolSharedAppUpdated = "[Shared Pool Update]: {process} was focused for {span}. Total Time {totalTime}";
		public string poolMerge = "[Pool Merge]: 3 Duplicate titles of {process} found. Merging all similar titles from now on.";
		public string bucketMerge = "[Bucket Merge]: 3 Duplicate titles of {process} found. Merging all similar titles from now on.";
		public string bucketAdd = "[Bucket Add]: {process}({title}) was focused for {span}";
		public FocusSetting focusSetting = FocusSetting.pool;
		public FocusSetting focusConsoleSetting = FocusSetting.pool;
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
