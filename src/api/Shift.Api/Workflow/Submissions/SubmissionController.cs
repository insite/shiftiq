using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Submissions")]
public class SubmissionController : ShiftControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific submission
    /// </summary>
    [HttpHead("workflow/submissions/{submission:guid}")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertSubmission")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid responseSession, CancellationToken cancellation = default)
    {
        var exists = await _submissionService.AssertAsync(responseSession, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of submissions that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions/collect")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Collect)]
    [ProducesResponseType<IEnumerable<SubmissionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Collect)]
    [ProducesResponseType<IEnumerable<SubmissionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissions_get")]
    [AliasFor("collectSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectSubmissions query, CancellationToken cancellation)
    {
        var models = await _submissionService.CollectAsync(query, cancellation);

        var count = await _submissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the submissions that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions/count")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountSubmissions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions/count")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissions_get")]
    [AliasFor("countSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountSubmissions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountSubmissions query, CancellationToken cancellation)
    {
        var count = await _submissionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of submissions that match specific criteria
    /// </summary>    
    [HttpPost("workflow/submissions/download")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions/download")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissions_get")]
    [AliasFor("downloadSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectSubmissions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectSubmissions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "Submissions", query.Filter.Format, User);

        var models = await _submissionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _submissionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific submission
    /// </summary>
    [HttpGet("workflow/submissions/{submission:guid}")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Retrieve)]
    [ProducesResponseType<SubmissionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveSubmission")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid responseSession, CancellationToken cancellation = default)
    {
        var model = await _submissionService.RetrieveAsync(responseSession, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of submissions that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions/search")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Search)]
    [ProducesResponseType<IEnumerable<SubmissionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchSubmissions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions/search")]
    [HybridAuthorize(Policies.Workflow.Submissions.Submission.Search)]
    [ProducesResponseType<IEnumerable<SubmissionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissions_get")]
    [AliasFor("searchSubmissions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchSubmissions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchSubmissions query, CancellationToken cancellation)
    {
        var matches = await _submissionService.SearchAsync(query, cancellation);

        var count = await _submissionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}