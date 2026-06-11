using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public partial class MailoutDrafted2
    {
        public class MailoutDrafted1 : Change
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
            public IDictionary<string, string> Variables { get; set; }
            public IList<string> Attachments { get; set; }
            public Guid? EventId { get; set; }

            public MailoutDrafted1(
                Guid mailoutId,
                DateTimeOffset scheduledOn,
                Guid senderId,
                string senderType,
                IDictionary<Guid, string> to,
                IDictionary<Guid, string> cc,
                IDictionary<Guid, string> bcc,
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

        public static MailoutDrafted2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<MailoutDrafted1>();

            var v2 = new MailoutDrafted2(
                v1.MailoutId,
                v1.ScheduledOn,
                v1.SenderId,
                v1.SenderType,
                v1.To.Select(x => new MailoutRecipientEmail(x.Key, x.Value)).ToArray(),
                v1.Cc.Select(x => new MailoutRecipientEmail(x.Key, x.Value)).ToArray(),
                v1.Bcc.Select(x => new MailoutRecipientEmail(x.Key, x.Value)).ToArray(),
                v1.Subject,
                v1.BodyText,
                v1.BodyHtml,
                v1.Variables,
                v1.Attachments,
                v1.EventId)
            {
                AggregateIdentifier = v1.AggregateIdentifier,
                AggregateVersion = v1.AggregateVersion,
                OriginOrganization = v1.OriginOrganization,
                OriginUser = v1.OriginUser,
                ChangeTime = v1.ChangeTime
            };

            return v2;
        }
    }
}
