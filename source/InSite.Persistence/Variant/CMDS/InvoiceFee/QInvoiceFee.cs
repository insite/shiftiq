using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class QInvoiceFee
    {
        public string BillingClassification { get; set; }
        public string CompanyName { get; set; }
        public string UserEmail { get; set; }

        public int InvoiceFeeKey { get; set; }
        public int SharedCompanyCount { get; set; }

        public decimal PricePerUserPerPeriodPerCompany { get; set; }

        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ThruDate { get; set; }
    }
}
