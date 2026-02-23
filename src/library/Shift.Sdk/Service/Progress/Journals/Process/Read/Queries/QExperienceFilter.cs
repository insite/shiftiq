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
        public Guid? UserDepratmentIdentifier { get; set; }
        public Guid? CompetencyStandardIdentifier { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }

        public string TrainingTypeExact { get; set; }
        public string EmployerContains { get; set; }
        public string SupervisorContains { get; set; }
        public DateTime? StartDateExact { get; set; }
        public DateTime? EndDateExact { get; set; }
        public int? HoursExact { get; set; }

        public DateTimeOffset? CreatedSince { get; set; }
        public DateTimeOffset? CreatedBefore { get; set; }

        public bool? IsValidated { get; set; }
    }
}
