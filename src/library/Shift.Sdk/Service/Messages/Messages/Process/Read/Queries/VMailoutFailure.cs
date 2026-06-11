using System;
using System.Collections.Generic;
using System.Text;

namespace InSite.Application.Messages.Read
{
    public class VMailoutFailure
    {
        public Guid? RecipientIdentifier { get; set; }
        public Guid MailoutIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string MessageType { get; set; }
        public string MessageName { get; set; }
        public string MessageSubject { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string UserEmail { get; set; }
        public string RecipientName { get; set; }
        public string PersonName { get; set; }
        public string DeliveryStatus { get; set; }
        public string DeliveryError { get; set; }

        public DateTimeOffset MailoutScheduled { get; set; }
        public DateTimeOffset? MailoutFailed { get; set; }
    }
}
