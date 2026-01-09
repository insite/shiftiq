using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class MessageRepository
    {
        private class SubscriberModel
        {
            public Guid UserIdentifier { get; set; }
            public ISubscriberPerson Person { get; set; }
            public VFollower[] Followers { get; set; }

            public static SubscriberModel[] Select(Guid organization, Guid message)
            {
                var search = MessageSearch.Instance;

                var subscribers = search.GetSubscribers(organization, message, null);

                var followers = search.GetFollowers(message)
                    .GroupBy(x => x.SubscriberIdentifier)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var subscriberIdentifiers = subscribers
                    .Select(x => x.UserIdentifier)
                    .Distinct()
                    .ToArray();

                var subscriberUsers = GetUsers(subscriberIdentifiers);

                return subscribers
                    .Where(x => subscriberUsers.Any(y => y == x.UserIdentifier))
                    .Select(x => new SubscriberModel
                    {
                        UserIdentifier = x.UserIdentifier,
                        Person = x,
                        Followers = followers.ContainsKey(x.UserIdentifier) ? followers[x.UserIdentifier] : new VFollower[0]
                    })
                    .ToArray();
            }
        }

        /// <remarks>
        /// Retrieve data from the database in batches of 500. Otherwise, Entity Framework fails with a stack overflow 
        /// exception when the list of subscribers is large. For example, when we have over 2500 subscribers, a call to
        /// UserSearch.Bind(x => x.UserIdentifier, x => identifiers.Any(y => y == x.UserIdentifier)) fails
        /// with a run-time exception.
        /// </remarks>
        public static List<Guid> GetUsers(Guid[] identifiers)
        {
            const int pageSize = 500;
            var subscriberUsers = new List<Guid>();

            // Calculate the number of pages required to process in chunks of 500.
            int pages = (int)Math.Ceiling((double)identifiers.Length / pageSize);

            for (int page = 0; page < pages; page++)
            {
                // Determine the subset of identifiers to process in the current chunk.
                var chunk = identifiers.Skip(page * pageSize).Take(pageSize);

                // Execute the query for the current chunk and append the results.
                var chunkResults = UserSearch.Bind(x => x.UserIdentifier, new UserFilter
                {
                    IncludeUserIdentifiers = chunk.ToArray()
                });

                subscriberUsers.AddRange(chunkResults);
            }

            return subscriberUsers;
        }

        public static EmailDraft GetEmail(Guid messageIdentifier)
        {
            var message = MessageSearch.Instance.GetMessage(messageIdentifier)
                ?? throw new MessageNotFoundException($"Message Not Found: Message {messageIdentifier}");

            return GetEmail(OrganizationSearch.Select(message.OrganizationIdentifier), message);
        }

        public static EmailDraft GetEmail(Guid organizationIdentifier, NotificationType alertType)
        {
            var messageName = alertType.ToString();

            var organization = OrganizationSearch.Select(organizationIdentifier)
                ?? throw new MessageNotFoundException(messageName);

            VMessage message = null;

            if (MessageSearch.Instance.MessageExists(organization.OrganizationIdentifier, messageName))
                message = GetMessage(organization.OrganizationIdentifier, messageName);

            else if (MessageSearch.Instance.MessageExists(OrganizationIdentifiers.CMDS, messageName))
                message = GetMessage(OrganizationIdentifiers.CMDS, messageName);

            else if (MessageSearch.Instance.MessageExists(OrganizationIdentifiers.Global, messageName))
                message = GetMessage(OrganizationIdentifiers.Global, messageName);

            else
            {
                var notification = Notifications.Select(alertType);
                if (notification.Body != null)
                    return GetEmail(organization, notification.Subject, notification.Body);
            }

            if (message == null)
                throw new MessageNotFoundException(messageName);

            return GetEmail(organization, message);
        }

        private static VMessage GetMessage(Guid organization, string messageName)
        {
            var filter = new MessageFilter
            {
                OrganizationIdentifier = organization,
                Name = messageName
            };

            var message = MessageSearch.Instance.GetMessage(filter);
            if (message == null)
                throw new MessageNotFoundException($"Message Not Found: Organization {organization} Message Name {messageName}");

            return message;
        }

        public static VMessage GetMessageWithoutException(Guid organization, string messageName)
        {
            var filter = new MessageFilter
            {
                OrganizationIdentifier = organization,
                Name = messageName
            };

            return MessageSearch.Instance.GetMessage(filter);
        }

        public static EmailDraft GetEmail(OrganizationState organization, VMessage message)
        {
            var content = TContentSearch.Instance.GetBlock(message.MessageIdentifier);

            var draft = EmailDraft.Create(
                organization.OrganizationIdentifier,
                message.MessageIdentifier,
                message.SenderIdentifier,
                message.AutoBccSubscribers
                );

            draft.ContentBody = content.Body.Text.Clone();
            draft.ContentPriority = GetEmailPriority(message.MessageName);
            draft.ContentSubject = content.Title.Text.Clone();
            draft.IsDisabled = message.IsDisabled;

            return draft;
        }

        public static List<EmailSubscriber> GetSubscribers(Guid organization, Guid message)
        {
            var subscribers = new List<EmailSubscriber>();

            var contacts = SubscriberModel.Select(organization, message);

            foreach (var contact in contacts)
                AddSubscriber(contact, subscribers);

            return subscribers;
        }

        public static EmailDraft GetEmail(OrganizationState organization, string subject, string body)
        {
            var draft = EmailDraft.Create(organization.OrganizationIdentifier, null, Guid.Empty, false);

            draft.OrganizationIdentifier = organization.OrganizationIdentifier;
            draft.ContentSubject = new MultilingualString(subject);
            draft.ContentBody = new MultilingualString(body);

            return draft;
        }

        public static string GetEmailPriority(NotificationType notification)
        {
            if (notification == NotificationType.AchievementCredentialsExpiredToday
                || notification == NotificationType.AchievementCredentialsExpiringInOneMonth
                || notification == NotificationType.AchievementCredentialsExpiringInTwoMonths
                || notification == NotificationType.AchievementCredentialsExpiringInThreeMonths)
                return "High";

            return null;
        }

        public static string GetEmailPriority(string notification)
        {
            if (Enum.TryParse(notification, out NotificationType notificationEnum))
                return GetEmailPriority(notificationEnum);

            return null;
        }

        private static void AddSubscriber(SubscriberModel contact, List<EmailSubscriber> subscribers)
        {
            if (contact.Person.UserEmail.IsEmpty())
                return;

            var subscriber = new EmailSubscriber
            {
                UserIdentifier = contact.UserIdentifier,
                UserFirstName = contact.Person.UserFirstName,
                UserLastName = contact.Person.UserLastName,
                UserFullName = contact.Person.UserFullName,
                UserEmail = contact.Person.UserEmail,
                UserEmailEnabled = contact.Person.UserEmailEnabled,
                UserEmailAlternate = contact.Person.UserEmailAlternate,
                UserEmailAlternateEnabled = contact.Person.UserEmailAlternateEnabled,
                Subscribed = contact.Person.Subscribed,
                PersonCode = contact.Person.PersonCode,
                Language = contact.Person.PersonLanguage,
            };

            foreach (var follower in contact.Followers)
                subscriber.Followers.Add(follower.FollowerIdentifier);

            subscribers.Add(subscriber);
        }
    }
}