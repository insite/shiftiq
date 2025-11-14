using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TApplicationFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? OpportunityIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public string EmployerName { get; set; }
        public string JobTitle { get; set; }

        public DateTime? DateUpdatedSince { get; set; }
        public DateTime? DateUpdatedBefore { get; set; }
    }
}
