using System;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Application.Organizations.Read;
using InSite.Application.Registrations.Read;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ExamAutomation
    {
        private readonly IEventSearch _eventSearch;
        private readonly IRegistrationSearch _registrationSearch;
        private readonly IOrganizationSearch _organizationSearch;
        private readonly IIdentityService _identityService;
        private readonly Action<ICommand> _sendCommand;

        public ExamAutomation(
            IEventSearch eventSearch,
            IRegistrationSearch registrationSearch,
            IOrganizationSearch organizationSearch,
            IIdentityService identityService,
            Action<ICommand> sendCommand)
        {
            _eventSearch = eventSearch;
            _registrationSearch = registrationSearch;
            _organizationSearch = organizationSearch;
            _identityService = identityService;
            _sendCommand = sendCommand;
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
                x => x.Registrations.Select(y => y.Attempt));

            foreach (var @event in events)
            {
                if (@event.Registrations.IsEmpty() || @event.Registrations.All(x => x.Attempt?.AttemptGraded == null))
                    continue;

                var regIds = @event.Registrations.Select(x => x.RegistrationIdentifier).ToArray();
                _sendCommand(new ValidateEventScores(@event.EventIdentifier, regIds));
                _sendCommand(new PublishEventScores(@event.EventIdentifier, regIds, true));
            }

            return events.Count;
        }
    }
}
