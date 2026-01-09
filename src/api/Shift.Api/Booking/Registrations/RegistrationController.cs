using Microsoft.AspNetCore.Mvc;

using Shift.Service.Booking;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Booking API: Registrations")]
public class RegistrationController : ShiftControllerBase
{
    private readonly RegistrationService _registrationService;

    public RegistrationController(RegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific registration
    /// </summary>
    [HttpHead("booking/registrations/{registration:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertRegistration")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid registration, CancellationToken cancellation = default)
    {
        var exists = await _registrationService.AssertAsync(registration, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of registrations that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations/collect")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrations")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectRegistrations query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("booking/registrations")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrations_get")]
    [AliasFor("collectRegistrations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectRegistrations query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectRegistrations query, CancellationToken cancellation)
    {
        var models = await _registrationService.CollectAsync(query, cancellation);

        var count = await _registrationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the registrations that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations/count")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrations")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountRegistrations query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("booking/registrations/count")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrations_get")]
    [AliasFor("countRegistrations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountRegistrations query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountRegistrations query, CancellationToken cancellation)
    {
        var count = await _registrationService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of registrations that match specific criteria
    /// </summary>    
    [HttpPost("booking/registrations/download")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrations")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectRegistrations query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("booking/registrations/download")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrations_get")]
    [AliasFor("downloadRegistrations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectRegistrations query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectRegistrations query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Booking", "Registrations", query.Filter.Format, User);

        var models = await _registrationService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _registrationService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific registration
    /// </summary>
    [HttpGet("booking/registrations/{registration:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Retrieve)]
    [ProducesResponseType<RegistrationModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveRegistration")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid registration, CancellationToken cancellation = default)
    {
        var model = await _registrationService.RetrieveAsync(registration, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of registrations that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations/search")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Search)]
    [ProducesResponseType<IEnumerable<RegistrationMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrations")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchRegistrations query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("booking/registrations/search")]
    [HybridAuthorize(Policies.Booking.Registrations.Registration.Search)]
    [ProducesResponseType<IEnumerable<RegistrationMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrations_get")]
    [AliasFor("searchRegistrations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchRegistrations query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchRegistrations query, CancellationToken cancellation)
    {
        var matches = await _registrationService.SearchAsync(query, cancellation);

        var count = await _registrationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}