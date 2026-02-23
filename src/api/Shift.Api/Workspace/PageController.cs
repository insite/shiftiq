using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workspace;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workspace API: Pages")]
public class PageController : ShiftControllerBase
{
    private readonly PageService _pageService;
    private readonly IPrincipalProvider _principalProvider;

    public PageController(PageService pageService, IPrincipalProvider principalProvider)
    {
        _pageService = pageService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific page
    /// </summary>
    [HttpHead("api/workspace/pages/{page:guid}")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertPage")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid page, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _pageService.AssertAsync(page, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of pages that match specific criteria
    /// </summary>
    [HttpPost("api/workspace/pages/collect")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PageModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectPages")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectPages query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/workspace/pages")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _pageService.CollectAsync(query, cancellation);

        var count = await _pageService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the pages that match specific criteria
    /// </summary>
    [HttpPost("api/workspace/pages/count")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countPages")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountPages query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/workspace/pages/count")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _pageService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of pages that match specific criteria
    /// </summary>    
    [HttpPost("api/workspace/pages/download")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadPages")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectPages query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/workspace/pages/download")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

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
    [HttpGet("api/workspace/pages/{page:guid}")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
    [ProducesResponseType<PageModel>(StatusCodes.Status200OK)]
    [EndpointName("retrievePage")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid page, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _pageService.RetrieveAsync(page, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of pages that match specific criteria
    /// </summary>
    [HttpPost("api/workspace/pages/search")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
    [ProducesResponseType<IEnumerable<PageMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchPages")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchPages query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/workspace/pages/search")]
    [HybridPermission("workspace/pages", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _pageService.SearchAsync(query, cancellation);

        var count = await _pageService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}