using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessment API: Questions")]
public class BankSpecificationController : ShiftControllerBase
{
    private readonly BankSpecificationService _bankSpecificationService;

    public BankSpecificationController(BankSpecificationService bankSpecificationService)
    {
        _bankSpecificationService = bankSpecificationService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank specification
    /// </summary>
    [HttpHead("assessment/banks-specifications/{specification:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertBankSpecification")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid spec, CancellationToken cancellation = default)
    {
        var exists = await _bankSpecificationService.AssertAsync(spec, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of bank specifications that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-specifications/collect")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Collect)]
    [ProducesResponseType<IEnumerable<BankSpecificationModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankSpecifications")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectBankSpecifications query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-specifications")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Collect)]
    [ProducesResponseType<IEnumerable<BankSpecificationModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectBankSpecifications_get")]
    [AliasFor("collectBankSpecifications")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectBankSpecifications query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectBankSpecifications query, CancellationToken cancellation)
    {
        var models = await _bankSpecificationService.CollectAsync(query, cancellation);

        var count = await _bankSpecificationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the bank specifications that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-specifications/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankSpecifications")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountBankSpecifications query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-specifications/count")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countBankSpecifications_get")]
    [AliasFor("countBankSpecifications")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountBankSpecifications query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountBankSpecifications query, CancellationToken cancellation)
    {
        var count = await _bankSpecificationService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of bank specifications that match specific criteria
    /// </summary>    
    [HttpPost("assessment/banks-specifications/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankSpecifications")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBankSpecifications query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-specifications/download")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBankSpecifications_get")]
    [AliasFor("downloadBankSpecifications")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectBankSpecifications query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectBankSpecifications query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Assessment", "BankSpecifications", query.Filter.Format, User);

        var models = await _bankSpecificationService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _bankSpecificationService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific bank specification
    /// </summary>
    [HttpGet("assessment/banks-specifications/{specification:guid}")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Retrieve)]
    [ProducesResponseType<BankSpecificationModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveBankSpecification")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid spec, CancellationToken cancellation = default)
    {
        var model = await _bankSpecificationService.RetrieveAsync(spec, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of bank specifications that match specific criteria
    /// </summary>
    [HttpPost("assessment/banks-specifications/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Search)]
    [ProducesResponseType<IEnumerable<BankSpecificationMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankSpecifications")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchBankSpecifications query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("assessment/banks-specifications/search")]
    [HybridAuthorize(Policies.Evaluation.Questions.BankSpecification.Search)]
    [ProducesResponseType<IEnumerable<BankSpecificationMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchBankSpecifications_get")]
    [AliasFor("searchBankSpecifications")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchBankSpecifications query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchBankSpecifications query, CancellationToken cancellation)
    {
        var matches = await _bankSpecificationService.SearchAsync(query, cancellation);

        var count = await _bankSpecificationService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}