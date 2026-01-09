namespace InSite.Domain.Integrations.Bambora
{
    public class BamboraCreditCard
    {
        public string number { get; set; }
        public string name { get; set; }
        public string expiry_month { get; set; }
        public string expiry_year { get; set; }
        public string cvd { get; set; }
    }
}
