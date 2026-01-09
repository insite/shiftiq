using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QEventFilter : Filter
    {
        public Guid? AttendeeUserIdentifier { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public Guid? EventInstructorIdentifier { get; set; }
        public Guid? ProgramIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RegistrationCandidateIdentifier { get; set; }
        public Guid[] VenueLocationIdentifier { get; set; }
        public Guid? PermissionGroupIdentifier { get; set; }

        public string EventBillingType { get; set; }
        public string EventClassCode { get; set; }
        public string EventFormat { get; set; }
        public string EventMaterialTrackingStatus { get; set; }
        public string EventPublicationStatus { get; set; }
        public string EventRequisitionStatus { get; set; }
        public string IncludeEventSchedulingStatus { get; set; }
        public string ExcludeEventSchedulingStatus { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public string EventType { get; set; }
        public string CommentKeyword { get; set; }
        public string ExamType { get; set; }
        public string Keyword { get; set; }
        public string Venue { get; set; }
        public string VenueOffice { get; set; }
        public string ExamFormName { get; set; }
        public string AppointmentType { get; set; }

        public bool? IsOpen { get; set; }
        public bool? IsRegistrationLocked { get; set; }
        public bool? IsResourceAssigned { get; set; }
        public bool? WithholdDistribution { get; set; }

        public int? EventNumber { get; set; }
        public int[] EventNumbers { get; set; }

        public DateTimeOffset? EventPublishedSince { get; set; }
        public DateTimeOffset? EventPublishedBefore { get; set; }
        public DateTimeOffset? EventScheduleEndSince { get; set; }
        public DateTimeOffset? EventScheduledSince { get; set; }
        public DateTimeOffset? EventScheduledBefore { get; set; }
        public DateTimeOffset? AttemptCompletedSince { get; set; }
        public DateTimeOffset? AttemptCompletedBefore { get; set; }
        public DateTimeOffset? DistributionExpectedSince { get; set; }
        public DateTimeOffset? DistributionExpectedBefore { get; set; }
        public DateTimeOffset? RegistrationDeadlineSince { get; set; }
        public DateTimeOffset? RegistrationDeadlineBefore { get; set; }

        public InclusionType UndistributedExamsInclusion { get; set; }

        public EventClassStatus[] EventClassStatuses { get; set; }

        public Guid? ExcludeEventIdentifier { get; set; }
        public EventAvailabilityType? Availability { get; set; }

        public QEventFilter()
        {
            UndistributedExamsInclusion = InclusionType.Include;
        }

        public QEventFilter Clone()
        {
            return (QEventFilter)MemberwiseClone();
        }
    }
}
