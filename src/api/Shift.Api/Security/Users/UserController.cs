using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Users")]
public class UserController : ShiftControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific user
    /// </summary>
    [HttpHead("security/users/{user:guid}")]
    [HybridAuthorize(Policies.Security.Users.User.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertUser")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid user, CancellationToken cancellation = default)
    {
        var exists = await _userService.AssertAsync(user, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of users that match specific criteria
    /// </summary>
    [HttpPost("security/users/collect")]
    [HybridAuthorize(Policies.Security.Users.User.Collect)]
    [ProducesResponseType<IEnumerable<UserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("collectUsers")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("security/users")]
    [HybridAuthorize(Policies.Security.Users.User.Collect)]
    [ProducesResponseType<IEnumerable<UserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("collectUsers_get")]
    [AliasFor("collectUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectUsers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectUsers query, CancellationToken cancellation)
    {
        var models = (await _userService.CollectAsync(query, cancellation))
            .Select(x => new UserMatch
            {
                UserIdentifier = x.UserIdentifier,
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
    [HttpPost("security/users/count")]
    [HybridAuthorize(Policies.Security.Users.User.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUsers")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("security/users/count")]
    [HybridAuthorize(Policies.Security.Users.User.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUsers_get")]
    [AliasFor("countUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountUsers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountUsers query, CancellationToken cancellation)
    {
        var count = await _userService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of users that match specific criteria
    /// </summary>    
    [HttpPost("security/users/download")]
    [HybridAuthorize(Policies.Security.Users.User.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUsers")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectUsers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("security/users/download")]
    [HybridAuthorize(Policies.Security.Users.User.Download)]
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
    [HttpGet("security/users/{user:guid}")]
    [HybridAuthorize(Policies.Security.Users.User.Retrieve)]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveUser")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid user, CancellationToken cancellation = default)
    {
        var model = await _userService.RetrieveAsync(user, cancellation);

        if (model == null)
            return NotFound();

        return Ok(new UserMatch
        {
            UserIdentifier = model.UserIdentifier,
            FullName = model.FullName
        });
    }

    /// <summary>
    /// Searches for the list of users that match specific criteria
    /// </summary>
    [HttpPost("security/users/search")]
    [HybridAuthorize(Policies.Security.Users.User.Search)]
    [ProducesResponseType<IEnumerable<UserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUsers")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("security/users/search")]
    [HybridAuthorize(Policies.Security.Users.User.Search)]
    [ProducesResponseType<IEnumerable<UserMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUsers_get")]
    [AliasFor("searchUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchUsers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchUsers query, CancellationToken cancellation)
    {
        var matches = await _userService.SearchAsync(query, cancellation);

        var count = await _userService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}