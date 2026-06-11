namespace Shift.Hub.Google
{
    public class TCountry
    {
        public string CountryCode { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string? CapitalCityName { get; set; }
        public string? ContinentCode { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public string? Languages { get; set; }
        public string? TopLevelDomain { get; set; }
        public Guid CountryId { get; set; }
    }
}
