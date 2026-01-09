using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseOpened : Change
    {
        public Guid Tenant { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Opened { get; set; }
        public string Source { get; set; }
        public DateTime? StartedDate { get; set; }
        public string Type { get; set; }

        public CaseOpened(Guid tenant, int number, string title, string description, DateTimeOffset opened, string source, string type, DateTime? startedDate)
        {
            Tenant = tenant;
            Number = number;
            Title = title;
            Description = description;
            Opened = opened;
            Source = source;
            StartedDate = startedDate;
            Type = type;
        }
    }

    public class CaseOpened2 : Change
    {
        public Guid Organization { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Opened { get; set; }
        public string Source { get; set; }
        public DateTimeOffset? Reported { get; set; }
        public string Type { get; set; }

        public CaseOpened2(Guid organization, int number, string title, string description, DateTimeOffset opened, string source, string type, DateTimeOffset? reported)
        {
            Organization = organization;
            Number = number;
            Title = title;
            Description = description;
            Opened = opened;
            Source = source;
            Reported = reported;
            Type = type;
        }

        public static CaseOpened2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<CaseOpened>();

            var v2 = new CaseOpened2(v1.Tenant, v1.Number, v1.Title, v1.Description, v1.Opened, v1.Source, v1.Type, v1.StartedDate)
            {
                AggregateIdentifier = v1.AggregateIdentifier,
                AggregateVersion = v1.AggregateVersion,
                OriginOrganization = v1.OriginOrganization,
                OriginUser = v1.OriginUser,
                ChangeTime = v1.ChangeTime
            };

            return v2;
        }
    }
}
