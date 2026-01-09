using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Submissions")]
public class SubmissionOptionController : ShiftControllerBase
{
    private readonly SubmissionOptionService _submissionOptionService;

    public SubmissionOptionController(SubmissionOptionService submissionOptionService)
    {
        _submissionOptionService = submissionOptionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific submission option
    /// </summary>
    [HttpHead("workflow/submissions-options/{submission:guid}/{option:guid}")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertSubmissionOption")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid responseSession, [FromRoute] Guid surveyOption, CancellationToken cancellation = default)
    {
        var exists = await _submissionOptionService.AssertAsync(responseSession, surveyOption, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of submission options that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions-options/collect")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Collect)]
    [ProducesResponseType<IEnumerable<SubmissionOptionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissionOptions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-options")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Collect)]
    [ProducesResponseType<IEnumerable<SubmissionOptionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissionOptions_get")]
    [AliasFor("collectSubmissionOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectSubmissionOptions query, CancellationToken cancellation)
    {
        var models = await _submissionOptionService.CollectAsync(query, cancellation);

        var count = await _submissionOptionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the submission options that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions-options/count")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissionOptions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-options/count")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissionOptions_get")]
    [AliasFor("countSubmissionOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountSubmissionOptions query, CancellationToken cancellation)
    {
        var count = await _submissionOptionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of submission options that match specific criteria
    /// </summary>    
    [HttpPost("workflow/submissions-options/download")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissionOptions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-options/download")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissionOptions_get")]
    [AliasFor("downloadSubmissionOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectSubmissionOptions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "SubmissionOptions", query.Filter.Format, User);

        var models = await _submissionOptionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _submissionOptionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific submission option
    /// </summary>
    [HttpGet("workflow/submissions-options/{submission:guid}/{option:guid}")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Retrieve)]
    [ProducesResponseType<SubmissionOptionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveSubmissionOption")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid responseSession, [FromRoute] Guid surveyOption, CancellationToken cancellation = default)
    {
        var model = await _submissionOptionService.RetrieveAsync(responseSession, surveyOption, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of submission options that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions-options/search")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Search)]
    [ProducesResponseType<IEnumerable<SubmissionOptionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissionOptions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-options/search")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionOption.Search)]
    [ProducesResponseType<IEnumerable<SubmissionOptionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissionOptions_get")]
    [AliasFor("searchSubmissionOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchSubmissionOptions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchSubmissionOptions query, CancellationToken cancellation)
    {
        var matches = await _submissionOptionService.SearchAsync(query, cancellation);

        var count = await _submissionOptionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}