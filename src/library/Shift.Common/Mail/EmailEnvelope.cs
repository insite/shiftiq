using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    /// <remarks>
    /// This interface represents the destination address for an email message sent to one specific
    /// recipient email address (and is optionally carbon-copied to other email addresses).
    /// </remarks>
    public interface IEmailEnvelope<T>
    {
        T To { get; set; }
        List<T> Cc { get; set; }
        List<T> Bcc { get; set; }
    }

    [Serializable]
    public class EmailEnvelope : IEmailEnvelope<string>
    {
        public EmailEnvelope()
        {
            Cc = new List<string>();
            Bcc = new List<string>();
        }

        public EmailEnvelope(string to) : this()
        {
            To = to;
        }

        public string To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
    }

    [Serializable]
    public class IdentifierEnvelope : IEmailEnvelope<Guid>
    {
        public IdentifierEnvelope()
        {
            Cc = new List<Guid>();
            Bcc = new List<Guid>();
        }

        public IdentifierEnvelope(Guid to) : this()
        {
            To = to;
        }

        public Guid To { get; set; }
        public List<Guid> Cc { get; set; }
        public List<Guid> Bcc { get; set; }
    }

    public class EmailEnvelopeAdapter
    {
        private readonly Func<Guid, string> _resolve;

        public EmailEnvelopeAdapter(Func<Guid, string> resolve)
        {
            _resolve = resolve;
        }

        public EmailEnvelope CreateEmailEnvelopes(IdentifierEnvelope draft)
        {
            EnsureNonNullLists(draft);

            var envelope = new EmailEnvelope(ResolveIdentifierToEmail(draft.To));

            ResolveIdentifiersToEmails(draft.Cc, envelope.Cc);

            ResolveIdentifiersToEmails(draft.Bcc, envelope.Bcc);

            return envelope;
        }

        private string ResolveIdentifierToEmail(Guid identifier)
        {
            var item = _resolve(identifier);

            return item;
        }

        private void ResolveIdentifiersToEmails(List<Guid> identifiers, List<string> emails)
        {
            foreach (var identifier in identifiers)
            {
                var item = _resolve(identifier);

                if (!string.IsNullOrEmpty(item))
                {
                    if (!emails.Contains(item, StringComparer.OrdinalIgnoreCase))
                    {
                        emails.Add(item);
                    }
                }
            }
        }

        private void EnsureNonNullLists(IdentifierEnvelope envelope)
        {
            if (envelope.Cc == null)
                envelope.Cc = new List<Guid>();

            if (envelope.Bcc == null)
                envelope.Bcc = new List<Guid>();
        }
    }
}