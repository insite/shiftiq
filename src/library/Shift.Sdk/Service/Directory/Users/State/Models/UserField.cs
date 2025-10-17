namespace InSite.Domain.Contacts
{
    public enum UserField
    {
        DefaultPassword,
        Email,
        EmailAlternate,
        EmailVerified,
        FirstName,
        FullName,
        Honorific,
        ImageUrl,
        Initials,
        LastName,
        LoginOrganizationCode,
        MiddleName,
        MultiFactorAuthenticationCode,
        OAuthProviderUserId,
        OldUserPasswordHash,
        PhoneMobile,
        TimeZone,
        UserPasswordHash,

        AccessGrantedToCmds,
        MultiFactorAuthentication,

        PrimaryLoginMethod,
        SecondaryLoginMethod,
        UserPasswordChangeRequested,

        AccountCloaked,
        DefaultPasswordExpired,
        UserLicenseAccepted,
        UserPasswordChanged,
        UserPasswordExpired,
        UtcArchived,
        UtcUnarchived,
    }
}
