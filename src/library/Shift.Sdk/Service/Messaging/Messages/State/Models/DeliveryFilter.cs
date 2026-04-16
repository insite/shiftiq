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
        public Guid? MailoutIdentifier { get; set; }
        public string RecipientAddress { get; set; }
        public string Status { get; set; }
        public string Keyword { get; set; }
    }
}
