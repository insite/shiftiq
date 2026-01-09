using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Events;

namespace InSite.Application.Events.Read
{
    /// <summary>
    /// Implements the projector for Event changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Events can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from changes to the projection tables). A processor,
    /// in contrast, should *never* replay past events.
    /// </remarks>
    public class EventChangeProjector
    {
        private static readonly HashSet<string> _obsoleteChanges = new HashSet<string>
        {
            "SeatRegistrantTypeChanged"
        };

        private readonly IEventStore _eventStore;

        public EventChangeProjector(IChangeQueue publisher, IChangeStore changeStore, IEventStore eventStore)
        {
            publisher.Subscribe<AppointmentScheduled>(Handle);
            publisher.Subscribe<AppointmentDescribed>(Handle);
            publisher.Subscribe<EventAttendeeAdded>(Handle);
            publisher.Subscribe<EventAttendeeRemoved>(Handle);
            publisher.Subscribe<EventBillingCodeEnabled>(Handle);
            publisher.Subscribe<EventCalendarColorModified>(Handle);
            publisher.Subscribe<EventCancelled>(Handle);
            publisher.Subscribe<EventCompleted>(Handle);
            publisher.Subscribe<EventCreditAssigned>(Handle);
            publisher.Subscribe<EventDescribed>(Handle);
            publisher.Subscribe<EventFormatChanged>(Handle);
            publisher.Subscribe<EventNotificationTriggered>(Handle);
            publisher.Subscribe<EventPublicationCompleted>(Handle);
            publisher.Subscribe<EventPublicationStarted>(Handle);
            publisher.Subscribe<EventRecoded>(Handle);
            publisher.Subscribe<EventRescheduled>(Handle);
            publisher.Subscribe<EventDurationChanged>(Handle);
            publisher.Subscribe<EventCreditHoursChanged>(Handle);
            publisher.Subscribe<EventAchievementAdded>(Handle);
            publisher.Subscribe<EventAchievementChanged>(Handle);
            publisher.Subscribe<EventRenumbered>(Handle);
            publisher.Subscribe<EventRetitled>(Handle);
            publisher.Subscribe<EventScoresPublished>(Handle);
            publisher.Subscribe<EventScoresValidated>(Handle);
            publisher.Subscribe<EventTimerCancelled>(Handle);
            publisher.Subscribe<EventTimerElapsed>(Handle);
            publisher.Subscribe<EventTimerStarted>(Handle);
            publisher.Subscribe<EventDeleted>(Handle);
            publisher.Subscribe<EventRegistrationWithLinkAllowed>(Handle);
            publisher.Subscribe<EventRequestStatusChanged>(Handle);
            publisher.Subscribe<EventScheduleStatusChanged>(Handle);
            publisher.Subscribe<EventVenueChanged2>(Handle);
            publisher.Subscribe<CapacityAdjusted>(Handle);
            publisher.Subscribe<CapacityDecreased>(Handle);
            publisher.Subscribe<CapacityIncreased>(Handle);
            publisher.Subscribe<ClassImported>(Handle);
            publisher.Subscribe<ClassScheduled2>(Handle);
            publisher.Subscribe<EventCommentPosted>(Handle);
            publisher.Subscribe<EventCommentDeleted>(Handle);
            publisher.Subscribe<EventCommentModified>(Handle);
            publisher.Subscribe<DistributionChanged>(Handle);
            publisher.Subscribe<DistributionOrdered>(Handle);
            publisher.Subscribe<DistributionTracked>(Handle);
            publisher.Subscribe<ExamAttemptsImported>(Handle);
            publisher.Subscribe<ExamFormAttached>(Handle);
            publisher.Subscribe<ExamFormDetached>(Handle);
            publisher.Subscribe<ExamScheduled2>(Handle);
            publisher.Subscribe<ExamTypeChanged>(Handle);
            publisher.Subscribe<IntegrationConfigured>(Handle);
            publisher.Subscribe<InvigilatorCapacityAdjusted>(Handle);
            publisher.Subscribe<MeetingScheduled2>(Handle);
            publisher.Subscribe<RegistrationEnabled>(Handle);
            publisher.Subscribe<RegistrationFieldModified>(Handle);
            publisher.Subscribe<EventPublished>(Handle);
            publisher.Subscribe<EventUnpublished>(Handle);
            publisher.Subscribe<SeatAdded>(Handle);
            publisher.Subscribe<SeatRevised>(Handle);
            publisher.Subscribe<SeatDeleted>(Handle);
            publisher.Subscribe<LearnerRegistrationGroupModified>(Handle);
            publisher.Subscribe<MandatorySurveyModified>(Handle);
            publisher.Subscribe<ExamMaterialReturned>(Handle);
            publisher.Subscribe<AppointmentTypeChanged>(Handle);
            publisher.Subscribe<EventRegistrationLocked>(Handle);
            publisher.Subscribe<EventRegistrationUnlocked>(Handle);
            publisher.Subscribe<EventAllowMultipleRegistrationsModified>(Handle);
            publisher.Subscribe<EventPersonCodeIsRequiredModified>(Handle);
            publisher.Subscribe<EventMessageConnected>(Handle);
            publisher.Subscribe<EventMessagePeriodModified>(Handle);
            publisher.Subscribe<EventMessageSent>(Handle);

            changeStore.RegisterObsoleteChangeTypes(_obsoleteChanges);

            _eventStore = eventStore;
        }

        public void Handle(EventTimerStarted e)
        {
            var timer = new QEventTimer
            {
                EventIdentifier = e.AggregateIdentifier,
                TimerDescription = e.Description,
                TimerStatus = "Started",
                TriggerCommand = e.Timer,
                TriggerTime = e.At
            };
            _eventStore.InsertTimer(timer);
        }

