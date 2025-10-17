using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Application.Messages.Read
{
    public interface IMessageSearch
    {
        List<ArchivedFollower> GetArchivedFollowers();
        List<ArchivedSubscriber> GetArchivedSubscribers();

        bool MessageExists(Guid id);

        int CountVMessages(MessageFilter filter);
        int CountMessages(MessageFilter filter);
        List<Counter> CountMessagesByType(MessageFilter filter);

        List<Counter> CountMessageReferences(Guid message);

        VMessage GetMessage(Guid message);
        VMessage GetMessage(MessageFilter filter);
        List<VMessage> GetMessages(MessageFilter filter);
        List<VMessage> GetVMessages(MessageFilter filter, params Expression<Func<VMessage, object>>[] includes);
        QMessage GetQMessage(Guid id);
        List<SearchVMessage> GetMessagesWithCount(MessageFilter filter);
        int CountMailouts(MailoutFilter filter);
        VMailout FindMailout(Guid mailout);
        List<VMailout> GetMailouts(MailoutFilter filter);

        int CountDeliveries(DeliveryFilter filter);
        QRecipient GetDelivery(Guid mailout, Guid recipientId);
        QRecipient GetDelivery(Guid mailout, string recipientAddress);
        List<QRecipient> GetDeliveries(Guid mailout);
        List<QRecipient> GetDeliveries(DeliveryFilter filter);
        DateTimeOffset? GetLastDeliveryDate(Guid message, Guid user);
        int CountLinks(Guid message);
        QLink FindLink(Guid id);
        List<QLink> FindLinks(Guid message);

        int CountSubscriberGroups(QSubscriberGroupFilter filter);
        VSubscriberGroup GetSubscriberGroup(Guid message, Guid contact);
        List<VSubscriberGroup> GetSubscriberGroups(Guid message);
        List<VSubscriberGroup> GetSubscriberGroups(QSubscriberGroupFilter filter);
        
        int CountSubscriberUsers(Guid message);
        int CountSubscriberUsers(QSubscriberUserFilter filter);
        ISubscriberPerson GetSubscriberUser(Guid message, Guid contact);
        List<ISubscriberPerson> GetSubscriberUsers(Guid message);
        List<ISubscriberPerson> GetSubscriberUsers(QSubscriberUserFilter filter);

        List<ISubscriberPerson> GetSubscribers(Guid organization, Guid messageIdentifier, Guid? recipientFilter);
        List<EmailAddress> GetSubscribersEmailAddresses(Guid organization, Guid messageIdentifier, Guid? recipientFilter);

        VFollower GetFollower(Guid message, Guid subscriber, Guid follower);
        VFollower GetFollower(Guid message, Guid subscriber, string follower);
        List<VFollower> GetFollowers(Guid message);
        List<VFollower> GetFollowers(QFollowerFilter filter);

        int CountClicks(VClickFilter filter);
        List<VClick> GetClicks(VClickFilter filter);
        
        Guid[] GetOrphanMessages();
        Dictionary<Guid, string> GetOneRecipientForEachMailout(Guid[] mailouts);
        
        string GetCarbonCopyEmails(ICollection<QCarbonCopy> carbonCopies);
    }
}