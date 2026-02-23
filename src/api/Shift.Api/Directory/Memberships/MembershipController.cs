using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Directory API: Memberships")]
public class MembershipController : ShiftControllerBase
{
    private readonly MembershipService _membershipService;
    private readonly IPrincipalProvider _principalProvider;

    public MembershipController(MembershipService membershipService, IPrincipalProvider principalProvider)
    {
        _membershipService = membershipService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific membership
    /// </summary>
    [HttpHead("api/directory/memberships/{membership:guid}")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertMembership")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid membership, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _membershipService.AssertAsync(membership, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of memberships that match specific criteria
    /// </summary>
    [HttpPost("api/directory/memberships/collect")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<MembershipModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectMemberships")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/directory/memberships")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var currentUserId = principal.User.Identifier;

        var models = await _membershipService.CollectAsync(query, currentUserId, cancellation);

        var count = await _membershipService.CountAsync(query, currentUserId, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the memberships that match specific criteria
    /// </summary>
    [HttpPost("api/directory/memberships/count")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countMemberships")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountMemberships query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/directory/memberships/count")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var currentUserId = principal.User.Identifier;

        var count = await _membershipService.CountAsync(query, currentUserId, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of memberships that match specific criteria
    /// </summary>    
    [HttpPost("api/directory/memberships/download")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadMemberships")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/directory/memberships/download")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var currentUserId = principal.User.Identifier;

        var exporter = new ExportHelper("Directory", "Memberships", query.Filter.Format, User);

        var models = await _membershipService
            .DownloadAsync(query, currentUserId, cancellation)
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
    [HttpGet("api/directory/memberships/{membership:guid}")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType<MembershipModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveMembership")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid membership, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _membershipService.RetrieveAsync(membership, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of memberships that match specific criteria
    /// </summary>
    [HttpPost("api/directory/memberships/search")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<MembershipMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchMemberships")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchMemberships query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/directory/memberships/search")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var currentUserId = principal.User.Identifier;

        var matches = await _membershipService.SearchAsync(query, currentUserId, cancellation);

        var count = await _membershipService.CountAsync(query, currentUserId, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}