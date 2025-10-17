using System;
using System.Collections.Generic;

using InSite.Application.Courses.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Integration;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Contacts.Read
{
    public class QUser
    {
        public Guid UserIdentifier { get; set; }

        public string DefaultPassword { get; set; }
        public string Email { get; set; }
        public string EmailAlternate { get; set; }
        public string EmailVerified { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Honorific { get; set; }
        public string ImageUrl { get; set; }
        public string Initials { get; set; }
        public string LastName { get; set; }
        public string LoginOrganizationCode { get; set; }
        public string MiddleName { get; set; }
        public string MultiFactorAuthenticationCode { get; set; }
        public string OAuthProviderUserId { get; set; }
        public string OldUserPasswordHash { get; set; }
        public string PhoneMobile { get; set; }
        public string SoundexFirstName { get; set; }
        public string SoundexLastName { get; set; }
        public string TimeZone { get; set; }
        public string UserPasswordHash { get; set; }

        public bool AccessGrantedToCmds { get; set; }
        public bool MultiFactorAuthentication { get; set; }

        public int? PrimaryLoginMethod { get; set; }
        public int? SecondaryLoginMethod { get; set; }
        public int? UserPasswordChangeRequested { get; set; }

        public DateTimeOffset? AccountCloaked { get; set; }
        public DateTimeOffset? DefaultPasswordExpired { get; set; }
        public DateTimeOffset? UserLicenseAccepted { get; set; }
        public DateTimeOffset? UserPasswordChanged { get; set; }
        public DateTimeOffset UserPasswordExpired { get; set; }
        public DateTimeOffset? UtcArchived { get; set; }
        public DateTimeOffset? UtcUnarchived { get; set; }

        public virtual ICollection<ApiRequest> ApiRequests { get; set; } = new HashSet<ApiRequest>();
        public virtual ICollection<QCourseEnrollment> CourseEnrollments { get; set; } = new HashSet<QCourseEnrollment>();
        public virtual ICollection<TProgramEnrollment> ProgramEnrollments { get; set; } = new HashSet<TProgramEnrollment>();
        public virtual ICollection<QMembership> Memberships { get; set; } = new HashSet<QMembership>();
        public virtual ICollection<QPerson> Persons { get; set; } = new HashSet<QPerson>();
        public virtual ICollection<QUserConnection> FromConnections { get; set; } = new HashSet<QUserConnection>();
        public virtual ICollection<QUserConnection> ToConnections { get; set; } = new HashSet<QUserConnection>();

        public bool IsNullPassword() => string.IsNullOrEmpty(UserPasswordHash)
            || PasswordHash.ValidatePassword(UserIdentifier.ToString(), UserPasswordHash);

        public bool IsDefaultPassword() => !string.IsNullOrEmpty(UserPasswordHash)
            && PasswordHash.ValidatePassword(DefaultPassword, UserPasswordHash);

        public void SetDefaultPassword(string password = null)
        {
            DefaultPassword = password ?? RandomStringGenerator.CreateUserPassword();
            DefaultPasswordExpired = DateTimeOffset.UtcNow.AddDays(Limits.DefaultPasswordLifetimeInDays);
            UserPasswordHash = PasswordHash.CreateHash(DefaultPassword);
            UserPasswordExpired = DateTimeOffset.UtcNow;
        }

        public void SetPassword(string password)
        {
            UserPasswordHash = PasswordHash.CreateHash(password);
            UserPasswordChanged = DateTimeOffset.UtcNow;
            UserPasswordExpired = DateTimeOffset.UtcNow.AddMonths(6);

            DefaultPassword = null;
            DefaultPasswordExpired = null;
        }

        public static readonly ICollection<string> DiffExclusions = new HashSet<string>
        {
            nameof(DefaultPassword),
            nameof(UserPasswordHash),
            nameof(OldUserPasswordHash),
            nameof(SoundexFirstName),
            nameof(SoundexLastName),
        };
    }
}