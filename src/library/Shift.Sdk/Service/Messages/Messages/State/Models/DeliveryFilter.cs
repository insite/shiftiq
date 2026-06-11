using System;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Serializable]
    public class DeliveryFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? MessageIdentifier { get; set; }
        public Guid? MailoutIdentifier
        {
            get => MailoutIdentifiers != null && MailoutIdentifiers.Length == 1 ? MailoutIdentifiers[0] : (Guid?)null;
            set => MailoutIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] MailoutIdentifiers { get; set; }
        public Guid? RecipientIdentifier
        {
            get => RecipientIdentifiers != null && RecipientIdentifiers.Length == 1 ? RecipientIdentifiers[0] : (Guid?)null;
            set => RecipientIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] RecipientIdentifiers { get; set; }
        public string RecipientAddress { get; set; }
        public string Status { get; set; }
        public string Keyword { get; set; }
    }
}
