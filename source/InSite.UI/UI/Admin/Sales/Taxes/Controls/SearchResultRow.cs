using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InSite.UI.Admin.Sales.Taxes.Controls
{
    public class SearchResultRow
    {
        public Guid TaxIdentifier { get; set; }
        public string TaxName { get; set; }
        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public decimal TaxPercent { get; set; }
    }
}