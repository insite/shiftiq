using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public class MailoutState
    {
        public Guid Identifier { get; set; }
        public string Status { get; set; }
        public string StatusReason { get; set; }
        public EmailAddress[] Recipients { get; set; }

        public MailoutState()
        {
            Recipients = new EmailAddress[0];
        }
    }

    public class UserSubscriberState
    {
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset Subscribed { get; set; }
    }

    public class GroupSubscriberState
    {
        public Guid GroupIdentifier { get; set; }
        public DateTimeOffset Subscribed { get; set; }
    }

    public class MessageState : AggregateState
    {
        [JsonProperty]
        public bool IsArchived { get; private set; }

        [JsonProperty]
        public bool IsDisabled { get; private set; }

        [JsonProperty]
        public bool AutoBccSubscribers { get; private set; }

        [JsonProperty]
        public bool IsLocked { get; private set; }

        [JsonProperty]
        public Guid Sender { get; private set; }

        [JsonProperty]
        public string Type { get; private set; }

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public MultilingualString Title { get; private set; }

        [JsonProperty]
        public MultilingualString ContentText { get; private set; }

        [JsonProperty]
        public List<LinkItem> Links { get; private set; }

        [JsonProperty]
        public Dictionary<Guid, MailoutState> Mailouts { get; private set; }

        [JsonProperty]
        public Dictionary<Guid, GroupSubscriberState> GroupSubscribers { get; private set; }

        [JsonProperty]
        public Dictionary<Guid, UserSubscriberState> UserSubscribers { get; private set; }

        public MessageState()
        {
            Links = new List<LinkItem>();
            Mailouts = new Dictionary<Guid, MailoutState>();
            GroupSubscribers = new Dictionary<Guid, GroupSubscriberState>();
            UserSubscribers = new Dictionary<Guid, UserSubscriberState>();
        }

        #region Methods (events)

        public void When(Classified e)
        {

        }

        public void When(FollowerAdded e)
        {

        }

        public void When(FollowerRemoved e)
        {

        }

        public void When(LinkCounterReset e) { }

        public void When(MessageCreated e)
        {
            Sender = e.Sender;
            Type = e.Type;
            Name = e.Name;
            Title = (e.Title?.Clone()) ?? new MultilingualString();
            ContentText = (e.ContentText?.Clone()) ?? new MultilingualString();
            Links = e.Links ?? new List<LinkItem>();
            GroupSubscribers.Clear();
            UserSubscribers.Clear();
            Mailouts = new Dictionary<Guid, MailoutState>();
        }

        public void When(ContentChanged e)
        {
            ContentText = e.ContentText.Clone();
            Links = MessageLinkExtractor.ExtractLinks(ContentText);
        }

        public void When(MessageRenamed _)
        {
        }

        public void When(MessageRetitled e)
        {
            Title = e.Title.Clone();
        }

        public void When(SenderChanged e)
        {
            Sender = e.Sender;
        }

        public void When(SubscriberAdded e)
        {
            if (e.IsGroup)
                AddGroupSubscriber(e.ContactIdentifier, e.ChangeTime);
            else
                AddUserSubscriber(e.ContactIdentifier, e.ChangeTime);
        }

        public void When(SubscriberRemoved e)
        {
            if (e.IsGroup)
                GroupSubscribers.Remove(e.ContactIdentifier);
            else
                UserSubscribers.Remove(e.ContactIdentifier);
        }

        public void When(SubscribersAdded e)
        {
            if (e.IsGroup)
            {
                foreach (var groupId in e.ContactIdentifiers)
                    AddGroupSubscriber(groupId, e.ChangeTime);
            }
            else
            {
                foreach (var userId in e.ContactIdentifiers)
                    AddUserSubscriber(userId, e.ChangeTime);
            }
        }

        public void When(SubscribersRemoved e)
        {
            if (e.IsGroup)
            {
                foreach (var contactId in e.ContactIdentifiers)
                    GroupSubscribers.Remove(contactId);
            }
            else
            {
                foreach (var contactId in e.ContactIdentifiers)
                    UserSubscribers.Remove(contactId);
            }
        }

        public void When(SurveyFormSubmissionCompleted _) { }

        public void When(SurveyFormSubmissionStarted _) { }

        public void When(MailoutScheduled2 e)
        {
            AddMailout(e.MailoutIdentifier, "Scheduled", e.Recipients);
        }

        public void When(MailoutCancelled e)
        {
            RemoveMailout(e.MailoutIdentifier);
        }

        public void When(MailoutAborted e)
        {
            ChangeMailoutStatus(e.Mailout, "Aborted", e.Reason);
        }

        public void When(MailoutStarted e)
        {
            ChangeMailoutStatus(e.MailoutIdentifier, "Started");
        }

        public void When(MailoutCompleted e)
        {
            ChangeMailoutStatus(e.MailoutIdentifier, "Completed");
        }

        public void When(DeliveryStarted2 e)
        {

        }

        public void When(CarbonCopyStarted2 e)
        {

        }

        public void When(CarbonCopyCompleted2 e) { }

        public void When(DeliveryCompleted2 e) { }

        public void When(DeliveryBounced e)
        {
        }

        public void When(SurveyFormAssigned e)
        {
        }

        public void When(MessageArchived e)
        {
            IsArchived = true;
        }

        public void When(MessageEnabled _)
        {
            IsDisabled = false;
        }

        public void When(MessageDisabled _)
        {
            IsDisabled = true;
        }

        public void When(AutoBccSubscribersEnabled _)
        {
            AutoBccSubscribers = true;
        }

        public void When(AutoBccSubscribersDisabled _)
        {
            AutoBccSubscribers = false;
        }

        public void When(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType != ObsoleteChangeType.MailoutScheduled)
                return;

            var v2 = MailoutScheduled2.Upgrade(e);

            When(v2);
        }

        #endregion

        public void AddMailout(Guid mailout, string status, IEnumerable<EmailAddress> recipients)
        {
            if (!MailoutExists(mailout))
            {
                var state = new MailoutState
                {
                    Identifier = mailout,
                    Status = status,
                    Recipients = recipients.Select(x => x.Clone()).ToArray()
                };
                Mailouts.Add(mailout, state);
            }
        }

        public void ChangeMailoutStatus(Guid mailout, string status, string reason = null)
        {
            if (MailoutExists(mailout))
            {
                var state = FindMailout(mailout);
                state.Status = status;
                state.StatusReason = reason;
            }
        }

        public MailoutState FindMailout(Guid mailout)
        {
            if (Mailouts.TryGetValue(mailout, out MailoutState state))
                return state;
            return null;
        }

        public void RemoveMailout(Guid mailout)
        {
            Mailouts.Remove(mailout);
        }

        public bool MailoutExists(Guid mailout)
        {
            return Mailouts.ContainsKey(mailout);
        }

        private void AddGroupSubscriber(Guid groupId, DateTimeOffset subscribedOn)
        {
            if (GroupSubscribers.ContainsKey(groupId))
                return;

            GroupSubscribers.Add(groupId, new GroupSubscriberState
            {
                GroupIdentifier = groupId,
                Subscribed = subscribedOn
            });
        }

        private void AddUserSubscriber(Guid userId, DateTimeOffset subscribedOn)
        {
            if (UserSubscribers.ContainsKey(userId))
                return;

            UserSubscribers.Add(userId, new UserSubscriberState
            {
                UserIdentifier = userId,
                Subscribed = subscribedOn
            });
        }
    }
}