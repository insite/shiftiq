using Microsoft.AspNetCore.Mvc;

using Shift.Common.Exceptions;
using Shift.Service.Integrations.Proctoring.Proview;

namespace Shift.Api.Integrations.Proctoring;

[HybridAuthorize]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ProviewController(IPrincipalProvider principalProvider, ProviewService proviewService) : ShiftControllerBase
{
    [HttpGet("api/integrations/proctoring/proview/session")]
    public async Task<ActionResult<ProviewSession>> RetrieveSessionAsync(Guid? form, string? resource, string attempt)
    {
        var principal = principalProvider.GetPrincipal();
        var organizationId = principal.OrganizationId;
        var userId = principal.UserId;
        var language = principal.User.Language ?? "en";

        return form != null
            ? await  proviewService.RetrieveByFormAsync(organizationId, userId, language, form.Value, attempt)
            : !string.IsNullOrEmpty(resource)
                ? await proviewService.RetrieveByResourceAsync(organizationId, userId, language, resource, attempt)
                : throw new ClientErrorException("Resource parameter is invalid");
    }

    public record ModifyInput(string ProviewSessionId);

    [HttpPut("api/integrations/proctoring/proview/session/{shiftSessionId}")]
    public async Task<IActionResult> ModifyProviewSessionIdAsync(string shiftSessionId, ModifyInput input)
    {
        var principal = principalProvider.GetPrincipal();
        var organizationId = principal.OrganizationId;
        var userId = principal.UserId;

        await proviewService.ModifyProviewSessionIdAsync(organizationId, userId, shiftSessionId, input.ProviewSessionId);

        return NoContent();
    }    
}