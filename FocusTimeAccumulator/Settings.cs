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
		public string poolAppCreated = "{time}-[Pool Add]: {title} added after being focused for {span}";
		public string poolAppUpdated = "{time}-[Pool Update]: {title} was focused for {span}. Total Time {totalTime}";
		public string poolSharedAppCreated = "{time}-[Shared Pool Add]: {process} added after being focused for {span}";
		public string poolSharedAppUpdated = "{time}-[Shared Pool Update]: {process} was focused for {span}. Total Time {totalTime}";
		public string bucketAdd = "{time}-[Bucket Add]: {process}({title}) was focused for {span}";
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
