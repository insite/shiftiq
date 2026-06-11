using System;

namespace Shift.Contract
{
    public partial class RecipientMatch
    {
        public Guid RecipientId { get; set; }
        public Guid MailoutId { get; set; }
        public string UserEmail { get; set; }
        public string PersonName { get; set; }
        public string DeliveryStatus { get; set; }
    }
}