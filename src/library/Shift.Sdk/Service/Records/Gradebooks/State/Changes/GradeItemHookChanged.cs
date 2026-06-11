using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemHookChanged : Change
    {
        public Guid Item { get; set; }
        public string Hook { get; set; }

        public GradeItemHookChanged(Guid item, string hook)
        {
            Item = item;
            Hook = hook;
        }
    }
}
