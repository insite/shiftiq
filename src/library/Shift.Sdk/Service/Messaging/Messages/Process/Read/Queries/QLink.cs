using System;

namespace InSite.Application.Messages.Read
{
    public class QLink
    {
        public Guid LinkIdentifier { get; set; }
        public Guid MessageIdentifier { get; set; }

        public String LinkText { get; set; }
        public String LinkUrl { get; set; }
        public String LinkUrlHash { get; set; }

        public Int32 ClickCount { get; set; }
        public Int32 UserCount { get; set; }
    }
}