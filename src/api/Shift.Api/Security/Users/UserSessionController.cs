using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Users")]
public class UserSessionController : ControllerBase
{
    private readonly UserSessionService _userSessionService;

    public UserSessionController(UserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    #region Queries

    /// <summary>
    /// Check for the existence of one specific user session
    /// </summary>
    [HttpHead("security/users-sessions/{session:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertUserSession")]
    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid session,
        CancellationToken cancellation = default)
    {
        var exists = await _userSessionService.AssertAsync(session, cancellation);
        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collect the list of user sessions that match specific criteria
    /// </summary>
    [HttpPost("security/users-sessions/collect")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Collect)]
    [ProducesResponseType<IEnumerable<UserSessionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUserSessions")]
    public async Task<ActionResult<IEnumerable<UserSessionModel>>> PostCollectAsync(
        [FromBody] CollectUserSessions query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpGet("security/users-sessions")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Collect)]
    [ProducesResponseType<IEnumerable<UserSessionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectUserSessions_get")]
    [AliasFor("collectUserSessions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserSessionModel>>> GetCollectAsync(
        [FromQuery] CollectUserSessions query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<UserSessionModel>>> CollectAsync(
        CollectUserSessions query,
        CancellationToken cancellation)
    {
        var models = await _userSessionService.CollectAsync(query, cancellation);

        var count = await _userSessionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Count the user sessions that match specific criteria
    /// </summary>
    [HttpPost("security/users-sessions/count")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUserSessions")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountUserSessions query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpGet("security/users-sessions/count")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countUserSessions_get")]
    [AliasFor("countUserSessions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountUserSessions query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(
        CountUserSessions query,
        CancellationToken cancellation)
    {
        var count = await _userSessionService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Download the list of user sessions that match specific criteria
    /// </summary>    
    [HttpPost("security/users-sessions/download")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUserSessions")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectUserSessions query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    [HttpGet("security/users-sessions/download")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadUserSessions_get")]
    [AliasFor("downloadUserSessions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectUserSessions query,
        CancellationToken cancellation = default)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        CollectUserSessions query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Security", "UserSessions", query.Filter.Format, User);

        var models = await _userSessionService.DownloadAsync(query, cancellation);

        var content = _userSessionService.Serialize(models, exporter.GetFileFormat());

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieve one specific user session
    /// </summary>
    [HttpGet("security/users-sessions/{session:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Retrieve)]
    [ProducesResponseType<UserSessionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveUserSession")]
    public async Task<ActionResult<UserSessionModel>> RetrieveAsync(
        [FromRoute] Guid session,
        CancellationToken cancellation = default)
    {
        var model = await _userSessionService.RetrieveAsync(session, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Search for the list of user sessions that match specific criteria
    /// </summary>
    [HttpPost("security/users-sessions/search")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Search)]
    [ProducesResponseType<IEnumerable<UserSessionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUserSessions")]
    public async Task<ActionResult<IEnumerable<UserSessionMatch>>> PostSearchAsync(
        [FromBody] SearchUserSessions query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpGet("security/users-sessions/search")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Search)]
    [ProducesResponseType<IEnumerable<UserSessionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchUserSessions_get")]
    [AliasFor("searchUserSessions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<UserSessionMatch>>> GetSearchAsync(
        [FromQuery] SearchUserSessions query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<UserSessionMatch>>> SearchAsync(
        SearchUserSessions query,
        CancellationToken cancellation)
    {
        var matches = await _userSessionService.SearchAsync(query, cancellation);

        var count = await _userSessionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("security/users-sessions")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Create)]
    [ProducesResponseType<UserSessionModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createUserSession")]
    public async Task<ActionResult<UserSessionModel>> CreateAsync(
        [FromBody] CreateUserSession create,
        CancellationToken cancellation = default)
    {
        var created = await _userSessionService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: SessionIdentifier {create.SessionIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _userSessionService.RetrieveAsync(create.SessionIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("security/users-sessions/{session:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteUserSession")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid session,
        CancellationToken cancellation = default)
    {
        var deleted = await _userSessionService.DeleteAsync(session, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("security/users-sessions/{session:guid}")]
    [HybridAuthorize(Policies.Security.Users.UserSession.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyUserSession")]
    public async Task<IActionResult> ModifyAsync(
        [FromBody] ModifyUserSession modify,
        CancellationToken cancellation = default)
    {
        var model = await _userSessionService.RetrieveAsync(modify.SessionIdentifier, cancellation);

        if (model is null)
            return NotFound($"UserSession not found: SessionIdentifier {modify.SessionIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _userSessionService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands
}