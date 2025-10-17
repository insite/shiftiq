namespace InSite.Domain.Records
{
    public class Lifetime
    {
        public int Quantity { get; set; }
        public string Unit { get; set; }

        public Lifetime() { }
        public Lifetime(int quantity, string unit)
        {
            Quantity = quantity;
            Unit = unit;
        }
    }
}
