using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Messages.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Invoices;
using InSite.Domain.Messages;
using InSite.Domain.Registrations;
using InSite.Domain.Users;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class CoreProcessManager
    {
        private readonly IChangeQueue _publisher;
        private readonly IEmailOutbox _postOffice;

        private readonly IContactSearch _contacts;
        private readonly IContentSearch _contents;
        private readonly IRegistrationSearch _registrations;

        public CoreProcessManager(
            IChangeQueue publisher,
            IEmailOutbox postOffice,
            IRegistrationSearch registrations,
            IContentSearch contents,
            IContactSearch contacts
            )
        {
            _publisher = publisher;
            _postOffice = postOffice;

            _contacts = contacts;
            _contents = contents;
            _registrations = registrations;

            _publisher.Subscribe<InvoicePaid>(Handle);
            _publisher.Subscribe<RegistrantContactInformationChanged>(Handle);
            _publisher.Subscribe<UserAccountArchived>(Handle);
            _publisher.Subscribe<UserAccountWelcomed>(Handle);
        }

        public void Handle(InvoicePaid e)
        {

        }

        public void Handle(RegistrantContactInformationChanged e)
        {
            var notice = Notifications.Select(NotificationType.RegistrantContactInformationChanged);
            var variables = BuildVariableList();
            var draft = Build(e.OriginOrganization, notice.Type, variables, null);

            _postOffice.Send(draft);

            MessageVariableList BuildVariableList()
            {
                var registration = _registrations.GetRegistration(
                    e.AggregateIdentifier,
                    x => x.Candidate, x => x.Event);

                var list = new MessageVariableList();
                list.AddValue("ContactLoginName", registration.Candidate.UserFullName);
                list.AddValue("ContactEmail", registration.Candidate.UserEmail);
                list.AddValue("EventName", registration.Event.EventTitle);

                var md = new StringBuilder();
                md.AppendLine("Field Name | Old Value | New Value");
                md.AppendLine("-- | -- | --");
                foreach (var changedField in e.ChangedFields)
                    md.AppendLine($"{changedField.FieldName} | {changedField.OldValue} | {changedField.NewValue}");
                list.AddValue("ContactChangedFields", md.ToString());

                return list;
            }
        }

        public void Handle(UserAccountArchived e)
        {
            var notice = Notifications.Select(NotificationType.UserAccountArchived);
            var variables = BuildVariableList(e.PersonName, e.PersonEmail, e.Status);
            var draft = Build(e.TenantIdentifier, notice.Type, variables, null);

            _postOffice.Send(draft);

            MessageVariableList BuildVariableList(string name, string email, string status)
            {
                var list = new MessageVariableList();

                list.AddValue("Name", name);
                list.AddValue("Email", email);
                list.AddValue("Status", status);

                return list;
            }
        }

        public void Handle(UserAccountWelcomed e)
        {
            _postOffice.Send(CreateEmail(e, e.OriginOrganization));
        }

        private EmailDraft Build(
            Guid organization,
            NotificationType notification,
            MessageVariableList variables,
            StringCollection attachments)
        {
            var message = MessageRepository.GetEmail(organization, notification);

            var draft = EmailDraft.Create(
                message.OrganizationIdentifier,
                message.MessageIdentifier,
                message.SenderIdentifier,
                message.AutoBccSubscribers
                );

            draft.MailoutIdentifier = UniqueIdentifier.Create();
            draft.MailoutScheduled = DateTimeOffset.UtcNow.AddMinutes(1);

            draft.ContentSubject = message.ContentSubject.Clone();
            draft.ContentBody = message.ContentBody.Clone();
            draft.ContentVariables = variables.ToDictionary();
            draft.ContentAttachments = attachments != null ? attachments.Cast<string>().ToList() : new List<string>();

            var subscribers = MessageRepository.GetSubscribers(message.OrganizationIdentifier, message.MessageIdentifier.Value);

            foreach (var subscriber in subscribers)
            {
                var recipient = MapRecipientHandle(organization, subscriber);
                if (recipient != null)
                    draft.Recipients.Add(recipient);
            }

            return draft;
        }

        public EmailDraft CreateEmail(UserAccountWelcomed e, Guid organizationId)
        {
            var organization = OrganizationSearch.Select(organizationId);

            var notice = Notifications.Select(NotificationType.UserAccountWelcomed, organization.Code);

            if (notice == null && organization.ParentOrganizationIdentifier.HasValue)
            {
                var parentOrganization = OrganizationSearch.Select(organization.ParentOrganizationIdentifier.Value);
                if (parentOrganization != null)
                    notice = Notifications.Select(NotificationType.UserAccountWelcomed, parentOrganization.Code);
            }

            var variables = BuildVariableList(e.UserFirstName, e.UserEmail, e.UserPassword, e.UserPasswordHash, e.TenantName, e.TenantCode);
            var draft = Build(e.TenantIdentifier, notice.Type, variables, null);

            if (e.UserAccessGranted.HasValue)
            {
                var handle = _contacts.GetEmailAddress(e.UserIdentifier.Value, e.TenantIdentifier);
                if (handle != null)
                    draft.Recipients.Add(handle);
            }

            return draft;

            MessageVariableList BuildVariableList(
                string firstName,
                string email,
                string password,
                string hash,
                string companyTitle,
                string organizationCode)
            {
                var list = new MessageVariableList();

                list.AddValue("FirstName", firstName);
                list.AddValue("Email", email);
                list.AddValue("CompanyTitle", companyTitle);
                list.AddValue("SignInUrl", $"${MessageVariable.AppUrl}/ui/lobby/signin");

                if (string.IsNullOrEmpty(password) && PasswordHash.ValidatePassword(Default.CmdsPassword, hash))
                {
                    password = Default.CmdsPassword;
                }

                if (!string.IsNullOrEmpty(password))
                {
                    list.AddValue("Password", password);
                    AddTranslations(_contents, list, "Password Initial Help", "PasswordInstruction", x => string.Format(x, password));
                }
                else
                {
                    list.AddValue("Password", "**********");
                    AddTranslations(_contents, list, "Password Forget Help", "PasswordInstruction");
                }

                return list;
            }
        }

        public Dictionary<string, string> GetWelcomeMsgPasswordInfo(string passwordPlainText, string passwordHash)
        {
            var temporaryPassword = string.Empty;
            if (string.IsNullOrEmpty(passwordPlainText) && PasswordHash.ValidatePassword(Default.CmdsPassword, passwordHash))
                temporaryPassword = Default.CmdsPassword;
            else if (!string.IsNullOrEmpty(passwordPlainText) && PasswordHash.ValidatePassword(passwordPlainText, passwordHash))
                temporaryPassword = passwordPlainText;

            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(temporaryPassword))
            {
                dict.Add("Password", temporaryPassword);
                AddTranslations(_contents, dict, "Password Initial Help", "PasswordInstruction", x => string.Format(x, temporaryPassword));
            }
            else
            {
                dict.Add("Password", "**********");
                AddTranslations(_contents, dict, "Password Initial Help", "PasswordInstruction", x => string.Format(x, temporaryPassword));
            }
            return dict;
        }

        public static void AddTranslations(IContentSearch search, Dictionary<string, string> dict,
            string labelName, string varName, Func<string, string> format = null)
        {
            var contents = search
                .SelectContainers(x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier
                    && x.ContentLabel == labelName
                    && x.ContainerType == ContentContainerType.Application)
                .Where(x => x.OrganizationIdentifier == OrganizationIdentifiers.Global)
                .ToList();

            if (contents.Count == 0)
                return;

            if (format == null)
                format = x => x;

            var contentBlock = search.GetBlock(contents);

            var translations = contentBlock[labelName].Text;
            foreach (var translation in translations)
                dict.Add(varName + ":" + translation.Key, format(translation.Value));
        }

        public static void AddTranslations(IContentSearch search, MessageVariableList list,
            string labelName, string varName, Func<string, string> format = null)
        {
            var contents = search
                .SelectContainers(x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier
                    && x.ContentLabel == labelName
                    && x.ContainerType == ContentContainerType.Application)
                .Where(x => x.OrganizationIdentifier == OrganizationIdentifiers.Global)
                .ToList();

            if (contents.Count == 0)
                return;

            if (format == null)
                format = x => x;

            var contentBlock = search.GetBlock(contents);

            var translations = contentBlock[labelName].Text;
            foreach (var translation in translations)
                list.AddValue(varName + ":" + translation.Key, format(translation.Value));
        }

        private EmailAddress MapRecipientHandle(Guid organization, EmailSubscriber item)
        {
            var email = EmailAddress.GetEnabledEmail(
                item.UserEmail,
                item.UserEmailEnabled,
                item.UserEmailAlternate,
                item.UserEmailAlternateEnabled);

            if (email.IsEmpty())
                return null;

            var lang = _contacts.GetPersonLanguage(item.UserIdentifier, organization);

            return new EmailAddress(item.UserIdentifier, email, item.UserFullName, item.PersonCode, lang);
        }
    }
}