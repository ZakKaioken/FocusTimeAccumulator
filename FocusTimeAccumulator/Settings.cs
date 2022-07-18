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
		public bool idleModeEnabled = true;
		public List<AppSetting> appSettings = new List<AppSetting>( );

		[Serializable]
		public class AppSetting
		{
			public string proc;
			public bool shared;
		}

	}
}
