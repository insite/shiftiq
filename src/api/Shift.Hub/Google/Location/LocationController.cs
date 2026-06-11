using Microsoft.AspNetCore.Mvc;

using Endpoints = Shift.Common.Integration.Google.Endpoints;

namespace Shift.Hub.Google
{
    [ApiController]
    [Route("google")]
    [ApiExplorerSettings(GroupName = "Integration: Google")]
    public class LocationController : ControllerBase
    {
        private readonly LocationSearch _search;

        public LocationController(LocationSearch search)
        {
            _search = search;
        }

        [HttpGet(Endpoints.Location.Countries)]
        [ProducesResponseType<Country>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<Country>>> CountriesAsync()
        {
            return await _search.GetCountriesAsync();
        }

        [HttpPost("contact/locations/countries")] // Deprecated URL
        [ProducesResponseType<Country>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<Country>>> CountriesDeprecateAsync() => await CountriesAsync();

        [HttpGet(Endpoints.Location.Provinces)]
        [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<Province>>> ProvincesAsync(string country)
        {
            return await _search.GetProvincesAsync(country);
        }

        [HttpGet(Endpoints.Location.States)]
        [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<Province>>> StatesAsync(string country) => await ProvincesAsync(country);

        [HttpPost("contact/locations/countries/{country}/provinces")] // Deprecated URL
        [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<Province>>> ProvincesDeprecateAsync(string country) => await ProvincesAsync(country);

        [HttpPost("contact/locations/countries/{country}/states")] // Deprecated URL
        [ProducesResponseType<Province>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<Province>>> StatesDeprecateAsync(string country) => await ProvincesAsync(country);
    }
}
