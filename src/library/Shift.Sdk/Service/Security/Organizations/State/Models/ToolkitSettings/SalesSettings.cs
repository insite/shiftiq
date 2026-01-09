using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class SalesSettings
    {
        public Guid? ProductClassEventVenueGroup { get; set; }
        public Guid? ProductCustomerGroup { get; set; }
        public Guid? ManagerGroup { get; set; }
        public Guid? LearnerGroup { get; set; }

        public bool IsEqual(SalesSettings other)
        {
            return ProductClassEventVenueGroup == other.ProductClassEventVenueGroup
                && ProductCustomerGroup == other.ProductCustomerGroup
                && ManagerGroup == other.ManagerGroup
                && LearnerGroup == other.LearnerGroup;
        }
    }
}
