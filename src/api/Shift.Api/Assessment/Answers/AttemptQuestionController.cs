using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class AttemptQuestionController : ShiftControllerBase
{
    private readonly AttemptQuestionService _attemptQuestionService;

    public AttemptQuestionController(AttemptQuestionService attemptQuestionService)
    {
        _attemptQuestionService = attemptQuestionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific attempt question
    /// </summary>
    [HttpHead("assessment/attempts-questions/{attempt:guid}/{question:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertAttemptQuestion")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var exists = await _attemptQuestionService.AssertAsync(attempt, question, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of attempt questions that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-questions/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Collect)]
    [ProducesResponseType<IEnumerable<AttemptQuestionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptQuestions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-questions")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Collect)]
    [ProducesResponseType<IEnumerable<AttemptQuestionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectAttemptQuestions_get")]
    [AliasFor("collectAttemptQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectAttemptQuestions query, CancellationToken cancellation)
    {
        var models = await _attemptQuestionService.CollectAsync(query, cancellation);

        var count = await _attemptQuestionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the attempt questions that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-questions/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptQuestions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-questions/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countAttemptQuestions_get")]
    [AliasFor("countAttemptQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountAttemptQuestions query, CancellationToken cancellation)
    {
        var count = await _attemptQuestionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of attempt questions that match specific criteria
    /// </summary>    
    [HttpPost("assessment/attempts-questions/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptQuestions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-questions/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadAttemptQuestions_get")]
    [AliasFor("downloadAttemptQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectAttemptQuestions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "AttemptQuestions", query.Filter.Format, User);

        var models = await _attemptQuestionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _attemptQuestionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific attempt question
    /// </summary>
    [HttpGet("assessment/attempts-questions/{attempt:guid}/{question:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Retrieve)]
    [ProducesResponseType<AttemptQuestionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveAttemptQuestion")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var model = await _attemptQuestionService.RetrieveAsync(attempt, question, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of attempt questions that match specific criteria
    /// </summary>
    [HttpPost("assessment/attempts-questions/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Search)]
    [ProducesResponseType<IEnumerable<AttemptQuestionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptQuestions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/attempts-questions/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.AttemptQuestion.Search)]
    [ProducesResponseType<IEnumerable<AttemptQuestionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchAttemptQuestions_get")]
    [AliasFor("searchAttemptQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchAttemptQuestions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchAttemptQuestions query, CancellationToken cancellation)
    {
        var matches = await _attemptQuestionService.SearchAsync(query, cancellation);

        var count = await _attemptQuestionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}