using System;

namespace InSite.Application.Messages.Read
{
    public class VClick
    {
        public Guid ClickIdentifier { get; set; }
        public Guid LinkIdentifier { get; set; }
        public Guid MessageIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public string MessageTitle { get; set; }
        public string UserBrowser { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        public string UserHostAddress { get; set; }

        public DateTimeOffset Clicked { get; set; }
    }
}
