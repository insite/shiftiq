using System;
using System.Collections.Generic;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MailoutDrafted : Change
    {
        public Guid MailoutId { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public Guid SenderId { get; set; }
        public string SenderType { get; set; }
        public IDictionary<Guid, string> To { get; set; }
        public IDictionary<Guid, string> Cc { get; set; }
        public IDictionary<Guid, string> Bcc { get; set; }
        public MultilingualString Subject { get; set; }
        public MultilingualString BodyText { get; set; }
        public MultilingualString BodyHtml { get; set; }
        public IList<string> Attachments { get; set; }
        public Guid? EventId { get; set; }

        public MailoutDrafted(
            Guid mailoutId, DateTimeOffset scheduledOn,
            Guid senderId, string senderType,
            IDictionary<Guid, string> to, IDictionary<Guid, string> cc, IDictionary<Guid, string> bcc,
            MultilingualString subject, MultilingualString bodyText, MultilingualString bodyHtml, IList<string> attachments,
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
            Attachments = attachments;
            EventId = eventId;
        }
    }
}
