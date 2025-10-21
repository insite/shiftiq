using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class DeliveryAdapter
    {
        private class DeliveryJob : IDeliveryJob
        {
            public string Language { get; set; }
            public EmailDraft Email { get; set; }
        }

        public static IDeliveryJob[] CreateEmailJobs(VMailout mailout, IEnumerable<QRecipient> deliveries)
        {
            var result = new List<DeliveryJob>();

            foreach (var langGroup in deliveries.GroupBy(x => x.PersonLanguage.IfNullOrEmpty(MultilingualString.DefaultLanguage).ToLower()))
            {
                var job = new DeliveryJob { Language = langGroup.Key };

                job.Email = MapToEmail(mailout, langGroup);
                job.Email.MailoutIdentifier = mailout.MailoutIdentifier;
                job.Email.MailoutCancelled = mailout.MailoutCancelled;
                job.Email.MailoutCompleted = mailout.MailoutCompleted;
                job.Email.MailoutScheduled = mailout.MailoutScheduled;
                job.Email.OrganizationIdentifier = mailout.OrganizationIdentifier;
                job.Email.SurveyIdentifier = mailout.SurveyIdentifier;
                job.Email.SurveyNumber = mailout.SurveyFormAsset;

                result.Add(job);
            }

            return result.ToArray();
        }

        private static EmailDraft MapToEmail(VMailout mailout, IEnumerable<QRecipient> deliveries)
        {
            var content = TContentSearch.Instance.GetBlock(mailout.MailoutIdentifier);

            var draft = EmailDraft.Create(
                    mailout.OrganizationIdentifier,
                    mailout.MessageIdentifier,
                    mailout.SenderIdentifier,
                    false
                );

            if (mailout.MessageIdentifier.HasValue)
            {
                var message = MessageSearch.Instance.GetMessage(mailout.MessageIdentifier.Value);
                if (message != null)
                    draft.AutoBccSubscribers = message.AutoBccSubscribers;
            }

            draft.ContentSubject = content.Title.Text;
            draft.ContentBody = content.Body.Text;
            draft.MessageIdentifier = mailout.MessageIdentifier ?? Guid.Empty;
            draft.Recipients = Map(deliveries);

            if (mailout.ContentAttachments.IsNotEmpty())
            {
                var attachments = JsonConvert.DeserializeObject<string[]>(mailout.ContentAttachments);

                foreach (var attachment in attachments)
                    draft.AttachFile(attachment);
            }

            if (mailout.ContentVariables.IsNotEmpty())
                draft.ContentVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(mailout.ContentVariables);

            return draft;
        }

        public static EmailAddressList Map(IEnumerable<QRecipient> entities)
        {
            var model = new EmailAddressList();

            foreach (var entity in entities)
            {
                var item = Map(entity);
                if (item != null)
                    model.Add(item);
            }

            return model;
        }

        public static EmailAddress Map(QRecipient entity)
        {
            if (entity.UserIdentifier == Guid.Empty || entity.UserIdentifier == UserIdentifiers.Someone)
                return null;

            var result = new EmailAddress(entity.UserIdentifier, entity.UserEmail, entity.PersonName, entity.PersonCode, entity.PersonLanguage);

            if (entity.RecipientVariables != null)
                result.Variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(entity.RecipientVariables);

            if (entity.CarbonCopies != null)
                result.Cc = entity.CarbonCopies.Select(x => x.UserIdentifier).ToList();

            return result;
        }

        public static DataTable ToDataTable(Guid? organization, EmailAddressList list)
        {
            var keys = list.GetVariableNames();

            // The recipient table must contain Email and Name columns.
            var table = new DataTable();
            table.Columns.Add("Email");
            table.Columns.Add("Name");
            table.Columns.Add("PersonCode");
            table.Columns.Add("Language");
            table.Columns.Add("RecipientIdentifier");
            table.Columns.Add("Cc");
            table.Columns.Add("Bcc");

            // Add custom mail-merge columns.
            foreach (var key in keys)
                table.Columns.Add(key);

            // Add rows.
            foreach (var recipient in list)
            {
                if (recipient.Address.IsEmpty())
                    continue;

                var row = table.NewRow();
                row["Email"] = recipient.Address;
                row["Name"] = recipient.DisplayName;
                row["PersonCode"] = recipient.Code;
                row["Language"] = recipient.Language;
                row["RecipientIdentifier"] = recipient.Identifier;
                row["Cc"] = GetEmailAddresses(organization, recipient.Cc);
                row["Bcc"] = GetEmailAddresses(organization, recipient.Bcc);

                foreach (var key in keys)
                {
                    if (recipient.Variables.ContainsKey(key))
                    {
                        var value = recipient.Variables[key];

                        if (key.EndsWith("Markdown"))
                            value = MessageHelper.CreateHtmlSnippet(value, true);

                        row[key] = value;
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private class UserEmailInfo
        {
            public string Email { get; set; }
            public string EmailAlternate { get; set; }
        }

        private static string GetEmailAddresses(Guid? organization, List<Guid> users)
        {
            if (users.IsEmpty())
                return null;

            UserEmailInfo[] emails;

            if (organization.HasValue)
            {
                var filter = new PersonFilter
                {
                    OrganizationIdentifier = organization.Value,
                    EmailEnabled = true,
                    IncludeUserIdentifiers = users.ToArray()
                };

                emails = PersonCriteria.Bind(
                    x => new UserEmailInfo
                    {
                        Email = x.User.Email,
                        EmailAlternate = x.User.EmailAlternate
                    },
                    filter);
            }
            else
            {
                var filter = new UserFilter
                {
                    IncludeUserIdentifiers = users.ToArray(),
                    PersonEmailEnabled = true
                };

                emails = UserSearch.Bind(
                    x => new UserEmailInfo
                    {
                        Email = x.Email,
                        EmailAlternate = x.EmailAlternate
                    },
                    filter);
            }

            var sb = new StringBuilder();

            for (var i = 0; i < emails.Length; i++)
            {
                var item = emails[i];

                var email = EmailAddress.GetEnabledEmail(item.Email, true, item.EmailAlternate, true);
                if (email == null)
                    continue;

                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append(email);
            }

            return sb.Length == 0 ? null : sb.ToString();
        }
    }
}