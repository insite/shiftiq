using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QExperienceFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? JournalSetupIdentifier { get; set; }
        public Guid? JournalIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }

        public string TrainingType { get; set; }

        public DateTimeOffset? CreatedSince { get; set; }
        public DateTimeOffset? CreatedBefore { get; set; }

        public bool? IsValidated { get; set; }
    }
}
