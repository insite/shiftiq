using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public partial class MailoutScheduled2
    {
        private class MailoutScheduled1 : Change
        {
            public Guid MailoutIdentifier { get; set; }
            public DateTimeOffset At { get; set; }

            public string PostOffice { get; set; }
            public Guid SenderIdentifier { get; set; }

            public MailoutScheduled1Recipient[] Recipients { get; set; }

            public MultilingualString Subject { get; set; }
            public MultilingualString BodyText { get; set; }
            public string BodyStyle { get; set; }
            public IDictionary<string, string> Variables { get; set; }

            public IEnumerable<string> Attachments { get; set; }

            public Guid? EventIdentifier { get; set; }
        }

        private class MailoutScheduled1Recipient
        {
            public Guid Identifier { get; set; }
            public string Address { get; set; }
            public string Name { get; set; }
            public string PersonCode { get; set; }
            public string Language { get; set; }

            public List<Guid> Cc { get; set; }
            public List<Guid> Bcc { get; set; }

            public Dictionary<string, string> Variables { get; set; }
        }

        public static MailoutScheduled2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<MailoutScheduled1>();

            var v2 = new MailoutScheduled2(v1.MailoutIdentifier, v1.At, v1.SenderIdentifier, CreateEmailAddressArray(v1.Recipients),
                v1.Subject, v1.BodyText, v1.Variables, v1.EventIdentifier, v1.Attachments)
            {
                AggregateIdentifier = v1.AggregateIdentifier,
                AggregateVersion = v1.AggregateVersion,
                OriginOrganization = v1.OriginOrganization,
                OriginUser = v1.OriginUser,
                ChangeTime = v1.ChangeTime
            };

            return v2;

            EmailAddress[] CreateEmailAddressArray(MailoutScheduled1Recipient[] recipients)
            {
                var result = new EmailAddress[recipients.Length];
                for (var i = 0; i < recipients.Length; i++)
                {
                    var handle = recipients[i];

                    var email = new EmailAddress(handle.Identifier, handle.Address, handle.Name, handle.PersonCode, handle.Language)
                    {
                        Cc = handle.Cc ?? new List<Guid>(),
                        Bcc = handle.Bcc ?? new List<Guid>(),
                        Variables = handle.Variables ?? new Dictionary<string, string>()
                    };

                    result[i] = email;
                }
                return result;
            }
        }
    }
}
