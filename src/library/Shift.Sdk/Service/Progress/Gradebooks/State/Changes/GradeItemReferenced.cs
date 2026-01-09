using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemReferenced : Change
    {
        public Guid Item { get; set; }
        public string Reference { get; set; }

        public GradeItemReferenced(Guid item, string reference)
        {
            Item = item;
            Reference = reference;
        }
    }
}
