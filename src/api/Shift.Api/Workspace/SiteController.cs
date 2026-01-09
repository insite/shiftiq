using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workspace;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workspace API: Sites")]
public class SiteController : ShiftControllerBase
{
    private readonly SiteService _siteService;

    public SiteController(SiteService siteService)
    {
        _siteService = siteService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific site
    /// </summary>
    [HttpHead("workspace/sites/{site:guid}")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertSite")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid site, CancellationToken cancellation = default)
    {
        var exists = await _siteService.AssertAsync(site, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of sites that match specific criteria
    /// </summary>
    [HttpPost("workspace/sites/collect")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Collect)]
    [ProducesResponseType<IEnumerable<SiteModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSites")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectSites query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workspace/sites")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Collect)]
    [ProducesResponseType<IEnumerable<SiteModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectSites_get")]
    [AliasFor("collectSites")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectSites query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectSites query, CancellationToken cancellation)
    {
        var models = await _siteService.CollectAsync(query, cancellation);

        var count = await _siteService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the sites that match specific criteria
    /// </summary>
    [HttpPost("workspace/sites/count")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSites")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountSites query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workspace/sites/count")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countSites_get")]
    [AliasFor("countSites")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountSites query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountSites query, CancellationToken cancellation)
    {
        var count = await _siteService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of sites that match specific criteria
    /// </summary>    
    [HttpPost("workspace/sites/download")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSites")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectSites query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workspace/sites/download")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSites_get")]
    [AliasFor("downloadSites")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectSites query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectSites query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workspace", "Sites", query.Filter.Format, User);

        var models = await _siteService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _siteService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific site
    /// </summary>
    [HttpGet("workspace/sites/{site:guid}")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Retrieve)]
    [ProducesResponseType<SiteModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveSite")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid site, CancellationToken cancellation = default)
    {
        var model = await _siteService.RetrieveAsync(site, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of sites that match specific criteria
    /// </summary>
    [HttpPost("workspace/sites/search")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Search)]
    [ProducesResponseType<IEnumerable<SiteMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSites")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchSites query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workspace/sites/search")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Search)]
    [ProducesResponseType<IEnumerable<SiteMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchSites_get")]
    [AliasFor("searchSites")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchSites query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchSites query, CancellationToken cancellation)
    {
        var matches = await _siteService.SearchAsync(query, cancellation);

        var count = await _siteService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}