
using System;

namespace Shift.Contract.Responses
{
    public class UserSearchResponse
    {
        public Guid Identifier { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string TimeZone { get; set; }

        public DateTimeOffset? LicenseAccepted { get; set; }
        public DateTimeOffset? PasswordChanged { get; set; }
        public DateTimeOffset? PasswordExpired { get; set; }
    }

    public class UserResponse
    {
        public Guid Identifier { get; set; }
    }
}