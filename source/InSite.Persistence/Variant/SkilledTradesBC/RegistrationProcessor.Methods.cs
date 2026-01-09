using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Events.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Messages;
using InSite.Domain.Registrations;
using InSite.Persistence.Integration.DirectAccess;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    public partial class RegistrationProcessor
    {
        private void CancelRegistrationNotificationTimer(IChange cause, Guid registration, NotificationType notification)
        {
            var timers = _registrations.GetTimers(new QRegistrationTimerFilter { RegistrationIdentifier = registration, TriggerTimeSince = DateTimeOffset.UtcNow });
            foreach (var timer in timers)
            {
                var trigger = _commands.GetCommand(timer.TriggerCommand) as TriggerNotification;
                if (trigger?.Name == notification.ToString())
                    _broker.Send(cause, new CancelRegistrationTimer(registration, timer.TriggerCommand));
            }
        }

        private bool EnableIntegrationWithDirectAccess(RegistrationPacket packet)
        {
            var reg = packet.Registration;
            if (reg == null || reg.Candidate == null || reg.Form == null)
                return false;

            var @event = reg.Event;
            if (@event == null || @event.ExamType == EventExamType.Test.Value)
                return false;

            var form = reg.Form;
            if (form == null)
                return false;

            return true;
        }

        private bool IsNotificationDisabled(RegistrationPacket packet)
        {
            var reg = packet.Registration;
            if (reg.ApprovalStatus == null)
                return true;

            var @event = reg.Event;
            if (@event.EventSchedulingStatus == null
             || !@event.EventSchedulingStatus.StartsWith("Approved")
             || @event.ExamType == EventExamType.Test.Value
             )
                return true;

            return false;
        }

        private void ExecuteMailout(IChange e, string notificationName)
        {
            try
            {
                var registration = _registrations.GetRegistration(
                    e.AggregateIdentifier,
                    x => x.Event,
                    x => x.Candidate,
                    x => x.Attempt.Questions,
                    x => x.Form,
                    x => x.Accommodations,
                    x => x.RegistrationInstructors
                );
                if (registration == null || registration.EventIdentifier == Guid.Empty)
                    return;

                QEvent @event = _events.GetEvent(
                    registration.EventIdentifier,
                    x => x.Attendees,
                    x => x.Attendees.Select(y => y.Person.User),
                    x => x.VenueCoordinator,
                    x => x.VenueLocation,
                    x => x.VenueOffice
                    );

                if (@event == null || @event.ExamType == EventExamType.Test.Value || !registration.ExamFormIdentifier.HasValue)
                    return;

                var form = _banks.GetFormData(registration.ExamFormIdentifier.Value);
                if (form == null)
                    return;

                var venueAddress = @event.VenueLocationIdentifier.HasValue ? _groups.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical) : null;

                var builder = new MessageBuilder(_contacts, _groups, _filePaths, _domain);
                var alert = Notifications.Select(notificationName);
                var email = builder.BuildRegistrationEmail(alert, @event, venueAddress, registration, form);
                if (email == null || email.IsDisabled)
                    return;

                if (alert.Courier == "Mailgun")
                {
                    var mailgunEmail = PrepareEmail(email, alert.Courier);
                    mailgunEmail.EventIdentifier = @event.EventIdentifier;

                    _mailgun.Send(mailgunEmail);
                }
                else
                {
                    _broker.Send(e, new ScheduleMailout(
                    email.MessageIdentifier ?? throw new ArgumentNullException("email.MessageIdentifier"),
                    email.MailoutIdentifier == Guid.Empty ? UniqueIdentifier.Create() : email.MailoutIdentifier,
                    email.SenderIdentifier,
                    email.MailoutScheduled ?? throw new ArgumentNullException("email.MailoutScheduled"),
                    email.Recipients.ToArray(),
                    email.ContentSubject,
                    email.ContentBody,
                    email.ContentVariables,
                    email.ContentAttachments, @event.EventIdentifier));
                }
            }
            catch (Exception ex)
            {
                // Because the Timeline engine executes projectors and processors synchronously, we cannot allow a failed mailout to prevent other
                // projectors and processors from executing. Therefore, if an exception occurs in this method, we invoke the error function (which
                // is expected to send a message to Sentry) and allow the Timeline engine to continue the flow of normal execution.
                var detailedEx = new ApplicationException($"ExecuteMailout failed. RegistrationId = {e.AggregateIdentifier}, Notification = {notificationName}", ex);
                _error?.Invoke(detailedEx);
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

            email.RecipientListTo = new Dictionary<Guid, string>();

            foreach (var recipient in email.Recipients)
            {
                if (recipient.Identifier == null)
                    throw new Exception($"The identifier for this recipient ({recipient.Address}) cannot be null.");

                email.RecipientListTo.Add(recipient.Identifier.Value, recipient.Address);
            }

            LoadCarbonCopyRecipients(email);

            return email;
        }

        private void LoadCarbonCopyRecipients(EmailDraft email)
        {
            var search = new QPersonSearch();

            if (email.RecipientListCc == null)
                email.RecipientListCc = new Dictionary<Guid, string>();

            foreach (var recipient in email.Recipients)
            {
                if (recipient.Cc.IsEmpty())
                    continue;

                foreach (var cc in recipient.Cc)
                {
                    if (email.RecipientListCc.ContainsKey(cc))
                        continue;

                    var person = search.GetPerson(cc, OrganizationIdentifiers.SkilledTradesBC, x => x.User);

                    if (person?.User == null)
                        continue;

                    if (person.EmailEnabled)
                        email.RecipientListCc.Add(person.User.UserIdentifier, person.User.Email);

                    else if (person.EmailAlternateEnabled)
                        email.RecipientListCc.Add(person.User.UserIdentifier, person.User.EmailAlternate);
                }
            }
        }

        private string GetCandidateCode(Guid registration)
        {
            var query = _registrations.GetRegistration(registration);
            if (query != null)
                return _contacts.BindFirstPerson(x => x.PersonCode, x => x.UserIdentifier == query.CandidateIdentifier);
            return null;
        }

        private RegistrationPacket GetRegistrationPacket(Guid id)
        {
            return new RegistrationPacket(id)
            {
                Registration = _registrations.GetRegistration(id, x => x.Event, x => x.Candidate, x => x.Form)
            };
        }

        private string[] RefreshIndividualFromDirectAccess(IChange e, Guid registration, string candidate, string exam, Guid caller)
        {
            var source = typeof(RegistrationProcessor).FullName + "." + nameof(RefreshIndividualFromDirectAccess);
            var errors = new List<string>();

            var request = new IndividualRequestInput { IndividualId = candidate, ExamFormCode = exam };
            var response = _da.IndividualRequest(caller, request);
            if (response?.Individuals != null)
            {
                foreach (var individual in response.Individuals)
                {
                    if (ValidateIndividual(individual, errors))
                    {
                        _daStore.Save(individual);

                        if (!string.IsNullOrEmpty(individual.ProgramType))
                            _broker.Send(e, new ChangeCandidateType(registration, individual.ProgramType));
                    }
                }
            }
            else
            {
                errors.Add("Direct Access did not respond to this request: IndividualRequest");
            }
            return errors.ToArray();
        }

        private void RemoveCandidateFromDirectAccess(IChange e, RegistrationPacket packet, Guid user, string verb)
        {
            var source = typeof(RegistrationProcessor).FullName + "." + nameof(RemoveCandidateFromDirectAccess);
            var description = string.Empty;

            try
            {
                try
                {
                    var userName = UserSearch.Bind(user, x => x.FullName);

                    var input = ApiRequestBuilder.GetExamEventCandidateInputForDelete(
                        packet.Registration.Event.EventClassCode ?? string.Empty,
                        packet.Registration.Form.FormCode,
                        packet.Registration.Form.FormTitle.NullIf("None").IfNullOrEmpty(packet.Registration.Form.FormName),
                        $"{verb} by {userName}");

                    description = $"Registration {packet.Registration.RegistrationIdentifier} {verb} by {userName} on Event {packet.Registration.Event.EventNumber}.";

                    _da.ExamEventCandidate(user, packet.Registration.Event.EventNumber.ToString(), packet.Registration.Candidate.PersonCode, input);
                }
                catch (System.Net.WebException network)
                {
                    OnDirectAccessError($"The web method ExamEventCandidate failed because the Direct Access API service reported an unexpected network error. {description} {network.Message}", source);
                }
                catch (WebServiceFailureException)
                {
                    OnDirectAccessError($"The web method ExamEventCandidate failed because the Direct Access API service reported an unexpected server error. {description}", source);
                }
                catch (WebServiceUnavailableException)
                {
                    OnDirectAccessError($"The web method ExamEventCandidate failed because the Direct Access API service is unavailable. {description}", source);
                }
                catch
                {
                    OnDirectAccessError($"An unexpected error happened for aggregate {packet.Identifier}. {description}", source);
                }
            }
            catch (NullReferenceException)
            {
                OnDirectAccessError($"An unexpected null reference occurred in the RegistrationCancelled change handler for aggregate {packet.Identifier}", source);
            }
        }

        private void OnDirectAccessError(string message, string source)
        {
            _mailgun.Send(OrganizationIdentifiers.SkilledTradesBC, new Domain.Messages.AlertUnhandledExceptionIntercepted { ExceptionMessage = message });
        }

        private void SetNotificationTimers(IChange cause, RegistrationPacket packet)
        {
            // ... otherwise proceed.

            var regId = packet.Registration.RegistrationIdentifier;

            // Start timers for the various SkilledTradesBC notifications.

            var now = DateTimeOffset.UtcNow;
            var start = packet.Registration.Event.EventScheduledStart;
            var fiveBusinessDaysPrior = Shift.Common.Calendar.AddBusinessDays(start, -5);
            var threeWeeksPrior = start.AddDays(-21);
            var examType = packet.Registration.Event.ExamType;
            var venue = packet.Registration.Event.VenueLocationIdentifier;
            var approvalStatus = packet.Registration.ApprovalStatus;
            var to = packet.Registration.Candidate.UserEmail;

            if (approvalStatus.StartsWith("Eligible"))
            {
                if (examType == EventExamType.Class.Value)
                {
                    StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA004, now, $"Immediately send ITA004 to {to}");
                    StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA005, fiveBusinessDaysPrior, $"Five (5) business days prior, send ITA005 to {to}", fiveBusinessDaysPrior > now);
                    if (packet.Registration.ApprovalStatus == "Eligible with Limitations")
                    {
                        StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA005, threeWeeksPrior, $"Three (3) weeks prior, send ITA005 to {to}", threeWeeksPrior > now);
                    }
                }
                else if (examType == EventExamType.Sitting.Value)
                {
                    StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA006, now, $"Immediately send ITA006 to {to}");
                    StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA007, fiveBusinessDaysPrior, $"Five (5) business days prior, send ITA007 to {to}", fiveBusinessDaysPrior > now);
                }
                else if (examType.StartsWith("Individual") && venue.HasValue)
                {
                    if (venue.Value == GetGroupId("ITA Richmond Office"))
                    {
                        StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA024, now, $"Immediately send ITA024 to {to}");
                    }
                    else if (GroupIsIn(venue.Value, "SBC"))
                    {
                        StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA008, now, $"Immediately send ITA008 to {to}");
                        StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA027, now, $"Immediately send ITA027 to Invigilating Office/Invigilator");
                    }
                    else if (GroupIsIn(venue.Value, "ITA/DRC"))
                    {
                        StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA020, now, $"Immediately send ITA020 to {to}");
                        StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA009, fiveBusinessDaysPrior, $"Five (5) business days prior, build and send ITA009 to {to}", fiveBusinessDaysPrior > now);
                    }
                }
                else if (examType == EventExamType.Arc.Value)
                {
                    StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA020, now, $"Immediately send ITA020 to {to}");
                }
            }
            else if (approvalStatus == "Not Eligible")
            {
                if (examType == EventExamType.Class.Value || examType == EventExamType.Arc.Value)
                {
                    StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA021, now, $"Immediately send ITA021 to {to}");
                    StartRegistrationNotificationTimer(cause, regId, NotificationType.ITA021, threeWeeksPrior, $"Three (3) weeks prior, send candidate notification ITA021 to {to}", threeWeeksPrior > now);
                }
            }
        }

        private static Guid? GetGroupId(string name)
        {
            using (var db = new InternalDbContext())
            {
                return db.QGroups
                    .Where(x => x.OrganizationIdentifier == OrganizationIdentifiers.SkilledTradesBC && x.GroupName == name)
                    .FirstOrDefault()?
                    .GroupIdentifier;
            }
        }

        private static bool GroupIsIn(Guid group, string parentGroupName)
        {
            using (var db = new InternalDbContext())
            {
                if (db.QGroups.Any(
                    x => x.GroupIdentifier == group
                      && x.Parent.GroupName == parentGroupName))
                    return true;

                if (db.QGroupConnections.Any(
                    x => x.ChildGroup.GroupIdentifier == group
                      && x.ParentGroup.GroupName == parentGroupName))
                    return true;
            }

            return false;
        }

        private void StartRegistrationNotificationTimer(IChange cause, Guid registration, NotificationType notification, DateTimeOffset when, string description, bool condition = true)
        {
            if (!condition)
                return;

            // Create a command to trigger the notification.
            var timer = new TriggerNotification(registration, notification.ToString());

            // Store the command for future reference.
            _broker.Bookmark(cause, timer, when);

            // Start the timer.
            _broker.Send(cause, new StartTimer(registration, timer.CommandIdentifier, when, description));
        }

        private void UpdateExamCandidateInDirectAccess(SynchronizationChanged cause, RegistrationPacket packet)
        {
            try
            {
                if (packet.Registration.Event.EventSchedulingStatus != null && packet.Registration.Event.EventSchedulingStatus.StartsWith("Approved"))
                {
                    if (packet.Registration.Event.EventPublicationStatus != "Published")
                        _broker.Send(cause, new Application.Events.Write.PublishEvent(packet.Registration.Event.EventIdentifier, null, null));

                    var input = ApiRequestBuilder.GetExamEventCandidateInputForUpdate(packet.Registration.Event, packet.Registration.Form, packet.Registration);
                    var output = _da.ExamEventCandidate(cause.OriginUser, packet.Registration.Event.EventNumber.ToString(), packet.Registration.Candidate.PersonCode, input);

                    cause.Process.Description = output.PublicationStatus;
                    cause.Process.Errors = StringHelper.Split(output.PublicationErrors);
                }
            }
            catch (Exception ex)
            {
                var source = typeof(RegistrationProcessor).FullName + "." + nameof(UpdateExamCandidateInDirectAccess);
                var message = "This request to the Direct Access API failed (ExamEventCandidate). " + CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                OnDirectAccessError(message, source);

                cause.Process.Description = message;
                cause.Process.Errors = new[] { message };
            }
        }

        private bool ValidateIndividual(Shift.Toolbox.Integration.DirectAccess.Individual individual, List<string> errors)
        {
            if (string.IsNullOrEmpty(individual.Email))
                individual.Email = $"{individual.IndividualKey}@itaportal.ca";

            if (string.IsNullOrEmpty(individual.FirstName))
                errors.Add("First Name is a required and cannot be empty");

            if (string.IsNullOrEmpty(individual.LastName))
                errors.Add("Last Name is a required and cannot be empty");

            if (errors.Count == 0)
                return true;

            return false;
        }

        private ProcessState VerifyIndividualInDirectAccess(IChange e, QRegistration registration)
        {
            var source = typeof(RegistrationProcessor).FullName + "." + nameof(VerifyIndividualInDirectAccess);

            var inputs = new VerificationInputVariables();
            var displays = new VerificationDisplayVariables();
            var verification = new Verification(inputs, displays);

            var problem = _events.GetScheduleProblem(registration.EventIdentifier, registration.CandidateIdentifier, registration.ExamFormIdentifier);
            var @event = registration.Event ?? _events.GetEvent(registration.EventIdentifier);
            var candidate = _contacts.GetPerson(registration.CandidateIdentifier, @event.OrganizationIdentifier);

            if (registration.ExamFormIdentifier.HasValue)
            {
                var form = _banks.GetFormData(registration.ExamFormIdentifier.Value);

                inputs.CandidateCode = candidate?.PersonCode;
                inputs.CurrentTime = DateTimeOffset.UtcNow;
                inputs.EventIsOnline = @event.EventFormat == EventExamFormat.Online.Value;
                inputs.EventTime = @event.EventScheduledStart;
                inputs.FormCode = form.Code;
                inputs.HolidayCalendar = _contacts.GetHolidays();

                inputs.ScheduleConflictCount = problem.SameDayEvents.Count;
                inputs.FormConflictCount = problem.SameFormEvents.Count;

                displays.CandidateName = candidate?.UserFullName;
                displays.EventType = @event.ExamType;

                try
                {
                    _da.VerifyActiveIndividual(e.OriginUser, inputs, displays);
                    _da.VerifyCorrespondingRegistration(e.OriginUser, inputs, displays);

                    verification.Calculate();
                }
                catch (WebServiceFailureException ex)
                {
                    verification.Errors = ex.GetMessages().ToList();
                    var errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                    OnDirectAccessError($"The web method VerifyActiveIndividual failed because the Direct Access API service reported an unexpected server error. {errors}", source);
                }
                catch (WebServiceUnavailableException ex)
                {
                    verification.Errors = ex.GetMessages().ToList();
                    var errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                    OnDirectAccessError($"The web method VerifyActiveIndividual failed because the Direct Access API service is unavailable. {errors}", source);
                }
                catch (Exception ex)
                {
                    verification.Errors = ex.GetMessages().ToList();
                    var errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");
                    OnDirectAccessError($"The web method VerifyActiveIndividual failed because the Direct Access API service returned an unexpected response. {errors}", source);
                }
            }

            // If there are no warnings and no errors then automatically approve the registration. The candidate is
            // eligible to write the exam.

            return new ProcessState
            {
                Execution = ExecutionState.Completed,
                Errors = verification.Errors.ToArray(),
                Warnings = verification.Warnings.ToArray()
            };
        }
    }
}
