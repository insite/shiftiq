using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeExamType : Command
    {
        public string Type { get; set; }

        public ChangeExamType(Guid aggregate, string type)
        {
            AggregateIdentifier = aggregate;
            Type = type;
        }
    }
}
