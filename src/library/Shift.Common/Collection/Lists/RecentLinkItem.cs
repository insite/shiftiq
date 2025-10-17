using System;

namespace Shift.Common
{
    [Serializable]
    public class RecentLinkItem
    {
        public string PageUrl { get; set; }
        public string ObserverUrl { get; set; }
        public string PageTitle { get; set; }
        public string Key { get; set; }
    }
}
