using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Domain.Events;

using Shift.Common;

namespace InSite.Application.Events.Read
{
    public interface IEventSearch
    {
        List<QEvent> GetEventsForReminder(DateTimeOffset start, DateTimeOffset end, bool ignoreScheduleStart);

        List<ApprenticeSummary> GetApprenticeSummary(QEventFilter filter);
        List<EventParticipationSummary> GetEventParticipationSummary(QEventFilter filter);
        List<RegistrationCertificateSummary> GetRegistrationCertificateSummary(QEventFilter filter);

        int CountEvents(QEventFilter filter);

        List<Counter> CountEventsByExamType(QEventFilter filter);

        QEvent GetEvent(Guid organization, string title);
        QEvent GetEvent(Guid id, params Expression<Func<QEvent, object>>[] includes);
        List<QEvent> GetEvents(IEnumerable<Guid> eventId, params Expression<Func<QEvent, object>>[] includes);
        List<QEvent> GetEvents(QEventFilter filter, params Expression<Func<QEvent, object>>[] includes);
        List<QEvent> GetRecentEvents(QEventFilter filter, int? take = null);

        ScheduleProblem GetScheduleProblem(Guid @event, Guid candidate, Guid? form);

        int CountComments(QEventCommentFilter filter);
        QComment GetComment(Guid comment);
        List<QComment> GetComments(QEventCommentFilter filter);

        List<VPerson> GetAttendeeUsers(Guid organizationIdentifier, string role);

        int CountAttendees(QEventAttendeeFilter filter);
        QEventAttendee GetAttendee(Guid @event, Guid contact, params Expression<Func<QEventAttendee, object>>[] includes);

        List<QEventAttendee> GetAttendees(Guid @event, params Expression<Func<QEventAttendee, object>>[] includes);
        List<QEventAttendee> GetAttendees(QEventAttendeeFilter filter, params Expression<Func<QEventAttendee, object>>[] includes);

        int CountEventAssessmentForms(QEventAssessmentFormFilter filter);
        List<QEventAssessmentForm> GetEventAssessmentForms(Guid @event);
        List<QEventAssessmentForm> GetEventAssessmentForms(QEventAssessmentFormFilter filter);

        int CountTimers(QEventTimerFilter filter);
        QEventTimer GetTimer(Guid timer);
        List<QEventTimer> GetTimers(QEventTimerFilter filter);
        List<QEventTimer> GetTimersThatShouldBeElapsed();

        QSeat GetSeat(Guid seatIdentifier);
        List<QSeat> GetSeats(Guid eventIdentifier, bool showHiddenSeats = true);
        int CountSeats(QSeatFilter filter);
        List<QSeat> GetSeats(QSeatFilter filter);

        QExamSessionsScheduled GetExamSessionsScheduled(Guid organizationIdentifier);
        QExamsWrittenByType GetExamsWrittenByType(Guid tennatIdentifier);
    }
}