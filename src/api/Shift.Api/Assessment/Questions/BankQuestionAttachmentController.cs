using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class BankQuestionAttachmentController : ShiftControllerBase
{
    private readonly BankQuestionAttachmentService _bankQuestionAttachmentService;

    public BankQuestionAttachmentController(BankQuestionAttachmentService bankQuestionAttachmentService)
    {
        _bankQuestionAttachmentService = bankQuestionAttachmentService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank question attachment
    /// </summary>
    [HttpHead("assessment/banks-questions-attachments/{question:guid}/{attachment:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertBankQuestionAttachment")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid question, [FromRoute] Guid upload, CancellationToken cancellation = default)
    {
        var exists = await _bankQuestionAttachmentService.AssertAsync(question, upload, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of bank question attachments that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-questions-attachments/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Collect)]
    [ProducesResponseType<IEnumerable<BankQuestionAttachmentModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankQuestionAttachments")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions-attachments")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Collect)]
    [ProducesResponseType<IEnumerable<BankQuestionAttachmentModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankQuestionAttachments_get")]
    [AliasFor("collectBankQuestionAttachments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectBankQuestionAttachments query, CancellationToken cancellation)
    {
        var models = await _bankQuestionAttachmentService.CollectAsync(query, cancellation);

        var count = await _bankQuestionAttachmentService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the bank question attachments that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-questions-attachments/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankQuestionAttachments")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions-attachments/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankQuestionAttachments_get")]
    [AliasFor("countBankQuestionAttachments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountBankQuestionAttachments query, CancellationToken cancellation)
    {
        var count = await _bankQuestionAttachmentService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of bank question attachments that match specific criteria
    /// </summary>    
    [HttpPost("assessment/banks-questions-attachments/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankQuestionAttachments")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions-attachments/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankQuestionAttachments_get")]
    [AliasFor("downloadBankQuestionAttachments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectBankQuestionAttachments query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "BankQuestionAttachments", query.Filter.Format, User);

        var models = await _bankQuestionAttachmentService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _bankQuestionAttachmentService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific bank question attachment
    /// </summary>
    [HttpGet("assessment/banks-questions-attachments/{question:guid}/{attachment:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Retrieve)]
    [ProducesResponseType<BankQuestionAttachmentModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveBankQuestionAttachment")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid question, [FromRoute] Guid upload, CancellationToken cancellation = default)
    {
        var model = await _bankQuestionAttachmentService.RetrieveAsync(question, upload, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of bank question attachments that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-questions-attachments/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Search)]
    [ProducesResponseType<IEnumerable<BankQuestionAttachmentMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankQuestionAttachments")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-questions-attachments/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankQuestionAttachment.Search)]
    [ProducesResponseType<IEnumerable<BankQuestionAttachmentMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankQuestionAttachments_get")]
    [AliasFor("searchBankQuestionAttachments")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchBankQuestionAttachments query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchBankQuestionAttachments query, CancellationToken cancellation)
    {
        var matches = await _bankQuestionAttachmentService.SearchAsync(query, cancellation);

        var count = await _bankQuestionAttachmentService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}