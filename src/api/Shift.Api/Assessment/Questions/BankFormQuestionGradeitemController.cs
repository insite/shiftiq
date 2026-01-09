using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class BankFormQuestionGradeitemController : ShiftControllerBase
{
    private readonly BankFormQuestionGradeitemService _bankFormQuestionGradeitemService;

    public BankFormQuestionGradeitemController(BankFormQuestionGradeitemService bankFormQuestionGradeitemService)
    {
        _bankFormQuestionGradeitemService = bankFormQuestionGradeitemService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank form question gradeitem
    /// </summary>
    [HttpHead("assessment/banks-forms-questions-gradeitems/{form:guid}/{question:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertBankFormQuestionGradeitem")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid form, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var exists = await _bankFormQuestionGradeitemService.AssertAsync(form, question, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of bank form question gradeitems that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-forms-questions-gradeitems/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Collect)]
    [ProducesResponseType<IEnumerable<BankFormQuestionGradeitemModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankFormQuestionGradeitems")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms-questions-gradeitems")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Collect)]
    [ProducesResponseType<IEnumerable<BankFormQuestionGradeitemModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankFormQuestionGradeitems_get")]
    [AliasFor("collectBankFormQuestionGradeitems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectBankFormQuestionGradeitems query, CancellationToken cancellation)
    {
        var models = await _bankFormQuestionGradeitemService.CollectAsync(query, cancellation);

        var count = await _bankFormQuestionGradeitemService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the bank form question gradeitems that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-forms-questions-gradeitems/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankFormQuestionGradeitems")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms-questions-gradeitems/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankFormQuestionGradeitems_get")]
    [AliasFor("countBankFormQuestionGradeitems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountBankFormQuestionGradeitems query, CancellationToken cancellation)
    {
        var count = await _bankFormQuestionGradeitemService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of bank form question gradeitems that match specific criteria
    /// </summary>    
    [HttpPost("assessment/banks-forms-questions-gradeitems/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankFormQuestionGradeitems")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms-questions-gradeitems/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankFormQuestionGradeitems_get")]
    [AliasFor("downloadBankFormQuestionGradeitems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectBankFormQuestionGradeitems query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "BankFormQuestionGradeitems", query.Filter.Format, User);

        var models = await _bankFormQuestionGradeitemService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _bankFormQuestionGradeitemService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific bank form question gradeitem
    /// </summary>
    [HttpGet("assessment/banks-forms-questions-gradeitems/{form:guid}/{question:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Retrieve)]
    [ProducesResponseType<BankFormQuestionGradeitemModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveBankFormQuestionGradeitem")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid form, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var model = await _bankFormQuestionGradeitemService.RetrieveAsync(form, question, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of bank form question gradeitems that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-forms-questions-gradeitems/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Search)]
    [ProducesResponseType<IEnumerable<BankFormQuestionGradeitemMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankFormQuestionGradeitems")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms-questions-gradeitems/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.AssessmentQuestionGradeitem.Search)]
    [ProducesResponseType<IEnumerable<BankFormQuestionGradeitemMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankFormQuestionGradeitems_get")]
    [AliasFor("searchBankFormQuestionGradeitems")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchBankFormQuestionGradeitems query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchBankFormQuestionGradeitems query, CancellationToken cancellation)
    {
        var matches = await _bankFormQuestionGradeitemService.SearchAsync(query, cancellation);

        var count = await _bankFormQuestionGradeitemService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}