using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Platform API: Routes")]
public class ActionController : ControllerBase
{
    private readonly ActionService _actionService;

    public ActionController(ActionService actionService)
    {
        _actionService = actionService;
    }

    [HttpHead("platform/actions/{id:guid}")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid action, CancellationToken cancellation = default)
    {
        var exists = await _actionService.AssertAsync(action, cancellation);

        return Ok(exists);
    }

    [HttpGet("platform/actions/{id:guid}")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Retrieve)]
    [ProducesResponseType(typeof(ActionModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<ActionModel>> RetrieveAsync([FromRoute] Guid action, CancellationToken cancellation = default)
    {
        var model = await _actionService.RetrieveAsync(action, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("platform/actions/count")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountActions query, CancellationToken cancellation = default)
    {
        var count = await _actionService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("platform/actions")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Collect)]
    [ProducesResponseType(typeof(IEnumerable<ActionModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActionModel>>> CollectAsync([FromQuery] CollectActions query, CancellationToken cancellation = default)
    {
        var models = await _actionService.CollectAsync(query, cancellation);

        var count = await _actionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("platform/actions/search")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Search)]
    [ProducesResponseType(typeof(IEnumerable<ActionMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActionMatch>>> SearchAsync([FromQuery] SearchActions query, CancellationToken cancellation = default)
    {
        var matches = await _actionService.SearchAsync(query, cancellation);

        var count = await _actionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("platform/actions")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Create)]
    [ProducesResponseType(typeof(ActionModel), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult<ActionModel>> CreateAsync([FromBody] CreateAction create, CancellationToken cancellation = default)
    {
        var created = await _actionService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: ActionIdentifier {create.ActionIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _actionService.RetrieveAsync(create.ActionIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpPut("platform/actions/{id:guid}")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Modify)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> ModifyAsync([FromBody] ModifyAction modify, CancellationToken cancellation = default)
    {
        var model = await _actionService.RetrieveAsync(modify.ActionIdentifier, cancellation);

        if (model is null)
            return NotFound($"Action not found: ActionIdentifier {modify.ActionIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _actionService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    [HttpDelete("platform/actions/{id:guid}")]
    [HybridAuthorize(Policies.Setup.Routes.Action.Delete)]
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