using Microsoft.AspNetCore.Mvc;

using Shift.Service.Workspace;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workspace API: Pages")]
public class PageController : ShiftControllerBase
{
    private readonly PageService _pageService;

    public PageController(PageService pageService)
    {
        _pageService = pageService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific page
    /// </summary>
    [HttpHead("workspace/pages/{page:guid}")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertPage")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid page, CancellationToken cancellation = default)
    {
        var exists = await _pageService.AssertAsync(page, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of pages that match specific criteria
    /// </summary>
    [HttpPost("workspace/pages/collect")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Collect)]
    [ProducesResponseType<IEnumerable<PageModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPages")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectPages query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("workspace/pages")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Collect)]
    [ProducesResponseType<IEnumerable<PageModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPages_get")]
    [AliasFor("collectPages")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectPages query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectPages query, CancellationToken cancellation)
    {
        var models = await _pageService.CollectAsync(query, cancellation);

        var count = await _pageService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the pages that match specific criteria
    /// </summary>
    [HttpPost("workspace/pages/count")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPages")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountPages query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("workspace/pages/count")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPages_get")]
    [AliasFor("countPages")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountPages query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountPages query, CancellationToken cancellation)
    {
        var count = await _pageService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of pages that match specific criteria
    /// </summary>    
    [HttpPost("workspace/pages/download")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPages")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPages query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("workspace/pages/download")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPages_get")]
    [AliasFor("downloadPages")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectPages query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectPages query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Workspace", "Pages", query.Filter.Format, User);

        var models = await _pageService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _pageService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific page
    /// </summary>
    [HttpGet("workspace/pages/{page:guid}")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Retrieve)]
    [ProducesResponseType<PageModel>(StatusCodes.Status200OK)]
    [EndpointName("retrievePage")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid page, CancellationToken cancellation = default)
    {
        var model = await _pageService.RetrieveAsync(page, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of pages that match specific criteria
    /// </summary>
    [HttpPost("workspace/pages/search")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Search)]
    [ProducesResponseType<IEnumerable<PageMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPages")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchPages query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("workspace/pages/search")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Search)]
    [ProducesResponseType<IEnumerable<PageMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPages_get")]
    [AliasFor("searchPages")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchPages query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchPages query, CancellationToken cancellation)
    {
        var matches = await _pageService.SearchAsync(query, cancellation);

        var count = await _pageService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}