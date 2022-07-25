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

        [Serializable]
        public class AppSpan
        {
            public string pageTitle;
            public TimeSpan span;
            public DateTime time;
            public int focusCount = 0;
            public Dictionary<string, Stat> stats = new Dictionary<string, Stat>( );

            public void AddStat( string stat, int count )
            {
                if ( stats.ContainsKey( stat ) )
                {
                    stats[ stat ].value += count;
                } else {
                    stats.Add( stat, new Stat(count) );
                }
            }
			public bool ClearStat( string stat, int count )
			{
				if ( stats.ContainsKey( stat ) )
				{
					stats[ stat ].value += count;
                    return true;
				}
                return false;
			}
			public class Stat {
                public int value;

				public static implicit operator int( Stat d ) => d.value;
				public static explicit operator Stat( int b ) => new Stat( b );

                public Stat(int value ) => this.value = value;
            }
        }
    }

}
