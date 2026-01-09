using System;

namespace Shift.Contract
{
    public class ModifyRegistrationAccommodation
    {
        public Guid AccommodationIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }

        public string AccommodationName { get; set; }
        public string AccommodationType { get; set; }

        public int? TimeExtension { get; set; }
    }
}