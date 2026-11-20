using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Write;
using InSite.Application.Integrations.Prometric;
using InSite.Application.Organizations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Registrations;

using Shift.Common;
using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Application.Registrations.Read
{
    public class RegistrationChangeProcessor
    {
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
            publisher.Subscribe<ExamFormAdded>(Handle);

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

            var registration = _registrations.GetRegistration(e.AggregateIdentifier,
                x => x.Candidate.User, x => x.Candidate.HomeAddress, x => x.Accommodations, x => x.Form,
                x => x.ExamForms.Select(f => f.Form));

            // Prometric requires an assessment form for every exam event registration. If the registration does not
            // have an assessment form, then we can't send any change request(s) to Prometric.
            if (registration == null || registration.ExamFormIdentifier == null && !registration.ExamForms.Any())
                return;

            var forms = ToExamForms(registration);
            if (e.Form.HasValue)
                forms = forms.Where(x => x.ExamFormIdentifier == e.Form.Value);

            foreach (var form in forms)
            {
                if (e.Status == RegistrationEligibility.Eligible)
                    _prometric.SaveEligibility(registration, form);

                else if (e.Status == RegistrationEligibility.NotEligible)
                    _prometric.DeleteEligibility(registration, form);
            }
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

        public void Handle(ExamFormAdded e)
        {
            CheckForPrometricIntegration(e.AggregateIdentifier, e.Form);
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
            if (!e.CancelEmptyEvent)
                return;

            var state = (Registration)e.AggregateState;
            if (state.PreviousEventIdentifier.HasValue)
                CancelEmptyEvent(state.PreviousEventIdentifier.Value);
        }

        private void CancelPendingTimers(Guid registration)
        {
            var filter = new QRegistrationTimerFilter { RegistrationIdentifier = registration, TimerStatus = "Started" };
            var timers = _registrations.GetTimers(filter);
            foreach (var timer in timers)
                _commander.Send(new CancelRegistrationTimer(registration, timer.TriggerCommand));
        }

        private void CheckForPrometricIntegration(Guid registrationId, Guid? formId = null)
        {
            var registration = _registrations.GetRegistration(registrationId, x => x.ExamForms);
            if (registration == null || !IsPrometricIntegrationEnabled(registration.OrganizationIdentifier))
                return;

            var forms = ToExamForms(registration);
            if (formId.HasValue)
                forms = forms.Where(x => x.ExamFormIdentifier == formId.Value);

            foreach (var form in forms)
            {
                if (registration.ApprovalStatus == Registered)
                {
                    _commander.Send(new ChangeEligibility(registrationId, form.ExamFormIdentifier, RegistrationEligibility.Eligible,
                        $"Add eligibility to Prometric because the approval status for " +
                        $"this event registration is {registration.ApprovalStatus} and " +
                        $"assessment form {form.ExamFormIdentifier} is assigned to it."));
                }

                else if (form.EligibilityStatus == RegistrationEligibility.NotEligible)
                {
                    _commander.Send(new ChangeEligibility(registrationId, form.ExamFormIdentifier, RegistrationEligibility.NotEligible,
                        "Remove eligibility from Prometric."));
                }
            }
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

        private static IEnumerable<QRegistrationExamForm> ToExamForms(QRegistration registration)
        {
            if (registration.ExamFormIdentifier != null)
                yield return ToExamForm(registration);

            foreach (var form in registration.ExamForms.OrderBy(x => x.Sequence).EmptyIfNull())
                yield return form;
        }

        private static QRegistrationExamForm ToExamForm(QRegistration registration) => new QRegistrationExamForm
        {
            RegistrationIdentifier = registration.RegistrationIdentifier,
            ExamFormIdentifier = registration.ExamFormIdentifier.Value,
            Form = registration.Form,
            AttemptIdentifier = registration.AttemptIdentifier,
            Password = registration.RegistrationPassword,
            ExamTimeLimit = registration.ExamTimeLimit,
            EligibilityProcess = registration.EligibilityProcess,
            EligibilityStatus = registration.EligibilityStatus,
            EligibilityUpdated = registration.EligibilityUpdated,
            Sequence = -1
        };
    }
}
