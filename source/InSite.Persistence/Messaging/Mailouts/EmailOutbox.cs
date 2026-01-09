using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class EmailOutbox : IEmailOutbox
    {
        private readonly MailgunServer _mailgun;

        private readonly EnvironmentName _environment;

        public EmailOutbox(MailgunServer mailgunSender, EnvironmentName environment)
        {
            _mailgun = mailgunSender;

            _environment = environment;
        }

        private class RecipientList
        {
            #region Classes

            public interface IRecipient
            {
                Guid? Identifier { get; }
                string Address { get; }
                string FirstName { get; }
                string LastName { get; }
                string FullName { get; }
                string PersonCode { get; }
                string Language { get; }
            }

            private class RecipientInfo : IRecipient
            {
                public Guid? Identifier { get; set; }
                public Guid? Organization { get; set; }
                public string Address { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string FullName { get; set; }
                public string PersonCode { get; set; }
                public string Language { get; set; }
            }

            #endregion

            #region Properties

            public IReadOnlyList<IRecipient> To => _isLocked ? _to : throw ApplicationError.Create("List is not locked");
            public IReadOnlyList<IRecipient> Cc => _isLocked ? _cc : throw ApplicationError.Create("List is not locked");
            public IReadOnlyList<IRecipient> Bcc => _isLocked ? _bcc : throw ApplicationError.Create("List is not locked");

            #endregion

            #region Fields

            private bool _isLocked = false;

            private List<RecipientInfo> _to = new List<RecipientInfo>();
            private List<RecipientInfo> _cc = new List<RecipientInfo>();
            private List<RecipientInfo> _bcc = new List<RecipientInfo>();

            #endregion

            #region Methods (add)

            public void AddTo(Guid user, Guid organization)
            {
                CheckIsLocked();
                AddItem(_to, user, organization);
            }

            public void AddTo(IEnumerable<Guid> users, Guid organization)
            {
                CheckIsLocked();

                if (users == null)
                    return;

                foreach (var user in users)
                    AddItem(_to, user, organization);
            }

            public void AddTo(IEnumerable<string> users, Guid organization)
            {
                CheckIsLocked();

                if (users == null)
                    return;

                foreach (var user in users)
                    AddItem(_to, user, organization);
            }

            public void AddTo(IEnumerable<EmailSubscriber> users, Guid organization)
            {
                CheckIsLocked();

                if (users == null)
                    return;

                foreach (var user in users)
                    AddItem(_to, user, organization);
            }

            public void AddCc(IEnumerable<Guid> users)
            {
                CheckIsLocked();

                if (users == null)
                    return;

                foreach (var user in users)
                    AddItem(_cc, user);
            }

            public void AddBcc(IEnumerable<Guid> users)
            {
                CheckIsLocked();

                if (users == null)
                    return;

                foreach (var user in users)
                    AddItem(_bcc, user);
            }

            public void AddBcc(IEnumerable<EmailSubscriber> users)
            {
                CheckIsLocked();

                if (users == null)
                    return;

                foreach (var user in users)
                    AddItem(_bcc, user);
            }

            private void CheckIsLocked()
            {
                if (_isLocked)
                    throw ApplicationError.Create("List is locked");
            }

            private void AddItem(List<RecipientInfo> list, Guid identifier, Guid? organization = null)
            {
                list.Add(new RecipientInfo
                {
                    Identifier = identifier,
                    Organization = organization
                });
            }

            private void AddItem(List<RecipientInfo> list, string address, Guid? organization = null)
            {
                if (EmailAddress.IsValidAddress(address))
                    return;

                list.Add(new RecipientInfo
                {
                    Address = address,
                    Organization = organization
                });
            }

            private void AddItem(List<RecipientInfo> list, EmailSubscriber user, Guid? organization = null)
            {
                var email = EmailAddress.GetEnabledEmail(user.UserEmail, user.UserEmailEnabled, user.UserEmailAlternate, user.UserEmailAlternateEnabled);
                if (email.IsEmpty())
                    return;

                list.Add(new RecipientInfo
                {
                    Identifier = user.UserIdentifier,
                    Organization = organization,
                    Address = email,
                    FirstName = user.UserFirstName,
                    LastName = user.UserLastName,
                    FullName = user.UserFullName,
                    PersonCode = user.PersonCode,
                    Language = user.Language
                });
            }

            #endregion

            #region Methods (analyse data)

            public void LoadRecipients(Guid organization)
            {
                _to = LoadRecipients(_to, new RecipientInfo[0], organization);
                _cc = LoadRecipients(_cc, _to, organization);
                _bcc = LoadRecipients(_bcc, _to.Concat(_cc), organization);

                _isLocked = true;
            }

            private static List<RecipientInfo> LoadRecipients(IEnumerable<RecipientInfo> input, IEnumerable<RecipientInfo> exclude, Guid organization)
            {
                var excludeIds = new HashSet<Guid>();
                var excludeAddresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var item in exclude)
                    AddExclude(item);

                var idItems = new List<RecipientInfo>();
                var addressItems = new List<RecipientInfo>();
                var otherItems = new List<RecipientInfo>();

                foreach (var item in input)
                {
                    var hasId = item.Identifier.HasValue;
                    var hasAddress = item.Address.IsNotEmpty();

                    if (hasId && !hasAddress)
                        idItems.Add(item);
                    else if (!hasId && hasAddress)
                        addressItems.Add(item);
                    else if (hasId && hasAddress)
                        otherItems.Add(item);
                }

                var result = new List<RecipientInfo>();

                foreach (var item in GetRecipientsById(idItems, organization))
                    AddItem(item);

                foreach (var item in GetRecipientsByAddress(addressItems, organization))
                    AddItem(item);

                foreach (var item in addressItems)
                    AddItem(item);

                foreach (var item in otherItems)
                    AddItem(item);

                var orgGroups = result
                    .Where(x => x.Identifier.HasValue && x.Organization.HasValue && x.Language.IsEmpty())
                    .GroupBy(x => x.Organization);
                foreach (var orgGroup in orgGroups)
                {
                    var filter = orgGroup.Select(x => x.Identifier.Value).ToArray();
                    var langMap = GetRecipientLanguages(filter, orgGroup.Key.Value);

                    foreach (var item in orgGroup)
                        if (langMap.TryGetValue(item.Identifier.Value, out var lang))
                            item.Language = lang;
                }

                return result;

                void AddItem(RecipientInfo item)
                {
                    var isExclude = excludeIds.Contains(item.Identifier.Value)
                        || excludeAddresses.Contains(item.Address);

                    if (isExclude)
                        return;

                    result.Add(item);

                    AddExclude(item);
                }

                void AddExclude(RecipientInfo item)
                {
                    if (item.Identifier.HasValue)
                        excludeIds.Add(item.Identifier.Value);

                    if (item.Address.IsNotEmpty())
                        excludeAddresses.Add(item.Address);
                }
            }

            private static IEnumerable<RecipientInfo> GetRecipientsById(IEnumerable<RecipientInfo> recipients, Guid organization)
            {
                if (!recipients.Any())
                    yield break;

                var data = GetRecipients(new PersonFilter
                {
                    OrganizationOrParentIdentifier = organization,
                    IncludeUserIdentifiers = recipients.Select(x => x.Identifier.Value).ToArray()
                }).ToDictionary(x => x.Identifier);

                foreach (var recipient in recipients)
                {
                    if (!data.TryGetValue(recipient.Identifier.Value, out var item))
                        continue;

                    recipient.Address = item.Address;
                    recipient.FirstName = item.FirstName;
                    recipient.LastName = item.LastName;
                    recipient.FullName = item.FullName;
                    recipient.PersonCode = item.PersonCode;

                    yield return recipient;
                }
            }

            private static IEnumerable<RecipientInfo> GetRecipientsByAddress(IEnumerable<RecipientInfo> recipients, Guid organization)
            {
                if (!recipients.Any())
                    yield break;

                var data = GetRecipients(new PersonFilter
                {
                    OrganizationOrParentIdentifier = organization,
                    EmailsExact = recipients.Select(x => x.Address).ToArray()
                })
                .ToDictionary(x => x.Address);

                foreach (var recipient in recipients)
                {
                    if (!data.TryGetValue(recipient.Address, out var item))
                        continue;

                    recipient.Identifier = item.Identifier;
                    recipient.FirstName = item.FirstName;
                    recipient.LastName = item.LastName;
                    recipient.FullName = item.FullName;
                    recipient.PersonCode = item.PersonCode;

                    yield return recipient;
                }
            }

            private static IEnumerable<RecipientInfo> GetRecipients(PersonFilter filter)
            {
                var persons = PersonCriteria.Bind(p => new
                {
                    p.UserIdentifier,
                    p.OrganizationIdentifier,
                    p.User.FirstName,
                    p.User.LastName,
                    p.User.FullName,
                    p.User.Email,
                    p.EmailEnabled,
                    p.User.EmailAlternate,
                    p.EmailAlternateEnabled,
                    p.PersonCode,
                    p.Language
                }, filter);

                var seenAddresses = new HashSet<string>();

                foreach (var person in persons)
                {
                    var email = EmailAddress.GetEnabledEmail(person.Email, person.EmailEnabled, person.EmailAlternate, person.EmailAlternateEnabled);
                    if (email.IsEmpty() || !seenAddresses.Add(email))
                        continue;

                    yield return new RecipientInfo
                    {
                        Identifier = person.UserIdentifier,
                        Organization = person.OrganizationIdentifier,
                        Address = email,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        FullName = person.FullName,
                        PersonCode = person.PersonCode,
                        Language = person.Language
                    };
                }
            }

            private static Dictionary<Guid, string> GetRecipientLanguages(Guid[] ids, Guid organization)
            {
                var data = UserSearch.Bind(
                    u => new
                    {
                        u.UserIdentifier,
                        PersonLanguage = u.Persons
                            .Where(p => p.OrganizationIdentifier == organization)
                            .Select(p => p.Language)
                            .FirstOrDefault()
                    },
                    new UserFilter
                    {
                        IncludeUserIdentifiers = ids
                    });

                return data.ToDictionary(x => x.UserIdentifier, x => x.PersonLanguage.IfNullOrEmpty(MultilingualString.DefaultLanguage));
            }

            #endregion
        }

        public void Send(EmailDraft email, string tag, bool isUnitTest = false, string type = null)
        {
            if (!email.SenderEnabled || email.RecipientListTo.Count == 0)
                return;

            email.MailoutSucceeded = false;
            email.MailoutStatusCode = null;
            email.MailoutStatus = "Pending";

            if (email.SenderType == "Mailgun")
            {
                if (email.RecipientListTo != null && email.RecipientListTo.Count == 1)
                {
                    var item = email.RecipientListTo.Single();
                    var envelope = CreateEmailVariables(item.Key, item.Value);

                    SendUsingMailgun(email, envelope, tag, type);
                }
                else
                {
                    foreach (var item in email.RecipientListTo)
                    {
                        var _email = CopyEmail(email, item.Key, item.Value);
                        var envelope = CreateEmailVariables(item.Key, item.Value);

                        SendUsingMailgun(_email, envelope, tag, type);
                    }
                }
            }

            EmailVariables CreateEmailVariables(Guid userId, string address)
            {
                var result = new EmailVariables(userId, address, email.OrganizationIdentifier, email.ContentVariables);

                if (result.RecipientEmail.IsEmpty())
                    result.RecipientEmail = address;

                var person = ServiceLocator.PersonSearch.GetPerson(userId, email.OrganizationIdentifier, x => x.User);

                if (person != null)
                {
                    if (!result.RecipientIdentifier.HasValue)
                        result.RecipientIdentifier = person.UserIdentifier;

                    if (result.RecipientCode.IsEmpty())
                        result.RecipientCode = person.PersonCode;

                    if (result.RecipientName.IsEmpty())
                        result.RecipientName = person.User.FullName;

                    if (result.RecipientNameFirst.IsEmpty())
                        result.RecipientNameFirst = person.User.FirstName;

                    if (result.RecipientNameLast.IsEmpty())
                        result.RecipientNameLast = person.User.LastName;

                    if (!result.OrganizationIdentifier.HasValue)
                        result.OrganizationIdentifier = person.OrganizationIdentifier;
                }

                return result;
            }
        }

        public void Send(EmailDraft email)
        {
            if (email.Recipients.Count == 0)
                return;

            var sender = TSenderSearch.Select(email.SenderIdentifier);

            if (sender.SenderType == "Mailgun")
                ScheduleUsingMailgun(email, sender, "Post Office");
        }

        private void ScheduleUsingMailgun(EmailDraft draft, TSender sender, string tag)
        {
            PrepareToSendMailout(draft);

            foreach (var recipient in draft.Recipients)
            {
                if (recipient.Identifier == null)
                    throw new Exception($"The identifier for this recipient ({recipient.Address}) cannot be null.");

                var message = MessageHelper.BuildMessage(draft, recipient.Language);

                var email = EmailDraft.Create(
                    draft.OrganizationIdentifier,
                    draft.MessageIdentifier,
                    draft.SenderIdentifier,
                    draft.AutoBccSubscribers
                    );

                email.MailoutIdentifier = draft.MailoutIdentifier;
                email.SurveyIdentifier = draft.SurveyIdentifier;
                email.SurveyNumber = draft.SurveyNumber;
                email.UserIdentifier = UserIdentifiers.Root;

                email.SenderEnabled = true;
                email.SenderType = "Mailgun";
                email.SystemMailbox = sender.SystemMailbox;
                email.SenderEmail = sender.SenderEmail;
                email.SenderName = sender.SenderName;

                email.RecipientListTo = new Dictionary<Guid, string> { { recipient.Identifier.Value, recipient.Address } };
                email.RecipientListCc = GetEmailAddresses(draft.OrganizationIdentifier, recipient.Cc);
                email.RecipientListBcc = GetEmailAddresses(draft.OrganizationIdentifier, recipient.Bcc);

                email.ContentSubject = new MultilingualString(message.Subject);
                email.ContentBody = new MultilingualString(message.Body);
                email.ContentPriority = draft.ContentPriority;
                email.ContentAttachments = draft.ContentAttachments.ToList();

                email.MailoutScheduled = draft.MailoutScheduled;

                if (recipient.Identifier == null)
                    throw new Exception($"No email address found for user identifier {recipient.Identifier}.");

                var envelope = new EmailVariables(recipient.Identifier.Value, recipient.Address, email.OrganizationIdentifier, recipient.Variables);

                SendUsingMailgun(email, envelope, tag);
            }
        }

        void PrepareToSendMailout(EmailDraft draft)
        {
            var mailout = ConvertDraftToMailout(draft);

            if (mailout.MailoutStatus == null)
                mailout.MailoutStatus = "Started";

            if (draft.Recipients != null && string.IsNullOrEmpty(mailout.RecipientListTo))
            {
                var to = new Dictionary<Guid, string>();

                foreach (var recipient in draft.Recipients)
                {
                    if (recipient.Identifier == null)
                        throw new Exception($"The identifier for this recipient ({recipient.Address}) cannot be null.");

                    to.Add(recipient.Identifier.Value, recipient.Address);
                }

                mailout.RecipientListTo = JsonConvert.SerializeObject(to);
            }

            MessageStore.InsertMailout(mailout);
        }

        private void SendUsingMailgun(EmailDraft email, EmailVariables envelope, string tag, string type = null)
        {
            var envPrefix = GetEnvironmentPrefix();

            var subject = envPrefix + email.ContentSubject.Default;
            subject = MessageHelper.ReplacePlaceholdersForMailgun(email.OrganizationIdentifier, email.SenderIdentifier, email.SurveyNumber, subject, envelope);

            var body = email.ContentBody.Default;
            body = MessageHelper.ReplacePlaceholdersForMailgun(email.OrganizationIdentifier, email.SenderIdentifier, email.SurveyNumber, body, envelope);
            body = MessageHelper.CreateHtmlBody(subject, body, false);

            email.ContentSubject.Default = subject;
            email.ContentBody.Default = body;

            if (_mailgun.SendEmail(email, tag, type))
                OnSendCompleted(email, envelope.RecipientIdentifier);
        }

        private static Dictionary<Guid, string> GetEmailAddresses(Guid organization, List<Guid> users)
        {
            if (users.IsEmpty())
                return new Dictionary<Guid, string>();

            var entities = PersonCriteria.Bind(
                x => new
                {
                    x.UserIdentifier,
                    x.User.Email,
                    x.EmailEnabled,
                    x.User.EmailAlternate,
                    x.EmailAlternateEnabled
                },
                new PersonFilter
                {
                    OrganizationOrParentIdentifier = organization,
                    IncludeUserIdentifiers = users.ToArray(),
                    EmailOrEmailAlternateEnabled = true
                });

            var result = new Dictionary<Guid, string>(entities.Length);

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];

                var email = EmailAddress.GetEnabledEmail(entity.Email, entity.EmailEnabled, entity.EmailAlternate, entity.EmailAlternateEnabled);
                if (email.IsNotEmpty() && !result.ContainsKey(entity.UserIdentifier))
                    result.Add(entity.UserIdentifier, email);
            }

            return result;
        }

        private string GetEnvironmentPrefix()
        {
            if (_environment == EnvironmentName.Production)
                return string.Empty;

            return $"[{_environment}] ";
        }

        public void SendAndReplacePlaceholders(EmailDraft email, string tag)
        {
            var replaceSmarterVariables =
                   email.SenderType != "SmarterMail"
                && email.RecipientListTo.Count == 1;

            if (replaceSmarterVariables)
            {
                var to = email.RecipientListTo.Single();

                var (subject, body) = ReplaceSmarterMailVariables(email.OrganizationIdentifier, to.Key, to.Value, email.ContentSubject.Default, email.ContentBody.Default);
                email.ContentSubject = new MultilingualString(subject);
                email.ContentBody = new MultilingualString(body);
            }

            Send(email, tag);
        }

        public EmailDraft[] Compose(
            EnvironmentName environment,
            NotificationType trigger,
            Guid organization,
            Guid user,
            Guid? recipient,
            Guid? message,
            StringDictionary variables,
            Guid[] to = null, Guid[] cc = null, Guid[] bcc = null
            )
        {
            return Compose(environment, trigger, organization, user, message, variables, draft =>
            {
                var subscribers = draft.IgnoreSubscribers
                    ? new List<EmailSubscriber>()
                    : MessageRepository.GetSubscribers(draft.OrganizationIdentifier, draft.MessageIdentifier.Value);

                var recipients = new RecipientList();

                if (recipient.HasValue)
                {
                    recipients.AddTo(recipient.Value, organization);
                    recipients.AddCc(cc);
                    recipients.AddBcc(bcc);

                    if (draft.AutoBccSubscribers)
                        recipients.AddBcc(subscribers);
                }
                else
                {
                    recipients.AddTo(subscribers, draft.OrganizationIdentifier);
                }

                if (to != null)
                    recipients.AddTo(to, organization);

                return recipients;
            });
        }

        private EmailDraft[] Compose(
            EnvironmentName environment,
            NotificationType trigger,
            Guid organizationId,
            Guid userId,
            Guid? messageId,
            StringDictionary variables,
            Func<EmailDraft, RecipientList> getRecipients
            )
        {
            var draft = messageId.HasValue
                ? MessageRepository.GetEmail(messageId.Value)
                : MessageRepository.GetEmail(organizationId, trigger);

            if (variables.IsNotEmpty())
            {
                foreach (string key in variables.Keys)
                    draft.ContentVariables[key] = variables[key];
            }

            var recipients = getRecipients(draft);

            recipients.LoadRecipients(organizationId);

            var sender = TSenderSearch.Select(draft.SenderIdentifier);
            var result = new List<QMailout>();

            foreach (var langGroup in recipients.To.GroupBy(x => x.Language.IfNullOrEmpty(MultilingualString.DefaultLanguage).ToLower()))
            {
                var message = MessageHelper.BuildMessage(draft, langGroup.Key);

                if (sender != null)
                    ReplaceSmarterMailVariableForMailgun(sender.SenderType, langGroup, message);

                var email = new QMailout
                {
                    MailoutIdentifier = UniqueIdentifier.Create(),
                    MessageIdentifier = draft.MessageIdentifier,
                    OrganizationIdentifier = organizationId,
                    SenderIdentifier = draft.SenderIdentifier,
                    UserIdentifier = userId,

                    ContentSubject = message.Subject,
                    ContentBodyHtml = message.Body
                };

                if (sender != null)
                {
                    email.SenderIdentifier = sender.SenderIdentifier;
                    email.SenderType = sender.SenderType;
                }

                email.RecipientListTo = GetEmails(langGroup);
                email.RecipientListCc = GetEmails(recipients.Cc);
                email.RecipientListBcc = GetEmails(recipients.Bcc);

                result.Add(email);
            }

            return ConvertMailoutToEmail(result);

            string GetEmails(IEnumerable<RecipientList.IRecipient> data)
            {
                if (data == null || data.Count() == 0)
                    return null;

                return JsonConvert.SerializeObject(data.ToDictionary(x => x.Identifier, x => x.Address));
            }
        }

        private static EmailDraft[] ConvertMailoutToEmail(List<QMailout> mailouts)
        {
            var list = new List<EmailDraft>();

            foreach (var mailout in mailouts)
            {
                var item = ConvertMailoutToEmail(mailout);
                if (item != null)
                    list.Add(item);
            }

            return list.ToArray();
        }

        public static EmailDraft ConvertMailoutToEmail(QMailout mailout)
        {
            var sender = TSenderSearch.Select(mailout.SenderIdentifier);

            if (sender == null)
                throw new SenderNotFoundException(mailout.SenderIdentifier);

            var email = EmailDraft.Create(
                mailout.OrganizationIdentifier,
                mailout.MessageIdentifier,
                mailout.SenderIdentifier,
                false
                );

            if (mailout.MessageIdentifier.HasValue)
            {
                var message = MessageSearch.Instance.GetMessage(mailout.MessageIdentifier.Value);
                if (message != null)
                    email.AutoBccSubscribers = message.AutoBccSubscribers;
            }

            email.MailoutIdentifier = mailout.MailoutIdentifier;
            email.UserIdentifier = mailout.UserIdentifier ?? UserIdentifiers.Someone;
            email.ContentBody = new MultilingualString(mailout.ContentBodyHtml);
            email.ContentSubject = new MultilingualString(mailout.ContentSubject);
            email.SenderStatus = mailout.SenderStatus;
            email.MailoutStatusCode = mailout.MailoutStatusCode;
            email.MailoutStatus = mailout.MailoutStatus;
            email.SenderEnabled = sender.SenderEnabled;
            email.SenderType = sender.SenderType;
            email.SenderEmail = sender.SenderEmail;
            email.SenderName = sender.SenderName;
            email.SystemMailbox = sender.SystemMailbox;
            email.MailoutSucceeded = mailout.MailoutStatus == "Completed" || mailout.MailoutStatus == "Succeeded";
            email.MailoutScheduled = mailout.MailoutScheduled;

            if (mailout.ContentAttachments.IsNotEmpty())
                email.ContentAttachments = JsonConvert.DeserializeObject<List<string>>(mailout.ContentAttachments);

            if (!string.IsNullOrEmpty(mailout.RecipientListTo))
                email.RecipientListTo = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(mailout.RecipientListTo);

            if (!string.IsNullOrEmpty(mailout.RecipientListCc))
                email.RecipientListCc = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(mailout.RecipientListCc);

            if (!string.IsNullOrEmpty(mailout.RecipientListBcc))
                email.RecipientListBcc = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(mailout.RecipientListBcc);

            return email;
        }

        private EmailDraft CopyEmail(EmailDraft email, Guid recipientId, string recipientEmail)
        {
            var draft = EmailDraft.Create(
                email.OrganizationIdentifier,
                email.MessageIdentifier,
                email.SenderIdentifier,
                email.AutoBccSubscribers
                );

            draft.ContentAttachments = email.ContentAttachments;
            draft.ContentBody = email.ContentBody;
            draft.ContentPriority = email.ContentPriority;
            draft.ContentSubject = email.ContentSubject;
            draft.ContentVariables = email.ContentVariables;
            draft.EventIdentifier = email.EventIdentifier;
            draft.MailoutIdentifier = UniqueIdentifier.Create();
            draft.MailoutScheduled = email.MailoutScheduled;
            draft.MailoutStatus = email.MailoutStatus;
            draft.MailoutStatusCode = email.MailoutStatusCode;
            draft.MailoutSucceeded = email.MailoutSucceeded;

            draft.RecipientListTo = new Dictionary<Guid, string>
            {
                { recipientId, recipientEmail }
            };
            draft.RecipientListCc = email.RecipientListCc;
            draft.RecipientListBcc = email.RecipientListBcc;

            draft.SenderEmail = email.SenderEmail;
            draft.SenderEnabled = email.SenderEnabled;
            draft.SenderName = email.SenderName;
            draft.SenderStatus = email.SenderStatus;
            draft.SenderType = email.SenderType;
            draft.SurveyIdentifier = email.SurveyIdentifier;
            draft.SurveyNumber = email.SurveyNumber;
            draft.SystemMailbox = email.SystemMailbox;
            draft.UserIdentifier = email.UserIdentifier;

            return draft;
        }

        /// <remarks>
        /// The mail merge feature in SmarterMail uses this convention for variables:
        ///    ##VariableName##
        /// 
        /// The URL for tracking clickthroughs looks like this:
        ///    /ui/lobby/messages/links/click?link={guid}&user=##RecipientIdentifier##
        ///    
        /// This does not work for an email sent using Mailgun because these variables are not dynamically replaced at 
        /// delivery time. Therefore (until SmarterMail is replaced), we need to replace this variable manually. To help 
        /// ensure there are no unexpected side-effects, this replacement is done only when the sender is Mailgun and 
        /// there is exactly one recipient.
        /// </remarks>
        private void ReplaceSmarterMailVariableForMailgun(
            string senderType,
            IEnumerable<RecipientList.IRecipient> data,
            MessageHelper.MessageInfo compiledMessage
            )
        {
            if (senderType != "Mailgun")
                return;

            var list = data.Where(x => x.Identifier.HasValue).ToList();
            if (list.Count != 1)
                return;

            var recipient = list[0];
            var handleList = new EmailAddressList
            {
                new EmailAddress(recipient.Identifier.Value, recipient.Address, recipient.FullName, recipient.PersonCode, recipient.Language)
                {
                    Variables =
                    {
                        ["FirstName"] = recipient.FirstName,
                        ["LastName"] = recipient.LastName
                    }
                }
            };

            var recipientData = DeliveryAdapter.ToDataTable(null, handleList);
            if (recipientData.Rows.Count != 1)
                return;

            var (subject, body) = ReplaceSmarterMailVariables(recipientData, 0, compiledMessage.Subject, compiledMessage.Body);

            compiledMessage.Subject = subject;
            compiledMessage.Body = body;
        }

        public static (string Subject, string Body) ReplaceSmarterMailVariables(
            DataTable recipientData,
            int recipientIndex,
            string subject,
            string body
            )
        {
            if (recipientData.Rows.Count == 0)
                return (subject, body);

            var row = recipientData.Rows[recipientIndex];

            foreach (DataColumn col in recipientData.Columns)
            {
                var dbValue = row[col];
                var value = dbValue != DBNull.Value ? dbValue.ToString() : null;

                var name = $"##{col.ColumnName}##";

                if (subject != null)
                    subject = subject.Replace(name, value);

                if (body != null)
                    body = body.Replace(name, value);
            }

            return (subject, body);
        }

        private static (string Subject, string Body) ReplaceSmarterMailVariables(
            Guid organizationId,
            Guid userId,
            string email,
            string subject,
            string body
            )
        {
            var person = PersonSearch.Select(organizationId, userId, x => x.User);
            if (person == null)
                return (subject, body);

            var recipient = new EmailAddress(userId, email, person.User.FullName, person.PersonCode, person.Language)
            {
                Variables =
                {
                    { "FirstName", person.User.FirstName },
                    { "LastName", person.User.LastName }
                }
            };

            var handleList = new EmailAddressList { recipient };
            var recipientData = DeliveryAdapter.ToDataTable(null, handleList);

            return ReplaceSmarterMailVariables(recipientData, 0, subject, body);
        }

        public static void OnSendCompleted(EmailDraft email, Guid? userId)
        {
            var mailout = TEmailSearch.Get(email.MailoutIdentifier);

            if (mailout == null)
            {
                mailout = ConvertDraftToMailout(email);
                if (string.IsNullOrWhiteSpace(mailout.RecipientListTo))
                    return;

                MessageStore.InsertMailout(mailout);
            }
            else
            {
                mailout.MailoutStatus = email.MailoutStatus;
                mailout.MailoutStatusCode = email.MailoutStatusCode;
                mailout.MailoutError = email.MailoutErrorReason;

                if (mailout.MailoutCompleted == null)
                {
                    if (email.MailoutSucceeded || email.MailoutStatus == "Completed" || email.MailoutStatus == "Succeeded")
                        mailout.MailoutCompleted = DateTimeOffset.UtcNow;
                }

                mailout.SenderStatus = email.SenderStatus;

                MessageStore.UpdateMailout(mailout);
            }

            if (userId != null)
            {
                var recipient = MessageSearch.Instance.GetDeliveryToUser(mailout.MailoutIdentifier, userId.Value);

                if (recipient != null)
                {
                    recipient.DeliveryStatus = mailout.MailoutStatus;

                    if (recipient.DeliveryCompleted == null)
                    {
                        if (email.MailoutSucceeded || email.MailoutStatus == "Completed" || email.MailoutStatus == "Succeeded")
                            recipient.DeliveryCompleted = DateTimeOffset.UtcNow;
                    }

                    MessageStore.UpdateRecipient(recipient);
                }
            }
        }

        public static QMailout ConvertDraftToMailout(EmailDraft draft)
        {
            var sender = TSenderSearch.Select(draft.SenderIdentifier);

            if (sender == null)
                throw new SenderNotFoundException(draft.SenderIdentifier);

            var mailout = new QMailout
            {
                ContentBodyHtml = draft.ContentBody.Default,
                ContentSubject = draft.ContentSubject.Default,
                EventIdentifier = draft.EventIdentifier,
                MessageIdentifier = draft.MessageIdentifier,
                MailoutIdentifier = draft.MailoutIdentifier,
                MailoutScheduled = DateTimeOffset.UtcNow,
                MailoutStatus = draft.MailoutStatus,
                MailoutStatusCode = draft.MailoutStatusCode,
                OrganizationIdentifier = draft.OrganizationIdentifier,
                SenderIdentifier = draft.SenderIdentifier,
                SenderStatus = draft.SenderStatus,
                SenderType = sender.SenderType,
                UserIdentifier = draft.UserIdentifier,
                MailoutError = draft.MailoutErrorReason
            };

            if (draft.ContentAttachments.IsNotEmpty())
                mailout.ContentAttachments = JsonConvert.SerializeObject(draft.ContentAttachments);

            if (!Calendar.IsEmpty(draft.MailoutScheduled))
                mailout.MailoutScheduled = draft.MailoutScheduled.Value;

            if (mailout.MailoutIdentifier == Guid.Empty)
                mailout.MailoutIdentifier = UniqueIdentifier.Create();

            if (mailout.MessageIdentifier != null)
            {
                var message = MessageSearch.Instance.GetMessage(mailout.MessageIdentifier.Value);
                mailout.MessageType = message?.MessageType;
                mailout.MessageName = message?.MessageName;
            }

            if (mailout.MessageType == null)
                mailout.MessageType = "None";

            if (mailout.MessageName == null)
                mailout.MessageName = mailout.ContentSubject;

            if (mailout.MailoutStarted == null)
                if (draft.MailoutStatus == "Pending" || draft.MailoutStatus == "Completed" || draft.MailoutStatus == "Succeeded")
                    mailout.MailoutStarted = DateTimeOffset.UtcNow;

            if (mailout.MailoutCompleted == null)
                if (draft.MailoutStatus == "Completed" || draft.MailoutStatus == "Succeeded")
                    mailout.MailoutCompleted = DateTimeOffset.UtcNow;

            if (draft.RecipientListTo != null && draft.RecipientListTo.Any())
                mailout.RecipientListTo = JsonConvert.SerializeObject(draft.RecipientListTo);

            if (draft.RecipientListCc != null && draft.RecipientListCc.Any())
                mailout.RecipientListCc = JsonConvert.SerializeObject(draft.RecipientListCc);

            if (draft.RecipientListBcc != null && draft.RecipientListBcc.Any())
                mailout.RecipientListBcc = JsonConvert.SerializeObject(draft.RecipientListBcc);

            return mailout;
        }
    }
}
