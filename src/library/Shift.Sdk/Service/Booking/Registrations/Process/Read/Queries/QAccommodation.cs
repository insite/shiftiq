using System;

namespace InSite.Application.Registrations.Read
{
    public class QAccommodation
    {
        public Guid AccommodationIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public string AccommodationType { get; set; }
        public string AccommodationName { get; set; }
        public int? TimeExtension { get; set; }

        public virtual QRegistration Registration { get; set; }
    }
}
