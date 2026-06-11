using System;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Messages.Write;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Events;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Application.Events.Read
{
    /// <summary>
    /// Implements the process manager for Event changes. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class EventChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IAlertMailer _mailer;

        private readonly IContactSearch _contacts;
        private readonly IEventSearch _events;
        private readonly IRegistrationSearch _registrations;
        private readonly IRecordSearch _records;

        public EventChangeProcessor(ICommander commander, IChangeQueue publisher, 
            IAlertMailer mailer,
            IContactSearch contacts, IEventSearch events, IRegistrationSearch registrations, IRecordSearch recordSearch)
        {
            _commander = commander;
            _mailer = mailer;

            _contacts = contacts;
            _events = events;
            _registrations = registrations;
            _records = recordSearch;

            publisher.Subscribe<EventCancelled>(Handle);
            publisher.Subscribe<EventDeleted>(Handle);
            publisher.Subscribe<EventTimerElapsed>(Handle);
            publisher.Subscribe<EventVenueChanged2>(Handle);
        }

        /// <summary>
        /// When an event is cancelled, related registrations may be cancelled also. Automatic cancellation of related
        /// registrations is optional; the EventCancelled change has a property to indicate whether or not this is
        /// expected by the invoker.
        /// </summary>
        public void Handle(EventCancelled c)
        {
            CancelPendingTimers(c.AggregateIdentifier);

            if (c.CancelRegistrations)
            {
                var registrations = _registrations.GetRegistrationsByEvent(c.AggregateIdentifier);
                foreach (var registration in registrations)
                    _commander.Send(new CancelRegistration(registration.RegistrationIdentifier, "Event cancelled. " + c.Reason, false));
            }
        }

        /// <summary>
        /// When an activity is voided, all related registrations must be voided and all references from Gradebooks too.
        /// </summary>
        public void Handle(EventDeleted c)
        {
            CancelPendingTimers(c.AggregateIdentifier);

            var registrations = _registrations.GetRegistrationsByEvent(c.AggregateIdentifier);
            foreach (var registration in registrations)
                _commander.Send(new DeleteRegistration(registration.RegistrationIdentifier, false));

            var records = _records.GetEventGradebooks(c.AggregateIdentifier);
            foreach (var record in records)
            {
                var isLocked = record.IsLocked;
                var id = record.GradebookIdentifier;

                if (isLocked)
                    _commander.Send(new UnlockGradebook(id));

                _commander.Send(new RemoveGradebookEvent(id, c.AggregateIdentifier));

                if (isLocked)
                    _commander.Send(new LockGradebook(id));
            }
        }

        public void Handle(EventTimerElapsed c)
        {
            _commander.Start(c.Timer);
        }

        public void Handle(EventVenueChanged2 c)
        {
            ProcessVenueChange(c, c.Office);
            ProcessVenueChange(c, c.Location);
        }

        private void ProcessVenueChange(IChange change, Guid? venue)
        {
            if (venue == null)
                return;

            VGroup group = _contacts.GetGroup(venue.Value);
            QEvent @event = _events.GetEvent(change.AggregateIdentifier);

            if (group == null || group.MessageToAdminWhenEventVenueChanged == null || @event == null)
                return;

            _mailer.Send(change.OriginOrganization, change.OriginUser,
                new AlertEventVenueChanged
                {
                    MessageIdentifier = group.MessageToAdminWhenEventVenueChanged,
                    GroupIdentifier = venue.Value,
                    GroupName = group.GroupName,
                    EventIdentifier = change.AggregateIdentifier,
                    EventDate = TimeZones.Format(@event.EventScheduledStart, @event.EventScheduledStart.GetTimeZone()),
                    EventName = @event.EventTitle,
                    EventNumber = @event.EventNumber.ToString()
                });
        }

        private void CancelPendingTimers(Guid @event)
        {
            var filter = new QEventTimerFilter { EventIdentifier = @event, TimerStatus = "Started" };
            var timers = _events.GetTimers(filter);
            foreach (var timer in timers)
                _commander.Send(new CancelEventTimer(@event, timer.TriggerCommand));
        }
    }
}