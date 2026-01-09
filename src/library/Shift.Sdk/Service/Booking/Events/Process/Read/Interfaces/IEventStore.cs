using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Events;

namespace InSite.Application.Events.Read
{
    public interface IEventStore
    {
        void DeleteAll();
        void DeleteEvent(Guid id);

        void InsertEvent(ClassImported e);
        void InsertEvent(ClassScheduled2 e);
        void InsertEvent(AppointmentScheduled e);
        void InsertEvent(ExamScheduled2 e);
        void InsertEvent(MeetingScheduled2 e);

        void UpdateEvent(CapacityAdjusted e);
        void UpdateEvent(EventRegistrationWithLinkAllowed e);
        void UpdateEvent(CapacityIncreased e);
        void UpdateEvent(CapacityDecreased e);
        void UpdateEvent(InvigilatorCapacityAdjusted e);
        void UpdateEvent(IntegrationConfigured e);

        void UpdateEvent(EventCalendarColorModified e);
        void UpdateEvent(EventCreditAssigned e);
        void UpdateEvent(EventCompleted e);
        void UpdateEvent(EventFormatChanged e);
        void UpdateEvent(EventNotificationTriggered e);
        void UpdateEvent(EventPublicationCompleted e);
        void UpdateEvent(EventPublicationStarted e);
        void UpdateEvent(EventRecoded e);
        void UpdateEvent(EventRescheduled e);
        void UpdateEvent(EventDurationChanged e);
        void UpdateEvent(EventCreditHoursChanged e);
        void UpdateEvent(EventAchievementAdded e);
        void UpdateEvent(EventAchievementChanged e);
        void UpdateEvent(EventRenumbered e);
        void UpdateEvent(EventRetitled e);
        void UpdateEvent(EventScoresPublished e);
        void UpdateEvent(EventScoresValidated e);
        void UpdateEvent(EventRequestStatusChanged e);
        void UpdateEvent(EventScheduleStatusChanged e);
        void UpdateEvent(EventVenueChanged2 e);
        void UpdateEvent(DistributionChanged e);
        void UpdateEvent(DistributionOrdered e);
        void UpdateEvent(DistributionTracked e);
        void UpdateEvent(ExamTypeChanged e);
        void UpdateEvent(EventDescribed e);
        void UpdateEvent(AppointmentDescribed e);
        void UpdateEvent(EventPublished e);
        void UpdateEvent(EventUnpublished e);
        void UpdateEvent(EventCancelled e);
        void UpdateEvent(ExamMaterialReturned e);
        void UpdateEvent(AppointmentTypeChanged e);
        void UpdateEvent(LearnerRegistrationGroupModified e);
        void UpdateEvent(MandatorySurveyModified e);
        void UpdateEvent(RegistrationFieldModified e);

        void UpdateEvent(IChange change, Action<QEvent> action);

        void InsertComment(Guid aggregate, Guid comment, Guid organization, Guid author, DateTimeOffset time, string text);
        void UpdateComment(Guid comment, Guid author, DateTimeOffset time, string text);
        void DeleteComment(Guid comment);

        void InsertContact(Guid aggregate, Guid contact, string role, DateTimeOffset time);
        void DeleteContact(Guid aggregate, Guid contact);

        void InsertExamForm(Guid aggregate, Guid form);
        void DeleteExamForm(Guid aggregate, Guid form);

        void DeleteTimer(Guid id);
        void InsertTimer(QEventTimer timer);
        void UpdateTimer(Guid id, string status);

        void InsertSeat(SeatAdded e);
        void Update(SeatRevised e);
        void RemoveSeat(SeatDeleted e);
    }
}