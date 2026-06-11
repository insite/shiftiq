using System;

using Shift.Common;

namespace InSite.Application.Messages.Read
{
    [Serializable]
    public class VClickFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }

        public DateTimeOffset? ClickedSince { get; set; }
        public DateTimeOffset? ClickedBefore { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public Guid MessageIdentifier { get; set; }
        public string MessageTitle { get; set; }
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public string UserBrowser { get; set; }
        public string UserHostAddress { get; set; }
    }
}
