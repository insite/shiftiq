using System;

namespace InSite.Application.Messages.Read
{
    public class Clickthrough
    {
        public Guid ClickthroughIdentifier { get; set; }

        public Guid LinkIdentifier { get; set; }
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }

        public Guid UserIdentifier { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }

        public string UserHostAddress { get; set; }
        public string UserBrowser { get; set; }

        public DateTimeOffset Clicked { get; set; }
    }
}