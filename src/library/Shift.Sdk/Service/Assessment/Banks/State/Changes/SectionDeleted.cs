using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SectionDeleted : Change
    {
        public Guid Section { get; set; }

        public SectionDeleted(Guid section)
        {
            Section = section;
        }
    }
}
