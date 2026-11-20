namespace InSite.UI.Portal.Billing.Utilities
{
    public class ProvinceTax
    {
        public static readonly ProvinceTax None = new ProvinceTax(0m, null, null);

        public decimal Rate { get; }
        public string Country { get; }
        public string Region { get; }

        public ProvinceTax(decimal rate, string country, string region)
        {
            Rate = rate;
            Country = country;
            Region = region;
        }
    }
}