        public void Handle(EventTimerElapsed e)
        {
            _eventStore.UpdateTimer(e.Timer, "Elapsed");
        }

        public void Handle(EventTimerCancelled e)
        {
            _eventStore.DeleteTimer(e.Timer);
        }

        public void Handle(ExamAttemptsImported e)
        {

        }

        public void Handle(ExamFormAttached e)
        {
            _eventStore.InsertExamForm(e.AggregateIdentifier, e.Form);
        }

        public void Handle(ExamFormDetached e)
        {
            _eventStore.DeleteExamForm(e.AggregateIdentifier, e.Form);
        }

        public void Handle(EventCompleted e) { _eventStore.UpdateEvent(e); }

        public void Handle(EventDescribed e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(AppointmentDescribed e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventFormatChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventPublicationCompleted e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventPublicationStarted e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventRecoded e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventDeleted e)
        {
            _eventStore.DeleteEvent(e.AggregateIdentifier);
        }

        public void Handle(EventRegistrationWithLinkAllowed e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventRequestStatusChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventRescheduled e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventDurationChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventCreditHoursChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventRenumbered e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventRetitled e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventScoresPublished e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventScoresValidated e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventScheduleStatusChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventVenueChanged2 e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(CapacityAdjusted e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(CapacityIncreased e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(CapacityDecreased e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(DistributionChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventAttendeeAdded e)
        {
            _eventStore.InsertContact(e.AggregateIdentifier, e.Contact, e.Role, e.ChangeTime);
        }

        public void Handle(EventAttendeeRemoved e)
        {
            _eventStore.DeleteContact(e.AggregateIdentifier, e.Contact);
        }

        public void Handle(EventBillingCodeEnabled e)
        {
            _eventStore.UpdateEvent(e, x => x.BillingCodeEnabled = e.Enabled);
        }

        public void Handle(EventCancelled e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventCalendarColorModified e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventCreditAssigned e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventNotificationTriggered e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventAchievementAdded e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventAchievementChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(ClassImported e)
        {
            _eventStore.InsertEvent(e);
        }

        public void Handle(AppointmentScheduled e)
        {
            _eventStore.InsertEvent(e);
        }

        public void Handle(ClassScheduled2 e)
        {
            _eventStore.InsertEvent(e);
        }

        public void Handle(EventCommentPosted e)
        {
            _eventStore.InsertComment(e.AggregateIdentifier, e.CommentIdentifier, e.OriginOrganization, e.AuthorIdentifier, e.ChangeTime, e.CommentText);
        }

        public void Handle(EventCommentModified e)
        {
            _eventStore.UpdateComment(e.CommentIdentifier, e.AuthorIdentifier, e.ChangeTime, e.CommentText);
        }

        public void Handle(EventCommentDeleted e)
        {
            _eventStore.DeleteComment(e.CommentIdentifier);
        }

        public void Handle(DistributionOrdered e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(DistributionTracked e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(ExamScheduled2 e)
        {
            _eventStore.InsertEvent(e);
        }

        public void Handle(ExamTypeChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(IntegrationConfigured e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(InvigilatorCapacityAdjusted e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(MeetingScheduled2 e)
        {
            _eventStore.InsertEvent(e);
        }

        public void Handle(RegistrationEnabled e)
        {

        }

        public void Handle(RegistrationFieldModified e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventPublished e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventUnpublished e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(SeatAdded e)
        {
            _eventStore.InsertSeat(e);
        }

        public void Handle(SeatRevised e)
        {
            _eventStore.Update(e);
        }

        public void Handle(LearnerRegistrationGroupModified e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(MandatorySurveyModified e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(SeatDeleted e)
        {
            _eventStore.RemoveSeat(e);
        }

        public void Handle(ExamMaterialReturned e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(AppointmentTypeChanged e)
        {
            _eventStore.UpdateEvent(e);
        }

        public void Handle(EventRegistrationLocked e)
        {
            _eventStore.UpdateEvent(e, x => x.RegistrationLocked = DateTimeOffset.UtcNow);
        }

        public void Handle(EventRegistrationUnlocked e)
        {
            _eventStore.UpdateEvent(e, x => x.RegistrationLocked = null);
        }

        public void Handle(EventAllowMultipleRegistrationsModified e)
        {
            _eventStore.UpdateEvent(e, x => x.AllowMultipleRegistrations = e.Value);
        }

        public void Handle(EventPersonCodeIsRequiredModified e)
        {
            _eventStore.UpdateEvent(e, x => x.PersonCodeIsRequired = e.Value);
        }

        public void Handle(EventMessageConnected e)
        {
            _eventStore.UpdateEvent(e, x =>
            {
                switch (e.MessageType)
                {
                    case EventMessageType.ReminderLearner:
                        x.WhenEventReminderRequestedNotifyLearnerMessageIdentifier = e.MessageId;
                        break;
                    case EventMessageType.ReminderInstructor:
                        x.WhenEventReminderRequestedNotifyInstructorMessageIdentifier = e.MessageId;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported message: {e.MessageType}");
                }
            });
        }

        public void Handle(EventMessagePeriodModified e)
        {
            _eventStore.UpdateEvent(e, x =>
            {
                x.SendReminderBeforeDays = e.SendReminderBeforeDays;
            });
        }

        public void Handle(EventMessageSent e)
        {
            _eventStore.UpdateEvent(e, x =>
            {
                x.ReminderMessageSent = e.ChangeTime;
            });
        }

        public void Handle(SerializedChange e)
        {
            // Obsolete changes go here
        }

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress)
        {
            // Clear all the existing data in the query store for this projection.
            _eventStore.DeleteAll();

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Event", null, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Event", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                handler.Invoke(this, new[] { e });
            }
        }
    }
}