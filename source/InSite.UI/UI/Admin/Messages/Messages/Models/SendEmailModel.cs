using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

namespace InSite.UI.Admin.Messages.Messages.Models
{
    [Serializable]
    public class SendEmailModel
    {
        [Serializable]
        public class RecipientInfo
        {
            public Guid UserIdentifier { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PersonCode { get; set; }
            public string Language { get; set; }
        }

        private readonly string _recipientType;
        private readonly Dictionary<Guid, RecipientInfo> _recipients = null;

        public string RecipientType => _recipientType;

        public int RecipientCount => _recipients?.Count ?? 0;

        public SendEmailModel(Guid organizationId, string recipientType, IEnumerable<Guid> recipientIds)
        {
            _recipientType = recipientType;
            _recipients = GetRecipients(organizationId, recipientIds);
        }

        private static Dictionary<Guid, RecipientInfo> GetRecipients(Guid organizationId, IEnumerable<Guid> recipientIds)
        {
            var contacts = PersonCriteria.Bind(
                x => new RecipientInfo
                {
                    UserIdentifier = x.UserIdentifier,
                    Email = x.User.Email,
                    FullName = x.User.FullName,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    PersonCode = x.PersonCode,
                    Language = x.Language
                },
                new PersonFilter
                {
                    OrganizationIdentifier = organizationId,
                    EmailEnabled = true,
                    IncludeUserIdentifiers = recipientIds.ToArray()
                });

            return contacts.ToDictionary(x => x.UserIdentifier, x => x);
        }

        public string GetConfirmText()
        {
            var count = Shift.Common.Humanizer.ToQuantity(_recipients.Count, _recipientType.ToLower());
            return $"Are you sure you want to send this message to {count}?";
        }

        public string GetSuccessText()
        {
            var count = Shift.Common.Humanizer.ToQuantity(_recipients.Count, _recipientType.ToLower());
            return $"Your message was successfully scheduled for delivery to {count}.";
        }

        public List<RecipientInfo> GetRecipients()
        {
            return _recipients
                .Select(x => x.Value)
                .GroupBy(x => x.Email, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.First()).OrderBy(x => x.Email)
                .ToList();
        }

        public string GetRecipientCount() =>
            Shift.Common.Humanizer.ToQuantity(_recipients.Count, _recipientType);

        public RecipientInfo GetRecipient(object o) =>
            o is Guid id ? GetRecipient(id) : default;

        public RecipientInfo GetRecipient(Guid id) =>
            _recipients.TryGetValue(id, out var result) ? result : default;
    }
}