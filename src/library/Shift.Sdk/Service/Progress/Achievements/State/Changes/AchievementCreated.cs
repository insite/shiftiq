using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementCreated : Change
    {
        public AchievementCreated(Guid tenant, string label, string title, string description, Expiration expiration, string source)
        {
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