using System;

using InSite.Domain.Messages;

namespace InSite.Application.Messages.Read
{
    public interface IMessageStore
    {
        void Delete(Guid aggregate);

        void InsertClickthrough(Guid id, Guid user, string ip, string browser);
        QLink InsertLink(Guid id);
        void InsertMessage(MessageCreated message);

        void UpdateMessage(CarbonCopyCompleted2 message);
        void UpdateMessage(CarbonCopyStarted2 message);
        void UpdateMessage(Classified message);
        void UpdateMessage(ContentChanged message);
        void UpdateMessage(DeliveryBounced message);
        void UpdateMessage(DeliveryCompleted2 message);
        void UpdateMessage(DeliveryStarted2 message);
        void UpdateMessage(LinkCounterReset message);
        void UpdateMessage(MailoutAborted message);
        void UpdateMessage(MailoutCancelled message);
        void UpdateMessage(MailoutCompleted message);
        void UpdateMessage(MailoutScheduled2 message);
        void UpdateMessage(MailoutStarted message);
        void UpdateMessage(MessageArchived message);
        void UpdateMessage(MessageDisabled message);
        void UpdateMessage(MessageEnabled message);
        void UpdateMessage(AutoBccSubscribersDisabled message);
        void UpdateMessage(AutoBccSubscribersEnabled message);
        void UpdateMessage(MessageRenamed message);
        void UpdateMessage(MessageRetitled message);
        void UpdateMessage(SenderChanged message);
        void UpdateMessage(SurveyFormAssigned message);

        void DeleteSubscriberGroup(Guid aggregate, Guid group);
        void InsertSubscriberGroup(Guid aggregate, Guid group, DateTimeOffset time);

        void DeleteSubscriberUser(Guid aggregate, Guid user);
        void InsertSubscriberUser(Guid aggregate, Guid user, DateTimeOffset time);

        void InsertFollower(Guid aggregate, Guid contact, Guid follower);
        void DeleteFollower(Guid aggregate, Guid contact, Guid follower);
    }
}