using FocusTimeAccumulator.IO;

namespace FocusTimeAccumulator.Features.Bucket
{
    [Serializable]
    public class BucketApp
    {
        public string name;
        public string productName;
        public string productDescription;
        public List<AppSpan> poolPackets = new List<AppSpan>( );
        public Dictionary<string, int> titles = new Dictionary<string, int>( );
        public BucketApp( string procName )
        {
            name = procName;
            productName = ProcessCache.GetProductName( procName );
            productDescription = ProcessCache.GetProductDescription( procName );
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
