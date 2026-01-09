using Engine.Api.Internal;

using Shift.Common;

namespace Engine.Api.Location;

public class LocationSearch
{
    private readonly ISqlDatabase _db;

    public LocationSearch(ISqlDatabase db)
    {
        _db = db;
    }

    public async Task<List<Country>> GetCountriesAsync()
    {
        return await _db.SelectAsync<Country>("select CountryCode as Code, CountryName as Name, CountryIdentifier as Identifier from contact.TCountry order by CountryName");
    }

    public async Task<List<Province>> GetProvincesAsync(string country)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@CountryCode", country }
        };

        return await _db.SelectAsync<Province>("select ProvinceCode as Code, ProvinceName as Name, CountryCode as Country, ProvinceNameTranslation as Translations from contact.TProvince where CountryCode = @CountryCode order by CountryCode, ProvinceName", parameters);
    }
}