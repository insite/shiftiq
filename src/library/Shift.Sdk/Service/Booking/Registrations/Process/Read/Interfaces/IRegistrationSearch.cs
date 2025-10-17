using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Domain.Registrations;

using Shift.Common;

namespace InSite.Application.Registrations.Read
{
    public interface IRegistrationSearch
    {
        // VAttendance

        int CountAttendances(VAttendanceFilter filter);
        List<VAttendance> GetAttendances(VAttendanceFilter filter);

        // Registrations

        int? GetMaxSequence(Guid eventIdentifier);

        List<QAccommodation> GetAccommodations(Guid registrationIdentifier);
        List<QAccommodation> GetAccommodations(IEnumerable<Guid> registrationIdentifiers);
        List<string> GetAccommodationTypes(Guid organizationIdentifier);

        List<ApprenticeCompletionRateReportItem> GetApprenticeCompletionRateReport(QRegistrationFilter filter);
        List<ApprenticeScoresReportItem> GetApprenticeScoresReport(QRegistrationFilter filter);

        List<string> GetApprovalStatuses(Guid organizationIdentifier);

        int CountRegistrations(QRegistrationFilter filter);

        List<VPerson> GetInstructors(Guid registrationIdentifier);

        QRegistration GetRegistration(Guid registration, params Expression<Func<QRegistration, object>>[] includes);
        QRegistration GetRegistration(QRegistrationFilter filter, params Expression<Func<QRegistration, object>>[] includes);

        List<QRegistration> GetRegistrations(QRegistrationFilter filter, params Expression<Func<QRegistration, object>>[] includes);

        Guid? GetRegistrationIdentifier(QRegistrationFilter filter);
        List<Guid> GetRegistrationIdentifiers(QRegistrationFilter filter);
        List<Guid> GetRegistrationCandidateIdentifiers(QRegistrationFilter filter);

        List<AttendeeListReportDataItem> GetRegistrationsForAttendeeListReport(QRegistrationFilter filter);

        Dictionary<Guid, List<QRegistration>> GetRegistrationsByEvents(List<Guid> events);

        List<QRegistration> GetRegistrationsByEvent(Guid @event,
            string filterText = null, Paging paging = null, string orderBy = null,
            bool includeAccommodations = false, bool includeAttempt = false, bool includeCandidate = false, bool includeForm = false, bool includeInstructors = false);

        List<QRegistration> GetRegistrationsByCandidate(Guid candidate, params Expression<Func<QRegistration, object>>[] includes);

        List<QRegistration> GetRegistrationsWithoutEvent();

        void Refresh(List<QRegistration> registrations);

        // Timers

        int CountTimers(QRegistrationTimerFilter filter);

        XRegistrationTimer GetTimer(Guid timer);

        List<XRegistrationTimer> GetTimers(QRegistrationTimerFilter filter);

        List<XRegistrationTimer> GetTimersThatShouldBeElapsed();

        List<RegistrationLearnerTypeModel> GetLearnerTypes(List<Guid> registrations);
    }
}