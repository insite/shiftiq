using System;

namespace Shift.Contract
{
    public class RegistrationDetailsMatch
    {
        public Guid RegistrationIdentifier { get; set; }
        public string ApprovalStatus { get; set; }
        public string Accommodation { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string PersonCode { get; set; }
        public string Email { get; set; }
        public string PreferredLanguage { get; set; }

        public string EventTitle { get; set; }
        public string EventVenue { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
        public DateTimeOffset? EventScheduledEnd { get; set; }
        public string Instructor { get; set; }
        public string InstructorEmail { get; set; }
        public string EventVenueAddress { get; set; }
        public Guid? VenueIdentifier { get; set; }
        public string BuildingAndRoom { get; set; }
    }
}
