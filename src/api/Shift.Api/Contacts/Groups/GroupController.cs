using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Contacts API: Groups")]
public class GroupController : ShiftControllerBase
{
    private readonly GroupService _groupService;
    private readonly IPrincipalProvider _principalProvider;

    public GroupController(GroupService groupService, IPrincipalProvider principalProvider)
    {
        _groupService = groupService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific group
    /// </summary>
    [HttpHead("api/contacts/groups/{group:guid}")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertGroup")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid group, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _groupService.AssertAsync(group, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of groups that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the result set to groups modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
    [HttpPost("api/contacts/groups/collect")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [EndpointName("collectGroups")]
    public async Task<ActionResult<IEnumerable<GroupModel>>> PostCollectAsync([FromBody] CollectGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/contacts/groups")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [EndpointName("collectGroups_get")]
    [AliasFor("collectGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<GroupModel>>> GetCollectAsync([FromQuery] CollectGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<GroupModel>>> CollectAsync(CollectGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _groupService.CollectAsync(query, cancellation);

        var count = await _groupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the groups that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the count to groups modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
    [HttpPost("api/contacts/groups/count")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [EndpointName("countGroups")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/contacts/groups/count")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [EndpointName("countGroups_get")]
    [AliasFor("countGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _groupService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of groups that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to download only groups modified within a specific window,
    /// which is useful for incremental integrations that already hold most of the data. Both parameters accept
    /// ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00". LastChangeTimeSince is inclusive;
    /// LastChangeTimeBefore is exclusive. When polling for deltas, subtract a small overlap (for example five
    /// minutes) from the previous poll time to allow for projection lag.
    /// </remarks>
    [HttpPost("api/contacts/groups/download")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGroups")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectGroups query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/contacts/groups/download")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGroups_get")]
    [AliasFor("downloadGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectGroups query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Directory", "Groups", query.Filter.Format, User);

        var models = await _groupService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _groupService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific group
    /// </summary>
    [HttpGet("api/contacts/groups/{group:guid}")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType(typeof(GroupModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveGroup")]
    public async Task<ActionResult<GroupModel>> RetrieveAsync([FromRoute] Guid group, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _groupService.RetrieveAsync(group, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of groups that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the result set to groups modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
    [HttpPost("api/contacts/groups/search")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [EndpointName("searchGroups")]
    public async Task<ActionResult<IEnumerable<GroupMatch>>> PostSearchAsync([FromBody] SearchGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/contacts/groups/search")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [EndpointName("searchGroups_get")]
    [AliasFor("searchGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<GroupMatch>>> GetSearchAsync([FromQuery] SearchGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<GroupMatch>>> SearchAsync(SearchGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _groupService.SearchAsync(query, cancellation);

        var count = await _groupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}