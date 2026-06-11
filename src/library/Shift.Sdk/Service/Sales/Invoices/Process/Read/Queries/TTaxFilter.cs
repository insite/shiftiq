using System;

using Shift.Common;

namespace InSite.Application.Invoices.Read
{
    [Serializable]
    public class TTaxFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string RegionCode { get; set; }
        public string TaxName { get; set; }
    }
}
