using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class OpenIssue : Command
    {
        public Guid Tenant { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Opened { get; set; }
        public string Source { get; set; }
        public DateTimeOffset? Reported { get; set; }
        public string Type { get; set; }

        public OpenIssue(Guid aggregate, Guid tenant, int number, string title, string description, DateTimeOffset opened, string source, string type, DateTimeOffset? reported)
        {
            AggregateIdentifier = aggregate;
            Tenant = tenant;
            Number = number;
            Title = title;
            Description = description;
            Opened = opened;
            Source = source;
            Reported = reported;
            Type = type;
        }
    }
}
