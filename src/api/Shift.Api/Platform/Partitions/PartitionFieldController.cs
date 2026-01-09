using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Platform API: Partitions")]
public class PartitionFieldController : ControllerBase
{
    private readonly PartitionFieldService _partitionFieldService;

    public PartitionFieldController(PartitionFieldService partitionFieldService)
    {
        _partitionFieldService = partitionFieldService;
    }

    [HttpHead("platform/partitions-fields/{setting:guid}")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid setting, CancellationToken cancellation = default)
    {
        var exists = await _partitionFieldService.AssertAsync(setting, cancellation);

        return Ok(exists);
    }

    [HttpGet("platform/partitions-fields/{setting:guid}")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Retrieve)]
    [ProducesResponseType(typeof(PartitionFieldModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<PartitionFieldModel>> RetrieveAsync([FromRoute] Guid setting, CancellationToken cancellation = default)
    {
        var model = await _partitionFieldService.RetrieveAsync(setting, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("platform/partitions-fields/count")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountPartitionFields query, CancellationToken cancellation = default)
    {
        var count = await _partitionFieldService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("platform/partitions-fields")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Collect)]
    [ProducesResponseType(typeof(IEnumerable<PartitionFieldModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PartitionFieldModel>>> CollectAsync([FromQuery] CollectPartitionFields query, CancellationToken cancellation = default)
    {
        var models = await _partitionFieldService.CollectAsync(query, cancellation);

        var count = await _partitionFieldService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("platform/partitions-fields/search")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Search)]
    [ProducesResponseType(typeof(IEnumerable<PartitionFieldMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PartitionFieldMatch>>> SearchAsync([FromQuery] SearchPartitionFields query, CancellationToken cancellation = default)
    {
        var matches = await _partitionFieldService.SearchAsync(query, cancellation);

        var count = await _partitionFieldService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpPost("platform/partitions-fields")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Create)]
    [ProducesResponseType(typeof(PartitionFieldModel), StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<ActionResult<PartitionFieldModel>> CreateAsync([FromBody] CreatePartitionField create, CancellationToken cancellation = default)
    {
        var created = await _partitionFieldService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: SettingIdentifier {create.SettingIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _partitionFieldService.RetrieveAsync(create.SettingIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpPut("platform/partitions-fields/{setting:guid}")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Modify)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailure), StatusCodes.Status400BadRequest, "application/json")]
    public async Task<IActionResult> ModifyAsync([FromBody] ModifyPartitionField modify, CancellationToken cancellation = default)
    {
        var model = await _partitionFieldService.RetrieveAsync(modify.SettingIdentifier, cancellation);

        if (model is null)
            return NotFound($"PartitionField not found: SettingIdentifier {modify.SettingIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _partitionFieldService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    [HttpDelete("platform/partitions-fields/{setting:guid}")]
    [HybridAuthorize(Policies.Setup.Partitions.PartitionSetting.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid setting, CancellationToken cancellation = default)
    {
        var deleted = await _partitionFieldService.DeleteAsync(setting, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}