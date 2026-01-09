using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Domain.Messages;

namespace InSite.Application.Messages.Write
{
    public class MessageCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public MessageCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AbortMailout>(Handle);
            commander.Subscribe<AddSubscriber>(Handle);
            commander.Subscribe<AddSubscribers>(Handle);
            commander.Subscribe<ArchiveMessage>(Handle);
            commander.Subscribe<AssignSurveyForm>(Handle);
            commander.Subscribe<CancelMailout>(Handle);
            commander.Subscribe<ChangeContent>(Handle);
            commander.Subscribe<ChangeSender>(Handle);
            commander.Subscribe<CompleteCarbonCopy>(Handle);
            commander.Subscribe<CompleteDelivery>(Handle);
            commander.Subscribe<CompleteMailout>(Handle);
            commander.Subscribe<CreateMessage>(Handle);
            commander.Subscribe<DisableMessage>(Handle);
            commander.Subscribe<EnableMessage>(Handle);
            commander.Subscribe<DisableAutoBccSubscribers>(Handle);
            commander.Subscribe<EnableAutoBccSubscribers>(Handle);
            commander.Subscribe<FollowSubscriber>(Handle);
            commander.Subscribe<RemoveMessageSubscriber>(Handle);
            commander.Subscribe<RemoveMessageSubscribers>(Handle);
            commander.Subscribe<RenameMessage>(Handle);
            commander.Subscribe<ResetLinkCounter>(Handle);
            commander.Subscribe<RetitleMessage>(Handle);
            commander.Subscribe<ScheduleMailout>(Handle);
            commander.Subscribe<StartCarbonCopy>(Handle);
            commander.Subscribe<StartDeliveries>(Handle);
            commander.Subscribe<StartDelivery>(Handle);
            commander.Subscribe<StartMailout>(Handle);
            commander.Subscribe<UnfollowSubscriber>(Handle);
        }

        private void Commit(MessageAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(AbortMailout c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AbortMailout(c.Mailout, c.Reason);
            Commit(aggregate, c);
        }

        public void Handle(CreateMessage c)
        {
            var aggregate = new MessageAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.Create(c.Tenant, c.SenderIdentifier, c.Type, c.Name, c.Title, c.ContentText, c.SurveyFormIdentifier);
            Commit(aggregate, c);
        }

        public void Handle(ChangeContent c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeContent(c.ContentText);
            Commit(aggregate, c);
        }

        public void Handle(DisableMessage c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DisableMessage();
            Commit(aggregate, c);
        }

        public void Handle(EnableMessage c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.EnableMessage();
            Commit(aggregate, c);
        }

        public void Handle(DisableAutoBccSubscribers c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DisableAutoBccSubscribers();
            Commit(aggregate, c);
        }

        public void Handle(EnableAutoBccSubscribers c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.EnableAutoBccSubscribers();
            Commit(aggregate, c);
        }

        public void Handle(RenameMessage c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.Rename(c.Name);
            Commit(aggregate, c);
        }

        public void Handle(ResetLinkCounter c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ResetLinkCounter(c.LinkIdentifier);
            Commit(aggregate, c);
        }

        public void Handle(RetitleMessage c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.Retitle(c.Title);

            Commit(aggregate, c);
        }

        public void Handle(ChangeSender c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeSender(c.Sender);

            Commit(aggregate, c);
        }

        public void Handle(ScheduleMailout c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            if (aggregate.Data.IsDisabled)
                return;

            aggregate.ScheduleMailout(c.MailoutIdentifier, c.At, c.SenderIdentifier, c.Recipients, c.Subject, c.Body, c.Variables, c.EventIdentifier, c.Attachments);

            Commit(aggregate, c);
        }

        public void Handle(StartMailout c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.StartMailout(c.MailoutIdentifier);

            Commit(aggregate, c);
        }

        public void Handle(CancelMailout c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.CancelMailout(c.MailoutIdentifier);

            Commit(aggregate, c);
        }

        public void Handle(CompleteMailout c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.CompleteMailout(c.Mailout);
            Commit(aggregate, c);
        }

        public void Handle(StartCarbonCopy c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.StartCarbonCopy(c.MailoutIdentifier, c.RecipientIdentifier, c.CcType, c.CcIdentifier);

            Commit(aggregate, c);
        }

        public void Handle(StartDelivery c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.StartDelivery(c.MailoutIdentifier, c.RecipientIdentifier);

            Commit(aggregate, c);
        }

        public void Handle(StartDeliveries c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            foreach (var recipient in c.Recipients)
            {
                aggregate.StartDelivery(c.MailoutIdentifier, recipient.Identifier.Value);

                if (recipient.Cc != null)
                {
                    foreach (var copy in recipient.Cc)
                        aggregate.StartCarbonCopy(c.MailoutIdentifier, recipient.Identifier.Value, "Cc", copy);
                }

                if (recipient.Bcc != null)
                {
                    foreach (var copy in recipient.Bcc)
                        aggregate.StartCarbonCopy(c.MailoutIdentifier, recipient.Identifier.Value, "Bcc", copy);
                }
            }

            Commit(aggregate, c);
        }

        public void Handle(CompleteDelivery c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.CompleteDelivery(c.MailoutIdentifier, c.RecipientIdentifier, c.Error);

            Commit(aggregate, c);
        }

        public void Handle(CompleteCarbonCopy c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.CompleteCarbonCopy(c.MailoutIdentifier, c.RecipientIdentifier, c.CcType, c.CcIdentifier, c.Error);

            Commit(aggregate, c);
        }

        public void Handle(AssignSurveyForm c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.AssignSurveyForm(c.SurveyFormIdentifier);

            Commit(aggregate, c);
        }

        public void Handle(ArchiveMessage c)
        {
            if (!_repository.Exists<MessageAggregate>(c.AggregateIdentifier))
                return;

            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ArchiveMessage();
            Commit(aggregate, c);
        }

        #region Contacts and Followers

        public void Handle(AddSubscriber c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AttachContact(c.ContactIdentifier, c.ContactRole, c.IsGroup);

            Commit(aggregate, c);
        }

        public void Handle(RemoveMessageSubscriber c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DetachContact(c.ContactIdentifier, c.IsGroup);

            Commit(aggregate, c);
        }

        public void Handle(AddSubscribers c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AttachContacts(c.Contacts, c.ContactRole, c.IsGroup);

            Commit(aggregate, c);
        }

        public void Handle(RemoveMessageSubscribers c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DetachContacts(c.Contacts, c.IsGroup);

            Commit(aggregate, c);
        }

        public void Handle(FollowSubscriber c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.FollowContact(c.ContactIdentifier, c.FollowerIdentifier);

            Commit(aggregate, c);
        }

        public void Handle(UnfollowSubscriber c)
        {
            var aggregate = _repository.Get<MessageAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.UnfollowContact(c.ContactIdentifier, c.FollowerIdentifier);

            Commit(aggregate, c);
        }

        #endregion
    }
}