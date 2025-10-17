using Microsoft.AspNetCore.Mvc;

using Shift.Service.Site;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workspace API: Sites")]
public class SiteController : ControllerBase
{
    private readonly SiteService _siteService;

    public SiteController(SiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpHead("workspace/sites/{site:guid}")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid site, CancellationToken cancellation = default)
    {
        var exists = await _siteService.AssertAsync(site, cancellation);

        return Ok(exists);
    }

    [HttpGet("workspace/sites/{site:guid}")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Retrieve)]
    [ProducesResponseType(typeof(SiteModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<SiteModel>> RetrieveAsync([FromRoute] Guid site, CancellationToken cancellation = default)
    {
        var model = await _siteService.RetrieveAsync(site, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("workspace/sites/count")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountSites query, CancellationToken cancellation = default)
    {
        var count = await _siteService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("workspace/sites")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Collect)]
    [ProducesResponseType(typeof(IEnumerable<SiteModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SiteModel>>> CollectAsync([FromQuery] CollectSites query, CancellationToken cancellation = default)
    {
        var models = await _siteService.CollectAsync(query, cancellation);

        var count = await _siteService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("workspace/sites/search")]
    [HybridAuthorize(Policies.Workspace.Sites.Site.Search)]
    [ProducesResponseType(typeof(IEnumerable<SiteMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SiteMatch>>> SearchAsync([FromQuery] SearchSites query, CancellationToken cancellation = default)
    {
        var matches = await _siteService.SearchAsync(query, cancellation);

        var count = await _siteService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    // This entity is a current-state projection of an aggregate event/change stream. This is the reason you do not see
    // any controller actions implemented here to create, modify, or delete this entity. Data changes to this entity are 
    // permitted only using Timeline commands.
}