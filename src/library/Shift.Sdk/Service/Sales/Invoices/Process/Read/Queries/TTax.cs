using System;
using System.ComponentModel;

namespace InSite.Application.Invoices.Read
{
    public class TTax
    {
        public Guid TaxIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        [DefaultValue("CA")]
        public string CountryCode { get; set; }

        public string RegionCode { get; set; }
        public string TaxName { get; set; }
        public decimal TaxRate { get; set; }
    }
}
