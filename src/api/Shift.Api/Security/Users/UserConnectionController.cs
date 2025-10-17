using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Users")]
public class UserConnectionController : ControllerBase
{
    private readonly UserConnectionService _userConnectionService;

    public UserConnectionController(UserConnectionService userConnectionService)
    {
        _userConnectionService = userConnectionService;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific user connection
    /// </summary>
    [HttpHead("security/users-connections/{from:guid}/{to:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertUserConnection")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid fromUser, [FromRoute] Guid toUser,
        CancellationToken cancellation = default)
    {
        var exists = await _userConnectionService.AssertAsync(fromUser, toUser, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of user connections that match specific criteria
    /// </summary>
    [HttpPost("security/users-connections/collect")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Collect)]
    [ProducesResponseType<IEnumerable<UserConnectionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUserConnections")]
    public async Task<ActionResult<IEnumerable<UserConnectionModel>>> PostCollectAsync(
        [FromBody] CollectUserConnections query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpGet("security/users-connections")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Collect)]
    [ProducesResponseType<IEnumerable<UserConnectionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUserConnections_get")]
    [AliasFor("collectUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserConnectionModel>>> GetCollectAsync(
        [FromQuery] CollectUserConnections query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<UserConnectionModel>>> CollectAsync(
        CollectUserConnections query,
        CancellationToken cancellation)
    {
        var models = await _userConnectionService.CollectAsync(query, cancellation);

        var count = await _userConnectionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the user connections that match specific criteria
    /// </summary>
    [HttpPost("security/users-connections/count")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUserConnections")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountUserConnections query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpGet("security/users-connections/count")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUserConnections_get")]
    [AliasFor("countUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountUserConnections query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(
        CountUserConnections query,
        CancellationToken cancellation)
    {
        var count = await _userConnectionService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of user connections that match specific criteria
    /// </summary>    
    [HttpPost("security/users-connections/download")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUserConnections")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectUserConnections query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    [HttpGet("security/users-connections/download")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUserConnections_get")]
    [AliasFor("downloadUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectUserConnections query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        CollectUserConnections query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Security", "UserConnections", query.Filter.Format, User);

        var models = await _userConnectionService.DownloadAsync(query, cancellation);

        var content = _userConnectionService.Serialize(models, exporter.GetFileFormat());

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific user connection
    /// </summary>
    [HttpGet("security/users-connections/{from:guid}/{to:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Retrieve)]
    [ProducesResponseType<UserConnectionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveUserConnection")]
    public async Task<ActionResult<UserConnectionModel>> RetrieveAsync(
        [FromRoute] Guid fromUser, [FromRoute] Guid toUser,
        CancellationToken cancellation = default)
    {
        var model = await _userConnectionService.RetrieveAsync(fromUser, toUser, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Search for the list of user connections that match specific criteria
    /// </summary>
    [HttpPost("security/users-connections/search")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Search)]
    [ProducesResponseType<IEnumerable<UserConnectionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUserConnections")]
    public async Task<ActionResult<IEnumerable<UserConnectionMatch>>> PostSearchAsync(
        [FromBody] SearchUserConnections query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpGet("security/users-connections/search")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Search)]
    [ProducesResponseType<IEnumerable<UserConnectionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUserConnections_get")]
    [AliasFor("searchUserConnections")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserConnectionMatch>>> GetSearchAsync(
        [FromQuery] SearchUserConnections query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<UserConnectionMatch>>> SearchAsync(
        SearchUserConnections query,
        CancellationToken cancellation)
    {
        var matches = await _userConnectionService.SearchAsync(query, cancellation);

        var count = await _userConnectionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("security/users-connections")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Create)]
    [ProducesResponseType<UserConnectionModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createUserConnection")]
    public async Task<ActionResult<UserConnectionModel>> CreateAsync(
        [FromBody] CreateUserConnection create,
        CancellationToken cancellation = default)
    {
        var created = await _userConnectionService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: FromUserIdentifier {create.FromUserIdentifier}ToUserIdentifier {create.ToUserIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _userConnectionService.RetrieveAsync(create.FromUserIdentifier, create.ToUserIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("security/users-connections/{from:guid}/{to:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteUserConnection")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid fromUser, [FromRoute] Guid toUser,
        CancellationToken cancellation = default)
    {
        var deleted = await _userConnectionService.DeleteAsync(fromUser, toUser, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("security/users-connections/{from:guid}/{to:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserConnection.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyUserConnection")]
    public async Task<IActionResult> ModifyAsync(
        [FromBody] ModifyUserConnection modify,
        CancellationToken cancellation = default)
    {
        var model = await _userConnectionService.RetrieveAsync(modify.FromUserIdentifier, modify.ToUserIdentifier, cancellation);

        if (model is null)
            return NotFound($"UserConnection not found: FromUserIdentifier {modify.FromUserIdentifier}ToUserIdentifier {modify.ToUserIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _userConnectionService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands
}