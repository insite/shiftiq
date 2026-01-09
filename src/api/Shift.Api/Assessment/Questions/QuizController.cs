using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class QuizController : ShiftControllerBase
{
    private readonly QuizService _quizService;

    public QuizController(QuizService quizService)
    {
        _quizService = quizService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific quiz
    /// </summary>
    [HttpHead("assessment/quizzes/{quiz:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertQuiz")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid quiz, CancellationToken cancellation = default)
    {
        var exists = await _quizService.AssertAsync(quiz, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of quizes that match specific criteria
    /// </summary>
    [HttpPost("assessment/quizzes/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Collect)]
    [ProducesResponseType<IEnumerable<QuizModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectQuizes")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectQuizes query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Collect)]
    [ProducesResponseType<IEnumerable<QuizModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectQuizes_get")]
    [AliasFor("collectQuizes")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectQuizes query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectQuizes query, CancellationToken cancellation)
    {
        var models = await _quizService.CollectAsync(query, cancellation);

        var count = await _quizService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the quizes that match specific criteria
    /// </summary>
    [HttpPost("assessment/quizzes/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countQuizes")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountQuizes query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countQuizes_get")]
    [AliasFor("countQuizes")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountQuizes query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountQuizes query, CancellationToken cancellation)
    {
        var count = await _quizService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of quizes that match specific criteria
    /// </summary>    
    [HttpPost("assessment/quizzes/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadQuizes")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectQuizes query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadQuizes_get")]
    [AliasFor("downloadQuizes")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectQuizes query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectQuizes query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "Quizes", query.Filter.Format, User);

        var models = await _quizService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _quizService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific quiz
    /// </summary>
    [HttpGet("assessment/quizzes/{quiz:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Retrieve)]
    [ProducesResponseType<QuizModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveQuiz")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid quiz, CancellationToken cancellation = default)
    {
        var model = await _quizService.RetrieveAsync(quiz, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of quizes that match specific criteria
    /// </summary>
    [HttpPost("assessment/quizzes/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Search)]
    [ProducesResponseType<IEnumerable<QuizMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchQuizes")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchQuizes query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/quizzes/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Search)]
    [ProducesResponseType<IEnumerable<QuizMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchQuizes_get")]
    [AliasFor("searchQuizes")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchQuizes query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchQuizes query, CancellationToken cancellation)
    {
        var matches = await _quizService.SearchAsync(query, cancellation);

        var count = await _quizService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("assessment/quizzes")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Create)]
    [ProducesResponseType<QuizModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createQuiz")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuiz create, CancellationToken cancellation = default)
    {
        var created = await _quizService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: QuizIdentifier {create.QuizIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _quizService.RetrieveAsync(create.QuizIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("assessment/quizzes/{quiz:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteQuiz")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid quiz, CancellationToken cancellation = default)
    {
        var deleted = await _quizService.DeleteAsync(quiz, cancellation);
        
        if (!deleted)
            return NotFound();
        
        return Ok();
    }

    [HttpPut("assessment/quizzes/{quiz:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Quiz.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyQuiz")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid quiz, [FromBody] ModifyQuiz modify, CancellationToken cancellation = default)
    {
        var model = await _quizService.RetrieveAsync(quiz, cancellation);

        if (model is null)
            return NotFound($"Quiz not found: QuizIdentifier {modify.QuizIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _quizService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();
        
        return Ok();
    }

    #endregion Commands
}