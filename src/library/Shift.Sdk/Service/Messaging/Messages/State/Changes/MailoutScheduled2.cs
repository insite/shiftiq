using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public partial class MailoutScheduled2 : Change
    {
        public Guid MailoutIdentifier { get; set; }
        public Guid SenderIdentifier { get; set; }
        public DateTimeOffset At { get; set; }
        public IEnumerable<EmailAddress> Recipients { get; set; }
        public MultilingualString Subject { get; set; }
        public MultilingualString BodyText { get; set; }
        public IDictionary<string, string> Variables { get; private set; }
        public IEnumerable<string> Attachments { get; set; }

        public Guid? EventIdentifier { get; set; }

        public MailoutScheduled2(Guid mailoutIdentifier, DateTimeOffset at, Guid sender, IEnumerable<EmailAddress> recipients, MultilingualString subject, MultilingualString body, IDictionary<string, string> variables, Guid? @event, IEnumerable<string> attachments)
        {
            MailoutIdentifier = mailoutIdentifier;
            SenderIdentifier = sender;
            At = at;
            Recipients = recipients;
            Subject = subject;
            BodyText = body;
            Variables = variables;
            EventIdentifier = @event;
            Attachments = attachments;
        }
    }
}
