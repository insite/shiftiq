using Microsoft.AspNetCore.Mvc;

using Shift.Service.Booking;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Booking API: Registrations")]
public class RegistrationAccommodationController : ShiftControllerBase
{
    private readonly RegistrationAccommodationService _registrationAccommodationService;

    public RegistrationAccommodationController(RegistrationAccommodationService registrationAccommodationService)
    {
        _registrationAccommodationService = registrationAccommodationService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific registration accommodation
    /// </summary>
    [HttpHead("booking/registrations-accommodations/{accommodation:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertRegistrationAccommodation")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid accommodation, CancellationToken cancellation = default)
    {
        var exists = await _registrationAccommodationService.AssertAsync(accommodation, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of registration accommodations that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-accommodations/collect")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationAccommodationModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrationAccommodations")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-accommodations")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationAccommodationModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrationAccommodations_get")]
    [AliasFor("collectRegistrationAccommodations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectRegistrationAccommodations query, CancellationToken cancellation)
    {
        var models = await _registrationAccommodationService.CollectAsync(query, cancellation);

        var count = await _registrationAccommodationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the registration accommodations that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-accommodations/count")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrationAccommodations")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-accommodations/count")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrationAccommodations_get")]
    [AliasFor("countRegistrationAccommodations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountRegistrationAccommodations query, CancellationToken cancellation)
    {
        var count = await _registrationAccommodationService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of registration accommodations that match specific criteria
    /// </summary>    
    [HttpPost("booking/registrations-accommodations/download")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrationAccommodations")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-accommodations/download")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrationAccommodations_get")]
    [AliasFor("downloadRegistrationAccommodations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectRegistrationAccommodations query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Booking", "RegistrationAccommodations", query.Filter.Format, User);

        var models = await _registrationAccommodationService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _registrationAccommodationService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific registration accommodation
    /// </summary>
    [HttpGet("booking/registrations-accommodations/{accommodation:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Retrieve)]
    [ProducesResponseType<RegistrationAccommodationModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveRegistrationAccommodation")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid accommodation, CancellationToken cancellation = default)
    {
        var model = await _registrationAccommodationService.RetrieveAsync(accommodation, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of registration accommodations that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-accommodations/search")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Search)]
    [ProducesResponseType<IEnumerable<RegistrationAccommodationMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrationAccommodations")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-accommodations/search")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationAccommodation.Search)]
    [ProducesResponseType<IEnumerable<RegistrationAccommodationMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrationAccommodations_get")]
    [AliasFor("searchRegistrationAccommodations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchRegistrationAccommodations query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchRegistrationAccommodations query, CancellationToken cancellation)
    {
        var matches = await _registrationAccommodationService.SearchAsync(query, cancellation);

        var count = await _registrationAccommodationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}