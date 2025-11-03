using System;

using Shift.Common;

namespace InSite.Application.Invoices.Read
{
    [Serializable]
    public class TProductFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? ProductIdentifier { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductType { get; set; }
        public bool? IsPublished { get; set; }
        public bool IsAvailableForSale { get; set; }
        public int? ProductQuantity { get; set; }
        public decimal? ProductPrice { get; set; }

        public TProductFilter Clone() => (TProductFilter)MemberwiseClone();
    }
}
