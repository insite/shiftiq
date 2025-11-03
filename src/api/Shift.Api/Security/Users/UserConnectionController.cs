using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Users")]
public class UserConnectionController : ShiftControllerBase
{
    private readonly UserConnectionService _userConnectionService;

    public UserConnectionController(UserConnectionService userConnectionService)
    {
        _userConnectionService = userConnectionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific user connection
    /// </summary>
    [HttpHead("security/users-connections/{from:guid}/{to:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertUserConnection")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid fromUser, [FromRoute] Guid toUser, CancellationToken cancellation = default)
    {
        var exists = await _userConnectionService.AssertAsync(fromUser, toUser, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of user connections that match specific criteria
    /// </summary>
    [HttpPost("security/users-connections/collect")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Collect)]
    [ProducesResponseType<IEnumerable<UserConnectionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUserConnections")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectUserConnections query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("security/users-connections")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Collect)]
    [ProducesResponseType<IEnumerable<UserConnectionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUserConnections_get")]
    [AliasFor("collectUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectUserConnections query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectUserConnections query, CancellationToken cancellation)
    {
        var models = await _userConnectionService.CollectAsync(query, cancellation);

        var count = await _userConnectionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the user connections that match specific criteria
    /// </summary>
    [HttpPost("security/users-connections/count")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUserConnections")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountUserConnections query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("security/users-connections/count")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUserConnections_get")]
    [AliasFor("countUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountUserConnections query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountUserConnections query, CancellationToken cancellation)
    {
        var count = await _userConnectionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of user connections that match specific criteria
    /// </summary>    
    [HttpPost("security/users-connections/download")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUserConnections")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectUserConnections query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("security/users-connections/download")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUserConnections_get")]
    [AliasFor("downloadUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectUserConnections query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectUserConnections query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Security", "UserConnections", query.Filter.Format, User);

        var models = await _userConnectionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _userConnectionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific user connection
    /// </summary>
    [HttpGet("security/users-connections/{from:guid}/{to:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Retrieve)]
    [ProducesResponseType<UserConnectionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveUserConnection")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid fromUser, [FromRoute] Guid toUser, CancellationToken cancellation = default)
    {
        var model = await _userConnectionService.RetrieveAsync(fromUser, toUser, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of user connections that match specific criteria
    /// </summary>
    [HttpPost("security/users-connections/search")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Search)]
    [ProducesResponseType<IEnumerable<UserConnectionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUserConnections")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchUserConnections query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("security/users-connections/search")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Search)]
    [ProducesResponseType<IEnumerable<UserConnectionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUserConnections_get")]
    [AliasFor("searchUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchUserConnections query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchUserConnections query, CancellationToken cancellation)
    {
        var matches = await _userConnectionService.SearchAsync(query, cancellation);

        var count = await _userConnectionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}