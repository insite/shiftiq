using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Registrations
{
    public class RegistrationAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new Registration();

        public Registration Data => (Registration)State;

        #region Methods (commands)

        public void AssignAttempt(Guid attempt)
        {
            Apply(new AttemptAssigned(attempt));
        }

        public void ChangeApproval(string status, string reason, ProcessState process, string previous)
        {
            Apply(new ApprovalChanged(status, reason, process, previous));
        }

        public void ChangeCandidateType(string type)
        {
            Apply(new CandidateTypeChanged(type));
        }

        public void ChangeEligibility(string status, string reason, ProcessState process)
        {
            Apply(new EligibilityChanged(status, reason, process));
        }

        public void ChangeSynchronization(string status, ProcessState process)
        {
            Apply(new SynchronizationChanged(status, process));
        }

        public void ChangeCandidate(Guid candidate)
        {
            Apply(new CandidateChanged(candidate));
        }

        public void ChangeEvent(Guid @event, string reason, bool cancelEmptyEvent)
        {
            Apply(new EventChanged(@event, reason, cancelEmptyEvent));
        }

        public void LimitExamTime(int? minutes)
        {
            Apply(new ExamTimeLimited(minutes));
        }

        public void AssignCustomer(Guid? customer)
        {
            var e = new CustomerAssigned(customer);

            Apply(e);
        }

        public void AssignEmployer(Guid? employer)
        {
            var e = new EmployerAssigned(employer);

            Apply(e);
        }

        public void AssignSeat(Guid? seat, decimal? price, string billingCustomer)
        {
            var e = new SeatAssigned(seat, price, billingCustomer);

            Apply(e);
        }

        public void AssignRegistrationFee(decimal? fee)
        {
            var e = new RegistrationFeeAssigned(fee);

            Apply(e);
        }

        public void AssignRegistrationHoursWorked(int? hoursWorked)
        {
            var e = new RegistrationHoursWorkedAssigned(hoursWorked);

            Apply(e);
        }

        public void AssignRegistrationPayment(Guid payment)
        {
            var e = new RegistrationPaymentAssigned(payment);

            Apply(e);
        }

        public void ChangeGrading(string status)
        {
            Apply(new GradingChanged(status));
        }

        public void ChangeRegistrationPassword(string password)
            => Apply(new RegistrationPasswordChanged(password));

        public void CancelRegistration(string reason, bool cancelEmptyEvent)
        {
            var e = new RegistrationCancelled(reason, cancelEmptyEvent);

            Apply(e);
        }

        public void AssignSchool(Guid school)
        {
            var e = new SchoolAssigned(school);

            Apply(e);
        }

        public void UnassignSchool()
        {
            var e = new SchoolUnassigned();

            Apply(e);
        }

        public void AddInstructor(Guid instructor)
        {
            var e = new InstructorAdded(instructor);

            Apply(e);
        }

        public void RemoveInstructor(Guid instructor)
        {
            var e = new InstructorRemoved(instructor);

            Apply(e);
        }

        public void UnassignExamForm()
        {
            Apply(new ExamFormUnassigned());
        }

        public void DeleteRegistration(bool cancelEmptyEvent)
        {
            Apply(new RegistrationDeleted(cancelEmptyEvent));
        }

        public void CancelTimer(Guid timer)
        {
            var e = new TimerCancelled(timer);

            Apply(e);
        }

        public void CommentRegistration(string comment)
        {
            var e = new RegistrationCommented(comment);

            Apply(e);
        }

        public void ElapseTimer(Guid timer)
        {
            var e = new TimerElapsed(timer);

            Apply(e);
        }

        public void GrantAccommodation(string type, string name, int? timeExtension)
        {
            var e = new AccommodationGranted(type, name, timeExtension);

            Apply(e);
        }

        public void RequestRegistration(Guid organization, Guid @event, Guid candidate, string attendanceStatus, string approvalStatus, decimal? fee, string comment, string source, int? sequence)
        {
            var password = RandomStringGenerator.CreatePasscode();

            var e = new RegistrationRequested(organization, @event, candidate, attendanceStatus, approvalStatus, password, fee, comment, source, sequence);

            Apply(e);
        }

        public void RevokeAccommodation(string type)
        {
            var e = new AccommodationRevoked(type);

            Apply(e);
        }

        public void AssignExamForm(Guid form, Guid? previousForm)
        {
            Apply(new ExamFormAssigned(form, previousForm));
        }

        public void StartTimer(Guid timer, DateTimeOffset at, string name)
        {
            var e = new TimerStarted(timer, at, name);

            Apply(e);
        }

        public void TakeAttendance(string status, decimal? quantity, string unit)
        {
            var e = new AttendanceTaken(status, quantity, unit);

            Apply(e);
        }

        public void TriggerNotification(string name)
        {
            var e = new NotificationTriggered(name);

            Apply(e);
        }

        public void ChangeGrade(string grade, decimal? score)
        {
            Apply(new GradeChanged(grade, score));
        }

        public void ChangeGrading(string status, string reason, ProcessState process)
        {
            Apply(new GradingChanged(status, reason, process));
        }

        public void IncludeRegistrationToT2202()
        {
            Apply(new RegistrationIncludedToT2202());
        }

        public void ExcludeRegistrationFromT2202()
        {
            Apply(new RegistrationExcludedFromT2202());
        }

        public void ChangeRegistrantContactInformation(RegistrantChangedField[] changedFields)
        {
            Apply(new RegistrantContactInformationChanged(changedFields));
        }

        public void ModifyRegistrationRequestedBy(Guid? registrationRequestedBy)
        {
            Apply(new RegistrationRequestedByModified(registrationRequestedBy));
        }

        public void ModifyRegistrationBillingCode(string billingCode)
        {
            if (Data.BillingCode == billingCode)
                return;

            Apply(new RegistrationBillingCodeModified(billingCode));
        }

        #endregion
    }
}
