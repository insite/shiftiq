using Microsoft.AspNetCore.Mvc;

namespace Shift.Api.Sites;

[ApiController]
[HybridAuthorize()]
[ApiExplorerSettings(GroupName = "Sites API: Pages")]
public class PageContentController(
    IPageService pageService,
    IPrincipalProvider principalProvider,
    IContentRetrieveService contentRetrieveService,
    IContentModifyService contentModifyService
) : ShiftControllerBase
{
    [HttpGet("api/sites/pages-contents/{page:guid}")]
    [HybridPermission("workspace/pages", DataAccess.Read | DataAccess.Update)]
    [ProducesResponseType(typeof(PageContentModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("retrievePageContent")]
    public async Task<ActionResult<PageContentModel>> RetrieveAsync([FromRoute] Guid page, CancellationToken cancellation = default)
    {
        var principal = principalProvider.GetPrincipal();
        var model = await pageService.RetrieveAsync(page, cancellation);

        if (model == null
            || !principalProvider.AllowOrganizationAccess(principal, model.OrganizationId)
            )
        {
            return NotFound();
        }

        var pageContent = await contentRetrieveService.RetrievePageContentAsync(model);

        return Ok(pageContent);
    }

    [HttpPut("api/sites/pages-contents/{page:guid}")]
    [HybridPermission("workspace/pages", DataAccess.Read | DataAccess.Update)]
    [EndpointName("modifyPageContent")]
    public async Task<ActionResult<Dictionary<int, Guid>>> ModifyAsync([FromRoute] Guid page, [FromBody] PageContentModifyModel modifyModel, CancellationToken cancellation = default)
    {
        var principal = principalProvider.GetPrincipal();
        var model = await pageService.RetrieveAsync(page, cancellation);

        if (model == null
            || !principalProvider.AllowOrganizationAccess(principal, model.OrganizationId)
            )
        {
            return NotFound();
        }

        var result = await contentModifyService.ModifyPageContentAsync(model, modifyModel, principal.User.Name);

        return Ok(result);
    }
}