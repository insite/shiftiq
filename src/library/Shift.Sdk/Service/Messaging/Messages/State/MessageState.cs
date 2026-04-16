using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Domain.Messages
{
    public class MailoutState
    {
        public Guid Identifier { get; set; }
        public string Status { get; set; }
        public string StatusReason { get; set; }
        public string StatusDescription { get; set; }
        public MailoutRecipientState[] Recipients { get; set; }
        public HashSet<Guid> CallbackIds { get; }

        public MailoutState()
        {
            Recipients = new MailoutRecipientState[0];
            CallbackIds = new HashSet<Guid>();
        }

        public MailoutRecipientState GetRecipient(string address)
        {
            return address.IsEmpty()
                ? null
                : Recipients.FirstOrDefault(x => string.Equals(x.Email.Address, address, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsCompleted()
        {
            return Recipients.IsEmpty() || Recipients
                .All(x => x.CallbackStatus == MailoutCallbackStatus.Rejected
                       || x.CallbackStatus == MailoutCallbackStatus.Delivered
                       || x.CallbackStatus == MailoutCallbackStatus.Failed);
        }
    }

    public class MailoutRecipientState
    {
        public EmailAddress Email { get; set; }
        public string CallbackStatus { get; set; }
        public DateTime? CallbackTimestamp { get; set; }
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

        public void When(MailoutCallbackHandled e)
        {
            var mailout = FindMailout(e.Mailout);
            if (!mailout.CallbackIds.Add(e.CallbackId))
                return;

            var allowHandle = e.Status == MailoutCallbackStatus.Accepted
                || e.Status == MailoutCallbackStatus.Delivered
                || e.Status == MailoutCallbackStatus.Failed
                || e.Status == MailoutCallbackStatus.Rejected;
            if (!allowHandle)
                return;

            var recipient = mailout.GetRecipient(e.Recipient);
            if (recipient == null)
                return;

            if (recipient.CallbackTimestamp.HasValue && recipient.CallbackTimestamp.Value > e.Timestamp)
                return;

            recipient.CallbackStatus = e.Status;
            recipient.CallbackTimestamp = e.Timestamp;
            (mailout.Status, mailout.StatusDescription) = CalculateMailoutStatus(mailout.Recipients);
        }

        public void When(MailoutStarted e)
        {
            ChangeMailoutStatus(e.MailoutIdentifier, "Started");
        }

        public void When(MailoutCompleted e)
        {
            ChangeMailoutStatus(e.MailoutIdentifier, "Completed");
        }

        public void When(MailoutDrafted e)
        {
            Mailouts.Add(e.MailoutId, new MailoutState
            {
                Identifier = e.MailoutId,
                Status = MailoutCallbackStatus.Drafted,
                Recipients = e.To
                    .Select(x => new MailoutRecipientState
                    {
                        Email = new EmailAddress(x.Key, x.Value, null, null, null),
                        CallbackStatus = MailoutCallbackStatus.Drafted,
                        CallbackTimestamp = null
                    })
                    .ToArray()
            });
        }

        public void When(MailoutQueued e)
        {
            var mailout = FindMailout(e.MailoutId);
            var recipient = mailout.GetRecipient(e.Recipient);

            if (recipient == null || recipient.CallbackStatus != MailoutCallbackStatus.Drafted)
                return;

            recipient.CallbackStatus = MailoutCallbackStatus.Queued;
            recipient.CallbackTimestamp = null;
            (mailout.Status, mailout.StatusDescription) = CalculateMailoutStatus(mailout.Recipients);
        }

        public void When(MailoutRejected e)
        {
            var mailout = FindMailout(e.MailoutId);

            if (e.Recipient.IsNotEmpty())
            {
                var recipient = mailout.GetRecipient(e.Recipient);
                if (recipient != null)
                    UpdateRecipient(recipient);
            }
            else
            {
                foreach (var recipient in mailout.Recipients)
                    UpdateRecipient(recipient);
            }

            (mailout.Status, mailout.StatusDescription) = CalculateMailoutStatus(mailout.Recipients);

            void UpdateRecipient(MailoutRecipientState r)
            {
                if (r.CallbackStatus != MailoutCallbackStatus.Drafted && r.CallbackStatus != MailoutCallbackStatus.Queued)
                    return;

                r.CallbackStatus = MailoutCallbackStatus.Rejected;
                r.CallbackTimestamp = null;
            }
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
            if (MailoutExists(mailout))
                return;

            var state = new MailoutState
            {
                Identifier = mailout,
                Status = status,
                Recipients = recipients
                    .Select(x => new MailoutRecipientState
                    {
                        Email = x.Clone(),
                        CallbackStatus = status,
                        CallbackTimestamp = null
                    })
                    .ToArray()
            };
            Mailouts.Add(mailout, state);
        }

        public void ChangeMailoutStatus(Guid mailout, string status, string reason = null)
        {
            if (!MailoutExists(mailout))
                return;

            var state = FindMailout(mailout);
            state.Status = status;
            state.StatusReason = reason;
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

        private (string status, string description) CalculateMailoutStatus(IEnumerable<MailoutRecipientState> recipients)
        {
            var statuses = recipients
                .GroupBy(x => x.CallbackStatus, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.Count(), StringComparer.OrdinalIgnoreCase);

            var queued = statuses.GetOrDefault(MailoutCallbackStatus.Queued);
            var accepted = statuses.GetOrDefault(MailoutCallbackStatus.Accepted);
            var rejected = statuses.GetOrDefault(MailoutCallbackStatus.Rejected);
            var delivered = statuses.GetOrDefault(MailoutCallbackStatus.Delivered);
            var failed = statuses.GetOrDefault(MailoutCallbackStatus.Failed);
            var total = recipients.Count();

            string status, description;

            if (delivered == total)
            {
                status = "Delivered";
                description = $"Delivered to all recipients";
            }
            else if (accepted == total)
            {
                status = "Accepted";
                description = $"Accepted for all recipients";
            }
            else if (failed == total)
            {
                status = "Failed";
                description = $"Failed for all recipients";
            }
            else if (rejected == total)
            {
                status = "Rejected";
                description = $"Rejected for all recipients";
            }
            else if (delivered > 0)
            {
                status = "Partially Delivered";
                description = $"Delivered to {delivered} of {total} recipients"
                    + GetDescriptionAddition(MailoutCallbackStatus.Delivered);
            }
            else if (accepted > 0)
            {
                status = "Partially Accepted";
                description = $"Accepted for {accepted} of {total} recipients"
                    + GetDescriptionAddition(MailoutCallbackStatus.Accepted);
            }
            else if (failed > 0)
            {
                status = "Partially Failed";
                description = $"Failed for {failed} of {total} recipients"
                    + GetDescriptionAddition(MailoutCallbackStatus.Failed);
            }
            else if (rejected > 0)
            {
                status = "Partially Rejected";
                description = $"Rejected for {rejected} of {total} recipients"
                    + GetDescriptionAddition(MailoutCallbackStatus.Rejected);
            }
            else
            {
                status = "Queued";
                description = $"Queued {queued} of {total} recipients"
                    + GetDescriptionAddition(MailoutCallbackStatus.Queued);
            }

            return (status, description);

            string GetDescriptionAddition(string excludeStatus)
            {
                var parts = new List<string>();

                if (queued > 0 && excludeStatus != MailoutCallbackStatus.Queued)
                    parts.Add($"{queued} queued");

                if (accepted > 0 && excludeStatus != MailoutCallbackStatus.Accepted)
                    parts.Add($"{accepted} accepted");

                if (delivered > 0 && excludeStatus != MailoutCallbackStatus.Delivered)
                    parts.Add($"{delivered} delivered");

                if (rejected > 0 && excludeStatus != MailoutCallbackStatus.Rejected)
                    parts.Add($"{rejected} rejected");

                if (failed > 0 && excludeStatus != MailoutCallbackStatus.Failed)
                    parts.Add($"{failed} failed");

                return parts.Count > 0 ? ", " + string.Join(", ", parts) : string.Empty;
            }
        }
    }
}