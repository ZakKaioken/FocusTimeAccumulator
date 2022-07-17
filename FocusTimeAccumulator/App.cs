namespace FocusTimeAccumulator
{
	[Serializable]
	public class App
	{
		public string name;
		public string proc;
		public TimeSpan span;
	}

	[Serializable]
	public class AppSetting {
		public string proc;
		public bool shared;
	}
}
