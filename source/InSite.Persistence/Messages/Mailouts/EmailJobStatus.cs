using System;

namespace InSite.Persistence
{
    [Serializable]
    public class EmailJobStatus
    {
        public Guid MessageIdentifier { get; set; }
        public Guid MailoutIdentifier { get; set; }

        public Guid DeliveryIdentifier { get; set; }
        public string DeliveryAddress { get; set; }

        public Guid RecipientIdentifier { get; set; }
        public string RecipientType { get; set; }
        public string RecipientAddress { get; set; }

        public DateTimeOffset? Completed { get; set; }
        public string Error { get; set; }
    }
}