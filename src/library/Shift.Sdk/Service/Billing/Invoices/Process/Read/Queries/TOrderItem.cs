using System;
using System.ComponentModel;

namespace InSite.Application.Invoices.Read
{
    public class TOrderItem
    {
        public Guid OrderItemIdentifier { get; set; }
        public Guid OrderIdentifier { get; set; }
        public Guid? ProductIdentifier { get; set; }
        public string ProductName { get; set; }
        public string OrderItemType { get; set; }

        [DefaultValue(1)]
        public int OrderItemQuantity { get; set; }

        [DefaultValue(0)]
        public decimal UnitPrice { get; set; }

        public decimal LineTotalAmount { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public virtual TOrder Order { get; set; }
        public virtual TProduct Product { get; set; }
    }
}
