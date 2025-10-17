using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Directory API: Memberships")]
public class MembershipController : ControllerBase
{
    private readonly MembershipService _membershipService;

    public MembershipController(MembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific membership
    /// </summary>
    [HttpHead("directory/memberships/{membership:guid}")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertMembership")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid membership,
        CancellationToken cancellation = default)
    {
        var exists = await _membershipService.AssertAsync(membership, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of memberships that match specific criteria
    /// </summary>
    [HttpPost("directory/memberships/collect")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Collect)]
    [ProducesResponseType<IEnumerable<MembershipModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectMemberships")]
    public async Task<ActionResult<IEnumerable<MembershipModel>>> PostCollectAsync(
        [FromBody] CollectMemberships query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpGet("directory/memberships")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Collect)]
    [ProducesResponseType<IEnumerable<MembershipModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectMemberships_get")]
    [AliasFor("collectMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<MembershipModel>>> GetCollectAsync(
        [FromQuery] CollectMemberships query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<MembershipModel>>> CollectAsync(
        CollectMemberships query,
        CancellationToken cancellation)
    {
        var models = await _membershipService.CollectAsync(query, cancellation);

        var count = await _membershipService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the memberships that match specific criteria
    /// </summary>
    [HttpPost("directory/memberships/count")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countMemberships")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountMemberships query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpGet("directory/memberships/count")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countMemberships_get")]
    [AliasFor("countMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountMemberships query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(
        CountMemberships query,
        CancellationToken cancellation)
    {
        var count = await _membershipService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of memberships that match specific criteria
    /// </summary>    
    [HttpPost("directory/memberships/download")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadMemberships")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectMemberships query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    [HttpGet("directory/memberships/download")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadMemberships_get")]
    [AliasFor("downloadMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectMemberships query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        CollectMemberships query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Directory", "Memberships", query.Filter.Format, User);

        var models = await _membershipService.DownloadAsync(query, cancellation);

        var content = _membershipService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific membership
    /// </summary>
    [HttpGet("directory/memberships/{membership:guid}")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Retrieve)]
    [ProducesResponseType<MembershipModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveMembership")]
    public async Task<ActionResult<MembershipModel>> RetrieveAsync(
        [FromRoute] Guid membership,
        CancellationToken cancellation = default)
    {
        var model = await _membershipService.RetrieveAsync(membership, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Search for the list of memberships that match specific criteria
    /// </summary>
    [HttpPost("directory/memberships/search")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Search)]
    [ProducesResponseType<IEnumerable<MembershipMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchMemberships")]
    public async Task<ActionResult<IEnumerable<MembershipMatch>>> PostSearchAsync(
        [FromBody] SearchMemberships query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpGet("directory/memberships/search")]
    [HybridAuthorize(Policies.Directory.Memberships.Membership.Search)]
    [ProducesResponseType<IEnumerable<MembershipMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchMemberships_get")]
    [AliasFor("searchMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<MembershipMatch>>> GetSearchAsync(
        [FromQuery] SearchMemberships query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<MembershipMatch>>> SearchAsync(
        SearchMemberships query,
        CancellationToken cancellation)
    {
        var matches = await _membershipService.SearchAsync(query, cancellation);

        var count = await _membershipService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}