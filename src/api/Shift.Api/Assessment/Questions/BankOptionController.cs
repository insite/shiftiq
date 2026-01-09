using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class BankOptionController : ShiftControllerBase
{
    private readonly BankOptionService _bankOptionService;

    public BankOptionController(BankOptionService bankOptionService)
    {
        _bankOptionService = bankOptionService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank option
    /// </summary>
    [HttpHead("assessment/banks-options/{bank:guid}/{option:int}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertBankOption")]
    public async Task<IActionResult> AssertAsync([FromRoute] int optionKey, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var exists = await _bankOptionService.AssertAsync(optionKey, question, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of bank options that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-options/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Collect)]
    [ProducesResponseType<IEnumerable<BankOptionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankOptions")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectBankOptions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-options")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Collect)]
    [ProducesResponseType<IEnumerable<BankOptionModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankOptions_get")]
    [AliasFor("collectBankOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectBankOptions query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectBankOptions query, CancellationToken cancellation)
    {
        var models = await _bankOptionService.CollectAsync(query, cancellation);

        var count = await _bankOptionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the bank options that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-options/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankOptions")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountBankOptions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-options/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankOptions_get")]
    [AliasFor("countBankOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountBankOptions query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountBankOptions query, CancellationToken cancellation)
    {
        var count = await _bankOptionService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of bank options that match specific criteria
    /// </summary>    
    [HttpPost("assessment/banks-options/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankOptions")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBankOptions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-options/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankOptions_get")]
    [AliasFor("downloadBankOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectBankOptions query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectBankOptions query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "BankOptions", query.Filter.Format, User);

        var models = await _bankOptionService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _bankOptionService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific bank option
    /// </summary>
    [HttpGet("assessment/banks-options/{bank:guid}/{option:int}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Retrieve)]
    [ProducesResponseType<BankOptionModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveBankOption")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] int optionKey, [FromRoute] Guid question, CancellationToken cancellation = default)
    {
        var model = await _bankOptionService.RetrieveAsync(optionKey, question, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of bank options that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-options/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Search)]
    [ProducesResponseType<IEnumerable<BankOptionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankOptions")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchBankOptions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-options/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankOption.Search)]
    [ProducesResponseType<IEnumerable<BankOptionMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankOptions_get")]
    [AliasFor("searchBankOptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchBankOptions query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchBankOptions query, CancellationToken cancellation)
    {
        var matches = await _bankOptionService.SearchAsync(query, cancellation);

        var count = await _bankOptionService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}