using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class DescribeGroup : Command
    {
        public string Category { get; }
        public string Code { get; }
        public string Description { get; }
        public string Label { get; }

        public DescribeGroup(Guid group, string category, string code, string description, string label)
        {
            AggregateIdentifier = group;
            Category = category.NullIfEmpty();
            Code = code.NullIfEmpty();
            Description = description.NullIfEmpty();
            Label = label.NullIfEmpty();
        }
    }
}
