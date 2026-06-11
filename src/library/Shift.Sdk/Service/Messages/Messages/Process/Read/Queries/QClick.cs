using System;

namespace InSite.Application.Messages.Read
{
    public class QClick
    {
        public Guid ClickIdentifier { get; set; }
        public Guid LinkIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string UserBrowser { get; set; }
        public string UserHostAddress { get; set; }
        
        public DateTimeOffset Clicked { get; set; }
    }
}