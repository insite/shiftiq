using System.Linq;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Registrations;

using Shift.Common;

namespace InSite.Application.Registrations.Write
{
    public class RegistrationCommandReceiver
    {
        private readonly ICommandQueue _commander;
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IRegistrationSearch _registrationSearch;
        private readonly IBankSearch _bankSearch;

        public RegistrationCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository, IRegistrationSearch registrationSearch, IBankSearch bankSearch)
        {
            _commander = commander;
            _publisher = publisher;
            _repository = repository;
            _registrationSearch = registrationSearch;
            _bankSearch = bankSearch;

            commander.Subscribe<AddInstructor>(Handle);
            commander.Subscribe<AssignAttempt>(Handle);
            commander.Subscribe<AssignCustomer>(Handle);
            commander.Subscribe<AssignEmployer>(Handle);
            commander.Subscribe<AssignExamForm>(Handle);
            commander.Subscribe<AssignRegistrationFee>(Handle);
            commander.Subscribe<AssignRegistrationHoursWorked>(Handle);
            commander.Subscribe<AssignRegistrationPayment>(Handle);
            commander.Subscribe<AssignSchool>(Handle);
            commander.Subscribe<AssignSeat>(Handle);
            commander.Subscribe<ChangeCandidateType>(Handle);
            commander.Subscribe<CancelRegistration>(Handle);
            commander.Subscribe<CancelRegistrationTimer>(Handle);
            commander.Subscribe<ChangeApproval>(Handle);
            commander.Subscribe<ChangeEligibility>(Handle);
            commander.Subscribe<ChangeCandidate>(Handle);
            commander.Subscribe<ChangeEvent>(Handle);
            commander.Subscribe<ChangeGrade>(Handle);
            commander.Subscribe<ChangeGrading>(Handle);
            commander.Subscribe<ChangeRegistrationPassword>(Handle);
            commander.Subscribe<ChangeSynchronization>(Handle);
            commander.Subscribe<CommentRegistration>(Handle);
            commander.Subscribe<ElapseRegistrationTimer>(Handle);
            commander.Subscribe<GrantAccommodation>(Handle);
            commander.Subscribe<LimitExamTime>(Handle);
            commander.Subscribe<RemoveInstructor>(Handle);
            commander.Subscribe<RequestRegistration>(Handle);
            commander.Subscribe<RevokeAccommodation>(Handle);
            commander.Subscribe<StartTimer>(Handle);
            commander.Subscribe<TakeAttendance>(Handle);
            commander.Subscribe<TriggerNotification>(Handle);
            commander.Subscribe<UnassignExamForm>(Handle);
            commander.Subscribe<UnassignSchool>(Handle);
            commander.Subscribe<DeleteRegistration>(Handle);
            commander.Subscribe<IncludeRegistrationToT2202>(Handle);
            commander.Subscribe<ExcludeRegistrationFromT2202>(Handle);
            commander.Subscribe<ChangeRegistrantContactInformation>(Handle);
            commander.Subscribe<ModifyRegistrationRequestedBy>(Handle);
            commander.Subscribe<ModifyRegistrationBillingCode>(Handle);
        }

