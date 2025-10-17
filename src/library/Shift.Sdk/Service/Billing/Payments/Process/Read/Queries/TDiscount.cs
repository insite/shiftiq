using System;

namespace InSite.Application.Payments.Read
{
    public class TDiscount
    {
        public Guid DiscountIdentifier { get; set; }
        public string DiscountCode { get; set; }
        public decimal DiscountPercent { get; set; }
        public string DiscountDescription { get; set; }
        public Guid OrganizationIdentifier { get; set; }
    }
}