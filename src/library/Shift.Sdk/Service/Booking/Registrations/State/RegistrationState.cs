using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class Registration : AggregateState
    {
        public Guid EventIdentifier { get; set; }
        public Guid? PreviousEventIdentifier { get; set; }

        public Guid CandidateIdentifier { get; set; }

        public Guid? FormIdentifier { get; set; }
        public Guid? PreviousFormIdentifier { get; set; }

        public string RegistrationPassword { get; set; }
        public string BillingCode { get; set; }

        public DateTimeOffset? Voided { get; set; }

        #region When

        public void When(AccommodationGranted _) { }

        public void When(AccommodationRevoked _) { }

        public void When(ApprovalChanged _) { }

        public void When(AttemptAssigned _) { }

        public void When(AttendanceTaken _) { }

        public void When(CustomerAssigned _) { }

        public void When(EligibilityChanged _) { }

        public void When(EmployerAssigned _) { }

        public void When(ExamFormAssigned e)
        {
            PreviousFormIdentifier = e.PreviousForm;
            FormIdentifier = e.Form;
        }

        public void When(ExamFormReplaced e) { }

        public void When(ExamFormUnassigned _)
        {
            PreviousFormIdentifier = FormIdentifier;
            FormIdentifier = null;
        }

        public void When(ExamTimeLimited _) { }

        public void When(GradeChanged _) { }

        public void When(GradingChanged _) { }

        public void When(InstructorAdded _) { }

        public void When(InstructorRemoved _) { }

        public void When(NotificationTriggered _) { }

        public void When(RegistrationCancelled _) { }

        public void When(RegistrationCommented _) { }

        public void When(RegistrationFeeAssigned _) { }

        public void When(RegistrationHoursWorkedAssigned _) { }

        public void When(RegistrationPaymentAssigned _) { }

        public void When(RegistrationPasswordChanged e) { RegistrationPassword = e.Password; }

        public void When(RegistrationRequested e)
        {
            EventIdentifier = e.Event;
            CandidateIdentifier = e.Candidate;
            RegistrationPassword = e.Password;
        }

        public void When(RegistrationDeleted e) { Voided = e.ChangeTime; }

        public void When(SchoolAssigned _) { }

        public void When(SchoolUnassigned _) { }

        public void When(SeatAssigned _) { }

        public void When(SynchronizationChanged _) { }

        public void When(CandidateChanged _) { }

        public void When(CandidateTypeChanged _) { }

        public void When(TimerCancelled _) { }

        public void When(TimerElapsed _) { }

        public void When(TimerStarted _) { }

        public void When(RegistrationIncludedToT2202 _) { }

        public void When(RegistrationExcludedFromT2202 _) { }

        public void When(RegistrantContactInformationChanged _) { }

        public void When(EventChanged e)
        {
            PreviousEventIdentifier = EventIdentifier;
            EventIdentifier = e.Event;
        }

        public void When(RegistrationRequestedByModified _) { }

        public void When(RegistrationBillingCodeModified e)
        {
            BillingCode = e.BillingCode;
        }

        #endregion
    }
}