        private void Commit(RegistrationAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddInstructor c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddInstructor(c.Instructor);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignAttempt c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (aggregate.Data.Voided.HasValue)
                    return;

                aggregate.AssignAttempt(c.Attempt);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignCustomer c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignCustomer(c.Customer);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignEmployer c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignEmployer(c.Employer);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignExamForm c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignExamForm(c.Form, c.PreviousForm);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignRegistrationFee c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignRegistrationFee(c.Fee);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignRegistrationHoursWorked c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignRegistrationHoursWorked(c.HoursWorked);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignRegistrationPayment c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignRegistrationPayment(c.Payment);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignSchool c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignSchool(c.School);
                Commit(aggregate, c);
            });
        }

        public void Handle(AssignSeat c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AssignSeat(c.Seat, c.Price, c.BillingCustomer);
                Commit(aggregate, c);
            });
        }

        public void Handle(CancelRegistration c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CancelRegistration(c.Reason, c.CancelEmptyEvent);
                Commit(aggregate, c);
            });
        }

        public void Handle(CancelRegistrationTimer c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CancelTimer(c.Timer);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeApproval c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeApproval(c.Status, c.Reason, c.Process, c.PreviousStatus);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeCandidateType c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeCandidateType(c.Type);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEligibility c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeEligibility(c.Status, c.Reason, c.Process);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGrade c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeGrade(c.Grade, c.Score);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGrading c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeGrading(c.Status);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeRegistrationPassword c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeRegistrationPassword(c.Password);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSynchronization c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSynchronization(c.Status, c.Process);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeCandidate c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeCandidate(c.Candidate);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeEvent c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (aggregate.Data.EventIdentifier == c.Event)
                    return;

                aggregate.RootAggregateIdentifier = c.Event;

                aggregate.ChangeEvent(c.Event, c.Reason, c.CancelEmptyEvent);
                Commit(aggregate, c);
            });
        }

        public void Handle(CommentRegistration c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CommentRegistration(c.Comment);
                Commit(aggregate, c);
            });
        }

        public void Handle(ElapseRegistrationTimer c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ElapseTimer(c.Timer);
                Commit(aggregate, c);
            });
        }

        public void Handle(GrantAccommodation c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.GrantAccommodation(c.Type, c.Name, c.TimeExtension);
                Commit(aggregate, c);
            });
        }

        public void Handle(LimitExamTime c)
        {
            var registration = _registrationSearch.GetRegistration(c.AggregateIdentifier, x => x.Accommodations);
            if (registration == null)
                return;

            var form = registration.ExamFormIdentifier.HasValue
                ? _bankSearch.GetFormData(registration.ExamFormIdentifier.Value)
                : null;

            var timeExtension = registration.Accommodations.Count > 0
                ? registration.Accommodations.Sum(x => x.TimeExtension ?? 0)
                : 0;
            var candidateTimeLimit = form != null
                ? form.Invigilation.TimeLimit + timeExtension
                : (int?)null;

            if (candidateTimeLimit == 0)
                candidateTimeLimit = null;

            if (registration.ExamTimeLimit == candidateTimeLimit)
                return;

            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.LimitExamTime(candidateTimeLimit);
                Commit(aggregate, c);
            });
        }

        public void Handle(RequestRegistration c)
        {
            var maxSequence = _registrationSearch.GetMaxSequence(c.Event) ?? 0;

            var aggregate = new RegistrationAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.Event };
            aggregate.RequestRegistration(c.Tenant, c.Event, c.Candidate, c.AttendanceStatus, c.ApprovalStatus, c.Fee, c.Comment, c.Source, maxSequence + 1);
            Commit(aggregate, c);
        }

        public void Handle(RemoveInstructor c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RemoveInstructor(c.Instructor);
                Commit(aggregate, c);
            });
        }

        public void Handle(RevokeAccommodation c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RevokeAccommodation(c.Type);
                Commit(aggregate, c);
            });
        }

        public void Handle(StartTimer c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.StartTimer(c.Timer, c.At, c.Description);
                Commit(aggregate, c);
            });
        }

        public void Handle(TakeAttendance c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.TakeAttendance(c.Status, c.Quantity, c.Unit);
                Commit(aggregate, c);
            });
        }

        public void Handle(TriggerNotification c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.TriggerNotification(c.Name);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnassignExamForm c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnassignExamForm();
                Commit(aggregate, c);
            });
        }

        public void Handle(UnassignSchool c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnassignSchool();
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteRegistration c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteRegistration(c.CancelEmptyEvent);
                Commit(aggregate, c);
            });
        }

        public void Handle(IncludeRegistrationToT2202 c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.IncludeRegistrationToT2202();
                Commit(aggregate, c);
            });
        }

        public void Handle(ExcludeRegistrationFromT2202 c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ExcludeRegistrationFromT2202();
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeRegistrantContactInformation c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeRegistrantContactInformation(c.ChangedFields);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyRegistrationRequestedBy c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyRegistrationRequestedBy(c.RegistrationRequestedBy);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyRegistrationBillingCode c)
        {
            _repository.LockAndRun<RegistrationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyRegistrationBillingCode(c.BillingCode.NullIfEmpty());
                Commit(aggregate, c);
            });
        }
    }
}