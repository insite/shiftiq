using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SectionAdded : Change
    {
        public Guid Form { get; set; }
        public Guid Section { get; set; }
        public Guid Criterion { get; set; }

        public SectionAdded(Guid form, Guid section, Guid criterion)
        {
            Form = form;
            Section = section;
            Criterion = criterion;
        }
    }
}
