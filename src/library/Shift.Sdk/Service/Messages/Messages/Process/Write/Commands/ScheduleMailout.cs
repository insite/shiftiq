using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Messages.Write
{
    public class ScheduleMailout : Command
    {
        [JsonConstructor]
        private ScheduleMailout()
        {

        }

        public ScheduleMailout(
            Guid messageId, Guid mailoutId, Guid senderId, DateTimeOffset scheduledOn,
            IList<EmailAddress> recipients, MultilingualString subject, MultilingualString body,
            IDictionary<string, string> variables, IList<string> attachments, Guid? @event)
        {
            AggregateIdentifier = messageId;
            MailoutIdentifier = mailoutId;
            SenderIdentifier = senderId;
            At = scheduledOn;
            Recipients = recipients;
            Subject = subject;
            Body = body;
            Variables = variables;
            Attachments = attachments;
            EventIdentifier = @event;
        }

        public Guid MailoutIdentifier { get; set; }
        public Guid SenderIdentifier { get; set; }
        public DateTimeOffset At { get; set; }
        public IList<EmailAddress> Recipients { get; set; }
        public MultilingualString Subject { get; set; }
        public MultilingualString Body { get; set; }
        public IDictionary<string, string> Variables { get; set; }
        public IList<string> Attachments { get; set; }
        public Guid? EventIdentifier { get; set; }
    }
}