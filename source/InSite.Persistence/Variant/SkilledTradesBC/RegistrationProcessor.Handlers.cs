using InSite.Application.Registrations.Write;
using InSite.Domain.Messages;
using InSite.Domain.Registrations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain changes 
    /// in a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes 
    /// purely reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a 
    /// state machine that is driven forward by incoming events (which may come from many aggregates). Some states will 
    /// have side effects, such as sending commands, sending HTTP requests to external web services, or sending emails.
    /// </remarks>
    public partial class RegistrationProcessor
    {
        public void Handle(ApprovalChanged e)
        {
            var packet = GetRegistrationPacket(e.AggregateIdentifier);
            bool statusChanged = packet.Registration.ApprovalStatus != e.PreviousStatus;
            bool forceExecution = e.Process?.Description == "Force Execution";

            if (statusChanged || forceExecution)
            {
                if (!IsNotificationDisabled(packet))
                {
                    SetNotificationTimers(e, packet);
                    _broker.Send(e, new ChangeSynchronization(e.AggregateIdentifier, "Push to Direct Access", null));
                }
            }

            if (statusChanged && e.PreviousStatus != null && e.Status != null)
            {
                CancelRegistrationNotificationTimer(e, e.AggregateIdentifier, NotificationType.ITA021);
            }
        }

        public void Handle(EligibilityChanged e)
        {
            if (e.Status != "Check Eligibility in DA")
                return;

            var registration = _registrations.GetRegistration(e.AggregateIdentifier, x => x.Event);
            if (registration == null)
                return;

            if (registration.Event == null)
                throw new UnscheduledRegistrationException("Verification cannot be started on an exam registration that has no event.");

            if (registration.Event.EventScheduledStart == null)
                throw new UnscheduledRegistrationException("Verification cannot be started on an exam registration that has no scheduled start date.");

            var process = VerifyIndividualInDirectAccess(e, registration);

            var id = registration.RegistrationIdentifier;

            // If there are no warnings and no errors then automatically approve the registration. The candidate is
            // eligible to write the exam.

            if (!process.HasErrors && !process.HasWarnings)
            {
                _broker.Send(e, new ChangeEligibility(id, "Eligible in DA", e.Reason, process));
                _broker.Send(e, new ChangeApproval(id, "Eligible", "Eligible in DA", null, registration.ApprovalStatus));
            }
            else
            {
                _broker.Send(e, new ChangeEligibility(id, "Not Eligible in DA", e.Reason, process));
            }
        }

        public void Handle(ExamFormAssigned e)
        {
            {
                var change = new ChangeSynchronization(e.AggregateIdentifier, "Pull from Direct Access", null);
                _broker.Send(e, change);
            }
            {
                var change = new ChangeEligibility(e.AggregateIdentifier, "Check Eligibility in DA", null);
                _broker.Send(e, change);
            }
        }

        public void Handle(GradingChanged e)
        {

        }

        public void Handle(NotificationTriggered e)
        {
            ExecuteMailout(e, e.Name);
        }

        public void Handle(RegistrationCancelled e)
        {
            var packet = GetRegistrationPacket(e.AggregateIdentifier);
            if (EnableIntegrationWithDirectAccess(packet))
            {
                RemoveCandidateFromDirectAccess(e, packet, e.OriginUser, "Cancelled");
                if (!IsNotificationDisabled(packet))
                    ExecuteMailout(e, "ITA022");
            }
        }

        public void Handle(RegistrationDeleted e)
        {
            var packet = GetRegistrationPacket(e.AggregateIdentifier);
            if (EnableIntegrationWithDirectAccess(packet))
            {
                RemoveCandidateFromDirectAccess(e, packet, e.OriginUser, "Voided");
                if (!IsNotificationDisabled(packet))
                    ExecuteMailout(e, "ITA022");
            }
        }

        public void Handle(SynchronizationChanged e)
        {
            if (e.Status == "Pull from Direct Access" || e.Status == "Push to Direct Access")
            {
                var registration = e.AggregateIdentifier;
                var operation = e.Status;
                var process = new ProcessState(ExecutionState.Started);
                process.Errors = null;

                if (operation == "Pull from Direct Access")
                {
                    var packet = GetRegistrationPacket(e.AggregateIdentifier);
                    process.Errors = RefreshIndividualFromDirectAccess(e, registration, GetCandidateCode(registration), packet?.Registration?.Form?.FormCode, e.OriginUser);
                }
                else if (operation == "Push to Direct Access")
                {
                    var packet = GetRegistrationPacket(e.AggregateIdentifier);
                    if (EnableIntegrationWithDirectAccess(packet))
                        UpdateExamCandidateInDirectAccess(e, packet);
                }

                if (process.Errors.IsEmpty())
                {
                    process.Indicator = Indicator.Success;
                    operation += " Succeeded";
                }
                else
                {
                    process.Indicator = Indicator.Danger;
                    operation += " Failed";
                }

                process.Execution = ExecutionState.Completed;

                _broker.Send(e, new ChangeSynchronization(registration, operation, process));
            }
        }
    }
}