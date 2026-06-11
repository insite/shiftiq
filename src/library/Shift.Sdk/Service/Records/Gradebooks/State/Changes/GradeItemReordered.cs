using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemReordered : Change
    {
        public GradeItemReordered(Guid? parent, Guid[] children)
        {
            Parent = parent;
            Children = children;
        }

        public Guid? Parent { get; set; }
        public Guid[] Children { get; set; }
    }
}