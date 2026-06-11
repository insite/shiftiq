using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QJournalSetupFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public string JournalSetupName { get; set; }
        public DateTimeOffset? JournalSetupCreatedSince { get; set; }
        public DateTimeOffset? JournalSetupCreatedBefore { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset? EventScheduledSince { get; set; }
        public DateTimeOffset? EventScheduledBefore { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }
        public bool? IsLocked { get; set; }

        public QJournalSetupFilter Clone()
        {
            return (QJournalSetupFilter)MemberwiseClone();
        }
    }
}