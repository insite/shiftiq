using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TOpportunityFilter : Filter
    {
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string JobLocation { get; set; }
        public string EmploymentType { get; set; }
        public string PositionType { get; set; }
        public string JobType { get; set; }
        public string PositionLevel { get; set; }

        public DateTime? PostedSince { get; set; }
        public DateTime? PostedBefore { get; set; }
        public DateTime? PublishedSince { get; set; }
        public DateTime? PublishedBefore { get; set; }

        public Guid? DepartmentGroupIdentifier { get; set; }
        public Guid? TemplateIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? OccupationStandardIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public bool? IsPublished { get; set; }
        public bool? IsClosed { get; set; }
        public bool? IsArchived { get; set; }
        public string Keywords { get; set; }
        public Guid? IsAppliedFor { get; set; }
        public bool? IsApplied { get; set; }
    }
}
