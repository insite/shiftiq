using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Domain.Registrations;

namespace InSite.Application.Registrations.Read
{
    public class RegistrationChangeProjector
    {
        private readonly IRegistrationStore _store;
        private readonly IRegistrationSearch _search;

        public RegistrationChangeProjector(IChangeQueue publisher, IRegistrationStore store, IRegistrationSearch search)
        {
            _store = store;
            _search = search;

            publisher.Subscribe<AccommodationGranted>(Handle);
            publisher.Subscribe<AccommodationRevoked>(Handle);
            publisher.Subscribe<ApprovalChanged>(Handle);
            publisher.Subscribe<AttemptAssigned>(Handle);
            publisher.Subscribe<AttendanceTaken>(Handle);
            publisher.Subscribe<CandidateChanged>(Handle);
            publisher.Subscribe<CandidateTypeChanged>(Handle);
            publisher.Subscribe<CustomerAssigned>(Handle);
            publisher.Subscribe<EligibilityChanged>(Handle);
            publisher.Subscribe<EmployerAssigned>(Handle);
            publisher.Subscribe<ExamFormAssigned>(Handle);
            publisher.Subscribe<ExamFormUnassigned>(Handle);
            publisher.Subscribe<ExamTimeLimited>(Handle);
            publisher.Subscribe<EventChanged>(Handle);
            publisher.Subscribe<GradeChanged>(Handle);
            publisher.Subscribe<GradingChanged>(Handle);
            publisher.Subscribe<InstructorAdded>(Handle);
            publisher.Subscribe<InstructorRemoved>(Handle);
            publisher.Subscribe<NotificationTriggered>(Handle);
            publisher.Subscribe<RegistrationCancelled>(Handle);
            publisher.Subscribe<RegistrationCommented>(Handle);
            publisher.Subscribe<RegistrationFeeAssigned>(Handle);
            publisher.Subscribe<RegistrationHoursWorkedAssigned>(Handle);
            publisher.Subscribe<RegistrationPaymentAssigned>(Handle);
            publisher.Subscribe<RegistrationPasswordChanged>(Handle);
            publisher.Subscribe<RegistrationRequested>(Handle);
            publisher.Subscribe<RegistrationDeleted>(Handle);
            publisher.Subscribe<SchoolAssigned>(Handle);
            publisher.Subscribe<SchoolUnassigned>(Handle);
            publisher.Subscribe<SeatAssigned>(Handle);
            publisher.Subscribe<SynchronizationChanged>(Handle);
            publisher.Subscribe<TimerCancelled>(Handle);
            publisher.Subscribe<TimerElapsed>(Handle);
            publisher.Subscribe<TimerStarted>(Handle);
            publisher.Subscribe<RegistrationIncludedToT2202>(Handle);
            publisher.Subscribe<RegistrationExcludedFromT2202>(Handle);
            publisher.Subscribe<RegistrantContactInformationChanged>(Handle);
            publisher.Subscribe<RegistrationBillingCodeModified>(Handle);
            publisher.Subscribe<RegistrationRequestedByModified>(Handle);
        }

        public void Handle(AccommodationGranted e)
            => _store.UpdateRegistration(e);

        public void Handle(AccommodationRevoked e)
            => _store.UpdateRegistration(e);

        public void Handle(ApprovalChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(EligibilityChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(AttemptAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(AttendanceTaken e)
            => _store.UpdateRegistration(e);

        public void Handle(CandidateChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(CandidateTypeChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(CustomerAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(EmployerAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(ExamFormAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(ExamFormUnassigned e)
            => _store.UpdateRegistration(e);

        public void Handle(ExamTimeLimited e)
            => _store.UpdateRegistration(e);

        public void Handle(EventChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(SeatAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(NotificationTriggered e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationCancelled e)
        {
            var registration = _search.GetRegistration(e.AggregateIdentifier);
            if (registration != null)
                _store.UpdateRegistration(e);
        }

        public void Handle(RegistrationDeleted e)
        {
            var registration = _search.GetRegistration(e.AggregateIdentifier);
            if (registration != null)
                _store.DeleteRegistration(e);
        }

        public void Handle(RegistrationRequested e)
            => _store.InsertRegistration(e);

        public void Handle(RegistrationCommented e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationFeeAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationHoursWorkedAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationPaymentAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationPasswordChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(SchoolAssigned e)
            => _store.UpdateRegistration(e);

        public void Handle(SchoolUnassigned e)
            => _store.UpdateRegistration(e);

        public void Handle(SynchronizationChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(InstructorAdded e)
            => _store.UpdateRegistration(e);

        public void Handle(InstructorRemoved e)
            => _store.UpdateRegistration(e);

        public void Handle(TimerCancelled e)
            => _store.DeleteTimer(e.Timer);

        public void Handle(TimerElapsed e)
            => _store.UpdateTimer(e.Timer, "Elapsed");

        public void Handle(GradeChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(GradingChanged e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationIncludedToT2202 e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationExcludedFromT2202 e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrantContactInformationChanged e) { }

        public void Handle(RegistrationBillingCodeModified e)
            => _store.UpdateRegistration(e);

        public void Handle(RegistrationRequestedByModified e)
            => _store.UpdateRegistration(e);

        public void Handle(TimerStarted e)
        {
            var timer = new QRegistrationTimer
            {
                RegistrationIdentifier = e.AggregateIdentifier,
                TimerDescription = e.Description,
                TimerStatus = "Started",
                TriggerCommand = e.Timer,
                TriggerTime = e.At
            };
            _store.InsertTimer(timer);
        }

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid id)
        {
            // Clear existing data in the query store for this projection.
            _store.Delete(id);

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Registration", id, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Registration", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });

                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}