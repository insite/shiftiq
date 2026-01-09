using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class BankController : ShiftControllerBase
{
    private readonly BankService _bankService;

    public BankController(BankService bankService)
    {
        _bankService = bankService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank
    /// </summary>
    [HttpHead("assessment/banks/{bank:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertBank")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid bank, CancellationToken cancellation = default)
    {
        var exists = await _bankService.AssertAsync(bank, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of banks that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Collect)]
    [ProducesResponseType<IEnumerable<BankModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBanks")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectBanks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/banks")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Collect)]
    [ProducesResponseType<IEnumerable<BankModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBanks_get")]
    [AliasFor("collectBanks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectBanks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectBanks query, CancellationToken cancellation)
    {
        var models = await _bankService.CollectAsync(query, cancellation);

        var count = await _bankService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the banks that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBanks")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountBanks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/banks/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBanks_get")]
    [AliasFor("countBanks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountBanks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountBanks query, CancellationToken cancellation)
    {
        var count = await _bankService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of banks that match specific criteria
    /// </summary>    
    [HttpPost("assessment/banks/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBanks")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBanks query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/banks/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBanks_get")]
    [AliasFor("downloadBanks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectBanks query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectBanks query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "Banks", query.Filter.Format, User);

        var models = await _bankService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _bankService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific bank
    /// </summary>
    [HttpGet("assessment/banks/{bank:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Retrieve)]
    [ProducesResponseType<BankModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveBank")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid bank, CancellationToken cancellation = default)
    {
        var model = await _bankService.RetrieveAsync(bank, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of banks that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Search)]
    [ProducesResponseType<IEnumerable<BankMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBanks")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchBanks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/banks/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.Bank.Search)]
    [ProducesResponseType<IEnumerable<BankMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBanks_get")]
    [AliasFor("searchBanks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchBanks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchBanks query, CancellationToken cancellation)
    {
        var matches = await _bankService.SearchAsync(query, cancellation);

        var count = await _bankService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}