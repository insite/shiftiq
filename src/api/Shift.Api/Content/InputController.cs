using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Content;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Content API: Inputs")]
public class InputController : ControllerBase
{
    private readonly InputService _inputService;

    public InputController(InputService inputService)
    {
        _inputService = inputService;
    }

    [HttpHead("content/inputs/{content:guid}")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid content, CancellationToken cancellation = default)
    {
        var exists = await _inputService.AssertAsync(content, cancellation);

        return Ok(exists);
    }

    [HttpGet("content/inputs/{content:guid}")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Retrieve)]
    [ProducesResponseType(typeof(InputModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<InputModel>> RetrieveAsync([FromRoute] Guid content, CancellationToken cancellation = default)
    {
        var model = await _inputService.RetrieveAsync(content, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("content/inputs/count")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountInputs query, CancellationToken cancellation = default)
    {
        var count = await _inputService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("content/inputs")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Collect)]
    [ProducesResponseType(typeof(IEnumerable<InputModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InputModel>>> CollectAsync([FromQuery] CollectInputs query, CancellationToken cancellation = default)
    {
        var models = await _inputService.CollectAsync(query, cancellation);

        var count = await _inputService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("content/inputs/search")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Search)]
    [ProducesResponseType(typeof(IEnumerable<InputMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InputMatch>>> SearchAsync([FromQuery] SearchInputs query, CancellationToken cancellation = default)
    {
        var matches = await _inputService.SearchAsync(query, cancellation);

        var count = await _inputService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("content/inputs")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Create)]
    [ProducesResponseType(typeof(InputModel), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult<InputModel>> CreateAsync([FromBody] CreateInput create, CancellationToken cancellation = default)
    {
        var created = await _inputService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: ContentIdentifier {create.ContentIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _inputService.RetrieveAsync(create.ContentIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpPut("content/inputs/{content:guid}")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Modify)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> ModifyAsync([FromBody] ModifyInput modify, CancellationToken cancellation = default)
    {
        var model = await _inputService.RetrieveAsync(modify.ContentIdentifier, cancellation);

        if (model is null)
            return NotFound($"Input not found: ContentIdentifier {modify.ContentIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _inputService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    [HttpDelete("content/inputs/{content:guid}")]
    [HybridAuthorize(Policies.Content.Inputs.Input.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid content, CancellationToken cancellation = default)
    {
        var deleted = await _inputService.DeleteAsync(content, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}