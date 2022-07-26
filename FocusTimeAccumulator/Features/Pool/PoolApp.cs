using System;

namespace FocusTimeAccumulator.Features.Pool
{
    [Serializable]
    public class PoolApp
    {
        public string name;
        public List<AppSpan> poolPackets = new List<AppSpan>( );
        public PoolApp( string procName )
        {
            name = procName;
        }
		public AppSpan? GetSpan(string pageTitle) {
			return poolPackets?.Where( s => s.pageTitle == pageTitle ).FirstOrDefault();
			
		}
        [Serializable]
        public class AppSpan
        {
            public string pageTitle;
            public TimeSpan span;
            public DateTime time;
            public int focusCount = 0;
            public Dictionary<string, Stat> stats = new Dictionary<string, Stat>( );

			/// <summary>
			/// Increases a statistic by a count. 
			/// If there is no stat, it will be created with the count.
			/// </summary>
			public void AddStat( string stat, int count )
            {
                if ( stats.ContainsKey( stat ) )
                    stats[ stat ] += count;
                 else 
                    stats.Add( stat, count );                
            }
			/// <summary>
			/// Increases a statistic by 1. 
			/// If there is no stat, it will be created with the count 1.
			/// </summary>
			public void AddStat( string stat )
			{
				if ( stats.ContainsKey( stat ) )				
					stats[ stat ]++;				
				else				
					stats.Add( stat, 1 );				
			}
			/// <summary>
			/// Gets a stat. 
			/// If there is no stat, it will be created with the count 0.
			/// </summary>
			public Stat? GetStat( string stat )
			{
				if ( stats.ContainsKey( stat ) )
					return (Stat?)stats[ stat ];
				else
				{
					stats.Add( stat, 0 );
					return (Stat?)stats[ stat ];
				}
			}

			/// <summary>
			/// Set a stat to some count.
			/// If there is no stat, it will be created with the count.
			/// </summary>
			public void SetStat( string stat, int count )
			{
				if ( stats.ContainsKey( stat ) )
					stats[ stat ].value = count;
				else
					stats.Add( stat, count );
			}

			/// <summary>
			/// Clear a stat, removing it from the dictionary
            /// If no stat exists or fails to be removed this will return false
			/// </summary>
			public bool ClearStat( string stat)
			{
                return stats.ContainsKey( stat ) && stats.Remove( stat );
            }

            public class Stat {
                public int value;

				public static implicit operator int( Stat d ) => d.value;
				public static implicit operator PoolApp.AppSpan.Stat( int b ) => new Stat( b );
				public Stat(int value ) => this.value = value;
				

				//allow support for directly modifying a stat using operators
				public static Stat operator +( Stat a, int b ) => a.value + b;
				public static Stat operator -( Stat a, int b ) => a.value - b;
				public static Stat operator +( Stat a, Stat b ) => a.value + b.value;
				public static Stat operator -( Stat a, Stat b ) => a.value - b.value;
				//allow for checking if stats are equal nor not 
				public static bool operator ==( Stat a, int b ) => a.value == b;
				public static bool operator !=( Stat a, int b ) => a.value != b;
				public static bool operator ==( Stat a, Stat b ) => a.value == b.value;
				public static bool operator !=( Stat a, Stat b ) => a.value != b.value;
			}
        }
    }

}
