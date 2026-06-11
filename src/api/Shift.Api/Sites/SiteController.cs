using Microsoft.AspNetCore.Mvc;

using Shift.Common;
using Shift.Service.Workspace;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Sites API: Sites")]
public class SiteController : ShiftControllerBase
{
    private readonly SiteService _siteService;
    private readonly IPrincipalProvider _principalProvider;

    public SiteController(SiteService siteService, IPrincipalProvider principalProvider)
    {
        _siteService = siteService;
        _principalProvider = principalProvider;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific site
    /// </summary>
    [HttpHead("api/sites/{site:guid}")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("assertSite")]
    public async Task<ActionResult> AssertAsync([FromRoute] Guid site, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var organizationId = _principalProvider.GetOrganizationId(principal);

        var exists = await _siteService.AssertAsync(site, organizationId, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of sites that match specific criteria
    /// </summary>
    [HttpPost("api/sites/collect")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [EndpointName("collectSites")]
    public async Task<ActionResult<IEnumerable<SiteModel>>> PostCollectAsync([FromBody] CollectSites query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("api/sites")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [EndpointName("collectSites_get")]
    [AliasFor("collectSites")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<SiteModel>>> GetCollectAsync([FromQuery] CollectSites query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<SiteModel>>> CollectAsync(CollectSites query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var models = await _siteService.CollectAsync(query, cancellation);

        var count = await _siteService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the sites that match specific criteria
    /// </summary>
    [HttpPost("api/sites/count")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [EndpointName("countSites")]
    public async Task<ActionResult<CountResult>> PostCountAsync([FromBody] CountSites query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("api/sites/count")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [EndpointName("countSites_get")]
    [AliasFor("countSites")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<CountResult>> GetCountAsync([FromQuery] CountSites query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<ActionResult<CountResult>> CountAsync(CountSites query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var count = await _siteService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of sites that match specific criteria
    /// </summary>    
    [HttpPost("api/sites/download")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadSites")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectSites query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("api/sites/download")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
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
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

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
    [HttpGet("api/sites/{site:guid}")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [ProducesResponseType(typeof(SiteModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrieveSite")]
    public async Task<ActionResult<SiteModel>> RetrieveAsync([FromRoute] Guid site, CancellationToken cancellation = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var model = await _siteService.RetrieveAsync(site, cancellation);

        if (model == null)
            return NotFound();

        if (!_principalProvider.AllowOrganizationAccess(principal, model.OrganizationId))
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Searches for the list of sites that match specific criteria
    /// </summary>
    [HttpPost("api/sites/search")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [EndpointName("searchSites")]
    public async Task<ActionResult<IEnumerable<SiteMatch>>> PostSearchAsync([FromBody] SearchSites query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("api/sites/search")]
    [HybridPermission("workspace/sites", DataAccess.Read)]
    [EndpointName("searchSites_get")]
    [AliasFor("searchSites")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<SiteMatch>>> GetSearchAsync([FromQuery] SearchSites query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<ActionResult<IEnumerable<SiteMatch>>> SearchAsync(SearchSites query, CancellationToken cancellation)
    {
        var principal = _principalProvider.GetPrincipal();

        _principalProvider.ValidateOrganizationId(principal, query);

        var matches = await _siteService.SearchAsync(query, cancellation);

        var count = await _siteService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries
}