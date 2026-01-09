using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Domain.Banks;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    public class MessageBuilder
    {
        public class RecipientItem
        {
            public Guid UserIdentifier { get; set; }
            public string UserEmail { get; set; }
            public bool UserEmailEnabled { get; set; }
            public string UserEmailAlternate { get; set; }
            public bool UserEmailAlternateEnabled { get; set; }
            public string PersonCode { get; set; }
            public string PersonLanguage { get; set; }

            public string UserName { get; set; }
            public string AttendeeRole { get; set; }

            public string RegistrationStatus { get; set; }
            public string AttendanceStatus { get; set; }

            public Guid? FormIdentifier { get; set; }
            public string FormTitle { get; set; }

            public Guid? RegistrationIdentifier { get; set; }
        }

        private readonly IContactSearch _contacts;
        private readonly string _domain;
        private readonly FilePaths _filePaths;
        private readonly EventProcessorHelper _helper;

        public MessageBuilder(IContactSearch contacts, IGroupSearch groupSearch, FilePaths filePaths, string domain)
        {
            _contacts = contacts;
            _filePaths = filePaths;
            _domain = domain;
            _helper = new EventProcessorHelper(groupSearch);
        }

        public EmailDraft BuildEventEmail(
            Notification notification,
            QEvent @event,
            QGroupAddress venueAddress,
            QBankForm form,
            QRegistration[] registrations,
            QAttempt[] attempts,
            Command[] warnings
            )
        {
            if (notification.Type == NotificationType.ITA014)
                if (!registrations.Any(x => x.GradingStatus == "Withheld"))
                    return null;

            var message = MessageRepository.GetEmail(OrganizationIdentifiers.SkilledTradesBC, notification.Type);
            if (message.IsDisabled)
                return message;

            var variables = new MessageVariableList($"https://ita.{_domain}", notification.Variables);

            if (form != null)
                CreateFormVariables(variables, form);

            var candidates = (notification.Type == NotificationType.ITA001 || notification.Type == NotificationType.ITA023)
                ? (registrations ?? new QRegistration[0])
                : registrations.Where(x => x.ApprovalStatus != "Not Eligible").ToArray();

            CreateEventVariables(variables, @event, venueAddress, candidates, attempts);

            if (form != null)
                AddEventInstructionUrl(variables, form);

            if (warnings != null)
            {
                var md = new StringBuilder();

                foreach (var warning in warnings)
                    if (warning is ChangeGrading change && change.Status == "Withheld")
                        md.Append("- " + change.Reason);

                variables.AddValue("WarningList", md.ToString());
            }

            var recipients = GetRecipients(notification, @event, registrations);

            var subscribers = MessageRepository.GetSubscribers(message.OrganizationIdentifier, message.MessageIdentifier.Value);

            foreach (var subscriber in subscribers)
            {
                var email = EmailAddress.GetEnabledEmail(subscriber.UserEmail, subscriber.UserEmailEnabled, subscriber.UserEmailAlternate, subscriber.UserEmailAlternateEnabled);
                if (email.IsEmpty())
                    continue;

                var lang = _contacts.GetPersonLanguage(subscriber.UserIdentifier, message.OrganizationIdentifier);

                recipients.Add(subscriber.UserIdentifier, email, subscriber.UserFullName, subscriber.PersonCode, lang);
            }

            CreateDelivery(notification, message, recipients, variables);

            if ((message.UserIdentifier == null || message.UserIdentifier == Guid.Empty) && notification.OriginUser.HasValue)
                message.UserIdentifier = notification.OriginUser.Value;

            return message;
        }

        public EmailDraft BuildRegistrationEmail(Notification notification, QEvent @event, QGroupAddress venueAddress, QRegistration registration, Form form)
        {
            var message = MessageRepository.GetEmail(OrganizationIdentifiers.SkilledTradesBC, notification.Type);
            if (message.IsDisabled)
                return message;

            var variables = new MessageVariableList($"https://ita.{_domain}", notification.Variables);

            CreateEventVariables(variables, @event, venueAddress, new[] { registration }, new[] { registration.Attempt });
            CreateFormVariables(variables, form);

            var candidateTimeLimit = Math.Round((registration.ExamTimeLimit ?? 0) / 60.0, 1);

            if (registration.Candidate == null)
                throw new ArgumentNullException("Unexpected null value: registration.Candidate");

            var recipients = GetRegistrationEmailRecipients(
                notification.Type,
                registration.CandidateIdentifier,
                registration.OrganizationIdentifier,
                registration.RegistrationInstructors,
                @event.Attendees
            );

            if (recipients.Count == 0)
                return null;

            variables.AddValue("CandidateCode", registration.Candidate.PersonCode);
            variables.AddValue("CandidateName", registration.Candidate.UserFullName);
            variables.AddValue("CandidateTimeLimit", $"{candidateTimeLimit:n1} h");

            variables.AddValue("RecipientName", registration.Candidate.UserFullName);
            variables.AddValue("CandidateAddress", string.Empty);

            if (registration.Event != null)
            {
                variables.AddValue("EventIdentifier", registration.Event.EventIdentifier.ToString());
                variables.AddValue("EventNumber", registration.Event.EventNumber.ToString());
            }

            AddCandidateAccommodation(variables, registration);
            AddCandidateInstructionDeadline(variables, @event, registration);
            AddCandidateInstructionSchedule(variables, @event, registration);
            AddCandidateInstructionEligibility(variables, @event, registration);
            AddEventInstructionUrl(variables, registration.Form);

            CreateDelivery(notification, message, recipients, variables);

            return message;
        }

        private EmailAddressList GetRegistrationEmailRecipients(
            NotificationType notificationType,
            Guid candidateId,
            Guid organizationId,
            ICollection<QRegistrationInstructor> registrationInstructors,
            ICollection<QEventAttendee> attendees
            )
        {
            var recipients = new EmailAddressList();

            if (notificationType == NotificationType.ITA016)
            {
                // Send this notification ONLY to instructors assigned to the candidate's registration.
                foreach (var group in registrationInstructors.GroupBy(x => x.OrganizationIdentifier))
                {
                    foreach (var address in _contacts.GetEmailAddresses(group.Select(x => x.InstructorIdentifier), group.Key))
                        recipients.Add(address);
                }
                return recipients;
            }
            else if (notificationType == NotificationType.ITA027)
            {
                var invigilators = GetContactIdentifiers(attendees, "Invigilating Office/Invigilator");

                if (invigilators.Length > 0)
                {
                    var emails = _contacts.GetEmailAddresses(invigilators, organizationId);
                    foreach (var address in emails)
                        recipients.Add(address);
                }

                return recipients;
            }

            var candidateRecipient = _contacts.GetEmailAddress(candidateId, organizationId);
            if (candidateRecipient == null)
                return recipients;

            recipients.Add(candidateRecipient);

            if (notificationType == NotificationType.ITA008
                || notificationType == NotificationType.ITA009
                || notificationType == NotificationType.ITA025
                )
            {
                foreach (var reader in GetContactIdentifiers(attendees, "Reader/Translator"))
                    candidateRecipient.Cc.Add(reader);
            }

            if (notificationType == NotificationType.ITA025)
            {
                foreach (var instructor in GetContactIdentifiers(attendees, "Training Provider"))
                    candidateRecipient.Cc.Add(instructor);

                foreach (var instructor in registrationInstructors)
                    candidateRecipient.Cc.Add(instructor.InstructorIdentifier);
            }

            return recipients;
        }

        private void AddEventInstructionUrl(MessageVariableList list, QBankForm form)
        {
            var type = form.BankLevelType;

            {
                if (type == "CofQ" || type == "EE" || type == "IPSE")
                {
                    var url = $"https://ita.{_domain}/files/notifications/att-001.exam-instructions.pdf";
                    list.AddValue("EventInstructionUrl", url);
                    list.AddValue("RecipientAttachment", DownloadFile(url, "Exam Instructions (CofQ EE IPSE).pdf"));
                }

                else if (type == "FE" || type == "SLE")
                {
                    var url = $"https://ita.{_domain}/files/notifications/att-002.exam-instructions.pdf";
                    list.AddValue("EventInstructionUrl", url);
                    list.AddValue("RecipientAttachment", DownloadFile(url, "Exam Instructions (FE SLE).pdf"));
                }

                else
                {
                    list.AddValue("EventInstructionUrl", "https://www.itabc.ca/exams/writing-your-exams");
                }
            }
        }

        private void AddCandidateAccommodation(MessageVariableList list, QRegistration registration)
        {
            list.AddValue("CandidateAccommodation", EventVariableBuilder.GetAccommodations(registration, false));
            list.AddValue("CandidateAccommodationName", EventVariableBuilder.GetAccommodationNames(registration));
            list.AddValue("CandidateAccommodationTimeExtension", EventVariableBuilder.GetAccommodationTimeExtension(registration));
            list.AddValue("CandidateAccommodationTable", EventVariableBuilder.GetAccommodationTable(registration));
        }

        private void AddCandidateInstructionDeadline(MessageVariableList list, QEvent @event, QRegistration registration)
        {
            var closing = string.Empty;
            if (registration.Event.EventScheduledStart.Hour != 0)
            {
                if (@event.VenueCoordinator?.UserPhone != null)
                    closing += $"Telephone: {@event.VenueCoordinator?.UserPhone}. ";

                closing += $"You will have until {registration.Event.EventScheduledStart.AddDays(45):MMMM d, yyyy} to write your exam before it is sent back to our office.";
            }
            list.AddValue("CandidateInstructionDeadline", closing);
        }

        private void AddCandidateInstructionEligibility(MessageVariableList list, QEvent @event, QRegistration registration)
        {
            var instruction = string.Empty;
            var reason = registration.ApprovalReason;

            if (registration.ApprovalStatus == "Not Eligible")
            {
                instruction += $"You are NOT ELIGIBLE for this exam: {reason}.\n\n";
                instruction += $"If you do not correct this issue there will not be an exam available for you. Confirmation of eligibility must be received by {@event.DistributionExpectedText} at 9:00 AM.\n\n";
                instruction += "Please confirm your eligibility to write this exam with your instructor and SkilledTradesBC Customer Service via phone or email. Please note that emails are processed within 5 business days; if your exam is distributing within this time please call SkilledTradesBC Customer Service.\n\n";
            }
            else if (registration.ApprovalStatus == "Eligible with Limitations")
            {
                instruction += $"You are ELIGIBLE WITH LIMITATIONS for this exam : {reason}.\n\n";
                instruction += "Please confirm your eligibility to write this exam with your instructor and SkilledTradesBC Customer Service via phone or email. Please note that emails are processed within 5 business days; if your exam is distributing within this time please call SkilledTradesBC Customer Service.\n\n";
            }

            list.AddValue("CandidateInstructionEligibility", instruction);
        }

        private void AddCandidateInstructionSchedule(MessageVariableList list, QEvent @event, QRegistration registration)
        {
            var opening = $"You have been scheduled to write {registration.Form.FormTitle}";

            if (registration.Accommodations.Count > 0)
            {
                opening += $" with a {string.Join(", ", registration.Accommodations.Select(x => x.AccommodationType).OrderBy(x => x))}";
            }

            opening += ".";

            if (registration.Event.EventScheduledStart.Hour != 0)
            {
                opening = "You have an exam in 5 days. "
                    + opening
                    + $"As per your request we have sent your exam to {@event.VenueLocation?.GroupName}. It is scheduled to arrive by {registration.Event.EventScheduledStart:MMMM d, yyyy}.";
            }

            list.AddValue("CandidateInstructionSchedule", opening);
        }

        private class RoleInfo
        {
            public string ExamType { get; set; }
            public string RoleName { get; set; }

            public bool IsMatch(QEvent @event, RecipientItem item)
            {
                return (ExamType == null || @event.ExamType == ExamType)
                    && item.AttendeeRole.Contains(RoleName);
            }
        }

        private List<RecipientItem> BindRecipients(string roleList, QEvent @event, QRegistration[] registrations)
        {
            var recipients = new List<RecipientItem>();
            if (@event.Attendees == null)
                return recipients;

            var roles = ParseRoleList(roleList);

            foreach (var attendee in @event.Attendees)
            {
                if (attendee.Person == null)
                    continue;

                var item = new RecipientItem
                {
                    UserIdentifier = attendee.UserIdentifier,
                    PersonCode = attendee.Person.PersonCode,
                    UserEmail = attendee.Person.UserEmail,
                    UserEmailEnabled = attendee.Person.UserEmailEnabled,
                    UserEmailAlternate = attendee.Person.UserEmailAlternate,
                    UserEmailAlternateEnabled = attendee.Person.UserEmailAlternateEnabled,
                    UserName = attendee.Person.UserFullName,
                    AttendeeRole = attendee.AttendeeRole,
                    PersonLanguage = attendee.Person.Language
                };

                var registration = registrations.FirstOrDefault(x => x.CandidateIdentifier == attendee.UserIdentifier);
                if (registration != null)
                {
                    item.RegistrationIdentifier = registration.RegistrationIdentifier;
                    item.RegistrationStatus = registration.ApprovalStatus;
                    item.AttendanceStatus = registration.AttendanceStatus;
                    item.FormIdentifier = registration.ExamFormIdentifier;
                    item.FormTitle = registration.Form?.FormTitle;
                }

                var isInNotificationRole = roles.Any(x => x.IsMatch(@event, item));
                if (isInNotificationRole)
                    recipients.Add(item);
            }

            return recipients;
        }

        private static RoleInfo[] ParseRoleList(string data)
        {
            var result = new List<RoleInfo>();
            var parts = data.Split(',');

            for (var i = 0; i < parts.Length; i++)
            {
                var item = parts[i].Trim();
                if (item.IsEmpty())
                    continue;

                RoleInfo info = null;

                var itemParts = item.Split(':');
                if (itemParts.Length == 1)
                    info = new RoleInfo { RoleName = item };
                else if (itemParts.Length == 2)
                    info = new RoleInfo
                    {
                        ExamType = itemParts[0].Trim().NullIfEmpty(),
                        RoleName = itemParts[1].Trim().NullIfEmpty(),
                    };

                if (info.RoleName.IsNotEmpty())
                    result.Add(info);
            }

            return result.ToArray();
        }

        private void CreateEventVariables(MessageVariableList list, QEvent @event, QGroupAddress venueAddress, QRegistration[] registrations, QAttempt[] attempts)
        {
            var builder = new EventVariableBuilder(_helper);
            builder.ExamEvent(list, @event, venueAddress);
            builder.CandidateRegistrationTable(list, registrations);
            builder.CandidateAuthenticationTable(list, registrations);

            var isSingleForm = !string.IsNullOrEmpty(list.GetValue("FormTitle"));

            builder.CandidateSubmissionTable(list, registrations, attempts, isSingleForm);
            builder.CandidatePublicationTable(list, registrations, attempts);

            var forms = registrations
                .Select(x => new { Identifier = x.ExamFormIdentifier, Title = x.Form?.FormTitle })
                .Distinct()
                .Select(x => new EventVariableBuilder.FormItem { Identifier = x.Identifier, Title = x.Title })
                .ToArray();

            builder.FormCompetencyTable(list, registrations, attempts, isSingleForm, forms);
            builder.CandidateCompetencyTable(list, registrations, attempts, isSingleForm, forms);
        }

        private void CreateDelivery(Notification notification, EmailDraft email, EmailAddressList recipients, MessageVariableList variables, List<string> attachments = null)
        {
            if (recipients.Count == 0)
                return;

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.OrganizationIdentifier = OrganizationIdentifiers.SkilledTradesBC;
            email.MailoutScheduled = DateTimeOffset.UtcNow;

            email.Recipients = recipients;
            email.ContentVariables = variables.ToDictionary();
            email.ContentAttachments = attachments ?? new List<string>();
        }

        private void CreateFormVariables(MessageVariableList list, Form form)
        {
            list.AddValue("FormName", form.Name);
            list.AddValue("FormTitle", form.Content.Title.Default);
            list.AddValue("MaterialsPermittedToCandidates", form.Specification.Bank.Content.MaterialsForParticipation.Default.IfNullOrEmpty("None"));
        }

        private void CreateFormVariables(MessageVariableList list, QBankForm form)
        {
            list.AddValue("FormTitle", form.FormTitle);
        }

        private string DownloadFile(string url, string name)
        {
            var folder = _filePaths.GetPhysicalPathToShareFolder("Files", "Temp", "SkilledTradesBC");
            Directory.CreateDirectory(folder);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var path = folder + name;

            if (File.Exists(path))
                return path;

            using (var client = new WebClient())
                client.DownloadFile(url, path);

            return path;
        }

        public string[] GetContactCodes(List<QEventAttendee> contacts, string role)
        {
            var trainers = contacts
                .Where(x => x.Person.PersonCode != null && x.AttendeeRole.StartsWith(role))
                .Select(x => x.Person.PersonCode);

            var codes = new List<string>();

            foreach (var trainer in trainers)
                codes.Add(trainer);

            return codes.ToArray();
        }

        public Guid[] GetContactIdentifiers(ICollection<QEventAttendee> attendees, string role)
        {
            return attendees
                .Where(x => x.AttendeeRole.StartsWith(role))
                .Select(x => x.Person.UserIdentifier)
                .ToArray();
        }

        private EmailAddressList GetRecipients(Notification notification, QEvent @event, QRegistration[] registrations)
        {
            var result = new EmailAddressList();
            var data = BindRecipients(notification.RecipientFunction, @event, registrations);

            foreach (var item in data)
            {
                var email = EmailAddress.GetEnabledEmail(item.UserEmail, item.UserEmailEnabled, item.UserEmailAlternate, item.UserEmailAlternateEnabled);
                if (email.IsEmpty())
                    continue;

                result.Add(
                    new EmailAddress(item.UserIdentifier, email, item.UserName, item.PersonCode, item.PersonLanguage)
                    {
                        Variables = { { "CandidateCode", item.PersonCode } }
                    });
            }

            return result;
        }
    }
}
