using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Permissions")]
public class PermissionController : ControllerBase
{
    private readonly PermissionService _permissionService;
    private readonly PermissionCache _permissionCache;

    public PermissionController(PermissionService permissionService, PermissionCache permissionCache)
    {
        _permissionService = permissionService;
        _permissionCache = permissionCache;
    }

    [HttpHead("api/security/permissions/{permission:guid}")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid permission, CancellationToken cancellation = default)
    {
        var exists = await _permissionService.AssertAsync(permission, cancellation);

        return Ok(exists);
    }

    [HttpGet("api/security/permissions/{permission:guid}")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    [ProducesResponseType(typeof(PermissionModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<PermissionModel>> RetrieveAsync([FromRoute] Guid permission, CancellationToken cancellation = default)
    {
        var model = await _permissionService.RetrieveAsync(permission, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("api/security/permissions/count")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountPermissions query, CancellationToken cancellation = default)
    {
        var count = await _permissionService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("api/security/permissions")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    [ProducesResponseType(typeof(IEnumerable<PermissionModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PermissionModel>>> CollectAsync([FromQuery] CollectPermissions query, CancellationToken cancellation = default)
    {
        var models = await _permissionService.CollectAsync(query, cancellation);

        var count = await _permissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("api/security/permissions/search")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    [ProducesResponseType(typeof(IEnumerable<PermissionMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PermissionMatch>>> SearchAsync([FromQuery] SearchPermissions query, CancellationToken cancellation = default)
    {
        var matches = await _permissionService.SearchAsync(query, cancellation);

        var count = await _permissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("api/security/permissions")]
    [HybridPermission("security/permissions", DataAccess.Create)]
    [ProducesResponseType(typeof(PermissionModel), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult<PermissionModel>> CreateAsync([FromBody] CreatePermission create, CancellationToken cancellation = default)
    {
        var created = await _permissionService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: PermissionIdentifier {create.PermissionId}. You cannot insert a duplicate object with the same primary key.");

        var model = await _permissionService.RetrieveAsync(create.PermissionId, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpPut("api/security/permissions/{permission:guid}")]
    [HybridPermission("security/permissions", DataAccess.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> ModifyAsync([FromBody] ModifyPermission modify, CancellationToken cancellation = default)
    {
        var model = await _permissionService.RetrieveAsync(modify.PermissionId, cancellation);

        if (model is null)
            return NotFound($"Permission not found: PermissionIdentifier {modify.PermissionId}. You cannot modify an object that is not in the database.");

        var modified = await _permissionService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    [HttpGet("api/security/permissions/refresh")]
    [HybridPermission("security/permissions", AuthorityAccess.Operator)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public IActionResult Refresh(CancellationToken cancellation = default)
    {
        _permissionCache.Refresh(null);

        return Ok();
    }
}