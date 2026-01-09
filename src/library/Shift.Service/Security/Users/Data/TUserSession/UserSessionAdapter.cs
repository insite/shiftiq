using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class UserSessionAdapter : IEntityAdapter
{
    public void Copy(ModifyUserSession modify, UserSessionEntity entity)
    {
        entity.AuthenticationErrorType = modify.AuthenticationErrorType;
        entity.AuthenticationErrorMessage = modify.AuthenticationErrorMessage;
        entity.SessionCode = modify.SessionCode;
        entity.SessionIsAuthenticated = modify.SessionIsAuthenticated;
        entity.SessionStarted = modify.SessionStarted;
        entity.SessionStopped = modify.SessionStopped;
        entity.SessionMinutes = modify.SessionMinutes;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.UserAgent = modify.UserAgent;
        entity.UserBrowser = modify.UserBrowser;
        entity.UserBrowserVersion = modify.UserBrowserVersion;
        entity.UserEmail = modify.UserEmail;
        entity.UserHostAddress = modify.UserHostAddress;
        entity.UserIdentifier = modify.UserIdentifier;
        entity.UserLanguage = modify.UserLanguage;
        entity.AuthenticationSource = modify.AuthenticationSource;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public UserSessionEntity ToEntity(CreateUserSession create)
    {
        var entity = new UserSessionEntity
        {
            AuthenticationErrorType = create.AuthenticationErrorType,
            AuthenticationErrorMessage = create.AuthenticationErrorMessage,
            SessionCode = create.SessionCode,
            SessionIsAuthenticated = create.SessionIsAuthenticated,
            SessionStarted = create.SessionStarted,
            SessionStopped = create.SessionStopped,
            SessionMinutes = create.SessionMinutes,
            OrganizationIdentifier = create.OrganizationIdentifier,
            UserAgent = create.UserAgent,
            UserBrowser = create.UserBrowser,
            UserBrowserVersion = create.UserBrowserVersion,
            UserEmail = create.UserEmail,
            UserHostAddress = create.UserHostAddress,
            UserIdentifier = create.UserIdentifier,
            UserLanguage = create.UserLanguage,
            SessionIdentifier = create.SessionIdentifier,
            AuthenticationSource = create.AuthenticationSource
        };
        return entity;
    }

    public IEnumerable<UserSessionModel> ToModel(IEnumerable<UserSessionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public UserSessionModel ToModel(UserSessionEntity entity)
    {
        var model = new UserSessionModel
        {
            AuthenticationErrorType = entity.AuthenticationErrorType,
            AuthenticationErrorMessage = entity.AuthenticationErrorMessage,
            SessionCode = entity.SessionCode,
            SessionIsAuthenticated = entity.SessionIsAuthenticated,
            SessionStarted = entity.SessionStarted,
            SessionStopped = entity.SessionStopped,
            SessionMinutes = entity.SessionMinutes,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            UserAgent = entity.UserAgent,
            UserBrowser = entity.UserBrowser,
            UserBrowserVersion = entity.UserBrowserVersion,
            UserEmail = entity.UserEmail,
            UserHostAddress = entity.UserHostAddress,
            UserIdentifier = entity.UserIdentifier,
            UserLanguage = entity.UserLanguage,
            SessionIdentifier = entity.SessionIdentifier,
            AuthenticationSource = entity.AuthenticationSource
        };

        return model;
    }

    public IEnumerable<UserSessionMatch> ToMatch(IEnumerable<UserSessionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public UserSessionMatch ToMatch(UserSessionEntity entity)
    {
        var match = new UserSessionMatch
        {
            SessionIdentifier = entity.SessionIdentifier

        };

        return match;
    }
}