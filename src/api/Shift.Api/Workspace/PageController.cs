using Microsoft.AspNetCore.Mvc;

using Shift.Service.Site;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Workspace API: Pages")]
public class PageController : ControllerBase
{
    private readonly PageService _pageService;

    public PageController(PageService pageService)
    {
        _pageService = pageService;
    }

    [HttpHead("workspace/pages/{page:guid}")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Assert)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> AssertAsync([FromRoute] Guid page, CancellationToken cancellation = default)
    {
        var exists = await _pageService.AssertAsync(page, cancellation);

        return Ok(exists);
    }

    [HttpGet("workspace/pages/{page:guid}")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Retrieve)]
    [ProducesResponseType(typeof(PageModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<PageModel>> RetrieveAsync([FromRoute] Guid page, CancellationToken cancellation = default)
    {
        var model = await _pageService.RetrieveAsync(page, cancellation);

        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("workspace/pages/count")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Count)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAsync([FromQuery] CountPages query, CancellationToken cancellation = default)
    {
        var count = await _pageService.CountAsync(query, cancellation);

        return Ok(count);
    }

    [HttpGet("workspace/pages")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Collect)]
    [ProducesResponseType(typeof(IEnumerable<PageModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PageModel>>> CollectAsync([FromQuery] CollectPages query, CancellationToken cancellation = default)
    {
        var models = await _pageService.CollectAsync(query, cancellation);

        var count = await _pageService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    [HttpGet("workspace/pages/search")]
    [HybridAuthorize(Policies.Workspace.Pages.Page.Search)]
    [ProducesResponseType(typeof(IEnumerable<PageMatch>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PageMatch>>> SearchAsync([FromQuery] SearchPages query, CancellationToken cancellation = default)
    {
        var matches = await _pageService.SearchAsync(query, cancellation);

        var count = await _pageService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    // This entity is a current-state projection of an aggregate event/change stream. This is the reason you do not see
    // any controller actions implemented here to create, modify, or delete this entity. Data changes to this entity are 
    // permitted only using Timeline commands.
}