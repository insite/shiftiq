using Microsoft.AspNetCore.Mvc;

using Shift.Service.Booking;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Events API: Events")]
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
    [HttpHead("api/events/{event:guid}")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertEvent")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid @event, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _eventService.AssertAsync(@event, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of events that match specific criteria
    /// </summary>
    [HttpPost("api/events/collect")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [EndpointName("collectEvents")]
    public async Task<ActionResult<IEnumerable<EventModel>>> PostCollectAsync([FromBody] CollectEvents query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/events")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [EndpointName("collectEvents_get")]
    [AliasFor("collectEvents")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<EventModel>>> GetCollectAsync([FromQuery] CollectEvents query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<EventModel>>> CollectAsync(CollectEvents query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _eventService.CollectAsync(query, cancellation);

        var count = await _eventService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the events that match specific criteria
    /// </summary>
    [HttpPost("api/events/count")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [EndpointName("countEvents")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountEvents query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/events/count")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [EndpointName("countEvents_get")]
    [AliasFor("countEvents")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountEvents query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountEvents query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _eventService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of events that match specific criteria
    /// </summary>    
    [HttpPost("api/events/download")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadEvents")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectEvents query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/events/download")]
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
    [HttpGet("api/events/{event:guid}")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [ProducesResponseType(typeof(EventModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveEvent")]
    public async Task<ActionResult<EventModel>> RetrieveAsync([FromRoute] Guid @event, CancellationToken cancellation = default)
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
    [HttpPost("api/events/search")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [EndpointName("searchEvents")]
    public async Task<ActionResult<IEnumerable<EventMatch>>> PostSearchAsync([FromBody] SearchEvents query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/events/search")]
    [HybridPermission("booking/events", DataAccess.Read)]
    [EndpointName("searchEvents_get")]
    [AliasFor("searchEvents")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<EventMatch>>> GetSearchAsync([FromQuery] SearchEvents query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<EventMatch>>> SearchAsync(SearchEvents query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _eventService.SearchAsync(query, cancellation);

        var count = await _eventService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}