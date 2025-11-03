using Microsoft.AspNetCore.Mvc;

using Shift.Service.Competency;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Competency API: Standards")]
public class StandardController : ShiftControllerBase
{
    private readonly StandardService _standardService;

    public StandardController(StandardService standardService)
    {
        _standardService = standardService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific standard
    /// </summary>
    [HttpHead("competency/standards/{standard:guid}")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertStandard")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid standard, CancellationToken cancellation = default)
    {
        var exists = await _standardService.AssertAsync(standard, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of standards that match specific criteria
    /// </summary>
    [HttpPost("competency/standards/collect")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Collect)]
    [ProducesResponseType<IEnumerable<StandardModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectStandards")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectStandards query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("competency/standards")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Collect)]
    [ProducesResponseType<IEnumerable<StandardModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectStandards_get")]
    [AliasFor("collectStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectStandards query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectStandards query, CancellationToken cancellation)
    {
        var models = await _standardService.CollectAsync(query, cancellation);

        var count = await _standardService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the standards that match specific criteria
    /// </summary>
    [HttpPost("competency/standards/count")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countStandards")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountStandards query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("competency/standards/count")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countStandards_get")]
    [AliasFor("countStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountStandards query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountStandards query, CancellationToken cancellation)
    {
        var count = await _standardService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of standards that match specific criteria
    /// </summary>    
    [HttpPost("competency/standards/download")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadStandards")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectStandards query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("competency/standards/download")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadStandards_get")]
    [AliasFor("downloadStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectStandards query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectStandards query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Competency", "Standards", query.Filter.Format, User);

        var models = await _standardService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _standardService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific standard
    /// </summary>
    [HttpGet("competency/standards/{standard:guid}")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Retrieve)]
    [ProducesResponseType<StandardModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveStandard")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid standard, CancellationToken cancellation = default)
    {
        var model = await _standardService.RetrieveAsync(standard, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of standards that match specific criteria
    /// </summary>
    [HttpPost("competency/standards/search")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Search)]
    [ProducesResponseType<IEnumerable<StandardMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchStandards")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchStandards query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("competency/standards/search")]
    [HybridAuthorize(Policies.Competency.Standards.Standard.Search)]
    [ProducesResponseType<IEnumerable<StandardMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchStandards_get")]
    [AliasFor("searchStandards")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchStandards query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchStandards query, CancellationToken cancellation)
    {
        var matches = await _standardService.SearchAsync(query, cancellation);

        var count = await _standardService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}