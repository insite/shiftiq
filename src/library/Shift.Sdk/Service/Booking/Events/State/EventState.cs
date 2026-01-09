using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Events
{
    public class EventExam
    {
        public string Type { get; set; }
        public DateTimeOffset? MaterialReturnShipmentReceived { get; set; }
        public string MaterialReturnShipmentCode { get; set; }
        public string MaterialReturnShipmentCondition { get; set; }
    }

    public class EventIntegration
    {
        public bool WithholdGrades { get; set; }
        public bool WithholdDistribution { get; set; }
    }

    public class EventDistribution
    {
        public string Process { get; set; }

        public DateTimeOffset? Ordered { get; set; }
        public DateTimeOffset? Expected { get; set; }
        public DateTimeOffset? Shipped { get; set; }
        public DateTimeOffset? ExamStarted { get; set; }
        public DateTimeOffset? Tracked { get; set; }
    }

    public class EventState : AggregateState
    {
        public EventType EventType { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public string EventFormat { get; set; }
        public string EventCalendarColor { get; set; }
        public DateTimeOffset? RegistrationLocked { get; set; }
        public Guid? MandatorySurveyFormIdentifier { get; set; }

        public bool AllowMultipleRegistrations { get; set; }
        public bool PersonCodeIsRequired { get; set; }

        public EventExam Exam { get; set; }
        public EventIntegration Integration { get; set; }
        public EventDistribution Distribution { get; set; }

        public string AppointmentType { get; set; }
        public bool BillingCodeEnabled { get; set; }

        public Guid? WhenEventReminderRequestedNotifyLearnerMessageIdentifier { get; set; }
        public Guid? WhenEventReminderRequestedNotifyInstructorMessageIdentifier { get; set; }
        public int? SendReminderBeforeDays { get; set; }

        public List<Guid> Candidates { get; set; }
        public List<Guid> Contacts { get; set; }

        public Dictionary<RegistrationFieldName, RegistrationField> RegistrationFields { get; set; }

        public EventState()
        {
            Exam = new EventExam();
            Integration = new EventIntegration();
            Distribution = new EventDistribution();

            Candidates = new List<Guid>();
            Contacts = new List<Guid>();
            RegistrationFields = new Dictionary<RegistrationFieldName, RegistrationField>();
        }

        public void When(EventAttendeeAdded e)
        {
            Contacts.Add(e.Contact);

            if (e.Role == "Exam Candidate")
                Candidates.Add(e.Contact);
        }

        public void When(EventAttendeeRemoved e)
        {
            Contacts.Remove(e.Contact);

            if (e.Role == "Exam Candidate" && Candidates.Contains(e.Contact))
                Candidates.Remove(e.Contact);
        }

        public void When(EventBillingCodeEnabled e)
        {
            BillingCodeEnabled = e.Enabled;
        }

        public void When(EventCancelled _) { }

        public void When(EventCompleted _) { }

        public void When(EventCreditAssigned _) { }

        public void When(EventDescribed _) { }

        public void When(AppointmentDescribed _) { }

        public void When(EventFormatChanged e)
        {
            EventFormat = e.Format;
        }

        public void When(EventCalendarColorModified e)
        {
            EventCalendarColor = e.CalendarColor;
        }

        public void When(EventNotificationTriggered _) { }

        public void When(EventPublicationStarted _) { }

        public void When(EventPublicationCompleted _) { }

        public void When(EventPublished _) { }

        public void When(EventUnpublished _) { }

        public void When(EventRecoded _) { }

        public void When(EventRescheduled e) { EventTime = e.StartTime; }

        public void When(EventDurationChanged _) { }

        public void When(EventCreditHoursChanged _) { }

        public void When(EventAchievementAdded _) { }

        public void When(EventAchievementChanged _) { }

        public void When(EventRenumbered _) { }

        public void When(EventRetitled _) { }

        public void When(EventScoresPublished _) { }

        public void When(EventTimerCancelled _) { }

        public void When(EventTimerElapsed _) { }

        public void When(EventTimerStarted _) { }

        public void When(EventVenueChanged2 _) { }

        public void When(EventDeleted _) { }

        public void When(EventRequestStatusChanged _) { }

        public void When(EventScheduleStatusChanged _) { }

        public void When(EventScoresValidated _) { }

        public void When(CapacityAdjusted _) { }

        public void When(CapacityDecreased _) { }

        public void When(CapacityIncreased _) { }

        public void When(ClassScheduled2 e)
        {
            EventType = EventType.Class;
            EventTime = e.StartTime;
        }

        public void When(ClassImported _)
        {
            EventType = EventType.Class;
        }

        public void When(EventCommentPosted _) { }

        public void When(EventCommentDeleted _) { }

        public void When(EventCommentModified _) { }

        public void When(IntegrationConfigured e)
        {
            Integration.WithholdGrades = e.WithholdGrades;
            Integration.WithholdDistribution = e.WithholdDistribution;
        }

        public void When(DistributionChanged e)
        {
            Distribution.Process = e.Process;
            Distribution.Ordered = e.Ordered;
            Distribution.Expected = e.Expected;
            Distribution.Shipped = e.Shipped;
            Distribution.ExamStarted = e.Used;
        }

        public void When(DistributionOrdered e)
        {
            Distribution.Ordered = e.ChangeTime;
        }

        public void When(DistributionReturned _) { }

        public void When(EventRegistrationWithLinkAllowed _) { }

        public void When(DistributionShipped _) { }

        public void When(DistributionTracked e)
        {
            Distribution.Tracked = e.ChangeTime;
        }

        public void When(ExamAttemptsImported _) { }

        public void When(ExamFormAttached _) { }

        public void When(ExamFormDetached _) { }

        public void When(ExamMaterialReturned e)
        {
            Exam.MaterialReturnShipmentCode = e.ReturnShipmentCode;
            Exam.MaterialReturnShipmentReceived = e.ReturnShipmentDate;
            Exam.MaterialReturnShipmentCondition = e.ReturnShipmentCondition;
        }

        public void When(ExamScheduled2 e)
        {
            EventType = EventType.Exam;
            EventTime = e.StartTime;
            EventFormat = e.Format;
            Exam.Type = e.Type;
        }

        public void When(AppointmentScheduled e)
        {
            EventType = EventType.Appointment;
            EventTime = e.StartTime;
        }

        public void When(ExamTypeChanged e)
        {
            Exam.Type = e.Type;
        }

        public void When(InvigilatorCapacityAdjusted _) { }

        public void When(MeetingScheduled2 e)
        {
            EventType = EventType.Meeting;
            EventTime = e.StartTime;
        }

        public void When(RegistrationEnabled _) { }

        public void When(RegistrationFieldModified e)
        {
            RegistrationFields[e.Field.FieldName] = e.Field.Clone();
        }

        public void When(SeatAdded _) { }

        public void When(SeatDeleted _) { }

        public void When(LearnerRegistrationGroupModified _) { }

        public void When(MandatorySurveyModified e)
        {
            MandatorySurveyFormIdentifier = e.SurveyForm;
        }

        public void When(SeatRevised _) { }

        public void When(AppointmentTypeChanged e)
        {
            AppointmentType = e.AppointmentType;
        }

        public void When(EventRegistrationLocked e)
        {
            RegistrationLocked = e.Locked;
        }

        public void When(EventRegistrationUnlocked _)
        {
            RegistrationLocked = null;
        }

        public void When(EventAllowMultipleRegistrationsModified e)
        {
            AllowMultipleRegistrations = e.Value;
        }

        public void When(EventPersonCodeIsRequiredModified e)
        {
            PersonCodeIsRequired = e.Value;
        }

        public void When(EventMessageConnected e)
        {
            switch (e.MessageType)
            {
                case EventMessageType.ReminderLearner:
                    WhenEventReminderRequestedNotifyLearnerMessageIdentifier = e.MessageId;
                    break;
                case EventMessageType.ReminderInstructor:
                    WhenEventReminderRequestedNotifyInstructorMessageIdentifier = e.MessageId;
                    break;
                default:
                    throw new ArgumentException($"Unsupported message: {e.MessageType}");
            }
        }

        public void When(EventMessagePeriodModified e)
        {
            SendReminderBeforeDays = e.SendReminderBeforeDays;
        }

        public void When(EventMessageSent _)
        {
        }

        public void When(SerializedChange _)
        {
            // Obsolete changes go here
        }
    }
}