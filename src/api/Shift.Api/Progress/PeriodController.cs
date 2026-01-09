using Microsoft.AspNetCore.Mvc;

using Shift.Service.Progress;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Periods")]
public class PeriodController : ShiftControllerBase
{
    private readonly PeriodService _periodService;

    public PeriodController(PeriodService periodService)
    {
        _periodService = periodService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific period
    /// </summary>
    [HttpHead("progress/periods/{period:guid}")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertPeriod")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid period, CancellationToken cancellation = default)
    {
        var exists = await _periodService.AssertAsync(period, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of periods that match specific criteria
    /// </summary>
    [HttpPost("progress/periods/collect")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Collect)]
    [ProducesResponseType<IEnumerable<PeriodModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPeriods")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectPeriods query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("progress/periods")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Collect)]
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
        var models = await _periodService.CollectAsync(query, cancellation);

        var count = await _periodService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the periods that match specific criteria
    /// </summary>
    [HttpPost("progress/periods/count")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPeriods")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountPeriods query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("progress/periods/count")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Count)]
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
        var count = await _periodService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of periods that match specific criteria
    /// </summary>    
    [HttpPost("progress/periods/download")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPeriods")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPeriods query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("progress/periods/download")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Download)]
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
    [HttpGet("progress/periods/{period:guid}")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Retrieve)]
    [ProducesResponseType<PeriodModel>(StatusCodes.Status200OK)]
    [EndpointName("retrievePeriod")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid period, CancellationToken cancellation = default)
    {
        var model = await _periodService.RetrieveAsync(period, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of periods that match specific criteria
    /// </summary>
    [HttpPost("progress/periods/search")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Search)]
    [ProducesResponseType<IEnumerable<PeriodMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPeriods")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchPeriods query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("progress/periods/search")]
    [HybridAuthorize(Policies.Progress.Periods.Period.Search)]
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
        var matches = await _periodService.SearchAsync(query, cancellation);

        var count = await _periodService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}