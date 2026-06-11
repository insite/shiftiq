using Microsoft.AspNetCore.Mvc;

using Shift.Service.Directory;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Contacts API: Memberships")]
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
    [HttpHead("api/contacts/memberships/{membership:guid}")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertMembership")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid membership, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _membershipService.AssertAsync(membership, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of memberships that match specific criteria
    /// </summary>
    [HttpPost("api/contacts/memberships/collect")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [EndpointName("collectMemberships")]
    public async Task<ActionResult<IEnumerable<MembershipModel>>> PostCollectAsync([FromBody] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/contacts/memberships")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [EndpointName("collectMemberships_get")]
    [AliasFor("collectMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<MembershipModel>>> GetCollectAsync([FromQuery] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<MembershipModel>>> CollectAsync(CollectMemberships query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

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
    [HttpPost("api/contacts/memberships/count")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [EndpointName("countMemberships")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountMemberships query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/contacts/memberships/count")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [EndpointName("countMemberships_get")]
    [AliasFor("countMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountMemberships query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountMemberships query, CancellationToken cancellation)
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
    [HttpPost("api/contacts/memberships/download")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadMemberships")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectMemberships query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/contacts/memberships/download")]
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
    [HttpGet("api/contacts/memberships/{membership:guid}")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [ProducesResponseType(typeof(MembershipModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveMembership")]
    public async Task<ActionResult<MembershipModel>> RetrieveAsync([FromRoute] Guid membership, CancellationToken cancellation = default)
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
    [HttpPost("api/contacts/memberships/search")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [EndpointName("searchMemberships")]
    public async Task<ActionResult<IEnumerable<MembershipMatch>>> PostSearchAsync([FromBody] SearchMemberships query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/contacts/memberships/search")]
    [HybridPermission("directory/memberships", DataAccess.Read)]
    [EndpointName("searchMemberships_get")]
    [AliasFor("searchMemberships")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<MembershipMatch>>> GetSearchAsync([FromQuery] SearchMemberships query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<MembershipMatch>>> SearchAsync(SearchMemberships query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var currentUserId = principal.User.Identifier;

        var matches = await _membershipService.SearchAsync(query, currentUserId, cancellation);

        var count = await _membershipService.CountAsync(query, currentUserId, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}