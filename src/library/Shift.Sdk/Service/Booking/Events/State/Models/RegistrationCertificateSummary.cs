using System;

namespace InSite.Domain.Events
{
    public class RegistrationCertificateSummary
    {
        public Guid AchievementIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public Guid EventIdentifier { get; set; }
        public string EventTitle { get; set; }
        public string VenueLocationName { get; set; }
        public int RegistrationCount { get; set; }
    }
}
