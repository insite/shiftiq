using System;

namespace InSite.Application.Standards.Read
{
    public class QStandardTier
    {
        public Guid RootStandardIdentifier { get; set; }
        public Guid ItemStandardIdentifier { get; set; }
        public int TierNumber { get; set; }
        public string TierName { get; set; }
    }
}
