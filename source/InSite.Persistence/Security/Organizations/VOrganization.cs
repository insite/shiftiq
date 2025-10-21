using System;
using System.Collections.Generic;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class VOrganization
    {
        public Guid? AdministratorUserIdentifier { get; set; }
        public Guid? GlossaryIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? ParentOrganizationIdentifier { get; set; }

        public string AccountStatus { get; set; }
        public string CompanyDomain { get; set; }
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

        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<VGroupDetail> Groups { get; set; } = new HashSet<VGroupDetail>();
        public virtual ICollection<Person> Persons { get; set; }
        public virtual ICollection<TSenderOrganization> Senders { get; set; }
        public virtual ICollection<Standard> Standards { get; set; }
        public virtual ICollection<StandardOrganization> StandardOrganizations { get; set; }
        public virtual ICollection<QSurveyForm> SurveyForms { get; set; }
        public virtual ICollection<TCollectionItem> CollectionItems { get; set; }

        public VOrganization()
        {
            Departments = new HashSet<Department>();
            Persons = new HashSet<Person>();
            Standards = new HashSet<Standard>();
            StandardOrganizations = new HashSet<StandardOrganization>();
            SurveyForms = new HashSet<QSurveyForm>();
            CollectionItems = new HashSet<TCollectionItem>();
        }
    }
}