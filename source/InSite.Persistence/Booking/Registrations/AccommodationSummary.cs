using System;

namespace InSite.Persistence
{
    public class AccommodationSummary
    {
        public Guid EventIdentifier { get; set; }
        public string AccommodationType { get; set; }
        public int RegistrationCount { get; set; }
    }
}
