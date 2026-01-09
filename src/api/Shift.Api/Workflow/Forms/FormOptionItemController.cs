using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Forms")]
public class FormOptionItemController : ShiftControllerBase
{
    private readonly FormOptionItemService _formOptionItemService;

    public FormOptionItemController(FormOptionItemService formOptionItemService)
    {
        _formOptionItemService = formOptionItemService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific form option item
    /// </summary>
    [HttpHead("workflow/forms-options-items/{item:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFormOptionItem")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid surveyOptionItem, CancellationToken cancellation = default)
    {
        var exists = await _formOptionItemService.AssertAsync(surveyOptionItem, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of form option items that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-options-items/collect")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Collect)]
    [ProducesResponseType<IEnumerable<FormOptionItemModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormOptionItems")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFormOptionItems query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-items")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Collect)]
    [ProducesResponseType<IEnumerable<FormOptionItemModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormOptionItems_get")]
    [AliasFor("collectFormOptionItems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFormOptionItems query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFormOptionItems query, CancellationToken cancellation)
    {
        var models = await _formOptionItemService.CollectAsync(query, cancellation);

        var count = await _formOptionItemService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the form option items that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-options-items/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormOptionItems")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFormOptionItems query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-items/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormOptionItems_get")]
    [AliasFor("countFormOptionItems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFormOptionItems query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFormOptionItems query, CancellationToken cancellation)
    {
        var count = await _formOptionItemService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of form option items that match specific criteria
    /// </summary>    
    [HttpPost("workflow/forms-options-items/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormOptionItems")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFormOptionItems query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-items/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormOptionItems_get")]
    [AliasFor("downloadFormOptionItems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFormOptionItems query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFormOptionItems query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "FormOptionItems", query.Filter.Format, User);

        var models = await _formOptionItemService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _formOptionItemService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific form option item
    /// </summary>
    [HttpGet("workflow/forms-options-items/{item:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Retrieve)]
    [ProducesResponseType<FormOptionItemModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFormOptionItem")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid surveyOptionItem, CancellationToken cancellation = default)
    {
        var model = await _formOptionItemService.RetrieveAsync(surveyOptionItem, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of form option items that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-options-items/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Search)]
    [ProducesResponseType<IEnumerable<FormOptionItemMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormOptionItems")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFormOptionItems query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-items/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionItem.Search)]
    [ProducesResponseType<IEnumerable<FormOptionItemMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormOptionItems_get")]
    [AliasFor("searchFormOptionItems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFormOptionItems query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFormOptionItems query, CancellationToken cancellation)
    {
        var matches = await _formOptionItemService.SearchAsync(query, cancellation);

        var count = await _formOptionItemService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}