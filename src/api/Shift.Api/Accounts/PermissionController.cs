using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Accounts API: Permissions")]
public class PermissionController : ShiftControllerBase
{
    private readonly PermissionService _permissionService;
    private readonly PermissionCache _permissionCache;
    private readonly IPrincipalProvider _principalProvider;

    public PermissionController(PermissionService permissionService, PermissionCache permissionCache, IPrincipalProvider principalProvider)
    {
        _permissionService = permissionService;
        _permissionCache = permissionCache;
        _principalProvider = principalProvider;
    }

    [HttpHead("api/accounts/permissions/{permission:guid}")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid permission, CancellationToken cancellation = default)
    {
        var exists = await _permissionService.AssertAsync(permission, cancellation);

        return Ok(exists);
    }

    [HttpGet("api/accounts/permissions/{permission:guid}")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    [ProducesResponseType(typeof(PermissionModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionModel>> RetrieveAsync([FromRoute] Guid permission, CancellationToken cancellation = default)
    {
        var model = await _permissionService.RetrieveAsync(permission, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("api/accounts/permissions/count")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountPermissions query, CancellationToken cancellation = default)
    {
        var count = await _permissionService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("api/accounts/permissions")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    public async Task<ActionResult<IEnumerable<PermissionModel>>> CollectAsync([FromQuery] CollectPermissions query, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        var models = await _permissionService.CollectAsync(query, cancellation);

        var count = await _permissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        Response.AddFingerprint(models);

        return Ok(models);
    }

    [HttpGet("api/accounts/permissions/search")]
    [HybridPermission("security/permissions", DataAccess.Read)]
    public async Task<ActionResult<IEnumerable<PermissionMatch>>> SearchAsync([FromQuery] SearchPermissions query, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        var matches = await _permissionService.SearchAsync(query, cancellation);

        var count = await _permissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("api/accounts/permissions")]
    [HybridPermission("security/permissions", DataAccess.Create)]
    [ProducesResponseType(typeof(PermissionModel), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePermission create, CancellationToken cancellation = default)
    {
        var created = await _permissionService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: PermissionIdentifier {create.PermissionId}. You cannot insert a duplicate object with the same primary key.");

        var model = await _permissionService.RetrieveAsync(create.PermissionId, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpPut("api/accounts/permissions/{permission:guid}")]
    [HybridPermission("security/permissions", DataAccess.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult> ModifyAsync([FromBody] ModifyPermission modify, CancellationToken cancellation = default)
    {
        var model = await _permissionService.RetrieveAsync(modify.PermissionId, cancellation);

        if (model is null)
            return NotFound($"Permission not found: PermissionIdentifier {modify.PermissionId}. You cannot modify an object that is not in the database.");

        var modified = await _permissionService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    [HttpGet("api/accounts/permissions/refresh")]
    [HybridPermission("security/permissions", AuthorityAccess.Operator)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Refresh(CancellationToken cancellation = default)
    {
        _permissionCache.Refresh(null);

        return Ok();
    }
}