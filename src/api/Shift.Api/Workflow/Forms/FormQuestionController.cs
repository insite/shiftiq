using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Forms")]
public class FormQuestionController : ShiftControllerBase
{
    private readonly FormQuestionService _formQuestionService;

    public FormQuestionController(FormQuestionService formQuestionService)
    {
        _formQuestionService = formQuestionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific form question
    /// </summary>
    [HttpHead("workflow/forms-questions/{question:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFormQuestion")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid surveyQuestion, CancellationToken cancellation = default)
    {
        var exists = await _formQuestionService.AssertAsync(surveyQuestion, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of form questions that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-questions/collect")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Collect)]
    [ProducesResponseType<IEnumerable<FormQuestionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormQuestions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFormQuestions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-questions")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Collect)]
    [ProducesResponseType<IEnumerable<FormQuestionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFormQuestions_get")]
    [AliasFor("collectFormQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFormQuestions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFormQuestions query, CancellationToken cancellation)
    {
        var models = await _formQuestionService.CollectAsync(query, cancellation);

        var count = await _formQuestionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the form questions that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-questions/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormQuestions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFormQuestions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-questions/count")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFormQuestions_get")]
    [AliasFor("countFormQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFormQuestions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFormQuestions query, CancellationToken cancellation)
    {
        var count = await _formQuestionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of form questions that match specific criteria
    /// </summary>    
    [HttpPost("workflow/forms-questions/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormQuestions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFormQuestions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-questions/download")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFormQuestions_get")]
    [AliasFor("downloadFormQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFormQuestions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFormQuestions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "FormQuestions", query.Filter.Format, User);

        var models = await _formQuestionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _formQuestionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific form question
    /// </summary>
    [HttpGet("workflow/forms-questions/{question:guid}")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Retrieve)]
    [ProducesResponseType<FormQuestionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFormQuestion")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid surveyQuestion, CancellationToken cancellation = default)
    {
        var model = await _formQuestionService.RetrieveAsync(surveyQuestion, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of form questions that match specific criteria
    /// </summary>
    [HttpPost("workflow/forms-questions/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Search)]
    [ProducesResponseType<IEnumerable<FormQuestionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormQuestions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFormQuestions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/forms-questions/search")]
    [HybridAuthorize(Policies.Workflow.Forms.FormQuestion.Search)]
    [ProducesResponseType<IEnumerable<FormQuestionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFormQuestions_get")]
    [AliasFor("searchFormQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFormQuestions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFormQuestions query, CancellationToken cancellation)
    {
        var matches = await _formQuestionService.SearchAsync(query, cancellation);

        var count = await _formQuestionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}