using System;
using System.Collections.Generic;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public partial class MailoutDrafted2 : Change
    {
        public Guid MailoutId { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public Guid SenderId { get; set; }
        public string SenderType { get; set; }
        public MailoutRecipientEmail[] To { get; set; }
        public MailoutRecipientEmail[] Cc { get; set; }
        public MailoutRecipientEmail[] Bcc { get; set; }
        public MultilingualString Subject { get; set; }
        public MultilingualString BodyText { get; set; }
        public MultilingualString BodyHtml { get; set; }
        public IDictionary<string, string> Variables { get; set; }
        public IList<string> Attachments { get; set; }
        public Guid? EventId { get; set; }

        public MailoutDrafted2(
            Guid mailoutId,
            DateTimeOffset scheduledOn,
            Guid senderId,
            string senderType,
            MailoutRecipientEmail[] to,
            MailoutRecipientEmail[] cc,
            MailoutRecipientEmail[] bcc,
            MultilingualString subject,
            MultilingualString bodyText,
            MultilingualString bodyHtml,
            IDictionary<string, string> variables,
            IList<string> attachments,
            Guid? eventId)
        {
            MailoutId = mailoutId;
            ScheduledOn = scheduledOn;
            SenderId = senderId;
            SenderType = senderType;
            To = to;
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            BodyText = bodyText;
            BodyHtml = bodyHtml;
            Variables = variables;
            Attachments = attachments;
            EventId = eventId;
        }
    }
}
