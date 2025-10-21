using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class OrganizationFilter : Filter
    {
        public string CompanyName { get; set; }
        public string CompanyDomain { get; set; }
        public string[] IncludeOrganizationCode { get; set; }
        public string[] ExcludeOrganizationCode { get; set; }
        public string OrganizationCode { get; set; }
        public bool? IsClosed { get; set; }
        public Guid[] OrganizationIdentifiers { get; set; }

        public OrganizationFilter Clone()
        {
            return (OrganizationFilter)MemberwiseClone();
        }
    }
}