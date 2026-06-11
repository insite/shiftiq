using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItemHook : Command
    {
        public Guid Item { get; set; }
        public string Hook { get; set; }

        public ChangeGradeItemHook(Guid record, Guid item, string hook)
        {
            AggregateIdentifier = record;
            Item = item;
            Hook = hook;
        }
    }
}
