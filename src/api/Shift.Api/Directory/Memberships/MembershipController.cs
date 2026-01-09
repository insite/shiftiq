using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Directory API: Memberships")]
public class MembershipController : ShiftControllerBase
{
    private readonly MembershipService _membershipService;

    public MembershipController(MembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific membership
    /// </summary>
    [HttpHead("directory/memberships/{membership:guid}")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertMembership")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid membership, CancellationToken cancellation = default)
    {
        var exists = await _membershipService.AssertAsync(membership, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of memberships that match specific criteria
    /// </summary>
    [HttpPost("directory/memberships/collect")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Collect)]
    [ProducesResponseType<IEnumerable<MembershipModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectMemberships")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("directory/memberships")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Collect)]
    [ProducesResponseType<IEnumerable<MembershipModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectMemberships_get")]
    [AliasFor("collectMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectMemberships query, CancellationToken cancellation)
    {
        var models = await _membershipService.CollectAsync(query, cancellation);

        var count = await _membershipService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the memberships that match specific criteria
    /// </summary>
    [HttpPost("directory/memberships/count")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countMemberships")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountMemberships query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("directory/memberships/count")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countMemberships_get")]
    [AliasFor("countMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountMemberships query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountMemberships query, CancellationToken cancellation)
    {
        var count = await _membershipService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of memberships that match specific criteria
    /// </summary>    
    [HttpPost("directory/memberships/download")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadMemberships")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("directory/memberships/download")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadMemberships_get")]
    [AliasFor("downloadMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectMemberships query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Directory", "Memberships", query.Filter.Format, User);

        var models = await _membershipService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _membershipService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific membership
    /// </summary>
    [HttpGet("directory/memberships/{membership:guid}")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Retrieve)]
    [ProducesResponseType<MembershipModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveMembership")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid membership, CancellationToken cancellation = default)
    {
        var model = await _membershipService.RetrieveAsync(membership, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of memberships that match specific criteria
    /// </summary>
    [HttpPost("directory/memberships/search")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Search)]
    [ProducesResponseType<IEnumerable<MembershipMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchMemberships")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchMemberships query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("directory/memberships/search")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Search)]
    [ProducesResponseType<IEnumerable<MembershipMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchMemberships_get")]
    [AliasFor("searchMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchMemberships query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchMemberships query, CancellationToken cancellation)
    {
        var matches = await _membershipService.SearchAsync(query, cancellation);

        var count = await _membershipService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}