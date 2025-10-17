using System.Text;

using Microsoft.AspNetCore.Mvc;

using Shift.Api.Internal;
using Shift.Common;
using Shift.Service.Gradebook;

namespace Shift.Api;

/// <remarks>
/// This entity models a current-state projection of an aggregate event/change stream. This is the reason there are no
/// methods here to create, modify, or delete this entity. Data changes are permitted only using Timeline commands.
/// </remarks>
[ApiController]
[ApiExplorerSettings(GroupName = "Progress API: Gradebooks")]
public class GradebookController : ControllerBase
{
    private readonly ILogger<GradebookController> _logger;
    private readonly ReleaseSettings _releaseSettings;
    private readonly DatabaseSettings _databaseSettings;
    private readonly GradebookService _gradebookService;

    public GradebookController(
        ILogger<GradebookController> logger,
        ReleaseSettings releaseSettings,
        DatabaseSettings databaseSettings,
        GradebookService gradebookService)
    {
        _logger = logger;
        _releaseSettings = releaseSettings;
        _databaseSettings = databaseSettings;
        _gradebookService = gradebookService;
    }

    // HTTP method and route
    [HttpHead("progress/gradebooks/{gradebook:guid}")]

    // Authorization and filters
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]

    // OpenAPI metadata
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertGradebook")]

    public async Task<ActionResult<bool>> AssertAsync(
        [FromRoute] Guid gradebook,
        CancellationToken cancellation = default)
    {
        var exists = await _gradebookService.AssertAsync(gradebook, cancellation);
        return exists ? Ok() : NotFound();
    }

    [HttpGet("progress/gradebooks/{gradebook:guid}")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType<GradebookModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveGradebook")]
    public async Task<ActionResult<GradebookModel>> RetrieveAsync(
        [FromRoute] Guid gradebook,
        CancellationToken cancellation = default)
    {
        var model = await _gradebookService.RetrieveAsync(gradebook, cancellation);
        return model != null ? Ok(model) : NotFound();
    }

    [HttpGet("progress/gradebooks/count")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countGradebooks")]
    public async Task<ActionResult<CountResult>> GetCountAsync(
        [FromQuery] CountGradebooks query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    [HttpPost("progress/gradebooks/count")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType(typeof(CountResult), StatusCodes.Status200OK)]
    [EndpointName("countGradebooks_post")]
    [AliasFor("countGradebooks")]
    public async Task<ActionResult<CountResult>> PostCountAsync(
        [FromBody] CountGradebooks query,
        CancellationToken cancellation = default)
        => await CountAsync(query, cancellation);

    private async Task<ActionResult<CountResult>> CountAsync(CountGradebooks query, CancellationToken cancellation)
    {
        var count = await _gradebookService.CountAsync(query, cancellation);
        return Ok(new CountResult(count));
    }

    [HttpGet("progress/gradebooks")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType(typeof(IEnumerable<GradebookModel>), StatusCodes.Status200OK)]
    [EndpointName("collectGradebooks")]
    public async Task<ActionResult<IEnumerable<GradebookModel>>> GetCollectAsync(
        [FromQuery] CollectGradebooks query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    [HttpPost("progress/gradebooks/collect")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType(typeof(IEnumerable<GradebookModel>), StatusCodes.Status200OK)]
    [EndpointName("collectGradebooks_post")]
    [AliasFor("collectGradebooks")]
    public async Task<ActionResult<IEnumerable<GradebookModel>>> PostCollectAsync(
        [FromBody] CollectGradebooks query,
        CancellationToken cancellation = default)
        => await CollectAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<GradebookModel>>> CollectAsync(
        [FromBody] CollectGradebooks query,
        CancellationToken cancellation)
    {
        var models = await _gradebookService.CollectAsync(query, cancellation);

        var count = await _gradebookService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("progress/gradebooks/search")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType(typeof(IEnumerable<GradebookMatch>), StatusCodes.Status200OK)]
    [EndpointName("searchGradebooks")]
    public async Task<ActionResult<IEnumerable<GradebookMatch>>> GetSearchAsync(
        [FromQuery] SearchGradebooks query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    [HttpPost("progress/gradebooks/search")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType(typeof(IEnumerable<GradebookMatch>), StatusCodes.Status200OK)]
    [EndpointName("searchGradebooks_post")]
    [AliasFor("searchGradebooks")]
    public async Task<ActionResult<IEnumerable<GradebookMatch>>> PostSearchAsync(
        [FromBody] SearchGradebooks query,
        CancellationToken cancellation = default)
        => await SearchAsync(query, cancellation);

    private async Task<ActionResult<IEnumerable<GradebookMatch>>> SearchAsync(
        [FromBody] SearchGradebooks query,
        CancellationToken cancellation)
    {
        var matches = await _gradebookService.SearchAsync(query, cancellation);

        var count = await _gradebookService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    [HttpGet("progress/gradebooks/download")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGradebooks")]
    public async Task<FileContentResult> GetDownloadAsync(
        [FromQuery] CollectGradebooks query,
        CancellationToken cancellation)
        => await DownloadAsync(query, cancellation);

    [HttpPost("progress/gradebooks/download")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGradebooks_post")]
    [AliasFor("downloadGradebooks")]
    public async Task<FileContentResult> PostDownloadAsync(
        [FromBody] CollectGradebooks query,
        CancellationToken cancellation)
        => await DownloadAsync(query, cancellation);

    private async Task<FileContentResult> DownloadAsync(
        [FromBody] CollectGradebooks query,
        CancellationToken cancellation)
    {
        var exporter = new ExportHelper("progress", "gradebook", query.Filter.Format, User);

        var models = await _gradebookService.DownloadAsync(query, cancellation);

        var content = _gradebookService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <remarks>
    /// DO NOT use allow this endpoint to be used in live Production environments. When time and budget permit, we will
    /// design and implement a fire-and-forget strategy managing a queue of large exports, reports, etc.
    /// </remarks>
    [HttpGet("progress/gradebooks/export")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    public ActionResult<ExportStarted> ExportAsync([FromQuery] CollectGradebooks query, CancellationToken cancellation)
    {
        // TODO: Ensure rate limiter is configured to throttle usage

        if (_releaseSettings.GetEnvironment().IsProduction())
            return Forbid("This API endpoint is not currently available for use in Production environments.");

        var exporter = new ExportManager(_databaseSettings, "progress", "gradebook");

        var start = exporter.Start(query.Filter.Format);

        // Fire and forget so the export is executed in the background.

        var task = Task.Run(async () =>
        {
            try
            {
                await _gradebookService.ExportAsync(start, query, cancellation);
            }
            catch (Exception ex)
            {
                // FIXME: Log error and mark export as failed
                // exporter.MarkAsFailed(start.ExportKey, ex.Message);
                _logger.LogError(ex, "Export failed for {ExportKey}", start.ExportKey);
            }
        });

        var started = exporter.Started(start, "progress/gradebooks/exports/{key}");

        return Ok(started);
    }

    [HttpGet("progress/gradebooks/exports/{key:guid}")]
    [HybridAuthorize(Policies.Progress.Gradebooks.Gradebook.Read)]
    public async Task<IActionResult> ExportAsync(string key)
    {
        // TODO: Replace the key with a value that has higher entropy. Perhaps validate a digital signature generated
        // and delivered to the client in the ExportStarted event.

        var helper = new ExportManager(_databaseSettings, "progress", "gradebook");

        var completed = helper.Find(key);

        if (completed.PhysicalFile == null)
            return NotFound();

        Response.Headers.Append("Content-Disposition", completed.ContentDisposition);

        Response.ContentType = completed.ContentType;

        await using var stream = new FileStream(completed.PhysicalFile, FileMode.Open, FileAccess.Read);

        await stream.CopyToAsync(Response.Body);

        return new EmptyResult();
    }


    // This entity is a current-state projection of an aggregate event/change stream. This is the reason you do not see
    // any controller actions implemented here to create, modify, or delete this entity. Data changes to this entity are 
    // permitted only using Timeline commands.
}