using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressAdded : Change
    {
        public ProgressAdded(Guid record, Guid user, Guid item)
        {
            Record = record;
            User = user;
            Item = item;
        }

        public Guid Record { get; set; }
        public Guid User { get; set; }
        public Guid Item { get; set; }
    }
}
