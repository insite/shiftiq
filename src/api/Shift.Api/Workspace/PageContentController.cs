using Microsoft.AspNetCore.Mvc;

namespace Shift.Api.Workspace;

[ApiController]
[ApiExplorerSettings(GroupName = "Workspace API: Pages' Content")]
public class PageContentController(
    IPageService pageService,
    IPrincipalProvider principalProvider,
    IContentRetrieveService contentRetrieveService,
    IContentModifyService contentModifyService
) : ShiftControllerBase
{
    [HttpGet("api/workspace/pages-contents/{page:guid}")]
    [HybridPermission("workspace/pages", DataAccess.Read | DataAccess.Update)]
    [ProducesResponseType<PageContentModel>(StatusCodes.Status200OK)]
    [EndpointName("retrievePageContent")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid page, CancellationToken cancellation = default)
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

    [HttpPost("api/workspace/pages-contents/{page:guid}")]
    [HybridPermission("workspace/pages", DataAccess.Read | DataAccess.Update)]
    [ProducesResponseType<PageContentModel>(StatusCodes.Status200OK)]
    [EndpointName("modifyPageContent")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid page, [FromBody] PageContentModifyModel modifyModel, CancellationToken cancellation = default)
    {
        var principal = principalProvider.GetPrincipal();
        var model = await pageService.RetrieveAsync(page, cancellation);

        if (model == null
            || !principalProvider.AllowOrganizationAccess(principal, model.OrganizationId)
            )
        {
            return NotFound();
        }

        await contentModifyService.ModifyPageContentAsync(model, modifyModel, principal.User.Name);

        return Ok();
    }
}