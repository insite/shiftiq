using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Directory API: Groups")]
public class GroupController : ShiftControllerBase
{
    private readonly GroupService _groupService;

    public GroupController(GroupService groupService)
    {
        _groupService = groupService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific group
    /// </summary>
    [HttpHead("directory/groups/{group:guid}")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertGroup")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid group, CancellationToken cancellation = default)
    {
        var exists = await _groupService.AssertAsync(group, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of groups that match specific criteria
    /// </summary>
    [HttpPost("directory/groups/collect")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Collect)]
    [ProducesResponseType<IEnumerable<GroupModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectGroups")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectGroups query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("directory/groups")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Collect)]
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
        var models = await _groupService.CollectAsync(query, cancellation);

        var count = await _groupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the groups that match specific criteria
    /// </summary>
    [HttpPost("directory/groups/count")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countGroups")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountGroups query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("directory/groups/count")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Count)]
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
        var count = await _groupService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of groups that match specific criteria
    /// </summary>    
    [HttpPost("directory/groups/download")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGroups")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectGroups query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("directory/groups/download")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Download)]
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
    [HttpGet("directory/groups/{group:guid}")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Retrieve)]
    [ProducesResponseType<GroupModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveGroup")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid group, CancellationToken cancellation = default)
    {
        var model = await _groupService.RetrieveAsync(group, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of groups that match specific criteria
    /// </summary>
    [HttpPost("directory/groups/search")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Search)]
    [ProducesResponseType<IEnumerable<GroupMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchGroups")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchGroups query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("directory/groups/search")]
    [HybridAuthorize(Policies.Directory.Groups.Group.Search)]
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
        var matches = await _groupService.SearchAsync(query, cancellation);

        var count = await _groupService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}