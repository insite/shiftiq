export interface ApiUserModel {
    UserIdentifier: string | null | undefined;

    AccessGrantedToCmds: boolean;
    MultiFactorAuthentication: boolean;

    DefaultPassword: string | null | undefined;
    Email: string | null | undefined;
    EmailAlternate: string | null | undefined;
    EmailVerified: string | null | undefined;
    FirstName: string | null | undefined;
    FullName: string | null | undefined;
    Honorific: string | null | undefined;
    ImageUrl: string | null | undefined;
    Initials: string | null | undefined;
    LastName: string | null | undefined;
    LoginOrganizationCode: string | null | undefined;
    MiddleName: string | null | undefined;
    MultiFactorAuthenticationCode: string | null | undefined;
    OAuthProviderUserId: string | null | undefined;
    OldUserPasswordHash: string | null | undefined;
    PhoneMobile: string | null | undefined;
    SoundexFirstName: string | null | undefined;
    SoundexLastName: string | null | undefined;
    TimeZone: string | null | undefined;
    UserPasswordHash: string | null | undefined;

    PrimaryLoginMethod: number | null | undefined;
    SecondaryLoginMethod: number | null | undefined;
    UserPasswordChangeRequested: number | null | undefined;

    AccountCloaked: string | null | undefined;
    DefaultPasswordExpired: string | null | undefined;
    UserLicenseAccepted: string | null | undefined;
    UserPasswordChanged: string | null | undefined;
    UserPasswordExpired: string | null | undefined;
    UtcArchived: string | null | undefined;
    UtcUnarchived: string | null | undefined;
}