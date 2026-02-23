using Microsoft.AspNetCore.Mvc;

using Shift.Service.Booking;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Booking API: Events")]
public class EventController : ShiftControllerBase
{
    private readonly EventService _eventService;
    private readonly IPrincipalProvider _principalProvider;

    public EventController(EventService eventService, IPrincipalProvider principalProvider)
    {
        _eventService = eventService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific event
    /// </summary>
    [HttpHead("api/booking/events/{event:guid}")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertEvent")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid @event, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _eventService.AssertAsync(@event, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of events that match specific criteria
    /// </summary>
    [HttpPost("api/booking/events/collect")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<EventModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectEvents")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectEvents query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/booking/events")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<EventModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectEvents_get")]
    [AliasFor("collectEvents")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectEvents query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectEvents query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _eventService.CollectAsync(query, cancellation);

        var count = await _eventService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the events that match specific criteria
    /// </summary>
    [HttpPost("api/booking/events/count")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countEvents")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountEvents query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/booking/events/count")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countEvents_get")]
    [AliasFor("countEvents")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountEvents query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountEvents query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _eventService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of events that match specific criteria
    /// </summary>    
    [HttpPost("api/booking/events/download")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadEvents")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectEvents query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/booking/events/download")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadEvents_get")]
    [AliasFor("downloadEvents")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectEvents query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectEvents query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Booking", "Events", query.Filter.Format, User);

        var models = await _eventService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _eventService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific event
    /// </summary>
    [HttpGet("api/booking/events/{event:guid}")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<EventModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveEvent")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid @event, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _eventService.RetrieveAsync(@event, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Search for the list of events that match specific criteria
    /// </summary>
    [HttpPost("api/booking/events/search")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<EventMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchEvents")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchEvents query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/booking/events/search")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<EventMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchEvents_get")]
    [AliasFor("searchEvents")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchEvents query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchEvents query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _eventService.SearchAsync(query, cancellation);

        var count = await _eventService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}