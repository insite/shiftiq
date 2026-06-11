using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ReorderGradeItem : Command
    {
        public ReorderGradeItem(Guid record, Guid? parent, Guid[] children)
        {
            AggregateIdentifier = record;
            Parent = parent;
            Children = children;
        }

        public Guid? Parent { get; set; }
        public Guid[] Children { get; set; }
    }
}