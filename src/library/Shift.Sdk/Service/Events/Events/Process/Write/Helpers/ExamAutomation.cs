using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Application.Organizations.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ExamAutomation
    {
        private const string AbsentAttendanceStatus = "Absent";
        private const string CancelledAttendanceStatus = "Withdrawn/Cancelled";
        private static readonly string[] ExpectedGradingStatuses = { null, "Unassigned", "Assigned", "Released" };

        private readonly IEventSearch _eventSearch;
        private readonly IOrganizationSearch _organizationSearch;
        private readonly IIdentityService _identityService;
        private readonly Action<ICommand> _sendCommand;
        private readonly Action<Exception> _sendError;

        public ExamAutomation(
            IEventSearch eventSearch,
            IOrganizationSearch organizationSearch,
            IIdentityService identityService,
            Action<ICommand> sendCommand,
            Action<Exception> sendError)
        {
            _eventSearch = eventSearch;
            _organizationSearch = organizationSearch;
            _identityService = identityService;
            _sendCommand = sendCommand;
            _sendError = sendError;
        }

        public int ValidateAndPublishGrades()
        {
            var now = DateTimeOffset.Now;
            var organization = _organizationSearch.GetModel(_identityService.GetCurrentOrganization());
            var eventSettings = organization.Toolkits.Events;

            var events = _eventSearch.GetExamEventsToValidateAndPublish(
                now,
                eventSettings.OnlineEventAutomationWindowHours,
                eventSettings.PaperEventAutomationWindowMonths,
                x => x.Registrations);

            foreach (var @event in events)
            {
                try
                {
                    if (!@event.IntegrationWithholdGrades)
                        ProcessEvent(@event);
                }
                catch (Exception ex)
                {
                    _sendError?.Invoke(new InvalidOperationException(
                        $"ExamAutomation failed to process Event {@event.EventNumber} ({@event.EventIdentifier})", ex));
                }
            }

            return events.Count;
        }

        private void ProcessEvent(QEvent @event)
        {
            var registrations = @event.Registrations
                .Where(x => x.AttendanceStatus != CancelledAttendanceStatus
                         && ExpectedGradingStatuses.Contains(x.GradingStatus))
                .ToArray();

            if (registrations.Length == 0)
                return;

            var hasGradeOrAbsent = registrations
                .Any(x => x.Grade.IsNotEmpty() || x.AttendanceStatus == AbsentAttendanceStatus);

            if (@event.EventFormat == EventExamFormat.Paper.Value)
            {
                if (hasGradeOrAbsent)
                    PublishRegistrations(@event.EventIdentifier, registrations);
            }
            else if (@event.EventFormat == EventExamFormat.Online.Value)
            {
                var unassigned = registrations
                    .Where(r => r.Grade.IsEmpty() && r.AttendanceStatus.IsEmpty())
                    .ToArray();

                if (!hasGradeOrAbsent && unassigned.IsEmpty())
                    return;

                foreach (var registration in unassigned)
                    _sendCommand(new TakeAttendance(registration.RegistrationIdentifier, AbsentAttendanceStatus, null, null));

                PublishRegistrations(@event.EventIdentifier, registrations);
            }
        }

        private void PublishRegistrations(Guid eventId, IEnumerable<QRegistration> registrations)
        {
            var regIds = registrations.Select(x => x.RegistrationIdentifier).ToArray();
            _sendCommand(new ValidateEventScores(eventId, regIds));
            _sendCommand(new PublishEventScores(eventId, regIds, true));
        }
    }
}
