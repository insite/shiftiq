using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Events.Write;
using InSite.Application.Integrations.Prometric;
using InSite.Application.Organizations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Registrations;

using Shift.Common;

namespace InSite.Application.Registrations.Read
{
    public class RegistrationChangeProcessor
    {
        private const string Eligible = "Eligible";
        private const string NotEligible = "Not Eligible";
        private const string Registered = "Registered";

        private readonly ICommander _commander;
        private readonly IOrganizationSearch _organizations;
        private readonly IRegistrationSearch _registrations;

        private readonly IPrometricApi _prometric;

        public RegistrationChangeProcessor(ICommander commander, IChangeQueue publisher,
            IOrganizationSearch organizations, IRegistrationSearch registrations,
            IPrometricApi prometric)
        {
            _commander = commander;
            _organizations = organizations;
            _registrations = registrations;

            _prometric = prometric;

            publisher.Subscribe<ApprovalChanged>(Handle);
            publisher.Subscribe<EligibilityChanged>(Handle);
            publisher.Subscribe<ExamFormAssigned>(Handle);
            publisher.Subscribe<ExamFormUnassigned>(Handle);

            publisher.Subscribe<RegistrationCancelled>(Handle);
            publisher.Subscribe<RegistrationDeleted>(Handle);

            publisher.Subscribe<TimerElapsed>(Handle);

            publisher.Subscribe<EventChanged>(Handle);
        }

        public void Handle(ApprovalChanged e)
        {
            CheckForPrometricIntegration(e.AggregateIdentifier);
        }

        public void Handle(EligibilityChanged e)
        {
            if (!IsPrometricIntegrationEnabled(e.OriginOrganization))
                return;

            var registration = _registrations.GetRegistration(
                e.AggregateIdentifier, 
                x => x.Candidate.User, x => x.Candidate.HomeAddress, x => x.Form, x => x.Accommodations);

            // Prometric requires an assessment form for every exam event registration. If the registration does not
            // have an assessment form, then we can't send any change request(s) to Prometric.

            if (registration.ExamFormIdentifier == null)
                return;

            if (e.Status == Eligible)
                _prometric.SaveEligibility(registration);

            else if (e.Status == NotEligible)
                _prometric.DeleteEligibility(registration);
        }

        public void Handle(ExamFormAssigned e)
        {
            if (e.PreviousForm == null)
                CheckForPrometricIntegration(e.AggregateIdentifier);
        }

        public void Handle(ExamFormUnassigned e)
        {
            CheckForPrometricIntegration(e.AggregateIdentifier);
        }

        public void Handle(RegistrationCancelled e)
        {
            CancelPendingTimers(e.AggregateIdentifier);

            if (e.CancelEmptyEvent)
            {
                var state = (Registration)e.AggregateState;
                CancelEmptyEvent(state.EventIdentifier);
            }
        }

        public void Handle(RegistrationDeleted e)
        {
            CancelPendingTimers(e.AggregateIdentifier);

            if (e.CancelEmptyEvent)
            {
                var state = (Registration)e.AggregateState;
                CancelEmptyEvent(state.EventIdentifier);
            }
        }

        public void Handle(TimerElapsed e)
        {
            _commander.Start(e.Timer);
        }

        public void Handle(EventChanged e)
        {
            if (e.CancelEmptyEvent)
            {
                var state = (Registration)e.AggregateState;
                if (state.PreviousEventIdentifier.HasValue)
                    CancelEmptyEvent(state.PreviousEventIdentifier.Value);
            }
        }

        private void CancelPendingTimers(Guid registration)
        {
            var filter = new QRegistrationTimerFilter { RegistrationIdentifier = registration, TimerStatus = "Started" };
            var timers = _registrations.GetTimers(filter);
            foreach (var timer in timers)
                _commander.Send(new CancelRegistrationTimer(registration, timer.TriggerCommand));
        }

        private void CheckForPrometricIntegration(Guid id)
        {
            var registration = _registrations.GetRegistration(id);
            if (registration == null || !IsPrometricIntegrationEnabled(registration.OrganizationIdentifier))
                return;

            if (registration.ApprovalStatus == Registered && registration.ExamFormIdentifier.HasValue)
                _commander.Send(new ChangeEligibility(id, Eligible, $"Add eligibility to Prometric because the approval status for this event registration is {registration.ApprovalStatus} and assessment form {registration.ExamFormIdentifier} is assigned to it."));

            else if (registration.EligibilityStatus == NotEligible)
                _commander.Send(new ChangeEligibility(id, NotEligible, $"Remove eligibility from Prometric."));
        }

        private bool IsPrometricIntegrationEnabled(Guid organizationId)
        {
            var prometric = _organizations.GetModel(organizationId)?.Integrations?.Prometric;

            return
                prometric != null
                && prometric.UserName != null
                && prometric.Password != null;
        }

        private void CancelEmptyEvent(Guid eventId)
        {
            if (eventId == Guid.Empty)
                return;

            var registrations = _registrations.GetRegistrationsByEvent(eventId);
            if (registrations.IsEmpty() || registrations.All(x => x.GradingStatus == "Cancelled"))
                _commander.Send(new CancelEvent(eventId, "Auto-cancellation: the event has no open registrations.", true));
        }
    }
}