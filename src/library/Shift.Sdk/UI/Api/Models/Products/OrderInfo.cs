using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class OrderInfo
    {
        public Guid Organization { get; set; }
        public Guid Customer { get; set; }
        public Guid? Invoice { get; set; }
        public int? InvoiceNumber { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public CheckoutInfo CheckoutInfo { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}