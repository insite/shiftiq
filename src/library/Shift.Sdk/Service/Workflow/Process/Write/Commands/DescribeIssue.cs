using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class DescribeIssue : Command
    {
        public string Description { get; set; }

        public DescribeIssue(Guid aggregate, string description)
        {
            AggregateIdentifier = aggregate;
            Description = description;
        }
    }
}
