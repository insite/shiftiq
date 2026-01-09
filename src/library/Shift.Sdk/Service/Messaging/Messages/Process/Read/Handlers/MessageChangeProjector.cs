using System;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Messages.Read
{
    /// <summary>
    /// Implements the projector for Message events.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Events can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from changes to the projection tables). A processor,
    /// in contrast, should *never* replay past events.
    /// </remarks>
    public class MessageChangeProjector
    {
        private readonly IMessageStore _store;
        private readonly IContentStore _contentStore;

        public MessageChangeProjector(IChangeQueue publisher, IChangeStore changeStore, IMessageStore store, IContentStore contentStore)
        {
            publisher.Subscribe<CarbonCopyCompleted2>(Handle);
            publisher.Subscribe<CarbonCopyStarted2>(Handle);
            publisher.Subscribe<Classified>(Handle);
            publisher.Subscribe<ContentChanged>(Handle);
            publisher.Subscribe<DeliveryBounced>(Handle);
            publisher.Subscribe<DeliveryCompleted2>(Handle);
            publisher.Subscribe<DeliveryStarted2>(Handle);
            publisher.Subscribe<FollowerAdded>(Handle);
            publisher.Subscribe<FollowerRemoved>(Handle);
            publisher.Subscribe<LinkCounterReset>(Handle);
            publisher.Subscribe<MailoutAborted>(Handle);
            publisher.Subscribe<MailoutCancelled>(Handle);
            publisher.Subscribe<MailoutCompleted>(Handle);
            publisher.Subscribe<MailoutScheduled2>(Handle);
            publisher.Subscribe<MailoutStarted>(Handle);
            publisher.Subscribe<MessageArchived>(Handle);
            publisher.Subscribe<MessageCreated>(Handle);
            publisher.Subscribe<MessageDisabled>(Handle);
            publisher.Subscribe<MessageEnabled>(Handle);
            publisher.Subscribe<AutoBccSubscribersDisabled>(Handle);
            publisher.Subscribe<AutoBccSubscribersEnabled>(Handle);
            publisher.Subscribe<MessageRenamed>(Handle);
            publisher.Subscribe<MessageRetitled>(Handle);
            publisher.Subscribe<SenderChanged>(Handle);
            publisher.Subscribe<SubscriberAdded>(Handle);
            publisher.Subscribe<SubscriberRemoved>(Handle);
            publisher.Subscribe<SubscribersAdded>(Handle);
            publisher.Subscribe<SubscribersRemoved>(Handle);
            publisher.Subscribe<SurveyFormAssigned>(Handle);
            publisher.Subscribe<SurveyFormSubmissionStarted>(Handle);
            publisher.Subscribe<SurveyFormSubmissionCompleted>(Handle);

            changeStore.RegisterObsoleteChangeTypes(new[]
            {
                ObsoleteChangeType.MailoutScheduled
            });

            _store = store;
            _contentStore = contentStore;
        }

        public void Handle(ContentChanged e)
        {
            var content = new ContentContainer();
            content.Body.Text = e.ContentText;
            SaveMessageContent(content, e);

            _store.UpdateMessage(e);
        }

        public void Handle(Classified e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(DeliveryBounced e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(DeliveryCompleted2 e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(DeliveryStarted2 e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(CarbonCopyStarted2 e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(CarbonCopyCompleted2 e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(LinkCounterReset e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(MailoutAborted e)
            => _store.UpdateMessage(e);

        public void Handle(MailoutCancelled e)
            => _store.UpdateMessage(e);

        public void Handle(MailoutCompleted e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(MailoutScheduled2 e)
        {
            _store.UpdateMessage(e);

            var content = new ContentContainer();
            content.Title.Text = e.Subject;
            content.Body.Text = e.BodyText;

            SaveMailoutContent(content, e.OriginOrganization, e.MailoutIdentifier);
        }

        public void Handle(MailoutStarted e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(MessageArchived e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(MessageCreated e)
        {
            _store.InsertMessage(e);

            var content = new ContentContainer();
            content.Title.Text = e.Title;
            content.Body.Text = e.ContentText;

            SaveMessageContent(content, e);
        }

        public void Handle(MessageDisabled e)
            => _store.UpdateMessage(e);

        public void Handle(MessageEnabled e)
            => _store.UpdateMessage(e);

        public void Handle(AutoBccSubscribersDisabled e)
            => _store.UpdateMessage(e);

        public void Handle(AutoBccSubscribersEnabled e)
            => _store.UpdateMessage(e);

        public void Handle(MessageRenamed e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(MessageRetitled e)
        {
            var content = new ContentContainer();
            content.Title.Text = e.Title;
            SaveMessageContent(content, e);

            _store.UpdateMessage(e);
        }

        public void Handle(SubscriberAdded e)
        {
            if (e.IsGroup)
                _store.InsertSubscriberGroup(e.AggregateIdentifier, e.ContactIdentifier, e.ChangeTime);
            else
                _store.InsertSubscriberUser(e.AggregateIdentifier, e.ContactIdentifier, e.ChangeTime);
        }

        public void Handle(SubscriberRemoved e)
        {
            if (e.IsGroup)
                _store.DeleteSubscriberGroup(e.AggregateIdentifier, e.ContactIdentifier);
            else
                _store.DeleteSubscriberUser(e.AggregateIdentifier, e.ContactIdentifier);
        }

        public void Handle(SubscribersAdded e)
        {
            if (e.IsGroup)
            {
                foreach (var contact in e.ContactIdentifiers)
                    _store.InsertSubscriberGroup(e.AggregateIdentifier, contact, e.ChangeTime);
            }
            else
            {
                foreach (var contact in e.ContactIdentifiers)
                    _store.InsertSubscriberUser(e.AggregateIdentifier, contact, e.ChangeTime);
            }
        }

        public void Handle(SubscribersRemoved e)
        {
            if (e.IsGroup)
            {
                foreach (var contact in e.ContactIdentifiers)
                    _store.DeleteSubscriberGroup(e.AggregateIdentifier, contact);
            }
            else
            {
                foreach (var contact in e.ContactIdentifiers)
                    _store.DeleteSubscriberUser(e.AggregateIdentifier, contact);
            }
        }

        public void Handle(FollowerAdded e)
        {
            _store.InsertFollower(e.AggregateIdentifier, e.ContactIdentifier, e.FollowerIdentifier);
        }

        public void Handle(FollowerRemoved e)
        {
            _store.DeleteFollower(e.AggregateIdentifier, e.ContactIdentifier, e.FollowerIdentifier);
        }

        public void Handle(SenderChanged e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(SurveyFormAssigned e)
        {
            _store.UpdateMessage(e);
        }

        public void Handle(SurveyFormSubmissionCompleted e)
        {
        }

        public void Handle(SurveyFormSubmissionStarted e)
        {
        }

        public void Handle(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType != ObsoleteChangeType.MailoutScheduled)
                return;

            var v2 = MailoutScheduled2.Upgrade(e);

            Handle(v2);
        }

        public void Replay(IChangeStore store, Guid aggregateId, Action<string, int, int, Guid> progress)
        {
            _store.Delete(aggregateId);

            var changes = store.GetChanges("Message", aggregateId, null);

            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Message", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                handler.Invoke(this, new[] { e });
            }
        }

        private void SaveMessageContent(ContentContainer content, IChange change)
        {
            _contentStore.SaveContainer(change.OriginOrganization, ContentContainerType.Message, change.AggregateIdentifier, content);
        }

        private void SaveMailoutContent(ContentContainer content, Guid organizationId, Guid mailoutId)
        {
            _contentStore.SaveContainer(organizationId, ContentContainerType.Mailout, mailoutId, content);
        }
    }
}