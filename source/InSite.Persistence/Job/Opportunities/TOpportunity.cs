using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class TOpportunity
    {
        public Guid? DepartmentGroupIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid EmployerUserIdentifier { get; set; }
        public Guid OpportunityIdentifier { get; set; }
        public Guid? OpportunityStatusItemIndentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? IndustryItemIdentifier { get; set; }
        public Guid? OccupationStandardIdentifier { get; set; }

        public string ApplicationEmail { get; set; }
        public string ApplicationRequirements { get; set; }
        public string ApplicationWebSiteUrl { get; set; }
        public string EmployerGroupDescription { get; set; }
        public string EmployerGroupName { get; set; }
        public string EmploymentType { get; set; }
        public string JobAttachmentUrl { get; set; }
        public string JobDescription { get; set; }
        public string JobLevel { get; set; }
        public string JobTitle { get; set; }
        public string LocationDescription { get; set; }
        public string LocationName { get; set; }
        public string LocationType { get; set; }
        public string SalaryOther { get; set; }

        public bool ApplicationEvergreen { get; set; }
        public bool? ApplicationRequiresLetter { get; set; }
        public bool? ApplicationRequiresResume { get; set; }

        public int? SalaryMaximum { get; set; }
        public int? SalaryMinimum { get; set; }

        public DateTimeOffset? ApplicationDeadline { get; set; }
        public DateTimeOffset? ApplicationOpen { get; set; }
        public DateTimeOffset? EmploymentStart { get; set; }
        public DateTimeOffset? WhenArchived { get; set; }
        public DateTimeOffset? WhenClosed { get; set; }
        public DateTimeOffset WhenCreated { get; set; }
        public DateTimeOffset? WhenModified { get; set; }
        public DateTimeOffset? WhenPublished { get; set; }

        public virtual Standard OccupationStandard { get; set; }

        public ICollection<TApplication> Applications { get; set; } = new HashSet<TApplication>();
    }
}
