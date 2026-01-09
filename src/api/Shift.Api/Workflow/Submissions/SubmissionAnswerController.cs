using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workflow;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workflow API: Submissions")]
public class SubmissionAnswerController : ShiftControllerBase
{
    private readonly SubmissionAnswerService _submissionAnswerService;

    public SubmissionAnswerController(SubmissionAnswerService submissionAnswerService)
    {
        _submissionAnswerService = submissionAnswerService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific submission answer
    /// </summary>
    [HttpHead("workflow/submissions-answers/{submission:guid}/{question:guid}")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertSubmissionAnswer")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid responseSession, [FromRoute] Guid surveyQuestion, CancellationToken cancellation = default)
    {
        var exists = await _submissionAnswerService.AssertAsync(responseSession, surveyQuestion, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of submission answers that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions-answers/collect")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Collect)]
    [ProducesResponseType<IEnumerable<SubmissionAnswerModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissionAnswers")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-answers")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Collect)]
    [ProducesResponseType<IEnumerable<SubmissionAnswerModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSubmissionAnswers_get")]
    [AliasFor("collectSubmissionAnswers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectSubmissionAnswers query, CancellationToken cancellation)
    {
        var models = await _submissionAnswerService.CollectAsync(query, cancellation);

        var count = await _submissionAnswerService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the submission answers that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions-answers/count")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissionAnswers")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-answers/count")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSubmissionAnswers_get")]
    [AliasFor("countSubmissionAnswers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountSubmissionAnswers query, CancellationToken cancellation)
    {
        var count = await _submissionAnswerService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of submission answers that match specific criteria
    /// </summary>    
    [HttpPost("workflow/submissions-answers/download")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissionAnswers")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-answers/download")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSubmissionAnswers_get")]
    [AliasFor("downloadSubmissionAnswers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectSubmissionAnswers query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workflow", "SubmissionAnswers", query.Filter.Format, User);

        var models = await _submissionAnswerService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _submissionAnswerService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific submission answer
    /// </summary>
    [HttpGet("workflow/submissions-answers/{submission:guid}/{question:guid}")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Retrieve)]
    [ProducesResponseType<SubmissionAnswerModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveSubmissionAnswer")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid responseSession, [FromRoute] Guid surveyQuestion, CancellationToken cancellation = default)
    {
        var model = await _submissionAnswerService.RetrieveAsync(responseSession, surveyQuestion, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of submission answers that match specific criteria
    /// </summary>
    [HttpPost("workflow/submissions-answers/search")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Search)]
    [ProducesResponseType<IEnumerable<SubmissionAnswerMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissionAnswers")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workflow/submissions-answers/search")]
    [HybridAuthorize(Policies.Workflow.Submissions.SubmissionAnswer.Search)]
    [ProducesResponseType<IEnumerable<SubmissionAnswerMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSubmissionAnswers_get")]
    [AliasFor("searchSubmissionAnswers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchSubmissionAnswers query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchSubmissionAnswers query, CancellationToken cancellation)
    {
        var matches = await _submissionAnswerService.SearchAsync(query, cancellation);

        var count = await _submissionAnswerService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}