using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Setup API: Routes")]
[Route("api/setup/actions")]
public class ActionController : ControllerBase
{
    private readonly ActionService _actionService;

    public ActionController(ActionService actionService)
    {
        _actionService = actionService;
    }

    [HttpHead("{id:guid}")]
    [HybridPermission("setup/actions", DataAccess.Read)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid action, CancellationToken cancellation = default)
    {
        var exists = await _actionService.AssertAsync(action, cancellation);

        return Ok(exists);
    }

    [HttpGet("{id:guid}")]
    [HybridPermission("setup/actions", DataAccess.Read)]
    [ProducesResponseType(typeof(ActionModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<ActionModel>> RetrieveAsync([FromRoute] Guid action, CancellationToken cancellation = default)
    {
        var model = await _actionService.RetrieveAsync(action, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("count")]
    [HybridPermission("setup/actions", DataAccess.Read)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountActions query, CancellationToken cancellation = default)
    {
        var count = await _actionService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet]
    [HybridPermission("setup/actions", DataAccess.Read)]
    [ProducesResponseType(typeof(IEnumerable<ActionModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActionModel>>> CollectAsync([FromQuery] CollectActions query, CancellationToken cancellation = default)
    {
        var models = await _actionService.CollectAsync(query, cancellation);

        var count = await _actionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("search")]
    [HybridPermission("setup/actions", DataAccess.Read)]
    [ProducesResponseType(typeof(IEnumerable<ActionMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActionMatch>>> SearchAsync([FromQuery] SearchActions query, CancellationToken cancellation = default)
    {
        var matches = await _actionService.SearchAsync(query, cancellation);

        var count = await _actionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost]
    [HybridPermission("setup/actions", DataAccess.Create)]
    [ProducesResponseType(typeof(ActionModel), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult<ActionModel>> CreateAsync([FromBody] CreateAction create, CancellationToken cancellation = default)
    {
        var created = await _actionService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: ActionIdentifier {create.ActionId}. You cannot insert a duplicate object with the same primary key.");

        var model = await _actionService.RetrieveAsync(create.ActionId, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpPut("{id:guid}")]
    [HybridPermission("setup/actions", DataAccess.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> ModifyAsync([FromBody] ModifyAction modify, CancellationToken cancellation = default)
    {
        var model = await _actionService.RetrieveAsync(modify.ActionId, cancellation);

        if (model is null)
            return NotFound($"Action not found: ActionIdentifier {modify.ActionId}. You cannot modify an object that is not in the database.");

        var modified = await _actionService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [HybridPermission("setup/actions", DataAccess.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid action, CancellationToken cancellation = default)
    {
        var deleted = await _actionService.DeleteAsync(action, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}