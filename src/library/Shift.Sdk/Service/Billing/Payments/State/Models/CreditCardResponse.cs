namespace InSite.Domain.Payments
{
    public class CreditCardResponse
    {
        public string CardType { get; set; }
        public string LastFour { get; set; }
        public int CvdResult { get; set; }
        public int Eci { get; set; }
        public int AddressMatch { get; set; }
        public int PostalResult { get; set; }
        public string MerchantData { get; set; }
        public string Contents { get; set; }
    }
}