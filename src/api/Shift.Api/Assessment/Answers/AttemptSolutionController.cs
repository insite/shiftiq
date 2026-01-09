using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class AttemptSolutionController : ShiftControllerBase
{
    private readonly AttemptSolutionService _attemptSolutionService;

    public AttemptSolutionController(AttemptSolutionService attemptSolutionService)
    {
        _attemptSolutionService = attemptSolutionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific attempt solution
    /// </summary>
    [HttpHead("assessment/attempts-solutions/{attempt:guid}/{question:guid}/{solution:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAttemptSolution")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, [FromRoute] Guid question, [FromRoute] Guid solution, CancellationToken cancellation = default)
    {
        var exists = await _attemptSolutionService.AssertAsync(attempt, question, solution, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of attempt solutions that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-solutions/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Collect)]
    [ProducesResponseType<IEnumerable<AttemptSolutionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptSolutions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-solutions")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Collect)]
    [ProducesResponseType<IEnumerable<AttemptSolutionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptSolutions_get")]
    [AliasFor("collectAttemptSolutions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAttemptSolutions query, CancellationToken cancellation)
    {
        var models = await _attemptSolutionService.CollectAsync(query, cancellation);

        var count = await _attemptSolutionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the attempt solutions that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-solutions/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptSolutions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-solutions/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptSolutions_get")]
    [AliasFor("countAttemptSolutions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAttemptSolutions query, CancellationToken cancellation)
    {
        var count = await _attemptSolutionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of attempt solutions that match specific criteria
    /// </summary>    
    [HttpPost("assessment/attempts-solutions/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptSolutions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-solutions/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptSolutions_get")]
    [AliasFor("downloadAttemptSolutions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAttemptSolutions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "AttemptSolutions", query.Filter.Format, User);

        var models = await _attemptSolutionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _attemptSolutionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific attempt solution
    /// </summary>
    [HttpGet("assessment/attempts-solutions/{attempt:guid}/{question:guid}/{solution:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Retrieve)]
    [ProducesResponseType<AttemptSolutionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAttemptSolution")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, [FromRoute] Guid question, [FromRoute] Guid solution, CancellationToken cancellation = default)
    {
        var model = await _attemptSolutionService.RetrieveAsync(attempt, question, solution, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of attempt solutions that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-solutions/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Search)]
    [ProducesResponseType<IEnumerable<AttemptSolutionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptSolutions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-solutions/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptSolution.Search)]
    [ProducesResponseType<IEnumerable<AttemptSolutionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptSolutions_get")]
    [AliasFor("searchAttemptSolutions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAttemptSolutions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAttemptSolutions query, CancellationToken cancellation)
    {
        var matches = await _attemptSolutionService.SearchAsync(query, cancellation);

        var count = await _attemptSolutionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}