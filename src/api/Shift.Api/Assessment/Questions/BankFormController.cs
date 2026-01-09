using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class BankFormController : ShiftControllerBase
{
    private readonly BankFormService _bankFormService;

    public BankFormController(BankFormService bankFormService)
    {
        _bankFormService = bankFormService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank form
    /// </summary>
    [HttpHead("assessment/banks-forms/{form:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertBankForm")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid form, CancellationToken cancellation = default)
    {
        var exists = await _bankFormService.AssertAsync(form, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of bank forms that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-forms/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Collect)]
    [ProducesResponseType<IEnumerable<BankFormModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankForms")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectBankForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Collect)]
    [ProducesResponseType<IEnumerable<BankFormModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankForms_get")]
    [AliasFor("collectBankForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectBankForms query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectBankForms query, CancellationToken cancellation)
    {
        var models = await _bankFormService.CollectAsync(query, cancellation);

        var count = await _bankFormService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the bank forms that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-forms/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankForms")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountBankForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankForms_get")]
    [AliasFor("countBankForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountBankForms query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountBankForms query, CancellationToken cancellation)
    {
        var count = await _bankFormService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of bank forms that match specific criteria
    /// </summary>    
    [HttpPost("assessment/banks-forms/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankForms")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBankForms query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankForms_get")]
    [AliasFor("downloadBankForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectBankForms query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectBankForms query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "BankForms", query.Filter.Format, User);

        var models = await _bankFormService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _bankFormService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific bank form
    /// </summary>
    [HttpGet("assessment/banks-forms/{form:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Retrieve)]
    [ProducesResponseType<BankFormModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveBankForm")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid form, CancellationToken cancellation = default)
    {
        var model = await _bankFormService.RetrieveAsync(form, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of bank forms that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-forms/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Search)]
    [ProducesResponseType<IEnumerable<BankFormMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankForms")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchBankForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-forms/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.Assessment.Search)]
    [ProducesResponseType<IEnumerable<BankFormMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankForms_get")]
    [AliasFor("searchBankForms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchBankForms query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchBankForms query, CancellationToken cancellation)
    {
        var matches = await _bankFormService.SearchAsync(query, cancellation);

        var count = await _bankFormService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}