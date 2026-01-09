using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Permissions")]
public class PermissionController : ControllerBase
{
    private readonly PermissionService _permissionService;

    public PermissionController(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpHead("security/permissions/{permission:guid}")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid permission, CancellationToken cancellation = default)
    {
        var exists = await _permissionService.AssertAsync(permission, cancellation);

        return Ok(exists);
    }

    [HttpGet("security/permissions/{permission:guid}")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Retrieve)]
    [ProducesResponseType(typeof(PermissionModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<PermissionModel>> RetrieveAsync([FromRoute] Guid permission, CancellationToken cancellation = default)
    {
        var model = await _permissionService.RetrieveAsync(permission, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("security/permissions/count")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountPermissions query, CancellationToken cancellation = default)
    {
        var count = await _permissionService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("security/permissions")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Collect)]
    [ProducesResponseType(typeof(IEnumerable<PermissionModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PermissionModel>>> CollectAsync([FromQuery] CollectPermissions query, CancellationToken cancellation = default)
    {
        var models = await _permissionService.CollectAsync(query, cancellation);

        var count = await _permissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("security/permissions/search")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Search)]
    [ProducesResponseType(typeof(IEnumerable<PermissionMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PermissionMatch>>> SearchAsync([FromQuery] SearchPermissions query, CancellationToken cancellation = default)
    {
        var matches = await _permissionService.SearchAsync(query, cancellation);

        var count = await _permissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("security/permissions")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Create)]
    [ProducesResponseType(typeof(PermissionModel), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult<PermissionModel>> CreateAsync([FromBody] CreatePermission create, CancellationToken cancellation = default)
    {
        var created = await _permissionService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: PermissionIdentifier {create.PermissionIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _permissionService.RetrieveAsync(create.PermissionIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpPut("security/permissions/{permission:guid}")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Modify)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> ModifyAsync([FromBody] ModifyPermission modify, CancellationToken cancellation = default)
    {
        var model = await _permissionService.RetrieveAsync(modify.PermissionIdentifier, cancellation);

        if (model is null)
            return NotFound($"Permission not found: PermissionIdentifier {modify.PermissionIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _permissionService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    [HttpDelete("security/permissions/{permission:guid}")]
    [HybridAuthorize(Policies.Security.Permissions.Permission.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid permission, CancellationToken cancellation = default)
    {
        var deleted = await _permissionService.DeleteAsync(permission, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}