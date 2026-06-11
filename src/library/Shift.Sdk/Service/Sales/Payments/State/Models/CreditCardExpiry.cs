namespace InSite.Domain.Payments
{
    public class CreditCardExpiry
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public static CreditCardExpiry Parse(string text)
        {
            var parts = text.Split('/');

            if (parts.Length != 2
                || !int.TryParse(parts[0], out int month)
                || !int.TryParse(parts[1], out int year)
                )
            {
                return null;
            }

            return new CreditCardExpiry
            {
                Month = month,
                Year = year
            };
        }
    }
}