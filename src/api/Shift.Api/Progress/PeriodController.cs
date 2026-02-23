using Microsoft.AspNetCore.Mvc;

using Shift.Service.Progress;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Periods")]
public class PeriodController : ShiftControllerBase
{
    private readonly PeriodService _periodService;
    private readonly IPrincipalProvider _principalProvider;

    public PeriodController(PeriodService periodService, IPrincipalProvider principalProvider)
    {
        _periodService = periodService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific period
    /// </summary>
    [HttpHead("api/progress/periods/{period:guid}")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertPeriod")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid period, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _periodService.AssertAsync(period, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of periods that match specific criteria
    /// </summary>
    [HttpPost("api/progress/periods/collect")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PeriodModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPeriods")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectPeriods query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/progress/periods")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PeriodModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPeriods_get")]
    [AliasFor("collectPeriods")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectPeriods query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectPeriods query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _periodService.CollectAsync(query, cancellation);

        var count = await _periodService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the periods that match specific criteria
    /// </summary>
    [HttpPost("api/progress/periods/count")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPeriods")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountPeriods query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/progress/periods/count")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPeriods_get")]
    [AliasFor("countPeriods")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountPeriods query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountPeriods query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _periodService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of periods that match specific criteria
    /// </summary>    
    [HttpPost("api/progress/periods/download")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPeriods")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPeriods query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/progress/periods/download")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPeriods_get")]
    [AliasFor("downloadPeriods")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectPeriods query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectPeriods query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Progress", "Periods", query.Filter.Format, User);

        var models = await _periodService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _periodService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific period
    /// </summary>
    [HttpGet("api/progress/periods/{period:guid}")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<PeriodModel>(StatusCodes.Status200OK)]
    [EndpointName("retrievePeriod")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid period, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _periodService.RetrieveAsync(period, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of periods that match specific criteria
    /// </summary>
    [HttpPost("api/progress/periods/search")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PeriodMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPeriods")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchPeriods query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/progress/periods/search")]
    [HybridPermission("progress/periods", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PeriodMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPeriods_get")]
    [AliasFor("searchPeriods")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchPeriods query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchPeriods query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _periodService.SearchAsync(query, cancellation);

        var count = await _periodService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}