using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific user
    /// </summary>
    [HttpHead("security/users/{user:guid}")]
    [HybridAuthorize(Policies.Security.Users.User.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertUser")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid user,
        CancellationToken cancellation = default)
    {
        var exists = await _userService.AssertAsync(user, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of users that match specific criteria
    /// </summary>
    [HttpPost("security/users/collect")]
    [HybridAuthorize(Policies.Security.Users.User.Collect)]
    [ProducesResponseType<IEnumerable<UserModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUsers")]
    public async Task<ActionResult<IEnumerable<UserModel>>> PostCollectAsync(
        [FromBody] CollectUsers query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpGet("security/users")]
    [HybridAuthorize(Policies.Security.Users.User.Collect)]
    [ProducesResponseType<IEnumerable<UserModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUsers_get")]
    [AliasFor("collectUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetCollectAsync(
        [FromQuery] CollectUsers query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<UserModel>>> CollectAsync(
        CollectUsers query,
        CancellationToken cancellation)
    {
        var models = await _userService.CollectAsync(query, cancellation);

        var count = await _userService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the users that match specific criteria
    /// </summary>
    [HttpPost("security/users/count")]
    [HybridAuthorize(Policies.Security.Users.User.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUsers")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountUsers query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpGet("security/users/count")]
    [HybridAuthorize(Policies.Security.Users.User.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUsers_get")]
    [AliasFor("countUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountUsers query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(
        CountUsers query,
        CancellationToken cancellation)
    {
        var count = await _userService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of users that match specific criteria
    /// </summary>    
    [HttpPost("security/users/download")]
    [HybridAuthorize(Policies.Security.Users.User.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUsers")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectUsers query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    [HttpGet("security/users/download")]
    [HybridAuthorize(Policies.Security.Users.User.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUsers_get")]
    [AliasFor("downloadUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectUsers query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        CollectUsers query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Security", "Users", query.Filter.Format, User);

        var models = await _userService.DownloadAsync(query, cancellation);

        var content = _userService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific user
    /// </summary>
    [HttpGet("security/users/{user:guid}")]
    [HybridAuthorize(Policies.Security.Users.User.Retrieve)]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveUser")]
    public async Task<ActionResult<UserModel>> RetrieveAsync(
        [FromRoute] Guid user,
        CancellationToken cancellation = default)
    {
        var model = await _userService.RetrieveAsync(user, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Search for the list of users that match specific criteria
    /// </summary>
    [HttpPost("security/users/search")]
    [HybridAuthorize(Policies.Security.Users.User.Search)]
    [ProducesResponseType<IEnumerable<UserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUsers")]
    public async Task<ActionResult<IEnumerable<UserMatch>>> PostSearchAsync(
        [FromBody] SearchUsers query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpGet("security/users/search")]
    [HybridAuthorize(Policies.Security.Users.User.Search)]
    [ProducesResponseType<IEnumerable<UserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUsers_get")]
    [AliasFor("searchUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserMatch>>> GetSearchAsync(
        [FromQuery] SearchUsers query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<UserMatch>>> SearchAsync(
        SearchUsers query,
        CancellationToken cancellation)
    {
        var matches = await _userService.SearchAsync(query, cancellation);

        var count = await _userService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}