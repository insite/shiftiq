using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountRegistrationAccommodations : Query<int>, IRegistrationAccommodationCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }

        public string AccommodationName { get; set; }
        public string AccommodationType { get; set; }

        public int? TimeExtension { get; set; }
    }
}