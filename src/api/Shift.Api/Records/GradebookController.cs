using Microsoft.AspNetCore.Mvc;

using Shift.Service.Progress;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Records API: Gradebooks")]
public class GradebookController : ShiftControllerBase
{
    private readonly GradebookService _gradebookService;
    private readonly DatabaseSettings _databaseSettings;
    private readonly ReleaseSettings _releaseSettings;
    private readonly ILogger<GradebookController> _logger;
    private readonly IPrincipalProvider _principalProvider;

    public GradebookController(GradebookService gradebookService, DatabaseSettings databaseSettings, ReleaseSettings releaseSettings, ILogger<GradebookController> logger, IPrincipalProvider principalProvider)
    {
        _gradebookService = gradebookService;
        _databaseSettings = databaseSettings;
        _releaseSettings = releaseSettings;
        _logger = logger;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific gradebook
    /// </summary>
    [HttpHead("api/records/gradebooks/{gradebook:guid}")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertGradebook")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid gradebook, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _gradebookService.AssertAsync(gradebook, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of gradebooks that match specific criteria
    /// </summary>
    [HttpPost("api/records/gradebooks/collect")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [EndpointName("collectGradebooks")]
    public async Task<ActionResult<IEnumerable<GradebookModel>>> PostCollectAsync([FromBody] CollectGradebooks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/records/gradebooks")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [EndpointName("collectGradebooks_get")]
    [AliasFor("collectGradebooks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<GradebookModel>>> GetCollectAsync([FromQuery] CollectGradebooks query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<GradebookModel>>> CollectAsync(CollectGradebooks query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _gradebookService.CollectAsync(query, principal.TimeZone, cancellation);

        var count = await _gradebookService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the gradebooks that match specific criteria
    /// </summary>
    [HttpPost("api/records/gradebooks/count")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [EndpointName("countGradebooks")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountGradebooks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/records/gradebooks/count")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [EndpointName("countGradebooks_get")]
    [AliasFor("countGradebooks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountGradebooks query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountGradebooks query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _gradebookService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of gradebooks that match specific criteria
    /// </summary>    
    [HttpPost("api/records/gradebooks/download")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadGradebooks")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectGradebooks query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/records/gradebooks/download")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var exporter = new ExportHelper("Progress", "Gradebooks", query.Filter.Format, User);

        var models = await _gradebookService
            .DownloadAsync(query, principal.TimeZone, cancellation)
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
    [HttpGet("api/records/gradebooks/{gradebook:guid}")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [ProducesResponseType(typeof(GradebookModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveGradebook")]
    public async Task<ActionResult<GradebookModel>> RetrieveAsync([FromRoute] Guid gradebook, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _gradebookService.RetrieveAsync(gradebook, principal.TimeZone, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of gradebooks that match specific criteria
    /// </summary>
    [HttpPost("api/records/gradebooks/search")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [EndpointName("searchGradebooks")]
    public async Task<ActionResult<IEnumerable<GradebookMatch>>> PostSearchAsync([FromBody] SearchGradebooks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/records/gradebooks/search")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    [EndpointName("searchGradebooks_get")]
    [AliasFor("searchGradebooks")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<GradebookMatch>>> GetSearchAsync([FromQuery] SearchGradebooks query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<GradebookMatch>>> SearchAsync(SearchGradebooks query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        if (!principal.Claims.Paging)
            query.DisablePaging();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _gradebookService.SearchAsync(query, cancellation);

        var count = await _gradebookService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    /// <remarks>
    /// DO NOT use allow this endpoint to be used in live Production environments. When time and budget permit, we will
    /// design and implement a fire-and-forget strategy managing a queue of large exports, reports, etc.
    /// </remarks>
    [HttpGet("api/records/gradebooks/export")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
    public ActionResult<ExportStarted> ExportAsync([FromQuery] CollectGradebooks query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        // TODO: Ensure rate limiter is configured to throttle usage

        if (_releaseSettings.GetEnvironment().IsProduction())
            return Forbid("This API endpoint is not currently available for use in Production environments.");

        var exporter = new ExportManager(_databaseSettings, "progress", "gradebook");

        var start = exporter.Start(query.Filter.Format);

        var timeZone = principal.TimeZone;

        // Fire and forget so the export is executed in the background.

        var task = Task.Run(async () =>
        {
            try
            {
                await _gradebookService.ExportAsync(start, query, timeZone, cancellation);
            }
            catch (Exception ex)
            {
                // FIXME: Log error and mark export as failed
                // exporter.MarkAsFailed(start.ExportKey, ex.Message);
                _logger.LogError(ex, "Export failed for {ExportKey}", start.ExportKey);
            }
        });

        var started = exporter.Started(start, "api/records/gradebooks/exports/{key}");

        return Ok(started);
    }

    [HttpGet("api/records/gradebooks/exports/{key:guid}")]
    [HybridPermission("progress/gradebooks", DataAccess.Read)]
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
}