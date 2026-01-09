using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Messages.Read;
using InSite.Application.Users.Write;
using InSite.Domain.Messages;

namespace InSite.Application.Contacts.Write
{
    public class ContactCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        private readonly IMessageSearch _messageSearch;

        public ContactCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository, IMessageSearch messageSearch)
        {
            _publisher = publisher;
            _repository = repository;
            _messageSearch = messageSearch;

            commander.Subscribe<ArchiveCmdsUser>(Handle);
        }

        private void Commit(MessageAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(ArchiveCmdsUser c)
        {
            var followers = _messageSearch.GetFollowers(new QFollowerFilter() { SubscriberIdentifier = c.User });
            var followings = _messageSearch.GetFollowers(new QFollowerFilter() { FollowerIdentifier = c.User });
            var subscribers = _messageSearch.GetSubscriberUsers(new QSubscriberUserFilter() { SubscriberIdentifier = c.User });

            foreach (var following in followings)
            {
                var message = _repository.Get<MessageAggregate>(following.MessageIdentifier, c.ExpectedVersion);
                message.UnfollowContact(following.SubscriberIdentifier, following.FollowerIdentifier);
                Commit(message, c);
            }

            foreach (var follower in followers)
            {
                var message = _repository.Get<MessageAggregate>(follower.MessageIdentifier, c.ExpectedVersion);
                message.UnfollowContact(follower.SubscriberIdentifier, follower.FollowerIdentifier);
                Commit(message, c);
            }

            foreach (var subscriber in subscribers)
            {
                var message = _repository.Get<MessageAggregate>(subscriber.MessageIdentifier, c.ExpectedVersion);
                message.DetachContact(subscriber.UserIdentifier, false);
                Commit(message, c);
            }
        }
    }
}
