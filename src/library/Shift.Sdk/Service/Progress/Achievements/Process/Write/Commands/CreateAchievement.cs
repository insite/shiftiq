using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Achievements.Write
{
    public class CreateAchievement : Command
    {
        public CreateAchievement(Guid achievement, Guid tenant, string label, string title, string description, Expiration expiration, string source = null)
        {
            AggregateIdentifier = achievement;
            Tenant = tenant;
            Label = label;
            Title = title;
            Description = description;
            Expiration = expiration;
            Source = source;
        }

        public Guid Tenant { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Expiration Expiration { get; set; }
        public string Source { get; set; }
    }
}