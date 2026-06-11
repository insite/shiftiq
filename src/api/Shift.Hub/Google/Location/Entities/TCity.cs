namespace Shift.Hub.Google
{
    public class TCity
    {
        public string CityName { get; set; } = null!;
        public string ProvinceCode { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
