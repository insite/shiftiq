using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class DescribeAchievement : Command
    {
        public DescribeAchievement(Guid achievement, string label, string title, string description, bool allowSelfDeclared)
        {
            AggregateIdentifier = achievement;
            Label = label;
            Title = title;
            Description = description;
            AllowSelfDeclared = allowSelfDeclared;
        }

        public string Label { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool AllowSelfDeclared { get; set; }
    }
}