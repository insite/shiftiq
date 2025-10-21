using System;

namespace InSite.Persistence
{
    internal class UserSearchResultItem
    {
        public DateTimeOffset UserPasswordExpired { get; internal set; }
        public DateTimeOffset? UserLicenseAccepted { get; internal set; }
        public string UserPasswordHash { get; internal set; }
        public string EmailVerified { get; internal set; }
        public string EmailAlternate { get; internal set; }
        public string Email { get; internal set; }
        public string FullName { get; internal set; }
        public Guid UserIdentifier { get; internal set; }
        public DateTimeOffset? UserAccessGranted { get; internal set; }
        public DateTimeOffset? LastAuthenticated { get; internal set; }
    }
}
