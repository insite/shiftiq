namespace InSite.Domain.Payments
{
    public class BillingAddress
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        
        public string PostalCode { get; set; }
    }
}