using System;

using Shift.Common;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class User
    {
        public Guid Identifier { get; set; }
        public Guid UserIdentifier => Identifier;

        public string Email { get; set; }
        public string EmailVerified { get; set; }

        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public string PasswordHash { get; set; }
        public string PersonCode { get; set; }

        public bool AccessGrantedToCmds { get; set; }
        public bool IsCloaked { get; set; }

        public DateTimeOffset? PasswordExpiry { get; set; }

        public string Phone { get; set; }
        public string TimeZoneId { get; set; }
        public TimeZoneInfo TimeZone { get; set; }

        public bool MultiFactorAuthentication { get; set; }
        public OtpModes ActiveOtpMode { get; set; }
        public DateTimeOffset? UserLicenseAccepted { get; set; }
    }
}
