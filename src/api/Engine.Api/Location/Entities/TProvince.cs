namespace Engine.Api.Location
{
    public class TProvince
    {
        public string CountryCode { get; set; } = null!;
        public string? ProvinceCode { get; set; }
        public string ProvinceName { get; set; } = null!;
        public string? ProvinceNameTranslation { get; set; }
    }
}
