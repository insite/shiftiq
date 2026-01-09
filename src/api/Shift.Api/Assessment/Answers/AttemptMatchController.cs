using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class AttemptMatchController : ShiftControllerBase
{
    private readonly AttemptMatchService _attemptMatchService;

    public AttemptMatchController(AttemptMatchService attemptMatchService)
    {
        _attemptMatchService = attemptMatchService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific attempt match
    /// </summary>
    [HttpHead("assessment/attempts-matches/{attempt:guid}/{question:guid}/{match:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAttemptMatch")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, [FromRoute] int matchSequence, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var exists = await _attemptMatchService.AssertAsync(attempt, matchSequence, question, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of attempt matches that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-matches/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Collect)]
    [ProducesResponseType<IEnumerable<AttemptMatchModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptMatches")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAttemptMatches query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-matches")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Collect)]
    [ProducesResponseType<IEnumerable<AttemptMatchModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptMatches_get")]
    [AliasFor("collectAttemptMatches")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAttemptMatches query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAttemptMatches query, CancellationToken cancellation)
    {
        var models = await _attemptMatchService.CollectAsync(query, cancellation);

        var count = await _attemptMatchService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the attempt matches that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-matches/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptMatches")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAttemptMatches query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-matches/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptMatches_get")]
    [AliasFor("countAttemptMatches")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAttemptMatches query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAttemptMatches query, CancellationToken cancellation)
    {
        var count = await _attemptMatchService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of attempt matches that match specific criteria
    /// </summary>    
    [HttpPost("assessment/attempts-matches/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptMatches")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAttemptMatches query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-matches/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptMatches_get")]
    [AliasFor("downloadAttemptMatches")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAttemptMatches query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAttemptMatches query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "AttemptMatches", query.Filter.Format, User);

        var models = await _attemptMatchService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _attemptMatchService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific attempt match
    /// </summary>
    [HttpGet("assessment/attempts-matches/{attempt:guid}/{question:guid}/{match:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Retrieve)]
    [ProducesResponseType<AttemptMatchModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAttemptMatch")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, [FromRoute] int matchSequence, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var model = await _attemptMatchService.RetrieveAsync(attempt, matchSequence, question, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of attempt matches that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-matches/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Search)]
    [ProducesResponseType<IEnumerable<AttemptMatchMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptMatches")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAttemptMatches query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-matches/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptMatch.Search)]
    [ProducesResponseType<IEnumerable<AttemptMatchMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptMatches_get")]
    [AliasFor("searchAttemptMatches")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAttemptMatches query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAttemptMatches query, CancellationToken cancellation)
    {
        var matches = await _attemptMatchService.SearchAsync(query, cancellation);

        var count = await _attemptMatchService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}