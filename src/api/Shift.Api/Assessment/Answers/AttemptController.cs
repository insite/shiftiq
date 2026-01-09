using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class AttemptController : ShiftControllerBase
{
    private readonly AttemptService _attemptService;

    public AttemptController(AttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific attempt
    /// </summary>
    [HttpHead("assessment/attempts/{attempt:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAttempt")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, CancellationToken cancellation = default)
    {
        var exists = await _attemptService.AssertAsync(attempt, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of attempts that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Collect)]
    [ProducesResponseType<IEnumerable<AttemptModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttempts")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAttempts query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Collect)]
    [ProducesResponseType<IEnumerable<AttemptModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttempts_get")]
    [AliasFor("collectAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAttempts query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAttempts query, CancellationToken cancellation)
    {
        var models = await _attemptService.CollectAsync(query, cancellation);

        var count = await _attemptService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the attempts that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttempts")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAttempts query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttempts_get")]
    [AliasFor("countAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAttempts query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAttempts query, CancellationToken cancellation)
    {
        var count = await _attemptService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of attempts that match specific criteria
    /// </summary>    
    [HttpPost("assessment/attempts/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttempts")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAttempts query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttempts_get")]
    [AliasFor("downloadAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAttempts query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAttempts query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "Attempts", query.Filter.Format, User);

        var models = await _attemptService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _attemptService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific attempt
    /// </summary>
    [HttpGet("assessment/attempts/{attempt:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Retrieve)]
    [ProducesResponseType<AttemptModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAttempt")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, CancellationToken cancellation = default)
    {
        var model = await _attemptService.RetrieveAsync(attempt, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of attempts that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Search)]
    [ProducesResponseType<IEnumerable<AttemptMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttempts")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAttempts query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.Attempt.Search)]
    [ProducesResponseType<IEnumerable<AttemptMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttempts_get")]
    [AliasFor("searchAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAttempts query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAttempts query, CancellationToken cancellation)
    {
        var matches = await _attemptService.SearchAsync(query, cancellation);

        var count = await _attemptService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}