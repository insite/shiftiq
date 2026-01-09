using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FieldsSwapped : Change
    {
        public Guid A { get; set; }
        public Guid B { get; set; }

        public FieldsSwapped(Guid a, Guid b)
        {
            A = a;
            B = b;
        }
    }
}
