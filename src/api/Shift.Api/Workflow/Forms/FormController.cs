using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Forms")]
public class FormController : ShiftControllerBase
{
    private readonly FormService _formService;

    public FormController(FormService formService)
    {
        _formService = formService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific form
    /// </summary>
    [HttpHead("workflow/forms/{form:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertForm")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid surveyForm, CancellationToken cancellation = default)
    {
        var exists = await _formService.AssertAsync(surveyForm, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of forms that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms/collect")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Collect)]
    [ProducesResponseType<IEnumerable<FormModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectForms")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/forms")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Collect)]
    [ProducesResponseType<IEnumerable<FormModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectForms_get")]
    [AliasFor("collectForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectForms query, CancellationToken cancellation)
    {
        var models = await _formService.CollectAsync(query, cancellation);

        var count = await _formService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the forms that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms/count")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countForms")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/forms/count")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countForms_get")]
    [AliasFor("countForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountForms query, CancellationToken cancellation)
    {
        var count = await _formService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of forms that match specific criteria
    /// </summary>    
    [HttpPost("workflow/forms/download")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadForms")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectForms query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/forms/download")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadForms_get")]
    [AliasFor("downloadForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectForms query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectForms query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "Forms", query.Filter.Format, User);

        var models = await _formService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _formService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific form
    /// </summary>
    [HttpGet("workflow/forms/{form:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Retrieve)]
    [ProducesResponseType<FormModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveForm")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid surveyForm, CancellationToken cancellation = default)
    {
        var model = await _formService.RetrieveAsync(surveyForm, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of forms that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms/search")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Search)]
    [ProducesResponseType<IEnumerable<FormMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchForms")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/forms/search")]
    [HybridAuthorize(Policies.Workflow.Forms.Form.Search)]
    [ProducesResponseType<IEnumerable<FormMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchForms_get")]
    [AliasFor("searchForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchForms query, CancellationToken cancellation)
    {
        var matches = await _formService.SearchAsync(query, cancellation);

        var count = await _formService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}