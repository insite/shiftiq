using System;

namespace InSite.Application.Messages.Read
{
    public class XSubscriberUser : ISubscriberPerson
    {
        public Guid MessageIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset Subscribed { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserFullName { get; set; }

        public string UserEmail { get; set; }
        public bool UserEmailEnabled { get; set; }
        public bool UserMarketingEmailEnabled { get; set; }
        public string UserEmailAlternate { get; set; }
        public bool UserEmailAlternateEnabled { get; set; }

        public string PersonLanguage { get; set; }
        public string PersonCode { get; set; }

        public string MessageName { get; set; }
        public Guid MessageOrganizationIdentifier { get; set; }
        public string MessageTitle { get; set; }
    }
}
