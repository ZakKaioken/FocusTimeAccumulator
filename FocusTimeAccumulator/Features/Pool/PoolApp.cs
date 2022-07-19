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
        }
    }

}
