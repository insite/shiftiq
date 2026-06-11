using System;

using InSite.Domain.Registrations;

namespace InSite.Application.Registrations.Read
{
    public interface IRegistrationStore
    {
        // Registrations

        void InsertRegistration(RegistrationRequested e);

        void UpdateRegistration(AccommodationGranted e);
        void UpdateRegistration(AccommodationRevoked e);
        void UpdateRegistration(ApprovalChanged e);
        void UpdateRegistration(AttemptAssigned e);
        void UpdateRegistration(AttendanceTaken e);
        void UpdateRegistration(CustomerAssigned e);
        void UpdateRegistration(EligibilityChanged e);
        void UpdateRegistration(EmployerAssigned e);
        void UpdateRegistration(ExamFormAssigned e);
        void UpdateRegistration(ExamFormUnassigned e);
        void UpdateRegistration(ExamTimeLimited e);
        void UpdateRegistration(GradeChanged e);
        void UpdateRegistration(GradingChanged e);
        void UpdateRegistration(InstructorAdded e);
        void UpdateRegistration(InstructorRemoved e);
        void UpdateRegistration(NotificationTriggered e);
        void UpdateRegistration(RegistrationCancelled e);
        void UpdateRegistration(RegistrationCommented e);
        void UpdateRegistration(RegistrationFeeAssigned e);
        void UpdateRegistration(RegistrationHoursWorkedAssigned e);
        void UpdateRegistration(RegistrationPasswordChanged e);
        void UpdateRegistration(RegistrationPaymentAssigned e);
        void UpdateRegistration(SchoolAssigned e);
        void UpdateRegistration(SchoolUnassigned e);
        void UpdateRegistration(SeatAssigned e);
        void UpdateRegistration(SynchronizationChanged e);
        void UpdateRegistration(CandidateChanged e);
        void UpdateRegistration(CandidateTypeChanged e);
        void UpdateRegistration(RegistrationIncludedToT2202 e);
        void UpdateRegistration(RegistrationExcludedFromT2202 e);
        void UpdateRegistration(EventChanged e);
        void UpdateRegistration(RegistrationRequestedByModified e);
        void UpdateRegistration(RegistrationBillingCodeModified e);

        void DeleteAll();
        void Delete(Guid id);
        void DeleteRegistration(RegistrationDeleted e);

        // Timers

        void InsertTimer(QRegistrationTimer timer);

        void UpdateTimer(Guid timer, string status);

        void DeleteTimer(Guid id);
    }
}