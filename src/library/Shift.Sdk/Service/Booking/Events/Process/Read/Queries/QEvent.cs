using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Events;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Application.Events.Read
{
    public class QEvent
    {
        public Guid EventIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? VenueCoordinatorIdentifier { get; set; }
        public Guid? VenueLocationIdentifier { get; set; }
        public Guid? VenueOfficeIdentifier { get; set; }
        public Guid? LearnerRegistrationGroupIdentifier { get; set; }
        public Guid? MandatorySurveyFormIdentifier { get; set; }
        public Guid? WhenEventReminderRequestedNotifyLearnerMessageIdentifier { get; set; }
        public Guid? WhenEventReminderRequestedNotifyInstructorMessageIdentifier { get; set; }

        public string EventBillingType { get; set; }
        public string EventClassCode { get; set; }
        public string Content { get; set; }
        public string EventCalendarColor { get; set; }
        public string EventDescription { get; set; }
        public string EventFormat { get; set; }
        public string EventPublicationStatus { get; set; }
        public string EventRequisitionStatus { get; set; }
        public string EventSchedulingStatus { get; set; }
        public string EventSource { get; set; }
        public string EventSummary { get; set; }
        public string EventTitle { get; set; }
        public string EventType { get; set; }
        public string DistributionCode { get; set; }
        public string DistributionErrors { get; set; }
        public string DistributionProcess { get; set; }
        public string DistributionStatus { get; set; }
        public string DurationUnit { get; set; }
        public string ExamMaterialReturnShipmentCode { get; set; }
        public string ExamMaterialReturnShipmentCondition { get; set; }
        public string ExamType { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string PublicationErrors { get; set; }
        public string VenueRoom { get; set; }
        public string AppointmentType { get; set; }
        public string RegistrationFields { get; set; }

        public bool WaitlistEnabled { get; set; }
        public bool? AllowRegistrationWithLink { get; set; }

        public bool IntegrationWithholdGrades { get; set; }
        public bool IntegrationWithholdDistribution { get; set; }
        public bool PersonCodeIsRequired { get; set; }
        public bool AllowMultipleRegistrations { get; set; }
        public bool BillingCodeEnabled { get; set; }

        public int EventNumber { get; set; }
        public int? CapacityMaximum { get; set; }
        public int? CapacityMinimum { get; set; }
        public int? DurationQuantity { get; set; }
        public int? ExamDurationInMinutes { get; set; }
        public int? InvigilatorMinimum { get; set; }
        public int? SendReminderBeforeDays { get; set; }

        public decimal? CreditHours { get; set; }

        public DateTimeOffset? EventScheduledEnd { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
        public DateTimeOffset? DistributionExpected { get; set; }
        public DateTimeOffset? DistributionOrdered { get; set; }
        public DateTimeOffset? DistributionShipped { get; set; }
        public DateTimeOffset? DistributionTracked { get; set; }
        public DateTimeOffset? ExamMaterialReturnShipmentReceived { get; set; }
        public DateTimeOffset? ExamStarted { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? RegistrationDeadline { get; set; }
        public DateTimeOffset? RegistrationStart { get; set; }
        public DateTimeOffset? RegistrationLocked { get; set; }
        public DateTimeOffset? ReminderMessageSent { get; set; }

        public virtual VGroup VenueLocation { get; set; }
        public virtual VGroup VenueOffice { get; set; }
        public virtual VPerson VenueCoordinator { get; set; }
        public virtual QAchievement Achievement { get; set; }

        public virtual ICollection<QEventAttendee> Attendees { get; set; } = new HashSet<QEventAttendee>();
        public virtual ICollection<QEventAssessmentForm> ExamForms { get; set; } = new HashSet<QEventAssessmentForm>();
        public virtual ICollection<QGradebook> Gradebooks { get; set; } = new HashSet<QGradebook>();
        public virtual ICollection<QJournalSetup> JournalSetups { get; set; } = new HashSet<QJournalSetup>();
        public virtual ICollection<QRegistration> Registrations { get; set; } = new HashSet<QRegistration>();
        public virtual ICollection<QSeat> Seats { get; set; } = new HashSet<QSeat>();
        public virtual ICollection<QGradebookEvent> GradebookEvents { get; set; } = new HashSet<QGradebookEvent>();
        public virtual ICollection<VEventGroupPermission> EventGroupPermissions { get; set; } = new HashSet<VEventGroupPermission>();

        #region Properties (calculated)

        public string EventDate => EventScheduledStart.FormatDateOnly();

        public string EventDateLess3Days => EventScheduledStart.AddDays(-3).FormatDateOnly();

        public string ExamDurationText
        {
            get
            {
                var minutes = ExamDurationInMinutes ?? 0;

                if (minutes == 0)
                    return "Not Specified";

                if (minutes % 30 == 0)
                    return ShiftHumanizer.ToQuantity(minutes / 60, "0.##", "hour");

                return ShiftHumanizer.ToQuantity(minutes, "minutes");
            }
        }

        public bool EventIsOnline => EventFormat == EventExamFormat.Online.Value;

        public int EventScheduledCountdownDays
        {
            get
            {
                if (EventScheduledStart != null)
                    return (int)(EventScheduledStart - DateTimeOffset.UtcNow).TotalDays;

                return 0;
            }
        }

        public string EventTime => EventScheduledStart.FormatTimeOnly();

        public string EventTimeLess30Minutes => EventScheduledStart.AddMinutes(-30).FormatTimeOnly();

        public string CandidateCountText => ShiftHumanizer.ToQuantity(Registrations.Count, "Candidate");

        public string FormCountText => ShiftHumanizer.ToQuantity(ExamForms.Count, "Form");

        public string DistributionCodeHtml
        {
            get
            {
                if (!DistributionCode.HasValue())
                    return string.Empty;

                return $"<a href='/ui/admin/integrations/bcmail/view?event={EventIdentifier}'>{DistributionCode}</a>";
            }
        }

        public string DistributionExpectedCountdownDays
        {
            get
            {
                if (DistributionExpected != null)
                    return ShiftHumanizer.ToQuantity((int)(DistributionExpected.Value - DateTimeOffset.UtcNow).TotalDays, "day");

                return "0 days";
            }
        }

        public string DistributionExpectedText
        {
            get
            {
                if (EventIsOnline)
                    return EventDateLess3Days;

                if (DistributionExpected != null)
                    return DistributionExpected.FormatDateOnly();

                return "Not Specified";
            }
        }

        public string VenueLocationName => VenueLocation?.GroupName;

        public string VenueOfficeName => VenueOffice?.GroupOffice ?? VenueOffice?.GroupName;

        public EventAvailabilityType Availability
        {
            get
            {
                if (RegistrationLocked.HasValue)
                    return EventAvailabilityType.Full;

                var confirmedRegistrationCount = ConfirmedRegisteredCount + InvitedRegistrationCount;

                if ((CapacityMinimum ?? 0) > 0 && confirmedRegistrationCount < CapacityMinimum)
                    return EventAvailabilityType.Under;
                else if ((CapacityMaximum ?? 0) > 0 && confirmedRegistrationCount > CapacityMaximum)
                    return EventAvailabilityType.Over;
                else if ((CapacityMaximum ?? 0) > 0 && confirmedRegistrationCount == CapacityMaximum)
                    return EventAvailabilityType.Full;
                else if (confirmedRegistrationCount == 0)
                    return EventAvailabilityType.Empty;
                else
                    return EventAvailabilityType.Open;
            }
        }

        public int ConfirmedRegisteredCount => Registrations
            .Where(
                x => (x.ApprovalStatus.IsEmpty() || StringHelper.Equals(x.ApprovalStatus, "Registered"))
                     && !StringHelper.Equals(x.AttendanceStatus, "Withdrawn/Cancelled"))
            .Count();

        public int WaitlistedRegistrationCount => Registrations
            .Where(
                x => StringHelper.Equals(x.ApprovalStatus, "Waitlisted")
                     && !StringHelper.Equals(x.AttendanceStatus, "Withdrawn/Cancelled"))
            .Count();

        public int InvitedRegistrationCount => Registrations
            .Where(
                x => StringHelper.Equals(x.ApprovalStatus, "Invitation Sent")
                     && !StringHelper.Equals(x.AttendanceStatus, "Withdrawn/Cancelled"))
            .Count();

        public EventClassStatus GetClassStatus()
        {
            if (StringHelper.Equals(EventSchedulingStatus, "Cancelled"))
                return EventClassStatus.Cancelled;

            if (GradebookEvents.Any(x => x.Gradebook.IsLocked))
                return EventClassStatus.Closed;

            if (EventScheduledEnd <= DateTimeOffset.Now)
                return EventClassStatus.Completed;

            if (EventScheduledStart <= DateTimeOffset.Now)
                return EventClassStatus.InProgress;

            if (string.Equals(EventPublicationStatus, "Published", StringComparison.OrdinalIgnoreCase))
                return EventClassStatus.Published;

            return EventClassStatus.Drafted;
        }

        public void SetRegistrationFields(ICollection<RegistrationField> fields)
        {
            RegistrationFields = fields.IsNotEmpty()
                ? JsonConvert.SerializeObject(fields.OrderBy(x => x.FieldName))
                : null;
        }

        public List<RegistrationField> GetRegistrationFields()
        {
            return RegistrationFields.IsEmpty()
                ? new List<RegistrationField>()
                : JsonConvert.DeserializeObject<List<RegistrationField>>(RegistrationFields);
        }

        public const int DistributionTimelineBusinessDays = 12;

        public static DateTimeOffset GetDefaultDistributionExpected(DateTimeOffset startDate)
        {
            return Calendar.AddBusinessDays(startDate, -DistributionTimelineBusinessDays);
        }

        #endregion
    }
}