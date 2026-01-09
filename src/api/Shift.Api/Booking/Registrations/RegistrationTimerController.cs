using Microsoft.AspNetCore.Mvc;

using Shift.Service.Booking;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Booking API: Registrations")]
public class RegistrationTimerController : ShiftControllerBase
{
    private readonly RegistrationTimerService _registrationTimerService;

    public RegistrationTimerController(RegistrationTimerService registrationTimerService)
    {
        _registrationTimerService = registrationTimerService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific registration timer
    /// </summary>
    [HttpHead("booking/registrations-timers/{timer:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertRegistrationTimer")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid triggerCommand, CancellationToken cancellation = default)
    {
        var exists = await _registrationTimerService.AssertAsync(triggerCommand, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of registration timers that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-timers/collect")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationTimerModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrationTimers")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-timers")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Collect)]
    [ProducesResponseType<IEnumerable<RegistrationTimerModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectRegistrationTimers_get")]
    [AliasFor("collectRegistrationTimers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectRegistrationTimers query, CancellationToken cancellation)
    {
        var models = await _registrationTimerService.CollectAsync(query, cancellation);

        var count = await _registrationTimerService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the registration timers that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-timers/count")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrationTimers")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-timers/count")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countRegistrationTimers_get")]
    [AliasFor("countRegistrationTimers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountRegistrationTimers query, CancellationToken cancellation)
    {
        var count = await _registrationTimerService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of registration timers that match specific criteria
    /// </summary>    
    [HttpPost("booking/registrations-timers/download")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrationTimers")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-timers/download")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadRegistrationTimers_get")]
    [AliasFor("downloadRegistrationTimers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectRegistrationTimers query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Booking", "RegistrationTimers", query.Filter.Format, User);

        var models = await _registrationTimerService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _registrationTimerService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific registration timer
    /// </summary>
    [HttpGet("booking/registrations-timers/{timer:guid}")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Retrieve)]
    [ProducesResponseType<RegistrationTimerModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveRegistrationTimer")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid triggerCommand, CancellationToken cancellation = default)
    {
        var model = await _registrationTimerService.RetrieveAsync(triggerCommand, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of registration timers that match specific criteria
    /// </summary>
    [HttpPost("booking/registrations-timers/search")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Search)]
    [ProducesResponseType<IEnumerable<RegistrationTimerMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrationTimers")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("booking/registrations-timers/search")]
    [HybridAuthorize(Policies.Booking.Registrations.RegistrationTimer.Search)]
    [ProducesResponseType<IEnumerable<RegistrationTimerMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchRegistrationTimers_get")]
    [AliasFor("searchRegistrationTimers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchRegistrationTimers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchRegistrationTimers query, CancellationToken cancellation)
    {
        var matches = await _registrationTimerService.SearchAsync(query, cancellation);

        var count = await _registrationTimerService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}