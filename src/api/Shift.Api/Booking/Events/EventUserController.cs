using Microsoft.AspNetCore.Mvc;

using Shift.Service.Booking;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Booking API: Events")]
public class EventUserController : ShiftControllerBase
{
    private readonly EventUserService _eventUserService;

    public EventUserController(EventUserService eventUserService)
    {
        _eventUserService = eventUserService;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific event user
    /// </summary>
    [HttpHead("booking/events-users/{event:guid}/{user:guid}")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertEventUser")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid @event, [FromRoute] Guid user, CancellationToken cancellation = default)
    {
        var exists = await _eventUserService.AssertAsync(@event, user, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of event users that match specific criteria
    /// </summary>
    [HttpPost("booking/events-users/collect")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Collect)]
    [ProducesResponseType<IEnumerable<EventUserModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectEventUsers")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectEventUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("booking/events-users")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Collect)]
    [ProducesResponseType<IEnumerable<EventUserModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectEventUsers_get")]
    [AliasFor("collectEventUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectEventUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectEventUsers query, CancellationToken cancellation)
    {
        var models = await _eventUserService.CollectAsync(query, cancellation);

        var count = await _eventUserService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the event users that match specific criteria
    /// </summary>
    [HttpPost("booking/events-users/count")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countEventUsers")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountEventUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("booking/events-users/count")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countEventUsers_get")]
    [AliasFor("countEventUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountEventUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountEventUsers query, CancellationToken cancellation)
    {
        var count = await _eventUserService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of event users that match specific criteria
    /// </summary>    
    [HttpPost("booking/events-users/download")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadEventUsers")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectEventUsers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("booking/events-users/download")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadEventUsers_get")]
    [AliasFor("downloadEventUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectEventUsers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectEventUsers query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Booking", "EventUsers", query.Filter.Format, User);

        var models = await _eventUserService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _eventUserService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific event user
    /// </summary>
    [HttpGet("booking/events-users/{event:guid}/{user:guid}")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Retrieve)]
    [ProducesResponseType<EventUserModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveEventUser")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid @event, [FromRoute] Guid user, CancellationToken cancellation = default)
    {
        var model = await _eventUserService.RetrieveAsync(@event, user, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Search for the list of event users that match specific criteria
    /// </summary>
    [HttpPost("booking/events-users/search")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Search)]
    [ProducesResponseType<IEnumerable<EventUserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchEventUsers")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchEventUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("booking/events-users/search")]
    [HybridAuthorize(Policies.Booking.Events.EventUser.Search)]
    [ProducesResponseType<IEnumerable<EventUserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchEventUsers_get")]
    [AliasFor("searchEventUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchEventUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchEventUsers query, CancellationToken cancellation)
    {
        var matches = await _eventUserService.SearchAsync(query, cancellation);

        var count = await _eventUserService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}