namespace FocusTimeAccumulator.Features.Bucket
{
    [Serializable]
    public class BucketApp
    {
        public string name;
        public List<AppSpan> poolPackets = new List<AppSpan>( );
        public Dictionary<string, int> titles = new Dictionary<string, int>( );
        public BucketApp(string procName)
        {
            name = procName;
        }

        [Serializable]
        public class AppSpan
        {
            public int pageTitle;
            public TimeSpan span;
            public DateTime time;
        }
    }

}
