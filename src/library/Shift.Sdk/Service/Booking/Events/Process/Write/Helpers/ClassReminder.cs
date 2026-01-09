using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Events;
using InSite.Domain.Messages;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Events.Write
{
    public class ClassReminder
    {
        const int AllowanceInHours = 12;

        private readonly IEventSearch _eventSearch;
        private readonly IRegistrationSearch _registrationSearch;
        private readonly IRecordSearch _recordSearch;
        private readonly IGroupSearch _groupSearch;
        private readonly ICommander _commander;
        private readonly IAlertMailer _alertMailer;
        private readonly Func<Guid, OrganizationState> _getOrganization;
        private readonly AppSettings _appSettings;

        public ClassReminder(
            IEventSearch eventSearch,
            IRegistrationSearch registrationSearch,
            IRecordSearch recordSearch,
            IGroupSearch groupSearch,
            ICommander commander,
            IAlertMailer alertMailer,
            Func<Guid, OrganizationState> getOrganization,
            AppSettings appSettings
        )
        {
            _eventSearch = eventSearch;
            _registrationSearch = registrationSearch;
            _recordSearch = recordSearch;
            _groupSearch = groupSearch;
            _commander = commander;
            _alertMailer = alertMailer;
            _getOrganization = getOrganization;
            _appSettings = appSettings;
        }

        public int CreateNotifications(Guid? eventIdentifier, bool ignoreScheduleStart)
        {
            var events = GetNotProceededEvents(eventIdentifier, ignoreScheduleStart);

            int count = 0;

            foreach (var @event in events)
            {
                var organization = _getOrganization(@event.OrganizationIdentifier);
                var venue = @event.VenueLocationIdentifier.HasValue ? _groupSearch.GetGroup(@event.VenueLocationIdentifier.Value) : null;
                var venueAddress = venue != null ? _groupSearch.GetAddress(venue.GroupIdentifier, AddressType.Physical) : null;

                count += ProceedEventForLearners(@event, organization, venue, venueAddress);
                count += ProceedEventForInstructors(@event, organization, venue, venueAddress);
            }

            return count;
        }

        private List<QEvent> GetNotProceededEvents(Guid? eventIdentifier, bool ignoreScheduleStart)
        {
            var events = eventIdentifier.HasValue && ignoreScheduleStart
                ? new List<QEvent> { _eventSearch.GetEvent(eventIdentifier.Value) }
                : _eventSearch.GetEventsForReminder(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(AllowanceInHours), ignoreScheduleStart);

            if (eventIdentifier.HasValue && !ignoreScheduleStart)
                events = events.FindAll(x => x.EventIdentifier == eventIdentifier);

            if (ignoreScheduleStart)
                return events;

            return events
                .Where(x =>
                    x.ReminderMessageSent == null
                    || x.ReminderMessageSent <= x.EventScheduledStart.AddDays(-x.SendReminderBeforeDays.Value).AddHours(-AllowanceInHours)
                )
                .ToList();
        }

        private int ProceedEventForLearners(QEvent @event, OrganizationState organization, QGroup venue, QGroupAddress venueAddress)
        {
            if (@event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier == null)
                return 0;

            var registrations = _registrationSearch.GetRegistrations(new QRegistrationFilter
            {
                EventIdentifier = @event.EventIdentifier,
                ApprovalStatus = "Registered"
            }, x => x.Candidate);

            var classUrl = UrlHelper.GetAbsoluteUrl(
                _appSettings.Security.Domain,
                _appSettings.Environment,
                $"/ui/portal/events/classes/outline?event={@event.EventIdentifier}",
                organization.OrganizationCode
            );

            var notifications = new List<(Guid, Notification)>();

            foreach (var registration in registrations)
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(registration.Candidate.UserTimeZone);

                var notification = new ClassReminderLearnerNotification
                {
                    OriginOrganization = @event.OrganizationIdentifier,
                    MessageIdentifier = @event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier.Value,
                    EventTitle = @event.EventTitle,
                    EventDate = TimeZones.FormatDateOnly(@event.EventScheduledStart, timeZone),
                    EventTime = TimeZones.FormatTimeOnly(@event.EventScheduledStart, timeZone),
                    VenueName = venue?.GroupName ?? "",
                    VenueStreet1 = venueAddress?.Street1 ?? "",
                    VenueCity = venueAddress?.City ?? "",
                    VenueProvince = venueAddress?.Province ?? "",
                    VenuePostalCode = venueAddress?.PostalCode ?? "",
                    ClassURL = classUrl
                };

                notifications.Add((registration.CandidateIdentifier, notification));
            }

            SendNotifications(@event.EventIdentifier, EventMessageType.ReminderLearner, @event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier.Value, notifications);

            return notifications.Count;
        }

        private int ProceedEventForInstructors(QEvent @event, OrganizationState organization, QGroup venue, QGroupAddress venueAddress)
        {
            if (@event.WhenEventReminderRequestedNotifyInstructorMessageIdentifier == null)
                return 0;

            var instructors = _eventSearch.GetAttendees(new QEventAttendeeFilter
            {
                EventIdentifier = @event.EventIdentifier,
                ContactRole = "Instructor"
            }, x => x.Person.User);

            var gradebook = _recordSearch.GetGradebooks(new QGradebookFilter
            {
                GradebookEventIdentifier = @event.EventIdentifier
            }).FirstOrDefault();

            var gradebookUrl = UrlHelper.GetAbsoluteUrl(
                _appSettings.Security.Domain,
                _appSettings.Environment,
                $"/ui/admin/records/gradebooks/instructors/gradebook-outline?id={gradebook?.GradebookIdentifier}",
                organization.OrganizationCode
            );

            var notifications = new List<(Guid, Notification)>();

            foreach (var instructor in instructors)
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(instructor.Person.User.UserTimeZone);

                var notification = new ClassReminderInstructorNotification
                {
                    OriginOrganization = @event.OrganizationIdentifier,
                    MessageIdentifier = @event.WhenEventReminderRequestedNotifyInstructorMessageIdentifier.Value,
                    EventTitle = @event.EventTitle,
                    EventDate = TimeZones.FormatDateOnly(@event.EventScheduledStart, timeZone),
                    EventTime = TimeZones.FormatTimeOnly(@event.EventScheduledStart, timeZone),
                    VenueName = venue?.GroupName ?? "",
                    VenueStreet1 = venueAddress?.Street1 ?? "",
                    VenueCity = venueAddress?.City ?? "",
                    VenueProvince = venueAddress?.Province ?? "",
                    VenuePostalCode = venueAddress?.PostalCode ?? "",
                    GradebookURL = gradebookUrl
                };

                notifications.Add((instructor.UserIdentifier, notification));
            }

            SendNotifications(@event.EventIdentifier, EventMessageType.ReminderLearner, @event.WhenEventReminderRequestedNotifyLearnerMessageIdentifier.Value, notifications);

            return notifications.Count;
        }

        private void SendNotifications(Guid eventId, EventMessageType messageType, Guid messageId, List<(Guid, Notification)> notifications)
        {
            if (notifications.Count == 0)
                return;

            var recipients = new List<Guid>();

            foreach (var (userId, notification) in notifications)
            {
                _alertMailer.Send(notification, userId);
                recipients.Add(userId);
            }

            _commander.Send(new SendEventMessage(eventId, messageType, messageId, recipients.ToArray()));
        }
    }
}
