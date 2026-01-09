using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Read;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Messages.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Events;
using InSite.Domain.Messages;
using InSite.Persistence.Integration.DirectAccess;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    public partial class EventProcessor
    {
        private void Approved(IChange e, QEvent @event)
        {
            // There are no alerts for Test exam events.
            if (@event.ExamType == EventExamType.Test.Value)
                return;

            var id = @event.EventIdentifier;
            var registrations = _registrations.GetRegistrationsByEvent(id, null, null, null, true).ToArray();

            var now = DateTimeOffset.UtcNow;
            var start = @event.EventScheduledStart;
            var twoBusinessDaysPrior = Calendar.AddBusinessDays(start, -2);
            var fiveBusinessDaysPrior = Calendar.AddBusinessDays(start, -5);

            if (@event.ExamType == EventExamType.Sitting.Value)
            {
                _broker.Send(e, new TriggerEventNotification(id, "ITA023"));
                StartEventAlertTimer(e, id, NotificationType.ITA023, fiveBusinessDaysPrior, $"Five (5) business days prior, send ITA023", fiveBusinessDaysPrior > now);
            }
            else
                _broker.Send(e, new TriggerEventNotification(id, "ITA001"));

            if (@event.EventFormat == EventExamFormat.Online.Value)
                StartEventAlertTimer(e, id, NotificationType.ITA003, twoBusinessDaysPrior, $"Two (2) business days prior, send ITA003", twoBusinessDaysPrior > now);

            if (@event.ExamType != EventExamType.Arc.Value)
            {
                var isARC = _helper.IsVenueARC(@event.VenueLocationIdentifier);

                foreach (var registration in registrations)
                {
                    _broker.Send(e, new ChangeApproval(registration.RegistrationIdentifier, registration.ApprovalStatus, registration.ApprovalReason, CommandBroker.CreateProcess("Force Execution"), registration.ApprovalStatus));

                    if (isARC)
                        _broker.Send(e, new TriggerNotification(registration.RegistrationIdentifier, NotificationType.ITA025.ToString()));
                }
            }
        }

        private void Comment(IChange e, string text)
        {
            _broker.Send(e, new PostComment(e.AggregateIdentifier, UniqueIdentifier.Create(), e.OriginUser, text));
        }

        private void Completed(IChange e, Guid @event)
        {
            var state = GetEventState(@event);

            // There is no automated publication to DA for Test exam events.
            if (state.Exam.Type == EventExamType.Test.Value)
                return;

            var fourHoursFromNow = DateTimeOffset.UtcNow.AddHours(4);

            // Create commands to validate and publish the exam results.

            var validate = new ValidateEventScores(@event, null);
            _broker.Bookmark(e, validate, fourHoursFromNow);
            _broker.Send(e, new StartEventTimer(@event, validate.CommandIdentifier, fourHoursFromNow, "Wait 4 hours, then validate exam results."));

            var publish = new PublishEventScores(@event, null, false);
            _broker.Bookmark(e, publish, fourHoursFromNow);
            _broker.Send(e, new StartEventTimer(@event, publish.CommandIdentifier, fourHoursFromNow, "Wait 4 hours, then publish exam results."));
        }

        private void Cancelled(IChange e, Guid @event)
        {
            _broker.Send(e, new ConfigureIntegration(@event, true, true));
        }

        private bool EnableIntegrationWithDirectAccess(Guid id, bool grades)
        {
            var query = GetEventQuery(id);
            if (query == null)
                return false;

            var state = GetEventState(id);
            return state != null
                && state.Exam.Type != EventExamType.Test.Value
                && !(grades && state.Integration.WithholdGrades);
        }

        private void ExecuteMailout(IChange e, Guid eventIdentifier, Notification alert)
        {
            try
            {
                alert.OriginOrganization = e.OriginOrganization;

                if (alert.OriginUser == null || alert.OriginUser == Guid.Empty)
                    alert.OriginUser = e.OriginUser;

                var @event = _events.GetEvent(eventIdentifier, x => x.VenueLocation, x => x.VenueOffice, x => x.VenueCoordinator, x => x.Attendees, x => x.Attendees.Select(y => y.Person));
                var venueAddress = @event.VenueLocationIdentifier.HasValue ? _groups.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical) : null;
                var registrations = _registrations.GetRegistrationsByEvent(@event.EventIdentifier, null, null, null, true, true, true, true).ToArray();
                var attempts = _attempts.GetAttemptsByEvent(@event.EventIdentifier, null, null, true).ToArray();

                var forms = new List<QBankForm>();
                var formIds = new HashSet<Guid>();
                foreach (var registration in registrations)
                {
                    if (registration.Form != null && !formIds.Contains(registration.ExamFormIdentifier.Value))
                    {
                        formIds.Add(registration.ExamFormIdentifier.Value);
                        forms.Add(registration.Form);
                    }
                }

                if (alert.Type == NotificationType.ITA001 || alert.Type == NotificationType.ITA003 || alert.Type == NotificationType.ITA023)
                {
                    var agent = new MessageBuilder(_contacts, _groups, _filePaths, _domain);
                    var email = agent.BuildEventEmail(alert, @event, venueAddress, null, registrations, attempts, null);

                    if (!AbortNotification(e, alert, email))
                    {
                        try
                        {
                            if (alert.Type == NotificationType.ITA003)
                            {
                                var path = GetRequestResult("ita", $"/api/events/scrap-paper?id={@event.EventIdentifier}");
                                if (path.IsNotEmpty() && !string.Equals(path, "OK", StringComparison.OrdinalIgnoreCase) && File.Exists(path))
                                    email.AttachFile(path);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Report this exception to Sentry. The mailout can proceed with or without an attached PDF.
                            _error?.Invoke(ex);
                        }

                        if (alert.Courier == "Mailgun")
                        {
                            _mailgun.Send(PrepareEmail(email, alert.Courier));
                        }
                        else
                        {
                            _broker.Send(e, new ScheduleMailout(
                                email.MessageIdentifier.Value,
                                email.MailoutIdentifier == Guid.Empty ? UniqueIdentifier.Create() : email.MailoutIdentifier,
                                email.SenderIdentifier,
                                email.MailoutScheduled.Value,
                                email.Recipients.ToArray(),
                                email.ContentSubject,
                                email.ContentBody,
                                email.ContentVariables,
                                email.ContentAttachments,
                                @event.EventIdentifier));
                        }
                    }
                }
                else
                {
                    foreach (var form in forms)
                    {
                        var formRegistrations = form != null
                            ? registrations
                                .Where(x => x.ExamFormIdentifier == form.FormIdentifier)
                                .OrderBy(x => x.Candidate.UserFullName)
                                .ToArray()
                            : registrations;

                        var agent = new MessageBuilder(_contacts, _groups, _filePaths, _domain);
                        var email = agent.BuildEventEmail(alert, @event, venueAddress, form, formRegistrations, attempts, null);

                        if (email != null && email.Recipients.Count > 0)
                        {
                            if (alert.Courier == "Mailgun")
                            {
                                _mailgun.Send(PrepareEmail(email, alert.Courier));
                            }
                            else
                            {
                                _broker.Send(e, new ScheduleMailout(
                                email.MessageIdentifier.Value,
                                email.MailoutIdentifier == Guid.Empty ? UniqueIdentifier.Create() : email.MailoutIdentifier,
                                email.SenderIdentifier,
                                email.MailoutScheduled.Value,
                                email.Recipients.ToArray(),
                                email.ContentSubject,
                                email.ContentBody,
                                email.ContentVariables,
                                email.ContentAttachments,
                                @event.EventIdentifier));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Because the Timeline engine executes projectors and processors synchronously, we cannot allow a failed mailout to prevent other
                // projectors and processors from executing. Therefore, if an exception occurs in this method, we invoke the error function (which
                // is expected to send a message to Sentry) and allow the Timeline engine to continue the flow of normal execution.
                _error?.Invoke(ex);
            }
        }

        private EmailDraft PrepareEmail(EmailDraft email, string courier)
        {
            var sender = TSenderSearch.Select(email.SenderIdentifier);

            email.SenderType = courier;
            email.SenderEnabled = true;

            if (sender != null)
            {
                email.SenderEmail = sender.SenderEmail;
                email.SystemMailbox = sender.SystemMailbox;
            }

            foreach (var recipient in email.Recipients)
            {
                if (recipient.Identifier == null)
                    throw new Exception($"The identifier for this recipient ({recipient.Address}) cannot be null.");

                email.RecipientListTo.Add(recipient.Identifier.Value, recipient.Address);
            }

            return email;
        }

        private bool AbortNotification(IChange e, Notification notification, EmailDraft draft)
        {
            if (draft == null)
            {
                Comment(e, $"Notification {notification.Type} will not be sent because the email draft is null.");
                return true;
            }

            if (draft.IsDisabled)
            {
                Comment(e, $"Notification {notification.Type} will not be sent because the message is disabled.");
                return true;
            }

            if (draft.Recipients.Count <= 0)
            {
                Comment(e, $"Notification {notification.Type} will not be sent because the email draft has no recipients.");
                return true;
            }

            return false;
        }

        private string GetRequestResult(string organizationCode, string relativeUrl)
        {
            var absoluteUrl = _urls.GetApplicationUrl(organizationCode) + relativeUrl;
            var response = Shift.Common.TaskRunner.RunSync(StaticHttpClient.Client.GetAsync, absoluteUrl);

            return Shift.Common.TaskRunner.RunSync(response.Content.ReadAsStringAsync);
        }

        private QEvent GetEventQuery(Guid id)
        {
            return _events.GetEvent(id);
        }

        private EventState GetEventState(Guid id)
        {
            return _changes.Get<EventAggregate>(id).Data;
        }

        private List<QRegistration> GetRegistrations(Guid @event, Guid[] registrations)
        {
            if (registrations.IsEmpty())
                return _registrations
                    .GetRegistrationsByEvent(@event, null, null, null, false, true, true)
                    .Where(x => x.IsPresent)
                    .ToList();

            return _registrations
                    .GetRegistrations(
                        new QRegistrationFilter { RegistrationIdentifiers = registrations },
                        x => x.Attempt,
                        x => x.Candidate,
                        x => x.Form,
                        x => x.RegistrationInstructors)
                    .Where(x => x.IsPresent)
                    .ToList();
        }

        private void UpdateEventInDirectAccess(EventPublicationStarted e, out string status, out string errors)
        {
            var source = typeof(RegistrationProcessor).FullName + "." + nameof(UpdateEventInDirectAccess);

            QEvent @event = _events.GetEvent(e.AggregateIdentifier, x => x.VenueLocation);
            var venueAddress = @event.VenueLocationIdentifier.HasValue ? _groups.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical) : null;

            var eventStatus = "New Request";
            if (@event.EventSchedulingStatus != null && (@event.EventSchedulingStatus == "Ready to Schedule" || @event.EventSchedulingStatus.StartsWith("Approved")))
                eventStatus = "Approved";

            var trainingProviders = _builder.GetContactCodes(_events.GetAttendees(e.AggregateIdentifier, x => x.Person), "Training Provider");

            var input = new ExamEventInput
            {
                EventNumber = @event.EventNumber,
                CivicAddress = new[] { venueAddress?.Street1, venueAddress?.City },
                DeliveryMethod = @event.EventFormat,
                End = string.Format("{0:yyyy-MM-dd}T{0:HH:mm}:00.000Z", @event.EventScheduledStart.AddMinutes(@event.ExamDurationInMinutes ?? 0)),
                EventStatus = eventStatus,
                EventType = @event.ExamType,
                Location = @event.VenueLocation.GroupName,

                Registration = string.Format("{0:yyyy-MM-dd}T{0:HH:mm}:00.000Z", @event.EventScheduledStart.AddMinutes(-30)),
                Room = @event.VenueRoom,
                ScheduledBy = 0,
                Start = string.Format("{0:yyyy-MM-dd}T{0:HH:mm}:00.000Z", @event.EventScheduledStart),
                TPContacts = trainingProviders,
                TZ = @event.EventScheduledStart.GetTimeZone().GetAbbreviation().GetAbbreviation(@event.EventScheduledStart)
            };

            if (int.TryParse(@event.VenueLocation.GroupCode, out int organizationId))
                input.OrganizationId = organizationId;

            try
            {
                var output = _da.ExamEvent(e.OriginUser, @event.EventNumber, input);
                status = string.Equals(output.PublicationStatus, "Succeeded", StringComparison.OrdinalIgnoreCase)
                    ? PublicationStatus.Published.GetDescription()
                    : PublicationStatus.Drafted.GetDescription();
                errors = output.PublicationErrors;
            }
            catch (WebServiceFailureException ex)
            {
                status = "Unpublished";
                errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                OnDirectAccessError($"The web method ExamEvent failed because the Direct Access API service reported an unexpected server error. {errors}", source);
            }
            catch (WebServiceUnavailableException ex)
            {
                status = "Unpublished";
                errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                OnDirectAccessError($"The web method ExamEvent failed because the Direct Access API service is unavailable. {errors}", source);
            }
            catch (Exception ex)
            {
                status = "Unpublished";
                errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                OnDirectAccessError($"The web method ExamEvent failed because the Direct Access API service returned an unexpected response. {errors}", source);
            }
        }

        private void OnDirectAccessError(string message, string source)
        {
            _mailgun.Send(OrganizationIdentifiers.SkilledTradesBC, new AlertUnhandledExceptionIntercepted { ExceptionMessage = message });
        }

        private void ValidateExamResults(EventScoresValidated e)
        {
            var @event = _events.GetEvent(e.AggregateIdentifier, x => x.VenueLocation, x => x.Attendees, x => x.Attendees.Select(y => y.Person));
            var attendees = @event.Attendees != null ? @event.Attendees.ToArray() : null;
            var registrations = GetRegistrations(@event.EventIdentifier, e.Registrations);
            ValidateAttendance(e, @event, registrations);
            ValidateAttempts(e, @event, registrations);
        }

        private void ValidateAttendance(IChange e, QEvent @event, List<QRegistration> registrations)
        {
            foreach (var reg in registrations)
            {
                var cutoff = DateTimeOffset.UtcNow.AddHours(-1);
                var attendanceOverdue = @event.EventScheduledStart < cutoff && (reg.AttendanceStatus == null || reg.AttendanceStatus == "Pending");
                if (attendanceOverdue)
                    _broker.Send(e, new TakeAttendance(reg.RegistrationIdentifier, "Absent", null, null));
            }
        }

        private void SubmitExamResultsToDirectAccess(EventScoresPublished e)
        {
            QEvent @event = _events.GetEvent(e.AggregateIdentifier, x => x.VenueLocation);

            List<QRegistration> registrations;

            if (e.Registrations.IsNotEmpty())
                registrations = _registrations
                    .GetRegistrations(
                        new QRegistrationFilter { RegistrationIdentifiers = e.Registrations },
                        x => x.Attempt, x => x.Candidate, x => x.Form, x => x.RegistrationInstructors)
                    .ToList();
            else
                registrations = _registrations.GetRegistrationsByEvent(@event.EventIdentifier, null, null, null, false, false, false, false, true);

            registrations = registrations
                .Where(x => !x.GradeWithheld.HasValue)
                .ToList();

            var code = SubmitExamResultsToDirectAccess(e, @event, registrations);

            if (e.AlertMessageEnabled)
            {
                if (@event.ExamType == EventExamType.Class.Value || @event.ExamType == EventExamType.Arc.Value)
                {
                    _broker.Send(e, new TriggerEventNotification(@event.EventIdentifier, NotificationType.ITA016.ToString()));
                }
                else if (@event.ExamType == EventExamType.Sitting.Value || @event.ExamType.StartsWith("Individual"))
                {
                    foreach (var registration in registrations)
                    {
                        if (registration.RegistrationInstructors.Count > 0 && registration.IsPresent)
                            _broker.Send(e, new TriggerNotification(registration.RegistrationIdentifier, NotificationType.ITA016.ToString()));
                    }
                }
            }
        }

        private string SubmitExamResultsToDirectAccess(IChange e, QEvent @event, List<QRegistration> registrations)
        {
            var request = ApiRequestBuilder.CreateExamRegistrationRequest(@event, registrations, _registrations, _assessments, _standards, _attempts, _contacts);

            if (request.Sessions.Count > 0)
            {
                var response = _da.SubmitExamData(e.OriginUser, request);

                var code = $"Direct Access Transaction Receipt #{response.ReceiptId}";

                foreach (var registration in registrations)
                {
                    var command = new ChangeGrading(registration.RegistrationIdentifier, "Published", code, null);
                    _broker.Send(e, command);
                }

                return code;
            }

            return null;
        }

        private void ValidateAttempts(IChange e, QEvent @event, List<QRegistration> registrations)
        {
            if (registrations.IsEmpty())
                registrations = _registrations.GetRegistrationsByEvent(@event.EventIdentifier, null, null, null, true, true, true, true);
            registrations = registrations.Where(x => x.Attempt != null).ToList();

            const decimal ThirtyPercent = 0.30m;
            const decimal SeventyPercent = 0.70m;

            var hasWarning = false;
            var commands = new List<Command>();

            // All of the attempts in the search results must be for the same class activity.

            if (@event.ExamType == EventExamType.Class.Value && registrations.Count > 0)
            {
                // Count the number of registrations that comprises half the class. Round 0.5 toward the nearest number
                // that is away from zero to handle the edge case where a class has only one registration.

                var halfCount = Math.Round((double)registrations.Count / 2, 0, MidpointRounding.AwayFromZero);

                // If half the class failed with a score of less than 70%...

                var lowScoreFailure = registrations.Count(x => !x.Attempt.AttemptIsPassing && !InstructorAttemptStore.AttemptIsPassing(x.Attempt.AttemptScore, SeventyPercent));
                if (lowScoreFailure >= halfCount)
                {
                    hasWarning = true;

                    var rate = Calculator.GetPercentDecimal(lowScoreFailure, registrations.Count);
                    var warning = $"{rate:p0} of the class failed with a score less than {SeventyPercent:p0}.";

                    // Withhold grades for IPSE exams only.

                    foreach (var registration in registrations)
                        if (registration.Form.BankLevelType == "CofQ" || registration.Form.BankLevelType == "IPSE")
                            commands.Add(Withhold(registration, warning));
                }
            }

            // If an attempt has a score less than 30% then withhold the grade for that registration.

            var below30 = registrations.Where(x => !InstructorAttemptStore.AttemptIsPassing(x.Attempt.AttemptScore, ThirtyPercent)).ToList();
            if (below30.Count > 0)
            {
                hasWarning = true;

                // If the grade is not already withheld then withhold it now.

                foreach (var registration in below30)
                    if (!commands.Any(x => x.AggregateIdentifier == registration.RegistrationIdentifier))
                        commands.Add(Withhold(registration, $"Score less than {ThirtyPercent:p0}"));
            }

            // Send commands and refresh the search results.

            foreach (var command in commands)
                _broker.Send(e, command);

            if (hasWarning)
            {
                ExecuteMailout(e, e.AggregateIdentifier, Notifications.Select(NotificationType.ITA013));

                if (commands.Count > 0)
                {
                    var now = DateTimeOffset.UtcNow;
                    var waitHours = (5 * 24);

                    int waitQuantity;
                    DateTimeOffset waitEnds;
                    string waitText = string.Empty;

                    if (waitHours >= 24)
                    {
                        waitQuantity = waitHours / 24;
                        waitEnds = Calendar.AddBusinessDays(now, waitQuantity);
                        waitText = $"Wait {waitQuantity} business days, then send ITA014 if grades are still withheld.";
                    }
                    else
                    {
                        waitQuantity = waitHours;
                        waitEnds = now.AddHours(waitHours);
                        waitText = $"Wait {waitHours} hours, then send ITA014 if grades are still withheld.";
                    }

                    StartEventAlertTimer(e, @event.EventIdentifier, NotificationType.ITA014, waitEnds, waitText, true);
                }
            }

            ChangeGrading Withhold(QRegistration registration, string reason)
            {
                var message = $"{registration.Candidate.UserFullName} ({registration.Candidate.PersonCode}): {reason}";
                return new ChangeGrading(registration.RegistrationIdentifier, "Withheld", message, null);
            }
        }

        /// <summary>
        /// Starts the timer for triggering an exam event alert.
        /// </summary>
        public void StartEventAlertTimer(IChange cause, Guid @event, NotificationType alert, DateTimeOffset when, string description, bool condition)
        {
            if (!condition)
                return;

            // Create a command to trigger the alert.
            var timer = new TriggerEventNotification(@event, alert.ToString());

            // Store the command for future reference.
            _broker.Bookmark(cause, timer, when);

            // Start the timer.
            _broker.Send(cause, new StartEventTimer(@event, timer.CommandIdentifier, when, description));
        }

        /// <summary>
        /// Cancels the timer(s) for triggering an exam event alert.
        /// </summary>
        public void CancelEventAlertTimer(IChange cause, Guid @event, NotificationType alert)
        {
            var timers = _events.GetTimers(new QEventTimerFilter { EventIdentifier = @event, TriggerTimeSince = DateTimeOffset.UtcNow });
            foreach (var timer in timers)
            {
                var trigger = _commands.GetCommand(timer.TriggerCommand) as TriggerEventNotification;
                if (trigger?.Name == alert.ToString())
                    _broker.Send(cause, new CancelEventTimer(@event, timer.TriggerCommand));
            }
        }

        public bool EventTimerExists(Guid @event, NotificationType alert)
        {
            var timers = _events.GetTimers(new QEventTimerFilter { EventIdentifier = @event });
            foreach (var timer in timers)
            {
                var trigger = _commands.GetCommand(timer.TriggerCommand) as TriggerEventNotification;
                if (trigger?.Name == alert.ToString())
                    return true;
            }
            return false;
        }

        private void ScheduleShipmentReminders(IChange cause, Guid eventId)
        {
            QEvent @event = _events.GetEvent(eventId);
            if (@event.EventFormat != EventExamFormat.Paper.Value)
                return;

            if (EventTimerExists(eventId, NotificationType.ITA017))
                return;

            var now = DateTimeOffset.UtcNow;
            var waitHours = (QEvent.DistributionTimelineBusinessDays / 2 * 24);

            int waitQuantity;
            string waitUnit;
            DateTimeOffset waitEnds, waitEndsAgain;

            if (waitHours >= 24)
            {
                waitUnit = "business days";
                waitQuantity = waitHours / 24;
                waitEnds = Calendar.AddBusinessDays(now, waitQuantity);
                waitEndsAgain = Calendar.AddBusinessDays(now, 2 * waitQuantity);
            }
            else
            {
                waitUnit = "hours";
                waitQuantity = waitHours;
                waitEnds = now.AddHours(waitHours);
                waitEndsAgain = now.AddHours(2 * waitHours);
            }

            StartEventAlertTimer(cause, eventId, NotificationType.ITA017, now, $"Immediately send ITA017", true);
            StartEventAlertTimer(cause, eventId, NotificationType.ITA017, waitEnds, $"{waitQuantity} {waitUnit} from now, send ITA017", true);
            StartEventAlertTimer(cause, eventId, NotificationType.ITA017, waitEndsAgain, $"{2 * waitQuantity} {waitUnit} from now, send ITA017", true);
        }

        private void ReScheduleNoReturnShipmentReceived(IChange cause, Guid eventId)
        {
            CancelEventAlertTimer(cause, eventId, NotificationType.ITA026);
            ScheduleNoReturnShipmentReceived(cause, eventId);
        }

        private void ReScheduleNoReturnShipmentReceived(IChange cause, QEvent @event)
        {
            CancelEventAlertTimer(cause, @event.EventIdentifier, NotificationType.ITA026);
            ScheduleNoReturnShipmentReceived(cause, @event);
        }

        private void ScheduleNoReturnShipmentReceived(IChange cause, Guid eventId)
        {
            ScheduleNoReturnShipmentReceived(cause, _events.GetEvent(eventId));
        }

        private void ScheduleNoReturnShipmentReceived(IChange cause, QEvent @event)
        {
            if (@event.EventFormat != EventExamFormat.Paper.Value || @event.ExamMaterialReturnShipmentReceived.HasValue)
                return;

            var status = @event.EventSchedulingStatus;
            var isStatusValid = status == "Under Review"
                || status == "Ready to Schedule"
                || status == "Approved with Errors"
                || status == "Approved"
                || status == "Completed"
                || status == "Pending";
            if (!isStatusValid)
                return;

            var eventId = @event.EventIdentifier;

            if (EventTimerExists(eventId, NotificationType.ITA017))
                return;

            var days = _helper.GetITA026DayCount(@event);

            if (days == EventProcessorHelper.ITA026DayCount.Days14)
            {
                var twoWeeks = @event.EventScheduledStart.AddDays(14);
                StartEventAlertTimer(cause, eventId, NotificationType.ITA026, twoWeeks, $"2 weeks after Scheduled Start Date, send ITA026", true);
            }
            else if (days == EventProcessorHelper.ITA026DayCount.Days60)
            {
                var twoMonths = @event.EventScheduledStart.AddDays(60);
                StartEventAlertTimer(cause, eventId, NotificationType.ITA026, twoMonths, $"60 days after Scheduled Start Date, send ITA026", true);
            }
        }
    }
}
