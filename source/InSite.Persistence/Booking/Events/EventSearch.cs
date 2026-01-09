using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Events;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class EventSearch : IEventSearch
    {
        private readonly IChangeRepository _repository;

        public EventSearch(IChangeRepository repository)
        {
            _repository = repository;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        private static readonly string[] ActiveApprovalStatuses = { "", "Registered", "Invitation Sent" };
        private const string WithdrawnCancelled = "Withdrawn/Cancelled";

        #region Comments

        public int CountComments(QEventCommentFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public QComment GetComment(Guid comment)
        {
            using (var db = CreateContext())
            {
                return db.QComments
                    .AsNoTracking()
                    .FirstOrDefault(x => x.CommentIdentifier == comment);
            }
        }

        public List<QComment> GetComments(QEventCommentFilter filter)
        {
            using (var db = CreateContext())
            {
                return db.QComments
                    .Where(x => x.EventIdentifier == filter.EventIdentifier)
                    .OrderByDescending(x => x.CommentPosted)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<QComment> CreateQuery(QEventCommentFilter filter, InternalDbContext db)
        {
            var query = db.QComments
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.EventIdentifier != Guid.Empty)
                query = query.Where(x => x.EventIdentifier == filter.EventIdentifier);

            if (filter.AuthorIdentifier != Guid.Empty)
                query = query.Where(x => x.AuthorUserIdentifier == filter.AuthorIdentifier);

            return query;
        }

        #endregion

        #region Contacts

        public List<VPerson> GetAttendeeUsers(Guid organizationIdentifier, string role)
        {
            using (var db = CreateContext())
            {
                return db.EventAttendees
                    .Where(x => x.Event.OrganizationIdentifier == organizationIdentifier && x.AttendeeRole == role)
                    .Select(x => x.Person)
                    .Distinct()
                    .OrderBy(x => x.UserFullName)
                    .ToList();
            }
        }

        public int CountAttendees(QEventAttendeeFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public QEventAttendee GetAttendee(Guid @event, Guid contact, params Expression<Func<QEventAttendee, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.EventAttendees
                    .AsNoTracking()
                    .ApplyIncludes(includes);

                return query.FirstOrDefault(x => x.EventIdentifier == @event && x.UserIdentifier == contact);
            }
        }

        public List<QEventAttendee> GetAttendees(Guid @event, params Expression<Func<QEventAttendee, object>>[] includes)
        {
            var filter = new QEventAttendeeFilter
            {
                EventIdentifier = @event
            };
            return GetAttendees(filter, includes);
        }

        public List<QEventAttendee> GetAttendees(QEventAttendeeFilter filter, params Expression<Func<QEventAttendee, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                if (string.IsNullOrEmpty(filter.OrderBy))
                    query = query.OrderBy(x => x.Person.UserFullName);
                else
                    query = query.OrderBy(filter.OrderBy);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private IQueryable<QEventAttendee> CreateQuery(QEventAttendeeFilter filter, InternalDbContext db)
        {
            var query = db.EventAttendees
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.EventIdentifier != Guid.Empty && filter.EventIdentifier != null)
                query = query.Where(x => x.EventIdentifier == filter.EventIdentifier);

            if (filter.GradebookIdentifier.HasValue)
            {
                query = query.Join(db.QGradebookEvents.Where(x => x.GradebookIdentifier == filter.GradebookIdentifier),
                        a => a.EventIdentifier,
                        b => b.EventIdentifier,
                        (a, b) => a
                    );
            }

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Event.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.ContactIdentifier != null)
                query = query.Where(x => x.UserIdentifier == filter.ContactIdentifier);

            if (filter.ContactIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.ContactIdentifiers.Contains(x.UserIdentifier));

            if (filter.ContactRole.HasValue())
                query = query.Where(x => x.AttendeeRole.Contains(filter.ContactRole));

            if (filter.ContactKeyword.IsNotEmpty())
                query = query.Where(x =>
                       x.Person.UserFullName.Contains(filter.ContactKeyword)
                    || x.Person.UserEmail.Contains(filter.ContactKeyword)
                    || x.Person.PersonCode.Contains(filter.ContactKeyword)
                );

            return query;
        }

        #endregion

        #region Event Assessment Forms

        public int CountEventAssessmentForms(QEventAssessmentFormFilter filter)
        {
            using (var db = CreateContext())
            {
                return db.EventAssessmentForms
                    .AsNoTracking()
                    .Count(x => x.EventIdentifier == filter.EventIdentifier);
            }
        }

        public List<QEventAssessmentForm> GetEventAssessmentForms(Guid @event)
        {
            return GetEventAssessmentForms(new QEventAssessmentFormFilter { EventIdentifier = @event });
        }

        public List<QEventAssessmentForm> GetEventAssessmentForms(QEventAssessmentFormFilter filter)
        {
            using (var db = CreateContext())
            {
                return db.EventAssessmentForms
                    .AsNoTracking()
                    .Include(x => x.Form)
                    .Where(x => x.EventIdentifier == filter.EventIdentifier)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        #endregion

        #region Events

        public List<QEvent> GetEventsForReminder(DateTimeOffset start, DateTimeOffset end, bool ignoreScheduleStart)
        {
            using (var db = CreateContext())
            {
                return db.Events.Where(x =>
                        x.SendReminderBeforeDays.HasValue
                        && (x.WhenEventReminderRequestedNotifyInstructorMessageIdentifier.HasValue || x.WhenEventReminderRequestedNotifyLearnerMessageIdentifier.HasValue)
                        && (
                            ignoreScheduleStart
                            || start <= x.EventScheduledStart && DbFunctions.AddDays(start, x.SendReminderBeforeDays.Value) >= x.EventScheduledStart
                            || end <= x.EventScheduledStart && DbFunctions.AddDays(end, x.SendReminderBeforeDays.Value) >= x.EventScheduledStart
                        )
                    )
                    .OrderBy(x => x.OrganizationIdentifier)
                    .ThenBy(x => x.EventScheduledStart)
                    .ToList();
            }
        }

        public List<ApprenticeSummary> GetApprenticeSummary(QEventFilter filter)
        {
            using (var db = CreateContext())
            {
                var classQuery = CreateQuery(filter, db);

                var result = classQuery
                    .Join(db.Registrations.Where(x =>
                            (x.ApprovalStatus == null || x.ApprovalStatus == "Registered")
                            && x.AttendanceStatus != "Withdrawn/Cancelled"
                        ),
                        a => a.EventIdentifier,
                        b => b.EventIdentifier,
                        (a, b) => new { Event = a, Registration = b }
                    )
                    .GroupBy(x => new
                    {
                        x.Event.EventIdentifier,
                        x.Event.EventTitle,
                        x.Event.AchievementIdentifier,
                        x.Event.Achievement.AchievementTitle
                    })
                    .Select(x => new ApprenticeSummary
                    {
                        EventIdentifier = x.Key.EventIdentifier,
                        EventTitle = x.Key.EventTitle,
                        AchievementIdentifier = x.Key.AchievementIdentifier,
                        AchievementTitle = x.Key.AchievementTitle,
                        MemberCount = x.Count(y => y.Registration.EmployerIdentifier != null && y.Registration.Employer.GroupStatus == "Active Member"),
                        NoEmployerCount = x.Count(y => y.Registration.EmployerIdentifier == null),
                        TotalCount = x.Count()
                    })
                    .ToList();

                var noRegistrations = classQuery
                    .Where(x => !x.Registrations.Where(y => y.ApprovalStatus == null || y.ApprovalStatus == "Registered").Any()
                    )
                    .Select(x => new ApprenticeSummary
                    {
                        EventIdentifier = x.EventIdentifier,
                        EventTitle = x.EventTitle,
                        AchievementIdentifier = x.AchievementIdentifier,
                        AchievementTitle = x.Achievement.AchievementTitle,
                        MemberCount = 0,
                        NoEmployerCount = 0,
                        TotalCount = 0
                    })
                    .ToList();

                result.AddRange(noRegistrations);

                return result;
            }
        }

        public List<EventParticipationSummary> GetEventParticipationSummary(QEventFilter filter)
        {
            using (var db = CreateContext())
            {
                var classQuery = CreateQuery(filter, db)
                    .Where(x => x.Achievement.AchievementLabel == "Level");

                var result = classQuery
                    .Join(db.Registrations.Where(
                            x => (x.ApprovalStatus == null || x.ApprovalStatus == "Registered")
                                && x.AttendanceStatus != "Withdrawn/Cancelled"
                        ),
                        a => a.EventIdentifier,
                        b => b.EventIdentifier,
                        (a, b) => new { Event = a, Registration = b }
                    )
                    .GroupBy(x => new
                    {
                        x.Event.EventIdentifier,
                        x.Event.EventScheduledStart,
                        x.Event.EventScheduledEnd,
                        x.Event.EventSchedulingStatus,
                        x.Event.AchievementIdentifier,
                        x.Event.Achievement.AchievementDescription,
                        x.Event.Achievement.AchievementTitle
                    })
                    .Select(x => new EventParticipationSummary
                    {
                        EventIdentifier = x.Key.EventIdentifier,
                        EventScheduledStart = x.Key.EventScheduledStart,
                        EventScheduledEnd = x.Key.EventScheduledEnd,
                        EventSchedulingStatus = x.Key.EventSchedulingStatus,
                        AchievementIdentifier = (Guid)x.Key.AchievementIdentifier,
                        AchievementDescription = x.Key.AchievementDescription,
                        AchievementTitle = x.Key.AchievementTitle,
                        RegistrationCount = x.Count()
                    })
                    .ToList();

                var noRegistrations = classQuery
                    .Where(x =>
                        x.EventSchedulingStatus == "Cancelled"
                        && !x.Registrations.Where(y => y.ApprovalStatus == null || y.ApprovalStatus == "Registered").Any()
                    )
                    .Select(x => new EventParticipationSummary
                    {
                        EventIdentifier = x.EventIdentifier,
                        EventScheduledStart = x.EventScheduledStart,
                        EventScheduledEnd = x.EventScheduledEnd,
                        EventSchedulingStatus = x.EventSchedulingStatus,
                        AchievementIdentifier = (Guid)x.AchievementIdentifier,
                        AchievementDescription = x.Achievement.AchievementDescription,
                        AchievementTitle = x.Achievement.AchievementTitle,
                        RegistrationCount = 0
                    })
                    .ToList();

                result.AddRange(noRegistrations);

                return result;
            }
        }

        public List<RegistrationCertificateSummary> GetRegistrationCertificateSummary(QEventFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Join(db.Registrations.Where(x => x.ApprovalStatus == null || x.ApprovalStatus == "Registered"),
                        a => a.EventIdentifier,
                        b => b.EventIdentifier,
                        (a, b) => new { Event = a, Registration = b }
                    )
                    .Join(db.QEnrollments,
                        a => new { UserIdentifier = a.Registration.CandidateIdentifier, EventIdentifier = (Guid?)a.Event.EventIdentifier },
                        b => new { UserIdentifier = b.LearnerIdentifier, b.Gradebook.EventIdentifier },
                        (a, b) => new { a.Event, a.Registration, Student = b }
                    )
                    .Join(db.QGradeItems.Where(x => x.Achievement.AchievementLabel == "Certificate"),
                        a => a.Student.GradebookIdentifier,
                        b => b.GradebookIdentifier,
                        (a, b) => new
                        {
                            b.Achievement.AchievementIdentifier,
                            b.Achievement.AchievementTitle,
                            a.Event.EventIdentifier,
                            a.Event.EventTitle,
                            VenueLocationName = a.Event.VenueLocation.GroupName
                        }
                    )
                    .GroupBy(x => new
                    {
                        x.AchievementIdentifier,
                        x.AchievementTitle,
                        x.EventIdentifier,
                        x.EventTitle,
                        x.VenueLocationName
                    })
                    .Select(x => new RegistrationCertificateSummary
                    {
                        AchievementIdentifier = x.Key.AchievementIdentifier,
                        AchievementTitle = x.Key.AchievementTitle,
                        EventIdentifier = x.Key.EventIdentifier,
                        EventTitle = x.Key.EventTitle,
                        VenueLocationName = x.Key.VenueLocationName,
                        RegistrationCount = x.Count()
                    })
                    .ToList();
            }
        }

        public int CountEvents(QEventFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<Counter> CountEventsByExamType(QEventFilter filter)
        {
            var list = new List<Counter>();

            var events = GetEvents(filter);

            var types = events.Select(x => x.ExamType).OrderBy(x => x).Distinct();

            foreach (var type in types)
            {
                var item = new Counter
                {
                    Name = type,
                    Value = events.Count(x => x.ExamType == type)
                };
                list.Add(item);
            }

            return list;
        }

        public QEvent GetEvent(Guid organization, string title)
        {
            using (var db = CreateContext())
            {
                return db.Events
                    .Where(x => x.OrganizationIdentifier == organization && x.EventTitle == title)
                    .FirstOrDefault();
            }
        }

        public QEvent GetEvent(Guid id, params Expression<Func<QEvent, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.Events.AsNoTracking()
                    .Where(x => x.EventIdentifier == id)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public List<QEvent> GetEvents(IEnumerable<Guid> eventId, params Expression<Func<QEvent, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.Events
                    .AsNoTracking()
                    .ApplyIncludes(includes);

                return query
                    .Where(x => eventId.Contains(x.EventIdentifier))
                    .ToList();
            }
        }

        public List<QEvent> GetEvents(QEventFilter filter, params Expression<Func<QEvent, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db, includes);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                {
                    query = query
                        .OrderByDescending(x => x.EventScheduledStart)
                        .ThenBy(x => x.EventIdentifier);
                }

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<QEvent> GetRecentEvents(QEventFilter filter, int? take = null)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db)
                    .OrderByDescending(x => x.LastChangeTime)
                    .AsQueryable();

                if (take.HasValue)
                    query = query.Take(take.Value);

                return query.ToList();
            }
        }

        public List<QEvent> GetEventsForCandidate(Guid candidate)
        {
            using (var db = CreateContext())
            {
                return db.Events.AsNoTracking()
                    .Where(x => x.Registrations.Any(y => y.CandidateIdentifier == candidate))
                    .ToList();
            }
        }

        public List<QRegistration> GetRegistrationsForCandidate(Guid candidate)
        {
            using (var db = CreateContext())
            {
                return db.Registrations.AsNoTracking()
                    .Include(x => x.Event)
                    .Where(x => x.CandidateIdentifier == candidate)
                    .ToList();
            }
        }

        public ScheduleProblem GetScheduleProblem(Guid @event, Guid candidate, Guid? form)
        {
            var e = GetEvent(@event);

            var problem = new ScheduleProblem();

            // Find the other events for which this candidate is registered.

            var events = GetEventsForCandidate(candidate);
            foreach (var item in events.Where(x => x.EventIdentifier != @event))
                if (item.EventScheduledStart.Date == e.EventScheduledStart.Date)
                    problem.SameDayEvents.Add(item.EventIdentifier);

            // Find the other registrations for this candidate.

            var registrations = GetRegistrationsForCandidate(candidate);
            foreach (var item in registrations.Where(x => x.EventIdentifier != @event))
            {
                var elapsed = Math.Abs((e.EventScheduledStart.Date - item.Event.EventScheduledStart.Date).TotalDays);
                if (item.ExamFormIdentifier == form && elapsed <= 30)
                    problem.SameFormEvents.Add(item.EventIdentifier);
            }

            return problem;
        }

        public static QEvent[] Search(Expression<Func<QEvent, bool>> filter, params Expression<Func<QEvent, object>>[] includes)
            => QEventReadHelper.Instance.Select(filter, includes);

        private static IQueryable<QEvent> CreateQuery(QEventFilter filter, InternalDbContext db, Expression<Func<QEvent, object>>[] includes = null)
        {
            var query = db.Events
                .ApplyIncludes(includes)
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.AttendeeUserIdentifier.HasValue)
                query = query.Where(x => x.Attendees.Any(y => y.UserIdentifier == filter.AttendeeUserIdentifier.Value));

            if (filter.AchievementIdentifier.HasValue)
                query = query.Where(x => x.AchievementIdentifier == filter.AchievementIdentifier);

            if (filter.RegistrationCandidateIdentifier.HasValue)
                query = query.Where(x => x.Registrations.Any(y => y.CandidateIdentifier == filter.RegistrationCandidateIdentifier));

            if (filter.EventInstructorIdentifier.HasValue)
                query = query.Where(x => x.Attendees.FirstOrDefault(y => y.UserIdentifier == filter.EventInstructorIdentifier && y.AttendeeRole == "Instructor") != null);

            if (filter.VenueLocationIdentifier.IsNotEmpty())
                query = query.Where(x => filter.VenueLocationIdentifier.Contains(x.VenueLocationIdentifier.Value));

            if (filter.PermissionGroupIdentifier.HasValue)
                query = query.Where(x => x.EventGroupPermissions.Any(y => y.GroupIdentifier == filter.PermissionGroupIdentifier));

            if (filter.EventType.IsNotEmpty())
                query = query.Where(x => x.EventType == filter.EventType);

            if (filter.EventClassCode.IsNotEmpty())
                query = query.Where(x => x.EventClassCode.Contains(filter.EventClassCode));

            if (filter.EventScheduledSince.HasValue)
                query = query.Where(x => x.EventScheduledStart >= filter.EventScheduledSince.Value);

            if (filter.EventScheduledBefore.HasValue)
                query = query.Where(x => x.EventScheduledStart < filter.EventScheduledBefore.Value);

            if (filter.EventScheduleEndSince.HasValue)
                query = query.Where(x => x.EventScheduledEnd >= filter.EventScheduleEndSince.Value);

            if (filter.DistributionExpectedSince.HasValue)
                query = query.Where(x => x.DistributionExpected >= filter.DistributionExpectedSince.Value);

            if (filter.DistributionExpectedBefore.HasValue)
                query = query.Where(x => x.DistributionExpected < filter.DistributionExpectedBefore.Value);

            if (filter.AttemptCompletedSince.HasValue)
                query = query.Where(x => x.Registrations.Any(y => y.Attempt.AttemptGraded >= filter.AttemptCompletedSince.Value));

            if (filter.AttemptCompletedBefore.HasValue)
                query = query.Where(x => x.Registrations.Any(y => y.Attempt.AttemptGraded < filter.AttemptCompletedBefore.Value));

            if (filter.RegistrationDeadlineSince.HasValue)
                query = query.Where(x => x.RegistrationDeadline >= filter.RegistrationDeadlineSince.Value);

            if (filter.RegistrationDeadlineBefore.HasValue)
                query = query.Where(x => x.RegistrationDeadline < filter.RegistrationDeadlineBefore.Value);

            if (filter.EventFormat.IsNotEmpty())
                query = query.Where(x => x.EventFormat == filter.EventFormat);

            if (filter.EventTitle.IsNotEmpty())
                query = query.Where(x => x.EventTitle.Contains(filter.EventTitle));

            if (filter.EventDescription.IsNotEmpty())
                query = query.Where(x => x.EventDescription != null && x.EventDescription.Contains(filter.EventDescription));

            if (filter.ExamFormName.IsNotEmpty())
                query = query.Where(x => x.ExamForms.Any(y => y.Form.FormName.Contains(filter.ExamFormName)));

            if (filter.ExamType.HasValue())
                query = query.Where(x => x.ExamType == filter.ExamType);

            if (filter.EventMaterialTrackingStatus.HasValue())
            {
                var status = filter.EventMaterialTrackingStatus;
                if (status == "Fully Returned")
                    query = query.Where(x => x.ExamMaterialReturnShipmentReceived.HasValue && x.ExamMaterialReturnShipmentCondition == "Full");
                if (status == "Partially Returned")
                    query = query.Where(x => x.ExamMaterialReturnShipmentReceived.HasValue && x.ExamMaterialReturnShipmentCondition == "Partial");
                if (status == "Not Returned")
                    query = query.Where(x => !x.ExamMaterialReturnShipmentReceived.HasValue);
            }

            if (filter.EventPublicationStatus.HasValue())
                query = query.Where(x => x.EventPublicationStatus == filter.EventPublicationStatus);

            if (filter.EventRequisitionStatus.IsNotEmpty())
                query = query.Where(x => x.EventRequisitionStatus == filter.EventRequisitionStatus);

            if (filter.IncludeEventSchedulingStatus.HasValue())
                query = query.Where(x => x.EventSchedulingStatus == filter.IncludeEventSchedulingStatus);

            if (filter.ExcludeEventSchedulingStatus.HasValue())
                query = query.Where(x => x.EventSchedulingStatus != filter.ExcludeEventSchedulingStatus);

            if (filter.VenueOffice.IsNotEmpty())
                query = query.Where(x => x.VenueLocation.GroupOffice.Contains(filter.VenueOffice));

            if (filter.Venue.IsNotEmpty())
                query = query.Where(x => x.VenueLocation.GroupName.Contains(filter.Venue) || x.VenueRoom.Contains(filter.Venue));

            if (filter.EventNumber.HasValue)
                query = query.Where(x => x.EventNumber == filter.EventNumber);

            if (filter.EventNumbers.IsNotEmpty())
                query = query.Where(x => filter.EventNumbers.Contains(x.EventNumber));

            if (filter.EventBillingType.IsNotEmpty())
                query = query.Where(x => x.EventBillingType == filter.EventBillingType);

            if (filter.CommentKeyword.IsNotEmpty())
            {
                var events = db.QComments.Where(comment => comment.CommentText.Contains(filter.CommentKeyword)).Select(comment => comment.EventIdentifier);
                query = query.Where(x => events.Any(e => e == x.EventIdentifier));
            }

            query = GetAvailabilityQuery(filter, query);

            if (filter.Keyword.IsNotEmpty())
                query = query.Where(x => x.EventTitle.Contains(filter.Keyword)
                    || x.EventType.Contains(filter.Keyword)
                    || x.EventBillingType.Contains(filter.Keyword)
                    || x.EventNumber.ToString().Contains(filter.Keyword));

            if (filter.IsOpen.HasValue)
            {
                var now = DateTimeOffset.UtcNow;
                var published = PublicationStatus.Published.GetDescription();

                if (filter.IsOpen.Value)
                {
                    query = query.Where(x =>
                        x.EventPublicationStatus == published
                        && (
                            x.RegistrationStart == null || x.RegistrationStart <= now
                        )
                        && (
                            x.RegistrationDeadline != null && x.RegistrationDeadline >= now
                            || x.RegistrationDeadline == null && x.EventScheduledStart >= now
                        )
                    );
                }
                else
                {
                    query = query.Where(x =>
                        x.EventPublicationStatus != published
                        || (
                            x.RegistrationStart != null && x.RegistrationStart > now
                           )
                        ||
                            x.RegistrationDeadline != null && x.RegistrationDeadline < now
                         || x.RegistrationDeadline == null && x.EventScheduledStart < now

                    );
                }
            }

            if (filter.IsResourceAssigned.HasValue)
            {
                if (filter.IsResourceAssigned.Value)
                    query = query.Where(x => x.AchievementIdentifier != null);
                else
                    query = query.Where(x => x.AchievementIdentifier == null);
            }

            if (filter.IsRegistrationLocked.HasValue)
            {
                query = filter.IsRegistrationLocked.Value
                    ? query.Where(x => x.RegistrationLocked != null)
                    : query.Where(x => x.RegistrationLocked == null);
            }

            if (filter.ExcludeEventIdentifier.HasValue)
                query = query.Where(x => x.EventIdentifier != filter.ExcludeEventIdentifier);

            if (filter.WithholdDistribution.HasValue)
                query = query.Where(x => x.IntegrationWithholdDistribution == filter.WithholdDistribution.Value);

            if (filter.UndistributedExamsInclusion == InclusionType.Only)
                query = query.Where(x => !x.DistributionOrdered.HasValue && !x.IntegrationWithholdDistribution);
            else if (filter.UndistributedExamsInclusion == InclusionType.Exclude)
                query = query.Where(x => x.DistributionOrdered.HasValue || x.IntegrationWithholdDistribution);

            if (filter.AppointmentType.HasValue())
                query = query.Where(x => x.AppointmentType == filter.AppointmentType);

            query = ApplyEventClassStatuses(filter.EventClassStatuses, query);

            return query;
        }

        private static IQueryable<QEvent> GetAvailabilityQuery(QEventFilter filter, IQueryable<QEvent> query)
        {
            if (!filter.Availability.HasValue) 
                return query;

            switch (filter.Availability.Value)
            {
                case EventAvailabilityType.Full:
                    return query.Where(e =>
                        e.RegistrationLocked != null || (
                            (e.CapacityMaximum ?? 0) > 0 &&
                            e.Registrations.Count(r =>
                                r.AttendanceStatus != WithdrawnCancelled &&
                                (r.ApprovalStatus == null || ActiveApprovalStatuses.Contains(r.ApprovalStatus))
                            ) == e.CapacityMaximum
                        )
                    );

                case EventAvailabilityType.Under:
                    return query.Where(e =>
                        e.RegistrationLocked == null &&
                        (e.CapacityMinimum ?? 0) > 0 &&
                        e.Registrations.Count(r =>
                            r.AttendanceStatus != WithdrawnCancelled &&
                            (r.ApprovalStatus == null || ActiveApprovalStatuses.Contains(r.ApprovalStatus))
                        ) < e.CapacityMinimum
                    );

                case EventAvailabilityType.Over:
                    return query.Where(e =>
                        e.RegistrationLocked == null &&
                        (e.CapacityMaximum ?? 0) > 0 &&
                        e.Registrations.Count(r =>
                            r.AttendanceStatus != WithdrawnCancelled &&
                            (r.ApprovalStatus == null || ActiveApprovalStatuses.Contains(r.ApprovalStatus))
                        ) > e.CapacityMaximum
                    );

                case EventAvailabilityType.Empty:
                    return query.Where(e =>
                        e.RegistrationLocked == null &&
                        (e.CapacityMinimum ?? 0) == 0 &&
                        e.Registrations.Count(r =>
                            r.AttendanceStatus != WithdrawnCancelled &&
                            (r.ApprovalStatus == null || ActiveApprovalStatuses.Contains(r.ApprovalStatus))
                        ) == 0
                    );

                case EventAvailabilityType.Open:
                    return query.Where(e =>
                        e.RegistrationLocked == null &&
                        ((e.CapacityMinimum ?? 0) == 0 ||
                         e.Registrations.Count(r =>
                             r.AttendanceStatus != WithdrawnCancelled &&
                             (r.ApprovalStatus == null || ActiveApprovalStatuses.Contains(r.ApprovalStatus))
                         ) >= e.CapacityMinimum) &&
                        ((e.CapacityMaximum ?? 0) == 0 ||
                         e.Registrations.Count(r =>
                             r.AttendanceStatus != WithdrawnCancelled &&
                             (r.ApprovalStatus == null || ActiveApprovalStatuses.Contains(r.ApprovalStatus))
                         ) < e.CapacityMaximum) &&
                        e.Registrations.Count(r =>
                            r.AttendanceStatus != WithdrawnCancelled &&
                            (r.ApprovalStatus == null || ActiveApprovalStatuses.Contains(r.ApprovalStatus))
                        ) > 0
                    );

                default:
                    return query;
            }
        }

        private static IQueryable<QEvent> ApplyEventClassStatuses(EventClassStatus[] statuses, IQueryable<QEvent> query)
        {
            if (statuses.IsEmpty())
                return query;

            var now = DateTimeOffset.Now;
            var predicate = PredicateBuilder.False<QEvent>();

            if (statuses.Contains(EventClassStatus.Drafted))
            {
                predicate = predicate.Or(e =>
                    e.EventSchedulingStatus != "Cancelled"
                    && e.EventPublicationStatus != "Published"
                    && e.EventScheduledStart > now
                    && (e.EventScheduledEnd == null || e.EventScheduledEnd > now)
                    && !e.GradebookEvents.Any(g => g.Gradebook.IsLocked)
                );
            }

            if (statuses.Contains(EventClassStatus.Published))
            {
                predicate = predicate.Or(e =>
                    e.EventSchedulingStatus != "Cancelled"
                    && e.EventPublicationStatus == "Published"
                    && e.EventScheduledStart > now
                    && (e.EventScheduledEnd == null || e.EventScheduledEnd > now)
                    && !e.GradebookEvents.Any(g => g.Gradebook.IsLocked)
                );
            }

            if (statuses.Contains(EventClassStatus.InProgress))
            {
                predicate = predicate.Or(e =>
                    e.EventSchedulingStatus != "Cancelled"
                    && e.EventScheduledStart <= now
                    && (e.EventScheduledEnd == null || e.EventScheduledEnd > now)
                    && !e.GradebookEvents.Any(g => g.Gradebook.IsLocked)
                );
            }

            if (statuses.Contains(EventClassStatus.Completed))
            {
                predicate = predicate.Or(e =>
                    e.EventSchedulingStatus != "Cancelled"
                    && e.EventScheduledEnd <= now
                    && !e.GradebookEvents.Any(g => g.Gradebook.IsLocked)
                );
            }

            if (statuses.Contains(EventClassStatus.Closed))
                predicate = predicate.Or(e => e.EventSchedulingStatus != "Cancelled" && e.GradebookEvents.Any(g => g.Gradebook.IsLocked));

            if (statuses.Contains(EventClassStatus.Cancelled))
                predicate = predicate.Or(e => e.EventSchedulingStatus == "Cancelled");

            return query.AsExpandable().Where(predicate);
        }

        private class QEventReadHelper : ReadHelper<QEvent>
        {
            public static readonly QEventReadHelper Instance = new QEventReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QEvent>, TResult> func)
            {
                using (var context = new InternalDbContext(false))
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Events.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Timers

        public int CountTimers(QEventTimerFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public QEventTimer GetTimer(Guid id)
        {
            using (var db = CreateContext())
                return db.EventTimers.FirstOrDefault(x => x.TriggerCommand == id);
        }

        public List<QEventTimer> GetTimers(QEventTimerFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.TriggerTime)
                    .ThenBy(x => x.TriggerCommand)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public List<QEventTimer> GetTimersThatShouldBeElapsed()
        {
            using (var db = CreateContext())
            {
                return db.EventTimers
                    .Where(x => x.TimerStatus == "Started" && x.TriggerTime <= DateTimeOffset.UtcNow)
                    .ToList();
            }
        }

        private IQueryable<QEventTimer> CreateQuery(QEventTimerFilter filter, InternalDbContext db)
        {
            var query = db.EventTimers
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.EventIdentifier.HasValue)
                query = query.Where(x => x.EventIdentifier == filter.EventIdentifier);

            if (filter.TimerDescription.HasValue())
                query = query.Where(x => x.TimerDescription.Contains(filter.TimerDescription));

            if (!string.IsNullOrWhiteSpace(filter.TimerStatus))
                query = query.Where(x => x.TimerStatus == filter.TimerStatus);

            if (filter.TriggerTimeSince.HasValue)
                query = query.Where(x => x.TriggerTime >= filter.TriggerTimeSince.Value);

            return query;
        }

        #endregion

        #region Seats

        public QSeat GetSeat(Guid seatIdentifier)
        {
            using (var db = new InternalDbContext(false))
                return db.Seats.Include(x => x.Event).FirstOrDefault(x => x.SeatIdentifier == seatIdentifier);
        }

        public List<QSeat> GetSeats(Guid eventIdentifier, bool showHiddenSeats = true)
        {
            using (var db = new InternalDbContext(false))
            {
                if (showHiddenSeats)
                    return db.Seats
                        .Where(x => x.EventIdentifier == eventIdentifier)
                        .OrderBy(x => x.OrderSequence)
                        .ThenBy(x => x.SeatTitle)
                        .ToList();
                else
                    return db.Seats
                    .Where(x => x.EventIdentifier == eventIdentifier)
                    .Where(x => x.IsAvailable == true)
                    .OrderBy(x => x.OrderSequence)
                    .ThenBy(x => x.SeatTitle)
                    .ToList();
            }
        }

        public int CountSeats(QSeatFilter filter)
        {
            using (var db = new InternalDbContext(false))
                return CreateQuery(filter, db).Count();
        }

        public List<QSeat> GetSeats(QSeatFilter filter)
        {
            using (var db = new InternalDbContext(false))
            {
                return CreateQuery(filter, db)
                    .Include(x => x.Event)
                    .Include(x => x.Event.Achievement)
                    .OrderBy(x => x.Event.EventTitle)
                    .ThenBy(x => x.OrderSequence)
                    .ThenBy(x => x.SeatTitle)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<QSeat> CreateQuery(QSeatFilter filter, InternalDbContext db)
        {
            var query = db.Seats
                .AsNoTracking()
                .AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Event.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.EventScheduledSince.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart >= filter.EventScheduledSince.Value);

            if (filter.EventScheduledBefore.HasValue)
                query = query.Where(x => x.Event.EventScheduledStart < filter.EventScheduledBefore.Value);

            if (filter.AchievementIdentifier.HasValue)
                query = query.Where(x => x.Event.AchievementIdentifier == filter.AchievementIdentifier);

            if (filter.EventTitle.HasValue())
                query = query.Where(x => x.Event.EventTitle.Contains(filter.EventTitle));

            if (filter.SeatTitle.HasValue())
                query = query.Where(x => x.SeatTitle.Contains(filter.SeatTitle));

            if (filter.IsAvailable.HasValue)
                query = query.Where(x => x.IsAvailable == filter.IsAvailable);

            if (filter.IsTaxable.HasValue)
                query = query.Where(x => x.IsTaxable == filter.IsTaxable);

            if (filter.EventIdentifier.HasValue)
                query = query.Where(x => x.EventIdentifier == filter.EventIdentifier);

            return query;
        }

        #endregion

        public QExamSessionsScheduled GetExamSessionsScheduled(Guid organizationIdentifier)
        {
            var result = new QExamSessionsScheduled();

            using (var db = CreateContext())
            {
                var now = DateTimeOffset.UtcNow;

                var organizationEvents = db.Events.Where(x => x.OrganizationIdentifier == organizationIdentifier);

                result.Classes = organizationEvents.Count(x => x.EventType == "Class");
                result.Sittings = organizationEvents.Count(x => x.EventType == "Sitting");
                result.Individuals = organizationEvents.Count(x => x.ExamType.StartsWith("Individual"));
                result.Online = organizationEvents.Count(x => x.EventFormat == EventExamFormat.Online.Value);

                var startMonthDate = new DateTime(now.Year, now.Month, 1);
                var endMonthDate = startMonthDate.AddMonths(1);

                result.Month = organizationEvents.Count(x => x.EventType == "Exam" &&
                    startMonthDate <= x.EventScheduledStart && x.EventScheduledStart <= endMonthDate);

                var startYearDate = new DateTime(now.Year, 1, 1);
                var endYearDate = startYearDate.AddYears(1);

                result.Year = organizationEvents.Count(x => x.EventType == "Exam" &&
                    startYearDate <= x.EventScheduledStart && x.EventScheduledStart <= endYearDate);
            }

            using (var db = new InternalDbContext(false))
            {
                result.Accommodated = db.Accommodations.Count(x => x.Registration.Event.OrganizationIdentifier == organizationIdentifier);
            }

            return result;
        }

        public QExamsWrittenByType GetExamsWrittenByType(Guid organizationIdentifier)
        {
            var result = new QExamsWrittenByType();

            using (var db = new InternalDbContext(false))
            {
                var organizationRegistrations = db.Registrations.Where(x => x.Event.OrganizationIdentifier == organizationIdentifier && x.Event.EventType == "Exam");

                result.CofQ = organizationRegistrations.Count(x => x.Form.Bank.BankLevel.StartsWith("CofQ"));
                result.IPSE = organizationRegistrations.Count(x => x.Form.Bank.BankLevel.StartsWith("IPSE"));

                result.Classes = organizationRegistrations.Count(x => x.Event.ExamType == EventExamType.Class.Value);
                result.SittingsIndividuals = organizationRegistrations.Count(x => x.Event.EventType == "Sitting" || x.Event.ExamType.StartsWith("Individual"));
            }

            return result;
        }
    }
}

