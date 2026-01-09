using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Forms")]
public class FormOptionListController : ShiftControllerBase
{
    private readonly FormOptionListService _formOptionListService;

    public FormOptionListController(FormOptionListService formOptionListService)
    {
        _formOptionListService = formOptionListService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific form option list
    /// </summary>
    [HttpHead("workflow/forms-options-lists/{list:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFormOptionList")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid surveyOptionList, CancellationToken cancellation = default)
    {
        var exists = await _formOptionListService.AssertAsync(surveyOptionList, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of form option lists that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-options-lists/collect")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Collect)]
    [ProducesResponseType<IEnumerable<FormOptionListModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormOptionLists")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFormOptionLists query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-lists")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Collect)]
    [ProducesResponseType<IEnumerable<FormOptionListModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormOptionLists_get")]
    [AliasFor("collectFormOptionLists")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFormOptionLists query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFormOptionLists query, CancellationToken cancellation)
    {
        var models = await _formOptionListService.CollectAsync(query, cancellation);

        var count = await _formOptionListService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the form option lists that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-options-lists/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormOptionLists")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFormOptionLists query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-lists/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormOptionLists_get")]
    [AliasFor("countFormOptionLists")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFormOptionLists query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFormOptionLists query, CancellationToken cancellation)
    {
        var count = await _formOptionListService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of form option lists that match specific criteria
    /// </summary>    
    [HttpPost("workflow/forms-options-lists/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormOptionLists")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFormOptionLists query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-lists/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormOptionLists_get")]
    [AliasFor("downloadFormOptionLists")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFormOptionLists query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFormOptionLists query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "FormOptionLists", query.Filter.Format, User);

        var models = await _formOptionListService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _formOptionListService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific form option list
    /// </summary>
    [HttpGet("workflow/forms-options-lists/{list:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Retrieve)]
    [ProducesResponseType<FormOptionListModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFormOptionList")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid surveyOptionList, CancellationToken cancellation = default)
    {
        var model = await _formOptionListService.RetrieveAsync(surveyOptionList, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of form option lists that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-options-lists/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Search)]
    [ProducesResponseType<IEnumerable<FormOptionListMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormOptionLists")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFormOptionLists query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-options-lists/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormOptionList.Search)]
    [ProducesResponseType<IEnumerable<FormOptionListMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormOptionLists_get")]
    [AliasFor("searchFormOptionLists")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFormOptionLists query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFormOptionLists query, CancellationToken cancellation)
    {
        var matches = await _formOptionListService.SearchAsync(query, cancellation);

        var count = await _formOptionListService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}