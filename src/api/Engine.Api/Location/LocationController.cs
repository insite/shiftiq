using Microsoft.AspNetCore.Mvc;

using Shift.Common;

using Endpoints = Shift.Common.Integration.Google.Endpoints;

namespace Engine.Api.Location;

[ApiController]
public class LocationController : ControllerBase
{
    private readonly LocationSearch _search;

    public LocationController(LocationSearch search)
    {
        _search = search;
    }

    [HttpGet(Endpoints.Location.Countries)]
    [ProducesResponseType<Country>(StatusCodes.Status200OK, "application/json")]
    [EndpointName("locationCountries")]
    public async Task<ActionResult<List<Country>>> CountriesAsync()
    {
        return await _search.GetCountriesAsync();
    }

    [HttpPost("contact/locations/countries")] // TODO: Deprecate old URL
    [ProducesResponseType<Country>(StatusCodes.Status200OK, "application/json")]
    [EndpointName("locationCountries_deprecate")]
    public async Task<ActionResult<List<Country>>> CountriesDeprecateAsync() => await CountriesAsync();

    [HttpGet(Endpoints.Location.Provinces)]
    [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
    [EndpointName("locationProvinces")]
    public async Task<ActionResult<List<Province>>> ProvincesAsync(string country)
    {
        return await _search.GetProvincesAsync(country);
    }

    [HttpGet(Endpoints.Location.States)]
    [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
    [EndpointName("locationStates")]
    public async Task<ActionResult<List<Province>>> StatesAsync(string country) => await ProvincesAsync(country);

    [HttpPost("contact/locations/countries/{country}/provinces")] // TODO: Deprecate old URL
    [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
    [EndpointName("locationProvinces_deprecate")]
    public async Task<ActionResult<List<Province>>> ProvincesDeprecateAsync(string country) => await ProvincesAsync(country);

    [HttpPost("contact/locations/countries/{country}/states")]    // TODO: Deprecate old URL
    [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
    [EndpointName("locationStates_deprecate")]
    public async Task<ActionResult<List<Province>>> StatesDeprecateAsync(string country) => await ProvincesAsync(country);
}