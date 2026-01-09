using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public class MessageAggregate : AggregateRoot
    {
        #region Properties

        public override AggregateState CreateState() => new MessageState();
        public MessageState Data => (MessageState)State;

        #endregion

        #region Methods (commands)

        public void AbortMailout(Guid mailout, string reason)
        {
            Apply(new MailoutAborted(mailout, reason));
        }

        public void AttachContact(Guid contact, string role, bool isGroup)
        {
            if (isGroup && Data.GroupSubscribers.ContainsKey(contact) || !isGroup && Data.UserSubscribers.ContainsKey(contact))
                return;

            var e = new SubscriberAdded(contact, role, isGroup);

            Apply(e);
        }

        public void DetachContact(Guid contact, bool isGroup)
        {
            if (isGroup && !Data.GroupSubscribers.ContainsKey(contact) || !isGroup && !Data.UserSubscribers.ContainsKey(contact))
                return;

            var e = new SubscriberRemoved(contact, null, isGroup);

            Apply(e);
        }

        public void FollowContact(Guid contact, Guid follower)
        {
            if (contact == follower)
                return;

            var e = new FollowerAdded(contact, follower);

            Apply(e);
        }

        public void UnfollowContact(Guid contact, Guid follower)
        {
            var e = new FollowerRemoved(contact, follower);

            Apply(e);
        }

        public void Classify(string description, string mailoutType, string recipientType, string recipientRole)
        {
            var e = new Classified(description, mailoutType, recipientType, recipientRole);

            Apply(e);
        }

        public void Create(Guid organization, Guid sender, string type, string name, MultilingualString title, MultilingualString text, Guid? survey)
        {
            if (title.Default.IsEmpty())
                throw ApplicationError.Create("The value of Title for default language is undefined");

            if (!text.IsEmpty && text.Default.IsEmpty())
                throw ApplicationError.Create("The value of ContentText for default language is undefined");

            var e = new MessageCreated(organization, sender, type, name, title, text, MessageLinkExtractor.ExtractLinks(text), survey);

            Apply(e);
        }

        public void ChangeContent(MultilingualString text)
        {
            if (Data.ContentText.IsEqual(text))
                return;

            if (!text.IsEmpty && text.Default.IsEmpty())
                throw ApplicationError.Create("The value of ContentText for default language is undefined");

            Apply(new ContentChanged(text, MessageLinkExtractor.ExtractLinks(text)));
        }

        public void Rename(string name)
        {
            if (Data.Name == name || name.IsEmpty())
                return;

            var e = new MessageRenamed(name);

            Apply(e);
        }

        public void Retitle(MultilingualString title)
        {
            if (title.IsEmpty || Data.Title.IsEqual(title))
                return;

            if (title.Default.IsEmpty())
                throw ApplicationError.Create("The value of Title for default language is undefined");

            var e = new MessageRetitled(title);

            Apply(e);
        }

        public void ChangeSender(Guid sender)
        {
            if (Data.Sender.Equals(sender))
                return;

            var e = new SenderChanged(sender);

            Apply(e);
        }

        public void AttachContacts(IEnumerable<Guid> recipients, string role, bool isGroup)
        {
            Guid[] addSubscribers = isGroup
                ? recipients.Where(x => !Data.GroupSubscribers.ContainsKey(x)).ToArray()
                : recipients.Where(x => !Data.UserSubscribers.ContainsKey(x)).ToArray();

            if (addSubscribers.Length == 0)
                return;

            var e = new SubscribersAdded(addSubscribers, role, isGroup);

            Apply(e);
        }

        public void DetachContacts(IEnumerable<Guid> recipients, bool isGroup)
        {
            var removeSubscribers = isGroup
                ? recipients.Where(x => Data.GroupSubscribers.ContainsKey(x)).ToArray()
                : recipients.Where(x => Data.UserSubscribers.ContainsKey(x)).ToArray();

            if (removeSubscribers.Length == 0)
                return;

            var e = new SubscribersRemoved(removeSubscribers, isGroup);

            Apply(e);
        }

        public void ScheduleMailout(Guid mailout, DateTimeOffset at, Guid sender, IList<EmailAddress> recipients, MultilingualString subject, MultilingualString body, IDictionary<string, string> variables, Guid? @event, IList<string> attachments)
        {
            if (Data.MailoutExists(mailout) || recipients.IsEmpty())
                return;

            var recipientsClone = new EmailAddress[recipients.Count];
            for (var i = 0; i < recipients.Count; i++)
            {
                var email = recipients[i];
                if (email.Address.IsEmpty())
                    throw ApplicationError.Create("The mailout recipient list contains a recipient without an email address.");

                recipientsClone[i] = email.Clone();
            }

            var e = new MailoutScheduled2(
                mailout,
                at,
                sender,
                recipientsClone,
                subject.Clone(),
                body.Clone(),
                new Dictionary<string, string>(variables.EmptyIfNull()),
                @event,
                attachments.EmptyIfNull().ToArray());

            Apply(e);
        }

        public void CancelMailout(Guid mailout)
        {
            Apply(new MailoutCancelled(mailout));
        }

        public void StartMailout(Guid mailout)
        {
            if (!Data.MailoutExists(mailout))
                return;

            var e = new MailoutStarted(mailout);

            Apply(e);
        }

        public void CompleteMailout(Guid mailout)
        {
            Apply(new MailoutCompleted(mailout));
        }

        public void StartDelivery(Guid mailout, Guid recipient)
        {
            if (!Data.MailoutExists(mailout))
                return;

            var state = Data.FindMailout(mailout);
            if (!state.Recipients.Any(x => x.Identifier == recipient))
                return;

            var e = new DeliveryStarted2(mailout, recipient);

            Apply(e);
        }

        public void StartCarbonCopy(Guid mailout, Guid recipient, string ccType, Guid cc)
        {
            if (!Data.MailoutExists(mailout))
                return;

            var state = Data.FindMailout(mailout);
            if (!state.Recipients.Any(x => x.Identifier == recipient))
                return;

            var e = new CarbonCopyStarted2(mailout, recipient, ccType, cc);

            Apply(e);
        }

        public void CompleteCarbonCopy(Guid mailout, Guid recipient, string ccType, Guid cc, string error)
        {
            if (!Data.MailoutExists(mailout))
                return;

            var state = Data.FindMailout(mailout);
            if (!state.Recipients.Any(x => x.Identifier == recipient))
                return;

            var e = new CarbonCopyCompleted2(mailout, recipient, ccType, cc, error);

            Apply(e);
        }

        public void CompleteDelivery(Guid mailout, Guid recipient, string error)
        {
            if (!Data.MailoutExists(mailout))
                return;

            var state = Data.FindMailout(mailout);
            if (!state.Recipients.Any(x => x.Identifier == recipient))
                return;

            var e = new DeliveryCompleted2(mailout, recipient, error);

            Apply(e);
        }

        public void BounceDelivery(string file, DateTimeOffset time, string type, string reason, string subject, string body, string address, Guid? mailout)
        {
            var e = new DeliveryBounced(file, time, type, reason, subject, body, address, mailout);

            Apply(e);
        }

        public void AssignSurveyForm(Guid survey)
        {
            var e = new SurveyFormAssigned(survey);

            Apply(e);
        }

        public void ArchiveMessage() => Apply(new MessageArchived());

        public void EnableMessage() => Apply(new MessageEnabled());

        public void DisableMessage() => Apply(new MessageDisabled());

        public void EnableAutoBccSubscribers() => Apply(new AutoBccSubscribersEnabled());

        public void DisableAutoBccSubscribers() => Apply(new AutoBccSubscribersDisabled());

        public void ResetLinkCounter(Guid linkIdentifier)
        {
            var e = new LinkCounterReset(linkIdentifier);

            Apply(e);
        }

        #endregion
    }
}