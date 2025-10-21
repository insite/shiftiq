using System;

namespace InSite.Persistence
{
    [Serializable]
    public class ConnectionModel
    {
        public int FromAsset { get; set; }
        public int FromKey { get; set; }

        public int ToAsset { get; set; }
        public int ToKey { get; set; }
    }
}
