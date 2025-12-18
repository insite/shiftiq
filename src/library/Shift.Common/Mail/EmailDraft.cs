using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Shift.Common
{
    /// <remarks>
    /// This interface is used to represent an email message that has not yet been sent. It 
    /// corresponds to Draft items in Gmail or Outook.
    /// </remarks>
    [Serializable]
    public class EmailDraft
    {
        private EmailDraft() { }

        public static EmailDraft Create(Guid organization, Guid? message, Guid sender,
            bool autoBccSubscribers
            )
        {
            var draft = new EmailDraft();

            draft.OrganizationIdentifier = organization;
            draft.MessageIdentifier = message;
            draft.SenderIdentifier = sender;

            draft.AutoBccSubscribers = autoBccSubscribers;

            return draft;
        }

        public Guid MailoutIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid SenderIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public List<string> ContentAttachments { get; set; } = new List<string>();
        public MultilingualString ContentBody { get; set; } = new MultilingualString();
        public string ContentPriority { get; set; }
        public MultilingualString ContentSubject { get; set; } = new MultilingualString();
        public Dictionary<string, string> ContentVariables { get; set; } = new Dictionary<string, string>();

        public string MailoutStatus { get; set; }
        public string MailoutStatusCode { get; set; }
        public string MailoutErrorReason { get; set; }

        public bool SenderEnabled { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SenderStatus { get; set; }
        public string SenderType { get; set; }
        public string SystemMailbox { get; set; }

        public bool AutoBccSubscribers { get; set; }
        public bool IgnoreSubscribers { get; set; }
        public bool MailoutSucceeded { get; set; }

        public bool IsDisabled { get; set; }

        public DateTimeOffset? MailoutScheduled { get; set; }
        public DateTimeOffset? MailoutCompleted { get; set; }
        public DateTimeOffset? MailoutCancelled { get; set; }

        public EmailAddressList Recipients { get; set; } = new EmailAddressList();

        public Dictionary<Guid, string> RecipientListTo { get; set; } = new Dictionary<Guid, string>();
        public Dictionary<Guid, string> RecipientListCc { get; set; } = new Dictionary<Guid, string>();
        public Dictionary<Guid, string> RecipientListBcc { get; set; } = new Dictionary<Guid, string>();

        public Dictionary<Guid, Guid[]> ContentCompetencies { get; set; } = new Dictionary<Guid, Guid[]>();
        public Dictionary<Guid, Guid[]> ContentCredentials { get; set; } = new Dictionary<Guid, Guid[]>();

        public void AttachFile(string path)
        {
            if (path.IsEmpty())
                return;

            if (System.IO.File.Exists(path) && !ContentAttachments.Any(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase)))
                ContentAttachments.Add(path);
        }

        [JsonIgnore]
        public Guid? EventIdentifier
        {
            get
            {
                return ContentVariables.TryGetValue(nameof(EventIdentifier), out var value) ? Guid.Parse(value) : (Guid?)null;
            }
            set
            {
                if (value != null)
                    ContentVariables[nameof(EventIdentifier)] = value?.ToString();
                else
                    ContentVariables.Remove(nameof(EventIdentifier));
            }
        }

        [JsonIgnore]
        public Guid? SurveyIdentifier
        {
            get
            {
                return ContentVariables.TryGetValue(nameof(SurveyIdentifier), out var value) ? Guid.Parse(value) : (Guid?)null;
            }
            set
            {
                if (value != null)
                    ContentVariables[nameof(SurveyIdentifier)] = value?.ToString();
                else
                    ContentVariables.Remove(nameof(SurveyIdentifier));
            }
        }

        [JsonIgnore]
        public int? SurveyNumber
        {
            get
            {
                return ContentVariables.TryGetValue(nameof(SurveyNumber), out var value) ? int.Parse(value) : (int?)null;
            }
            set
            {
                if (value != null)
                    ContentVariables[nameof(SurveyNumber)] = value?.ToString();
                else
                    ContentVariables.Remove(nameof(SurveyNumber));
            }
        }
    }
}