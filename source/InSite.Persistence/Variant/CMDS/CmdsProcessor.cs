using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public partial class CmdsProcessor
    {
        private EmailDraft BuildEmail(CmdsCollegeCertificationRequested e)
        {
            var notice = Notifications.Select(NotificationType.CmdsCollegeCertificationRequested);
            var variables = BuildVariableList(e);

            return BuildEmail(OrganizationIdentifiers.CMDS, notice, variables, null);

            MessageVariableList BuildVariableList(CmdsCollegeCertificationRequested request)
            {
                var list = new MessageVariableList();

                list.AddValue("Name", request.Name);
                list.AddValue("Email", request.Email);

                list.AddValue("ProfileNumber", request.ProfileNumber);
                list.AddValue("ProfileTitle", request.ProfileTitle);
                list.AddValue("Institution", request.Institution);

                list.AddValue("CoreHoursRequired", request.CoreHoursRequired);
                list.AddValue("CoreHoursCompleted", request.CoreHoursCompleted);
                list.AddValue("NonCoreHoursRequired", request.NonCoreHoursRequired);
                list.AddValue("NonCoreHoursCompleted", request.NonCoreHoursCompleted);

                return list;
            }
        }

        private EmailDraft BuildEmail(CmdsCompetenciesExpired e)
        {
            var notice = Notifications.Select(NotificationType.CmdsCompetenciesExpired);
            var email = MessageRepository.GetEmail(OrganizationIdentifiers.CMDS, notice.Type);

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.MailoutScheduled = DateTimeOffset.UtcNow.AddMinutes(1);
            email.ContentVariables = BuildVariableList().ToDictionary();
            email.Recipients = BuildRecipientList();

            email.IgnoreSubscribers = true;

            return email;

            EmailAddressList BuildRecipientList()
            {
                // Find all of the CMDS competencies for this contact that are expiring.
                // Exclude those for which any notification has already been done.

                var subscribers = MessageRepository.GetSubscribers(email.OrganizationIdentifier, email.MessageIdentifier.Value);

                var list = new EmailAddressList();
                var expiries = CompetencyRepository.SelectExpiredCompetencies(e.Notified);
                var people = expiries.Select(x => x.UserIdentifier).Distinct().ToList();
                var subscriptions = subscribers
                    .Where(x => people.Contains(x.UserIdentifier))
                    .OrderBy(x => x.UserEmail)
                    .ToList();

                foreach (var subscription in subscriptions)
                {
                    var competencies = expiries
                        .Where(x => x.UserIdentifier == subscription.UserIdentifier && subscription.Subscribed < x.DateExpired)
                        .OrderBy(x => x.CompetencyTitle)
                        .ToList();

                    if (competencies.Count == 0)
                        continue;

                    var recipient = MapRecipientHandle(e.OriginOrganization, subscription);
                    if (recipient == null)
                        continue;

                    var markdown = CreateMarkdownForCompetencyList(competencies);

                    recipient.Variables.Add("FirstName", subscription.UserFirstName);
                    recipient.Variables.Add("CompetencyMarkdown", markdown);

                    foreach (var follower in subscription.Followers)
                        recipient.Cc.Add(follower);

                    list.Add(recipient);

                    email.ContentCompetencies.Add(subscription.UserIdentifier, competencies.Select(x => x.CompetencyStandardIdentifier).ToArray());
                }

                return list;
            }

            MessageVariableList BuildVariableList()
            {
                var list = new MessageVariableList();
                list.AddValue("SignInUrl", $"${MessageVariable.AppUrl}/ui/lobby/signin");
                return list;
            }

            string CreateMarkdownForCompetencyList(List<CompetencyRepository.ExpiringCompetency> competencies)
            {
                var md = new StringBuilder();

                var companies = competencies.Select(x => new { x.OrganizationIdentifier, x.CompanyName, x.DepartmentNames })
                    .OrderBy(x => x.CompanyName)
                    .Distinct()
                    .ToList();

                foreach (var company in companies)
                {
                    md.AppendFormat("**{0}:** ", company.CompanyName);
                    md.AppendFormat("{0}\n\n", company.DepartmentNames);
                    md.AppendLine();
                    md.AppendLine();

                    var list = competencies
                        .Where(x => x.OrganizationIdentifier == company.OrganizationIdentifier)
                        .OrderBy(x => x.CompetencyTitle)
                        .ToList();

                    md.AppendFormat("**Competencies Expired ({0}):**", list.Count);
                    md.AppendLine();
                    md.AppendLine();
                    foreach (var item in list)
                    {
                        md.Append($"*{item.CompetencyNumber}*: {item.CompetencyTitle}\n\n- Validated {item.DateCompleted:MMMM d, yyyy} - Expires {item.DateExpired:MMMM d, yyyy}");
                        md.AppendLine();
                        md.AppendLine();
                    }
                }

                return md.ToString();
            }
        }

        private EmailDraft BuildEmail(CmdsCompetencyChanged e)
        {
            var notice = Notifications.Select(NotificationType.CmdsCompetencyChanged);
            var variables = BuildVariableList(e.Author, e.Change);

            return BuildEmail(OrganizationIdentifiers.CMDS, notice, variables, null);

            MessageVariableList BuildVariableList(string author, string change)
            {
                var list = new MessageVariableList();

                list.AddValue("AuthorName", author);
                list.AddValue("ChangeMarkdown", change);

                return list;
            }
        }

        private EmailDraft BuildEmail(CmdsAchievementChanged e)
        {
            var notice = Notifications.Select(NotificationType.CmdsResourceChanged);
            var variables = BuildVariableList(e.Change, e.AuthorName, e.AuthorEmail, e);

            return BuildEmail(OrganizationIdentifiers.CMDS, notice, variables, null);

            MessageVariableList BuildVariableList(string change, string authorName, string authorEmail, CmdsAchievementChanged resource)
            {
                var list = new MessageVariableList();

                list.AddValue("ChangeType", change);

                list.AddValue("AuthorName", authorName);
                list.AddValue("AuthorEmail", authorEmail);

                list.AddValue("AchievementTitle", resource.Title);
                list.AddValue("ResourceDescription", resource.Description);
                list.AddValue("ResourceHours", resource.Hours);

                if (resource.Lifetime > 0)
                    list.AddValue("ResourceLifetime", $"{resource.Lifetime} Months");
                else
                    list.AddValue("ResourceLifetime", string.Empty);

                return list;
            }
        }

        private EmailDraft BuildEmail(CmdsAchievementExpirationDelivered e)
        {
            if (e.ReminderType == ReminderType.Today)
                return BuildEmailForExpiredResources(e);
            else
                return BuildEmailForExpiringResources(e);
        }

        private EmailDraft BuildEmailForExpiredResources(CmdsAchievementExpirationDelivered e)
        {
            NotificationType type = NotificationType.AchievementCredentialsExpiredToday;

            var notice = Notifications.Select(type);
            var email = MessageRepository.GetEmail(OrganizationIdentifiers.CMDS, notice.Type);

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.MailoutScheduled = DateTimeOffset.UtcNow.AddMinutes(1);
            email.ContentVariables = BuildVariableList().ToDictionary();
            email.ContentPriority = MessageRepository.GetEmailPriority(type);
            email.Recipients = BuildRecipientList();

            email.IgnoreSubscribers = true;

            return email;

            EmailAddressList BuildRecipientList()
            {
                // Find all of the CMDS resources for this contact that are expiring.
                // Exclude those for which any notification has already been done.

                var subscribers = MessageRepository.GetSubscribers(email.OrganizationIdentifier, email.MessageIdentifier.Value);

                var list = new EmailAddressList();

                var expiries = VCmdsAchievementSearch
                    .SelectCredentialExpirations(ReminderType.Today)
                    .GroupBy(x => x.Credential.UserIdentifier)
                    .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Credential.AchievementTitle).ToArray());

                var subscriptions = subscribers
                    .Where(x => expiries.ContainsKey(x.UserIdentifier))
                    .OrderBy(x => x.UserEmail)
                    .ToArray();

                foreach (var subscription in subscriptions)
                {
                    var credentials = expiries[subscription.UserIdentifier]
                        .Where(x => subscription.Subscribed < x.Credential.CredentialExpired)
                        .ToArray();

                    if (credentials.Length == 0)
                        continue;

                    var recipient = MapRecipientHandle(e.OriginOrganization, subscription);
                    if (recipient == null)
                        continue;

                    var markdown = CreateMarkdownForResourceList(credentials);

                    recipient.Variables.Add("FirstName", subscription.UserFirstName);
                    recipient.Variables.Add("CredentialsMarkdown", markdown);

                    foreach (var follower in subscription.Followers)
                        recipient.Cc.Add(follower);

                    list.Add(recipient);

                    email.ContentCredentials.Add(subscription.UserIdentifier, credentials.Select(x => x.Credential.CredentialIdentifier).ToArray());
                }

                return list;
            }

            MessageVariableList BuildVariableList()
            {
                var list = new MessageVariableList();
                list.AddValue("SignInUrl", $"${MessageVariable.AppUrl}/ui/lobby/signin");
                return list;
            }

            string CreateMarkdownForResourceList(IEnumerable<VCmdsAchievementSearch.CredentialExpirationModel> resources)
            {
                var md = new StringBuilder();

                var companies = resources.Select(x => new { x.OrganizationIdentifier, x.OrganizationName, x.DepartmentNames })
                    .OrderBy(x => x.OrganizationName)
                    .Distinct()
                    .ToList();

                foreach (var company in companies)
                {
                    md.AppendFormat("**{0}:** ", company.OrganizationName);
                    md.AppendFormat("{0}\n\n", company.DepartmentNames);
                    md.AppendLine();
                    md.AppendLine();

                    var list = resources
                        .Where(x => x.OrganizationIdentifier == company.OrganizationIdentifier)
                        .OrderBy(x => x.Credential.AchievementLabel)
                        .ThenBy(x => x.Credential.AchievementTitle)
                        .ToList();

                    md.AppendFormat("**Achievements Expired ({0}):**", list.Count);
                    md.AppendLine();
                    md.AppendLine();
                    foreach (var item in list)
                    {
                        md.Append($"- {item.Credential.AchievementLabel} :: {item.Credential.AchievementTitle} - *expired {item.Credential.CredentialExpired:MMMM d, yyyy}*\n\n");
                        md.AppendLine();
                        md.AppendLine();
                    }
                }

                return md.ToString();
            }
        }

        private EmailDraft BuildEmailForExpiringResources(CmdsAchievementExpirationDelivered e)
        {
            var type = NotificationType.AchievementCredentialsExpiringInOneMonth;

            if (e.ReminderType == ReminderType.InOneMonth)
                type = NotificationType.AchievementCredentialsExpiringInOneMonth;

            if (e.ReminderType == ReminderType.InTwoMonths)
                type = NotificationType.AchievementCredentialsExpiringInTwoMonths;

            if (e.ReminderType == ReminderType.InThreeMonths)
                type = NotificationType.AchievementCredentialsExpiringInThreeMonths;

            var notice = Notifications.Select(type);
            var email = MessageRepository.GetEmail(OrganizationIdentifiers.CMDS, notice.Type);

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.MailoutScheduled = DateTimeOffset.UtcNow.AddMinutes(1);
            email.ContentVariables = BuildVariableList().ToDictionary();
            email.ContentPriority = MessageRepository.GetEmailPriority(type);
            email.Recipients = BuildRecipientList(e.ReminderType);

            email.IgnoreSubscribers = true;

            return email;

            EmailAddressList BuildRecipientList(ReminderType reminderType)
            {
                var subscribers = MessageRepository.GetSubscribers(email.OrganizationIdentifier, email.MessageIdentifier.Value);

                var list = new EmailAddressList();
                var expirations = VCmdsAchievementSearch.SelectCredentialExpirations(reminderType);
                var people = expirations.Select(x => x.Credential.UserIdentifier).Distinct().ToList();
                var subscriptions = subscribers
                    .Where(x => people.Contains(x.UserIdentifier))
                    .OrderBy(x => x.UserEmail)
                    .ToList();

                foreach (var subscription in subscriptions)
                {
                    var resources = expirations
                        .Where(x => x.Credential.UserIdentifier == subscription.UserIdentifier && subscription.Subscribed < x.Credential.CredentialExpirationExpected)
                        .OrderBy(x => x.Credential.AchievementTitle)
                        .ToList();
                    if (resources.Count == 0)
                        continue;

                    var recipient = MapRecipientHandle(e.OriginOrganization, subscription);
                    if (recipient == null)
                        continue;

                    var markdown = CreateMarkdownForResourceList(resources, reminderType);

                    recipient.Variables.Add("FirstName", subscription.UserFirstName);
                    recipient.Variables.Add("CredentialsMarkdown", markdown);

                    foreach (var follower in subscription.Followers)
                        recipient.Cc.Add(follower);

                    list.Add(recipient);

                    email.ContentCredentials.Add(subscription.UserIdentifier, resources.Select(x => x.Credential.CredentialIdentifier).ToArray());
                }

                return list;
            }

            MessageVariableList BuildVariableList()
            {
                var list = new MessageVariableList();
                list.AddValue("SignInUrl", $"${MessageVariable.AppUrl}/ui/lobby/signin");
                return list;
            }

            string CreateMarkdownForResourceList(IEnumerable<VCmdsAchievementSearch.CredentialExpirationModel> resources, ReminderType reminderType)
            {
                var md = new StringBuilder();

                var companies = resources.Select(x => new { x.OrganizationIdentifier, x.OrganizationName, x.DepartmentNames })
                    .OrderBy(x => x.OrganizationName)
                    .Distinct()
                    .ToList();

                foreach (var company in companies)
                {
                    md.AppendFormat("**{0}:** ", company.OrganizationName);
                    md.AppendFormat("{0}\n\n", company.DepartmentNames);
                    md.AppendLine();
                    md.AppendLine();

                    var list = resources
                        .Where(x => x.OrganizationIdentifier == company.OrganizationIdentifier)
                        .OrderBy(x => x.Credential.AchievementLabel)
                        .OrderBy(x => x.Credential.AchievementTitle)
                        .ToList();

                    md.AppendFormat("**Achievements Expiring Within {0} ({1}):**", Shift.Common.Humanizer.ToQuantity((int)reminderType, "Month"), list.Count);
                    md.AppendLine();
                    md.AppendLine();

                    foreach (var item in list)
                    {
                        md.Append($"- {item.Credential.AchievementLabel} :: {item.Credential.AchievementTitle} - *expires {item.Credential.CredentialExpirationExpected:MMMM d, yyyy}*\n\n");
                        md.AppendLine();
                        md.AppendLine();
                    }
                }

                return md.ToString();
            }
        }

        private EmailDraft BuildEmail(CmdsTrainingRegistrationSubmitted e)
        {
            var notice = Notifications.Select(NotificationType.CmdsTrainingRegistrationSubmitted);
            var variables = BuildVariableList(e);

            return BuildEmail(OrganizationIdentifiers.CMDS, notice, variables, null);

            MessageVariableList BuildVariableList(CmdsTrainingRegistrationSubmitted request)
            {
                var list = new MessageVariableList();

                list.AddValue("SessionIdentifier", request.SessionIdentifier);
                list.AddValue("SessionTitle", request.SessionTitle);

                list.AddValue("RegistrantName", request.RegistrantName);
                list.AddValue("RegistrantEmail", request.RegistrantEmail);
                list.AddValue("RegistrantCompany", request.RegistrantCompany);

                list.AddValue("Comment", request.Comment);

                return list;
            }
        }

        private EmailDraft BuildEmail(Guid organization, Notification notification, MessageVariableList variables, StringCollection attachments)
        {
            var email = MessageRepository.GetEmail(organization, notification.Type);

            email.MailoutIdentifier = UniqueIdentifier.Create();
            email.MailoutScheduled = DateTimeOffset.UtcNow.AddMinutes(1);
            email.ContentVariables = variables.ToDictionary();
            email.ContentAttachments = attachments != null ? attachments.Cast<string>().ToList() : new List<string>();
            email.Recipients = GetSubscribers();

            email.IgnoreSubscribers = true;

            return email;

            EmailAddressList GetSubscribers()
            {
                var subscribers = MessageRepository.GetSubscribers(email.OrganizationIdentifier, email.MessageIdentifier.Value);

                var list = new EmailAddressList();

                foreach (var subscription in subscribers)
                {
                    var recipient = MapRecipientHandle(organization, subscription);
                    if (recipient != null)
                        list.Add(recipient);
                }

                return list;
            }
        }

        private EmailAddress MapRecipientHandle(Guid organization, EmailSubscriber item)
        {
            var email = item.UserEmail; // CMDS users are not permitted to opt-out from CMDS notifications.
            // var email = EmailAddress.GetEnabledEmail(item.UserEmail, item.UserEmailEnabled, item.UserEmailAlternate, item.UserEmailAlternateEnabled);
            if (email.IsEmpty())
                return null;

            var lang = _contacts.GetPersonLanguage(item.UserIdentifier, organization);

            return new EmailAddress(item.UserIdentifier, email, item.UserFullName, item.PersonCode, lang);
        }

        #region Methods (mailout send)

        public bool Send(EmailDraft message, Notification notice, NotificationMode mode, DateTimeOffset when, Action<ICommand> send)
        {
            if (notice == null || message == null || message.MailoutStatus == "Disabled")
                return false;

            if (mode != NotificationMode.Silent)
            {
                if (mode == NotificationMode.Counterfeit)
                    message.MailoutCompleted = when.UtcDateTime;

                _postOffice.Send(message);
            }

            if (notice.Type == NotificationType.CmdsCompetenciesExpired)
                SentCmdsCompetenciesExpired(message, when);
            else
                CredentialExpirationReminderDelivered(message, when, GetReminderType(notice.Type), send);

            return true;
        }

        private void SentCmdsCompetenciesExpired(EmailDraft email, DateTimeOffset notified)
        {
            foreach (var user in email.ContentCompetencies.Keys)
                foreach (var resourceID in email.ContentCompetencies[user])
                    CompetencyRepository.UpdateNotified(resourceID, user, notified);
        }

        private void CredentialExpirationReminderDelivered(EmailDraft email, DateTimeOffset notified, ReminderType type, Action<ICommand> send)
        {
            var learners = email.ContentCredentials.Keys.Distinct().ToList();

            foreach (var learner in learners)
            {
                var credentials = email.ContentCredentials[learner].Distinct().ToList();

                foreach (var credential in credentials)
                {
                    send(new DeliverExpirationReminder(credential, type, notified));
                }
            }
        }

        private ReminderType GetReminderType(NotificationType type)
        {
            if (type == NotificationType.AchievementCredentialsExpiredToday)
                return ReminderType.Today;
            else if (type == NotificationType.AchievementCredentialsExpiringInOneMonth)
                return ReminderType.InOneMonth;
            else if (type == NotificationType.AchievementCredentialsExpiringInTwoMonths)
                return ReminderType.InTwoMonths;
            else if (type == NotificationType.AchievementCredentialsExpiringInThreeMonths)
                return ReminderType.InThreeMonths;
            else
                throw new NotImplementedException();
        }

        #endregion
    }
}