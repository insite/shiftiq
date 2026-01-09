using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class AttemptOptionController : ShiftControllerBase
{
    private readonly AttemptOptionService _attemptOptionService;

    public AttemptOptionController(AttemptOptionService attemptOptionService)
    {
        _attemptOptionService = attemptOptionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific attempt option
    /// </summary>
    [HttpHead("assessment/attempts-options/{attempt:guid}/{question:guid}/{option:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAttemptOption")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, [FromRoute] int optionKey, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var exists = await _attemptOptionService.AssertAsync(attempt, optionKey, question, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of attempt options that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-options/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Collect)]
    [ProducesResponseType<IEnumerable<AttemptOptionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptOptions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAttemptOptions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-options")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Collect)]
    [ProducesResponseType<IEnumerable<AttemptOptionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptOptions_get")]
    [AliasFor("collectAttemptOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAttemptOptions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAttemptOptions query, CancellationToken cancellation)
    {
        var models = await _attemptOptionService.CollectAsync(query, cancellation);

        var count = await _attemptOptionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the attempt options that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-options/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptOptions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAttemptOptions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-options/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptOptions_get")]
    [AliasFor("countAttemptOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAttemptOptions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAttemptOptions query, CancellationToken cancellation)
    {
        var count = await _attemptOptionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of attempt options that match specific criteria
    /// </summary>    
    [HttpPost("assessment/attempts-options/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptOptions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAttemptOptions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-options/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptOptions_get")]
    [AliasFor("downloadAttemptOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAttemptOptions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAttemptOptions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "AttemptOptions", query.Filter.Format, User);

        var models = await _attemptOptionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _attemptOptionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific attempt option
    /// </summary>
    [HttpGet("assessment/attempts-options/{attempt:guid}/{question:guid}/{option:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Retrieve)]
    [ProducesResponseType<AttemptOptionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAttemptOption")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, [FromRoute] int optionKey, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var model = await _attemptOptionService.RetrieveAsync(attempt, optionKey, question, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of attempt options that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-options/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Search)]
    [ProducesResponseType<IEnumerable<AttemptOptionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptOptions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAttemptOptions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-options/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptOption.Search)]
    [ProducesResponseType<IEnumerable<AttemptOptionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptOptions_get")]
    [AliasFor("searchAttemptOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAttemptOptions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAttemptOptions query, CancellationToken cancellation)
    {
        var matches = await _attemptOptionService.SearchAsync(query, cancellation);

        var count = await _attemptOptionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}