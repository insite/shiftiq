using System;

namespace Shift.Sdk.UI
{
    public class InvoiceItem
    {
        public Guid Identifier { get; set; }
        public Guid Product { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? TaxRate { get; set; }
        public string TaxCountry { get; set; }
        public string TaxRegion { get; set; }
    }
}
