using System;

namespace InSite.Application.Messages.Read
{
    public interface ISubscriberPerson
    {
        Guid MessageIdentifier { get; set; }
        string MessageName { get; set; }
        Guid MessageOrganizationIdentifier { get; set; }
        string MessageTitle { get; set; }
        string PersonLanguage { get; set; }
        DateTimeOffset Subscribed { get; set; }
        string UserEmail { get; set; }
        string UserEmailAlternate { get; set; }
        bool UserEmailAlternateEnabled { get; set; }
        bool UserEmailEnabled { get; set; }
        bool UserMarketingEmailEnabled { get; set; }
        string UserFirstName { get; set; }
        string UserFullName { get; set; }
        Guid UserIdentifier { get; set; }
        string UserLastName { get; set; }
        string PersonCode { get; set; }
    }
}