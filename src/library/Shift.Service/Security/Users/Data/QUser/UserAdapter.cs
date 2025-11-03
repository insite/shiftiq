using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class UserAdapter : IEntityAdapter
{
    public void Copy(ModifyUser modify, UserEntity entity)
    {
        entity.DefaultPassword = modify.DefaultPassword;
        entity.DefaultPasswordExpired = modify.DefaultPasswordExpired;
        entity.Email = modify.Email;
        entity.EmailVerified = modify.EmailVerified;
        entity.FirstName = modify.FirstName;
        entity.Honorific = modify.Honorific;
        entity.ImageUrl = modify.ImageUrl;
        entity.Initials = modify.Initials;
        entity.AccessGrantedToCmds = modify.AccessGrantedToCmds;
        entity.LastName = modify.LastName;
        entity.MiddleName = modify.MiddleName;
        entity.FullName = modify.FullName;
        entity.SoundexFirstName = modify.SoundexFirstName;
        entity.SoundexLastName = modify.SoundexLastName;
        entity.TimeZone = modify.TimeZone;
        entity.UserLicenseAccepted = modify.UserLicenseAccepted;
        entity.UserPasswordChanged = modify.UserPasswordChanged;
        entity.UserPasswordExpired = modify.UserPasswordExpired;
        entity.UserPasswordHash = modify.UserPasswordHash;
        entity.UtcArchived = modify.UtcArchived;
        entity.UtcUnarchived = modify.UtcUnarchived;
        entity.MultiFactorAuthentication = modify.MultiFactorAuthentication;
        entity.MultiFactorAuthenticationCode = modify.MultiFactorAuthenticationCode;
        entity.EmailAlternate = modify.EmailAlternate;
        entity.PhoneMobile = modify.PhoneMobile;
        entity.AccountCloaked = modify.AccountCloaked;
        entity.PrimaryLoginMethod = modify.PrimaryLoginMethod;
        entity.SecondaryLoginMethod = modify.SecondaryLoginMethod;
        entity.OAuthProviderUserId = modify.OAuthProviderUserId;
        entity.LoginOrganizationCode = modify.LoginOrganizationCode;
        entity.OldUserPasswordHash = modify.OldUserPasswordHash;
        entity.UserPasswordChangeRequested = modify.UserPasswordChangeRequested;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public UserEntity ToEntity(CreateUser create)
    {
        var entity = new UserEntity
        {
            DefaultPassword = create.DefaultPassword,
            DefaultPasswordExpired = create.DefaultPasswordExpired,
            Email = create.Email,
            EmailVerified = create.EmailVerified,
            FirstName = create.FirstName,
            Honorific = create.Honorific,
            ImageUrl = create.ImageUrl,
            Initials = create.Initials,
            AccessGrantedToCmds = create.AccessGrantedToCmds,
            LastName = create.LastName,
            MiddleName = create.MiddleName,
            FullName = create.FullName,
            SoundexFirstName = create.SoundexFirstName,
            SoundexLastName = create.SoundexLastName,
            UserIdentifier = create.UserIdentifier,
            TimeZone = create.TimeZone,
            UserLicenseAccepted = create.UserLicenseAccepted,
            UserPasswordChanged = create.UserPasswordChanged,
            UserPasswordExpired = create.UserPasswordExpired,
            UserPasswordHash = create.UserPasswordHash,
            UtcArchived = create.UtcArchived,
            UtcUnarchived = create.UtcUnarchived,
            MultiFactorAuthentication = create.MultiFactorAuthentication,
            MultiFactorAuthenticationCode = create.MultiFactorAuthenticationCode,
            EmailAlternate = create.EmailAlternate,
            PhoneMobile = create.PhoneMobile,
            AccountCloaked = create.AccountCloaked,
            PrimaryLoginMethod = create.PrimaryLoginMethod,
            SecondaryLoginMethod = create.SecondaryLoginMethod,
            OAuthProviderUserId = create.OAuthProviderUserId,
            LoginOrganizationCode = create.LoginOrganizationCode,
            OldUserPasswordHash = create.OldUserPasswordHash,
            UserPasswordChangeRequested = create.UserPasswordChangeRequested
        };
        return entity;
    }

    public IEnumerable<UserModel> ToModel(IEnumerable<UserEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public UserModel ToModel(UserEntity entity)
    {
        var model = new UserModel
        {
            DefaultPassword = entity.DefaultPassword,
            DefaultPasswordExpired = entity.DefaultPasswordExpired,
            Email = entity.Email,
            EmailVerified = entity.EmailVerified,
            FirstName = entity.FirstName,
            Honorific = entity.Honorific,
            ImageUrl = entity.ImageUrl,
            Initials = entity.Initials,
            AccessGrantedToCmds = entity.AccessGrantedToCmds,
            LastName = entity.LastName,
            MiddleName = entity.MiddleName,
            FullName = entity.FullName,
            SoundexFirstName = entity.SoundexFirstName,
            SoundexLastName = entity.SoundexLastName,
            UserIdentifier = entity.UserIdentifier,
            TimeZone = entity.TimeZone,
            UserLicenseAccepted = entity.UserLicenseAccepted,
            UserPasswordChanged = entity.UserPasswordChanged,
            UserPasswordExpired = entity.UserPasswordExpired,
            UserPasswordHash = entity.UserPasswordHash,
            UtcArchived = entity.UtcArchived,
            UtcUnarchived = entity.UtcUnarchived,
            MultiFactorAuthentication = entity.MultiFactorAuthentication,
            MultiFactorAuthenticationCode = entity.MultiFactorAuthenticationCode,
            EmailAlternate = entity.EmailAlternate,
            PhoneMobile = entity.PhoneMobile,
            AccountCloaked = entity.AccountCloaked,
            PrimaryLoginMethod = entity.PrimaryLoginMethod,
            SecondaryLoginMethod = entity.SecondaryLoginMethod,
            OAuthProviderUserId = entity.OAuthProviderUserId,
            LoginOrganizationCode = entity.LoginOrganizationCode,
            OldUserPasswordHash = entity.OldUserPasswordHash,
            UserPasswordChangeRequested = entity.UserPasswordChangeRequested
        };

        return model;
    }

    public IEnumerable<UserMatch> ToMatch(IEnumerable<UserEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public UserMatch ToMatch(UserEntity entity)
    {
        var match = new UserMatch
        {
            UserIdentifier = entity.UserIdentifier,
            FullName = entity.FullName
        };

        return match;
    }
}
