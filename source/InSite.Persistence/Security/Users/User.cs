using System;
using System.Collections.Generic;

using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class User
    {
        public Guid UserIdentifier { get; set; }

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
        public string PhoneMobile { get; set; }
        public string SoundexFirstName { get; set; }
        public string SoundexLastName { get; set; }
        public string TimeZone { get; set; }

        public bool AccessGrantedToCmds { get; set; }
        public bool MultiFactorAuthentication { get; set; }

        public LoginMethods? PrimaryLoginMethod { get; set; }
        public LoginMethods? SecondaryLoginMethod { get; set; }
        public string OAuthProviderUserId { get; set; }

        public DateTimeOffset? AccountCloaked { get; set; }
        public DateTimeOffset? UserLicenseAccepted { get; set; }
        public DateTimeOffset? UtcArchived { get; set; }
        public DateTimeOffset? UtcUnarchived { get; set; }

        public string DefaultPassword { get; set; }
        public DateTimeOffset? DefaultPasswordExpired { get; set; }
        public string UserPasswordHash { get; set; }
        public DateTimeOffset? UserPasswordChanged { get; set; }
        public DateTimeOffset UserPasswordExpired { get; set; }
        public string OldUserPasswordHash { get; set; }
        public int? UserPasswordChangeRequested { get; set; }

        public bool IsArchived => UtcArchived.HasValue;
        public bool IsCloaked => AccountCloaked.HasValue;

        public virtual TUserAuthenticationFactor UserAuthenticationFactorInfo { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }

        public virtual ICollection<ContactExperience> ContactExperiences { get; set; }
        public virtual ICollection<DepartmentProfileUser> DepartmentProfiles { get; set; }

        public virtual ICollection<StandardValidation> UserValidations { get; set; }
        public virtual ICollection<StandardValidation> ValidatorValidations { get; set; }

        public virtual ICollection<UserConnection> UpstreamConnections { get; set; }
        public virtual ICollection<UserConnection> DownstreamConnections { get; set; }

        public virtual ICollection<StandardValidationChange> UserStandardValidationChanges { get; set; } = new HashSet<StandardValidationChange>();
        public virtual ICollection<StandardValidationChange> AuthorStandardValidationChanges { get; set; } = new HashSet<StandardValidationChange>();

        public virtual ICollection<VCmdsCredential> CmdsCredentials { get; set; } = new HashSet<VCmdsCredential>();

        public virtual ICollection<QLearnerProgramSummary> QLearnerProgramSummaries { get; set; }
        public virtual ICollection<TPersonField> PersonFields { get; set; } = new HashSet<TPersonField>();

        public User()
        {
            Persons = new HashSet<Person>();
            Memberships = new HashSet<Membership>();
            UpstreamConnections = new HashSet<UserConnection>();
            DownstreamConnections = new HashSet<UserConnection>();

            ContactExperiences = new HashSet<ContactExperience>();
            DepartmentProfiles = new HashSet<DepartmentProfileUser>();

            UserValidations = new HashSet<StandardValidation>();
            ValidatorValidations = new HashSet<StandardValidation>();

            QLearnerProgramSummaries = new HashSet<QLearnerProgramSummary>();
        }

        public bool IsNewPasswordValid(string password)
        {
            if (!string.IsNullOrEmpty(DefaultPassword) && DefaultPassword == password)
                return false;

            if (!string.IsNullOrEmpty(UserPasswordHash) && PasswordHash.ValidatePassword(password, UserPasswordHash))
                return false;

            return true;
        }

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
            // Enforce this business rule in the UI layer.
            // if (!string.IsNullOrEmpty(DefaultPassword) && password == DefaultPassword)
            //    throw new ApplicationError("New password can't be the same as default password");

            UserPasswordHash = PasswordHash.CreateHash(password);
            UserPasswordChanged = DateTimeOffset.UtcNow;
            UserPasswordExpired = DateTimeOffset.UtcNow.AddMonths(6);

            DefaultPassword = null;
            DefaultPasswordExpired = null;
        }
    }
}
