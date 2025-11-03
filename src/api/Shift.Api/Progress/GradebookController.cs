using Microsoft.AspNetCore.Mvc;

using Shift.Service.Progress;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Gradebooks")]
public class GradebookController : ShiftControllerBase
{
    private readonly GradebookService _gradebookService;

    public GradebookController(GradebookService gradebookService)
    {
        _gradebookService = gradebookService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific gradebook
    /// </summary>
    [HttpHead("progress/gradebooks/{gradebook:guid}")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertGradebook")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid gradebook, CancellationToken cancellation = default)
    {
        var exists = await _gradebookService.AssertAsync(gradebook, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of gradebooks that match specific criteria
    /// </summary>
    [HttpPost("progress/gradebooks/collect")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Collect)]
    [ProducesResponseType<IEnumerable<GradebookModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectGradebooks")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectGradebooks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("progress/gradebooks")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Collect)]
    [ProducesResponseType<IEnumerable<GradebookModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectGradebooks_get")]
    [AliasFor("collectGradebooks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectGradebooks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectGradebooks query, CancellationToken cancellation)
    {
        var models = await _gradebookService.CollectAsync(query, cancellation);

        var count = await _gradebookService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the gradebooks that match specific criteria
    /// </summary>
    [HttpPost("progress/gradebooks/count")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countGradebooks")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountGradebooks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("progress/gradebooks/count")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countGradebooks_get")]
    [AliasFor("countGradebooks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountGradebooks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountGradebooks query, CancellationToken cancellation)
    {
        var count = await _gradebookService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of gradebooks that match specific criteria
    /// </summary>    
    [HttpPost("progress/gradebooks/download")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGradebooks")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectGradebooks query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("progress/gradebooks/download")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGradebooks_get")]
    [AliasFor("downloadGradebooks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectGradebooks query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectGradebooks query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Progress", "Gradebooks", query.Filter.Format, User);

        var models = await _gradebookService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _gradebookService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific gradebook
    /// </summary>
    [HttpGet("progress/gradebooks/{gradebook:guid}")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Retrieve)]
    [ProducesResponseType<GradebookModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveGradebook")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid gradebook, CancellationToken cancellation = default)
    {
        var model = await _gradebookService.RetrieveAsync(gradebook, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of gradebooks that match specific criteria
    /// </summary>
    [HttpPost("progress/gradebooks/search")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Search)]
    [ProducesResponseType<IEnumerable<GradebookMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchGradebooks")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchGradebooks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("progress/gradebooks/search")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Search)]
    [ProducesResponseType<IEnumerable<GradebookMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchGradebooks_get")]
    [AliasFor("searchGradebooks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchGradebooks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchGradebooks query, CancellationToken cancellation)
    {
        var matches = await _gradebookService.SearchAsync(query, cancellation);

        var count = await _gradebookService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}