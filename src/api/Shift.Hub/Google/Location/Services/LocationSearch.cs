using Shift.Common;

namespace Shift.Hub.Google
{
    public class LocationSearch
    {
        private readonly ISqlDatabase _db;

        public LocationSearch(ISqlDatabase db)
        {
            _db = db;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            return await _db.SelectAsync<Country>("select country_code as Code, country_name as Name, country_id as Identifier from country order by country_name");
        }

        public async Task<List<Province>> GetProvincesAsync(string country)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@CountryCode", country }
            };

            return await _db.SelectAsync<Province>("select province_code as Code, province_name as Name, country_code as Country, province_name_translations as Translations from province where country_code = @CountryCode order by country_code, province_name", parameters);
        }
    }
}
