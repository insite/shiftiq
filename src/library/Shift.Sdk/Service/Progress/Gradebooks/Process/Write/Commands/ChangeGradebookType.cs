using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradebookType : Command
    {
        public ChangeGradebookType(Guid record, GradebookType type, Guid? framework)
        {
            AggregateIdentifier = record;
            Type = type;
            Framework = framework;
        }

        public GradebookType Type { get; set; }
        public Guid? Framework { get; set; }
    }
}
