using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTimeAccumulator
{
	[Serializable]
	public class Settings
	{
		public TimeSpan idleTime = TimeSpan.FromMinutes( 3 );
		public TimeSpan tickTime = TimeSpan.FromSeconds( 1 );
		public int maxTitleLength = 128;
		public bool idleModeEnabled = true;
		public FocusSetting focusSetting = FocusSetting.pool;
		public List<AppSetting> appSettings = new List<AppSetting>( );
		public List<string> similarSuggestions = new List<string>( );
		public Settings() {
			appSettings.Add(new() { 
					proc = "",
					similarTitles = "",
					combineSimilarTitles = false, 
					shared = false
			});
		}

		[Serializable]
		public class AppSetting
		{
			public string proc;
			public string similarTitles;
			public bool shared;
			public bool combineSimilarTitles;
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
