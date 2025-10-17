using System;
using System.Collections.Generic;

namespace InSite.Application.Messages.Read
{
    public class QRecipient
    {
        public virtual QMailout Mailout { get; set; }
        public virtual ICollection<QCarbonCopy> CarbonCopies { get; set; } = new HashSet<QCarbonCopy>();

        public Guid MailoutIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string DeliveryError { get; set; }
        public string DeliveryStatus { get; set; }
        public string PersonCode { get; set; }
        public string PersonLanguage { get; set; }
        public string PersonName { get; set; }
        public string RecipientVariables { get; set; }
        public string UserEmail { get; set; }

        public DateTimeOffset? DeliveryCompleted { get; set; }
        public DateTimeOffset? DeliveryStarted { get; set; }
    }

    public class QCarbonCopy
    {
        public virtual QRecipient Recipient { get; set; }

        public Guid CarbonCopyIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid RecipientIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}