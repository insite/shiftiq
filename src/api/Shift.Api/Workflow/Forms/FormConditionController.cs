using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Forms")]
public class FormConditionController : ShiftControllerBase
{
    private readonly FormConditionService _formConditionService;

    public FormConditionController(FormConditionService formConditionService)
    {
        _formConditionService = formConditionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific form condition
    /// </summary>
    [HttpHead("workflow/forms-conditions/{question:guid}/{option:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFormCondition")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid maskedSurveyQuestion, [FromRoute] Guid maskingSurveyOptionItem, CancellationToken cancellation = default)
    {
        var exists = await _formConditionService.AssertAsync(maskedSurveyQuestion, maskingSurveyOptionItem, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of form conditions that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-conditions/collect")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Collect)]
    [ProducesResponseType<IEnumerable<FormConditionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormConditions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFormConditions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-conditions")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Collect)]
    [ProducesResponseType<IEnumerable<FormConditionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormConditions_get")]
    [AliasFor("collectFormConditions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFormConditions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFormConditions query, CancellationToken cancellation)
    {
        var models = await _formConditionService.CollectAsync(query, cancellation);

        var count = await _formConditionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the form conditions that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-conditions/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormConditions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFormConditions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-conditions/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormConditions_get")]
    [AliasFor("countFormConditions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFormConditions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFormConditions query, CancellationToken cancellation)
    {
        var count = await _formConditionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of form conditions that match specific criteria
    /// </summary>    
    [HttpPost("workflow/forms-conditions/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormConditions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFormConditions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-conditions/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormConditions_get")]
    [AliasFor("downloadFormConditions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFormConditions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFormConditions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "FormConditions", query.Filter.Format, User);

        var models = await _formConditionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _formConditionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific form condition
    /// </summary>
    [HttpGet("workflow/forms-conditions/{question:guid}/{option:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Retrieve)]
    [ProducesResponseType<FormConditionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFormCondition")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid maskedSurveyQuestion, [FromRoute] Guid maskingSurveyOptionItem, CancellationToken cancellation = default)
    {
        var model = await _formConditionService.RetrieveAsync(maskedSurveyQuestion, maskingSurveyOptionItem, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of form conditions that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-conditions/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Search)]
    [ProducesResponseType<IEnumerable<FormConditionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormConditions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFormConditions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-conditions/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormCondition.Search)]
    [ProducesResponseType<IEnumerable<FormConditionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormConditions_get")]
    [AliasFor("searchFormConditions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFormConditions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFormConditions query, CancellationToken cancellation)
    {
        var matches = await _formConditionService.SearchAsync(query, cancellation);

        var count = await _formConditionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}