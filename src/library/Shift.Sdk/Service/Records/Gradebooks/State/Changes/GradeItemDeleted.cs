using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemDeleted : Change
    {
        public GradeItemDeleted(Guid item)
        {
            Item = item;
        }

        public Guid Item { get; set; }
    }
}