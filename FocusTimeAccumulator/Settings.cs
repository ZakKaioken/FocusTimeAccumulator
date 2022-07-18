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

		public Settings() {
			appSettings.Add(new() { 
					proc = "", 
					shared = true
			});
		}

		[Serializable]
		public class AppSetting
		{
			public string proc;
			public bool shared;
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
