using System;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Application.Messages.Read
{
    public class MessageChangeProcessor
    {
        private readonly ICommander _commander;

        private readonly IMessageSearch _messages;
        private readonly IContactSearch _contacts;

        public MessageChangeProcessor(ICommander commander, IChangeQueue publisher, IMessageSearch messages, IContactSearch contacts)
        {
            _commander = commander;

            _messages = messages;
            _contacts = contacts;

            publisher.Subscribe<MailoutCallbackHandled>(Handle);
        }

        private void Handle(MailoutCallbackHandled e)
        {
            if (e.Status == "Failed")
            {
                if (e.Recipient.IsEmpty())
                    return;

                var recipients = _messages.GetDeliveries(new DeliveryFilter
                {
                    MailoutIdentifier = e.Mailout,
                    RecipientAddress = e.Recipient
                });
                if (recipients.IsEmpty())
                    return;

                var recipient = recipients[0];
                var person = _contacts.GetPerson(recipient.UserIdentifier, recipient.OrganizationIdentifier);
                if (person == null)
                    return;

                if (person.UserEmailEnabled && string.Equals(person.UserEmail, e.Recipient, StringComparison.OrdinalIgnoreCase))
                {
                    var comment = GetComment("email", person.UserEmail);

                    _commander.Send(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.EmailEnabled, false));
                    _commander.Send(new ModifyPersonComment(person.PersonIdentifier, CommentActionType.Author, comment));
                }

                if (person.UserEmailAlternateEnabled && string.Equals(person.UserEmailAlternate, e.Recipient, StringComparison.OrdinalIgnoreCase))
                {
                    var comment = GetComment("alternate email", person.UserEmailAlternate);

                    _commander.Send(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.EmailAlternateEnabled, false));
                    _commander.Send(new ModifyPersonComment(person.PersonIdentifier, CommentActionType.Author, comment));
                }

                PersonComment GetComment(string emailType, string emailAddress)
                {
                    return new PersonComment
                    {
                        Comment = UniqueIdentifier.Create(),
                        Text = $"The {emailType} address {emailAddress} has been disabled due to a permanent delivery failure (mailout {e.Mailout}).",

                        Container = person.UserIdentifier,
                        ContainerType = "Person",

                        Author = UserIdentifiers.Maintenance,
                        AuthorName = "System Maintenance",

                        Organization = person.OrganizationIdentifier,

                        Topic = person.UserIdentifier
                    };
                }
            }
        }
    }
}
