using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class DivisionFilter : Filter
    {
        public Guid[] DivisionIdentifiers { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public string DivisionName { get; set; }
        public string DivisionCode { get; set; }
        public DateTimeOffsetRange Created { get; set; }
        public string CompanyName { get; set; }

        public DivisionFilter()
        {
            Created = new DateTimeOffsetRange();
        }

        public DivisionFilter Clone()
        {
            var clone = (DivisionFilter)MemberwiseClone();

            clone.Created = Created?.Clone();

            return clone;
        }
    }
}
