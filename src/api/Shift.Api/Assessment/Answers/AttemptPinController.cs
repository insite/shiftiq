using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class AttemptPinController : ShiftControllerBase
{
    private readonly AttemptPinService _attemptPinService;

    public AttemptPinController(AttemptPinService attemptPinService)
    {
        _attemptPinService = attemptPinService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific attempt pin
    /// </summary>
    [HttpHead("assessment/attempts-pins/{attempt:guid}/{question:guid}/{pin:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAttemptPin")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, [FromRoute] int pinSequence, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var exists = await _attemptPinService.AssertAsync(attempt, pinSequence, question, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of attempt pins that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-pins/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Collect)]
    [ProducesResponseType<IEnumerable<AttemptPinModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptPins")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAttemptPins query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-pins")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Collect)]
    [ProducesResponseType<IEnumerable<AttemptPinModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptPins_get")]
    [AliasFor("collectAttemptPins")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAttemptPins query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAttemptPins query, CancellationToken cancellation)
    {
        var models = await _attemptPinService.CollectAsync(query, cancellation);

        var count = await _attemptPinService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the attempt pins that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-pins/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptPins")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAttemptPins query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-pins/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptPins_get")]
    [AliasFor("countAttemptPins")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAttemptPins query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAttemptPins query, CancellationToken cancellation)
    {
        var count = await _attemptPinService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of attempt pins that match specific criteria
    /// </summary>    
    [HttpPost("assessment/attempts-pins/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptPins")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAttemptPins query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-pins/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptPins_get")]
    [AliasFor("downloadAttemptPins")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAttemptPins query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAttemptPins query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "AttemptPins", query.Filter.Format, User);

        var models = await _attemptPinService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _attemptPinService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific attempt pin
    /// </summary>
    [HttpGet("assessment/attempts-pins/{attempt:guid}/{question:guid}/{pin:int}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Retrieve)]
    [ProducesResponseType<AttemptPinModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAttemptPin")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, [FromRoute] int pinSequence, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var model = await _attemptPinService.RetrieveAsync(attempt, pinSequence, question, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of attempt pins that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-pins/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Search)]
    [ProducesResponseType<IEnumerable<AttemptPinMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptPins")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAttemptPins query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-pins/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptPin.Search)]
    [ProducesResponseType<IEnumerable<AttemptPinMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptPins_get")]
    [AliasFor("searchAttemptPins")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAttemptPins query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAttemptPins query, CancellationToken cancellation)
    {
        var matches = await _attemptPinService.SearchAsync(query, cancellation);

        var count = await _attemptPinService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}