using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Answers")]
public class QuizAttemptController : ShiftControllerBase
{
    private readonly QuizAttemptService _quizAttemptService;

    public QuizAttemptController(QuizAttemptService quizAttemptService)
    {
        _quizAttemptService = quizAttemptService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific quiz attempt
    /// </summary>
    [HttpHead("assessment/quizzes-attempts/{attempt:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertQuizAttempt")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid attempt, CancellationToken cancellation = default)
    {
        var exists = await _quizAttemptService.AssertAsync(attempt, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of quiz attempts that match specific criteria
    /// </summary>
    [HttpPost("assessment/quizzes-attempts/collect")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Collect)]
    [ProducesResponseType<IEnumerable<QuizAttemptModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectQuizAttempts")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectQuizAttempts query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes-attempts")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Collect)]
    [ProducesResponseType<IEnumerable<QuizAttemptModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectQuizAttempts_get")]
    [AliasFor("collectQuizAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectQuizAttempts query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectQuizAttempts query, CancellationToken cancellation)
    {
        var models = await _quizAttemptService.CollectAsync(query, cancellation);

        var count = await _quizAttemptService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the quiz attempts that match specific criteria
    /// </summary>
    [HttpPost("assessment/quizzes-attempts/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countQuizAttempts")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountQuizAttempts query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes-attempts/count")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countQuizAttempts_get")]
    [AliasFor("countQuizAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountQuizAttempts query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountQuizAttempts query, CancellationToken cancellation)
    {
        var count = await _quizAttemptService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of quiz attempts that match specific criteria
    /// </summary>    
    [HttpPost("assessment/quizzes-attempts/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadQuizAttempts")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectQuizAttempts query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes-attempts/download")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadQuizAttempts_get")]
    [AliasFor("downloadQuizAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectQuizAttempts query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectQuizAttempts query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "QuizAttempts", query.Filter.Format, User);

        var models = await _quizAttemptService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _quizAttemptService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific quiz attempt
    /// </summary>
    [HttpGet("assessment/quizzes-attempts/{attempt:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Retrieve)]
    [ProducesResponseType<QuizAttemptModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveQuizAttempt")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid attempt, CancellationToken cancellation = default)
    {
        var model = await _quizAttemptService.RetrieveAsync(attempt, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of quiz attempts that match specific criteria
    /// </summary>
    [HttpPost("assessment/quizzes-attempts/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Search)]
    [ProducesResponseType<IEnumerable<QuizAttemptMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchQuizAttempts")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchQuizAttempts query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes-attempts/search")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Search)]
    [ProducesResponseType<IEnumerable<QuizAttemptMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchQuizAttempts_get")]
    [AliasFor("searchQuizAttempts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchQuizAttempts query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchQuizAttempts query, CancellationToken cancellation)
    {
        var matches = await _quizAttemptService.SearchAsync(query, cancellation);

        var count = await _quizAttemptService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("assessment/quizzes-attempts")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Create)]
    [ProducesResponseType<QuizAttemptModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createQuizAttempt")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuizAttempt create, CancellationToken cancellation = default)
    {
        var created = await _quizAttemptService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: AttemptIdentifier {create.AttemptIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _quizAttemptService.RetrieveAsync(create.AttemptIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("assessment/quizzes-attempts/{attempt:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteQuizAttempt")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid attempt, CancellationToken cancellation = default)
    {
        var deleted = await _quizAttemptService.DeleteAsync(attempt, cancellation);
        
        if (!deleted)
            return NotFound();
        
        return Ok();
    }

    [HttpPut("assessment/quizzes-attempts/{attempt:guid}")]
    [HybridAuthorize(Policies.Evaluation.Answers.QuizAttempt.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyQuizAttempt")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid attempt, [FromBody] ModifyQuizAttempt modify, CancellationToken cancellation = default)
    {
        var model = await _quizAttemptService.RetrieveAsync(attempt, cancellation);

        if (model is null)
            return NotFound($"QuizAttempt not found: AttemptIdentifier {modify.AttemptIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _quizAttemptService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();
        
        return Ok();
    }

    #endregion Commands
}