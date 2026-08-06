using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Accounts API: Users")]
public class UserController : ShiftControllerBase
{
    private readonly UserService _userService;
    private readonly IPrincipalProvider _principalProvider;

    public UserController(UserService userService, IPrincipalProvider principalProvider)
    {
        _userService = userService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific user
    /// </summary>
    [HttpHead("api/accounts/users/{user:guid}")]
    [HybridPermission("security/users", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertUser")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid user, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _userService.AssertAsync(user, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of users that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the result set to users modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
    [HttpPost("api/accounts/users/collect")]
    [HybridPermission("security/users", DataAccess.Read)]
    [EndpointName("collectUsers")]
    public async Task<ActionResult<IEnumerable<UserMatch>>> PostCollectAsync([FromBody] CollectUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/accounts/users")]
    [HybridPermission("security/users", DataAccess.Read)]
    [EndpointName("collectUsers_get")]
    [AliasFor("collectUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserMatch>>> GetCollectAsync([FromQuery] CollectUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<UserMatch>>> CollectAsync(CollectUsers query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = (await _userService.CollectAsync(query, cancellation))
            .Select(x => new UserMatch
            {
                UserId = x.UserId,
                FullName = x.FullName
            })
            .ToList();

        var count = await _userService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the users that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the count to users modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
    [HttpPost("api/accounts/users/count")]
    [HybridPermission("security/users", DataAccess.Read)]
    [EndpointName("countUsers")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/accounts/users/count")]
    [HybridPermission("security/users", DataAccess.Read)]
    [EndpointName("countUsers_get")]
    [AliasFor("countUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountUsers query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _userService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of users that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to download only users modified within a specific window,
    /// which is useful for incremental integrations that already hold most of the user data. Both parameters accept
    /// ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00". LastChangeTimeSince is inclusive;
    /// LastChangeTimeBefore is exclusive. When polling for deltas, subtract a small overlap (for example five
    /// minutes) from the previous poll time to allow for projection lag.
    /// </remarks>
    [HttpPost("api/accounts/users/download")]
    [HybridPermission("security/users", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUsers")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectUsers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/accounts/users/download")]
    [HybridPermission("security/users", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUsers_get")]
    [AliasFor("downloadUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectUsers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectUsers query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Security", "Users", query.Filter.Format, User);

        var models = await _userService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _userService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific user
    /// </summary>
    [HttpGet("api/accounts/users/{user:guid}")]
    [HybridPermission("security/users", DataAccess.Read)]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveUser")]
    public async Task<ActionResult<UserModel>> RetrieveAsync([FromRoute] Guid user, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var model = await _userService.RetrieveAsync(user, organizationId, cancellation);

        if (model == null)
            return NotFound();

        MaskPasswordRelatedValues(model);

        return Ok(model);
    }

    private void MaskPasswordRelatedValues(UserModel model)
    {
        var mask = new string('*', 8);
        model.DefaultPassword = mask;
        model.MultiFactorAuthenticationCode = mask;
        model.OAuthProviderUserId = mask;
        model.OldUserPasswordHash = mask;
        model.UserPasswordHash = mask;
    }

    /// <summary>
    /// Searches for the list of users that match specific criteria
    /// </summary>
    /// <remarks>
    /// Use LastChangeTimeSince and LastChangeTimeBefore to restrict the result set to users modified within a
    /// specific window. Both parameters accept ISO 8601 date-time values, for example "2026-07-06T14:00:00+00:00".
    /// LastChangeTimeSince is inclusive; LastChangeTimeBefore is exclusive.
    /// </remarks>
    [HttpPost("api/accounts/users/search")]
    [HybridPermission("security/users", DataAccess.Read)]
    [EndpointName("searchUsers")]
    public async Task<ActionResult<IEnumerable<UserMatch>>> PostSearchAsync([FromBody] SearchUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/accounts/users/search")]
    [HybridPermission("security/users", DataAccess.Read)]
    [EndpointName("searchUsers_get")]
    [AliasFor("searchUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserMatch>>> GetSearchAsync([FromQuery] SearchUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<UserMatch>>> SearchAsync(SearchUsers query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _userService.SearchAsync(query, cancellation);

        var count = await _userService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}