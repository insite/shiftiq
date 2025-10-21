using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class MessageLinkFilter: Filter
    {
        public int? MessageId { get; set; }
        public Guid? MessageIdentifier { get; set; }
    }
}
