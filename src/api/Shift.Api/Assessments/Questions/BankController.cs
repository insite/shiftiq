using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Assessment;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessments API: Questions")]
public class BankController : ShiftControllerBase
{
    private readonly BankService _bankService;
    private readonly IPrincipalProvider _principalProvider;

    public BankController(BankService bankService, IPrincipalProvider principalProvider)
    {
        _bankService = bankService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific bank
    /// </summary>
    [HttpHead("api/assessments/banks/{bank:guid}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertBank")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid bank, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();
        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _bankService.AssertAsync(bank, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of banks that match specific criteria
    /// </summary>
    [HttpPost("api/assessments/banks/collect")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("collectBanks")]
    public async Task<ActionResult<IEnumerable<BankModel>>> PostCollectAsync([FromBody] CollectBanks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/assessments/banks")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("collectBanks_get")]
    [AliasFor("collectBanks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<BankModel>>> GetCollectAsync([FromQuery] CollectBanks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<BankModel>>> CollectAsync(CollectBanks query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _bankService.CollectAsync(query, cancellation);

        var count = await _bankService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the banks that match specific criteria
    /// </summary>
    [HttpPost("api/assessments/banks/count")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("countBanks")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountBanks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/assessments/banks/count")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("countBanks_get")]
    [AliasFor("countBanks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountBanks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountBanks query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _bankService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of banks that match specific criteria
    /// </summary>    
    [HttpPost("api/assessments/banks/download")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadBanks")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectBanks query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/assessments/banks/download")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

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
    [HttpGet("api/assessments/banks/{bank:guid}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [ProducesResponseType(typeof(BankModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveBank")]
    public async Task<ActionResult<BankModel>> RetrieveAsync([FromRoute] Guid bank, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _bankService.RetrieveAsync(bank, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of banks that match specific criteria
    /// </summary>
    [HttpPost("api/assessments/banks/search")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("searchBanks")]
    public async Task<ActionResult<IEnumerable<BankMatch>>> PostSearchAsync([FromBody] SearchBanks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/assessments/banks/search")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("searchBanks_get")]
    [AliasFor("searchBanks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<BankMatch>>> GetSearchAsync([FromQuery] SearchBanks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<BankMatch>>> SearchAsync(SearchBanks query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();
        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _bankService.SearchAsync(query, cancellation);

        var count = await _bankService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}