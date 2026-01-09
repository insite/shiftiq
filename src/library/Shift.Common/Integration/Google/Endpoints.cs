namespace Shift.Common.Integration.Google
{
    public static class Endpoints
    {
        public const string Token = "token";

        public static class Location
        {
            public const string Countries = "location/countries";
            public const string Provinces = "location/countries/{country}/provinces";
            public const string States = "location/countries/{country}/states";
        }

        public static class Translation
        {
            public const string Translate = "translation/translate";
        }
    }
}