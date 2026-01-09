using System;

namespace InSite.UI.Portal.Billing.Models
{
    public class CartProductItem
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public string Summary { get; set; }
        public string PriceFormatted => UnitPrice > 0 ? string.Format("{0:C}", UnitPrice) : string.Empty;
    }
}