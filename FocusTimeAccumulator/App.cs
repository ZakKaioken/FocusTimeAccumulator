namespace FocusTimeAccumulator
{
	[Serializable]
	public class App
	{
		public string name;
		public List<AppSpan> packets = new List<AppSpan>();

		public App( string procName )
		{
			name = procName;
		}

		[Serializable]
		public class AppSpan
		{
			public string pageTitle;
			public TimeSpan span;
			public DateTime time;
		}
	}

}
