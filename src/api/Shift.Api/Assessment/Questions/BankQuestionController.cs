using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class BankQuestionController : ShiftControllerBase
{
    private readonly BankQuestionService _bankQuestionService;

    public BankQuestionController(BankQuestionService bankQuestionService)
    {
        _bankQuestionService = bankQuestionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank question
    /// </summary>
    [HttpHead("assessment/banks-questions/{question:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertBankQuestion")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var exists = await _bankQuestionService.AssertAsync(question, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of bank questions that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-questions/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Collect)]
    [ProducesResponseType<IEnumerable<BankQuestionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankQuestions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectBankQuestions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Collect)]
    [ProducesResponseType<IEnumerable<BankQuestionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankQuestions_get")]
    [AliasFor("collectBankQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectBankQuestions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectBankQuestions query, CancellationToken cancellation)
    {
        var models = await _bankQuestionService.CollectAsync(query, cancellation);

        var count = await _bankQuestionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the bank questions that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-questions/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankQuestions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountBankQuestions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankQuestions_get")]
    [AliasFor("countBankQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountBankQuestions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountBankQuestions query, CancellationToken cancellation)
    {
        var count = await _bankQuestionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of bank questions that match specific criteria
    /// </summary>    
    [HttpPost("assessment/banks-questions/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankQuestions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBankQuestions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankQuestions_get")]
    [AliasFor("downloadBankQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectBankQuestions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectBankQuestions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "BankQuestions", query.Filter.Format, User);

        var models = await _bankQuestionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _bankQuestionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific bank question
    /// </summary>
    [HttpGet("assessment/banks-questions/{question:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Retrieve)]
    [ProducesResponseType<BankQuestionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveBankQuestion")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var model = await _bankQuestionService.RetrieveAsync(question, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of bank questions that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-questions/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Search)]
    [ProducesResponseType<IEnumerable<BankQuestionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankQuestions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchBankQuestions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestion.Search)]
    [ProducesResponseType<IEnumerable<BankQuestionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankQuestions_get")]
    [AliasFor("searchBankQuestions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchBankQuestions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchBankQuestions query, CancellationToken cancellation)
    {
        var matches = await _bankQuestionService.SearchAsync(query, cancellation);

        var count = await _bankQuestionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}