using System;

using Shift.Common;

namespace InSite.Application.Payments.Read
{
    [Serializable]
    public class TDiscountFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string DiscountCode { get; set; }
        public string DiscountDescription { get; set; }
    }
}
