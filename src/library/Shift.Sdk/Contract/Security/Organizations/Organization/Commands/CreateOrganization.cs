using System;

namespace Shift.Contract
{
    public class CreateOrganization
    {
        public Guid? AdministratorUserId { get; set; }
        public Guid? GlossaryId { get; set; }
        public Guid OrganizationId { get; set; }

        public string AccountStatus { get; set; }
        public string CompanyName { get; set; }
        public string CompanySize { get; set; }
        public string CompanySummary { get; set; }
        public string CompanyTitle { get; set; }
        public string CompanyWebSiteUrl { get; set; }
        public string CompetencyAutoExpirationMode { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationData { get; set; }
        public string OrganizationLogoUrl { get; set; }
        public string PersonFullNamePolicy { get; set; }
        public string StandardContentLabels { get; set; }
        public string TimeZone { get; set; }

        public int? CompetencyAutoExpirationDay { get; set; }
        public int? CompetencyAutoExpirationMonth { get; set; }

        public DateTimeOffset? AccountClosed { get; set; }
        public DateTimeOffset? AccountOpened { get; set; }
    }
}