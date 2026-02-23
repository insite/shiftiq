using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Directory API: Groups")]
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
    [HttpHead("api/directory/groups/{group:guid}")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertGroup")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid group, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _groupService.AssertAsync(group, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of groups that match specific criteria
    /// </summary>
    [HttpPost("api/directory/groups/collect")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<GroupModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectGroups")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/directory/groups")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<GroupModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectGroups_get")]
    [AliasFor("collectGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _groupService.CollectAsync(query, cancellation);

        var count = await _groupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the groups that match specific criteria
    /// </summary>
    [HttpPost("api/directory/groups/count")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countGroups")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/directory/groups/count")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countGroups_get")]
    [AliasFor("countGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _groupService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of groups that match specific criteria
    /// </summary>    
    [HttpPost("api/directory/groups/download")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGroups")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectGroups query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/directory/groups/download")]
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
    [HttpGet("api/directory/groups/{group:guid}")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<GroupModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveGroup")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid group, CancellationToken cancellation = default)
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
    [HttpPost("api/directory/groups/search")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<GroupMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchGroups")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/directory/groups/search")]
    [HybridPermission("directory/groups", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<GroupMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchGroups_get")]
    [AliasFor("searchGroups")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchGroups query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _groupService.SearchAsync(query, cancellation);

        var count = await _groupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}