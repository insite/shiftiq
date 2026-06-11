using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api.Assessments.Workshops;

[ApiController]
[ApiExplorerSettings(GroupName = "Assessments API: Workshops")]
public class SpecWorkshopController(
    IPrincipalProvider principalProvider,
    BankSpecificationReader specificationReader,
    OrganizationService organizationService,
    OrganizationAdapter organizationAdapter,
    ISpecWorkshopRetrieveService retrieveService,
    ISpecWorkshopModifyService modifyService
) : ShiftControllerBase
{
    [HttpPut("api/assessments/workshops/specs/{specificationId}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("modifySpecWorkshop")]
    public async Task<ActionResult<SpecWorkshop>> ModifySpecAsync(Guid specificationId, SpecWorkshop.Input input)
    {
        var principal = principalProvider.GetPrincipal();
        var entity = await specificationReader.RetrieveAsync(specificationId);

        if (entity == null
            || !principalProvider.AllowOrganizationAccess(principal, entity.OrganizationIdentifier)
            )
        {
            return NotFound();
        }

        var result = await modifyService.ModifyAsync(entity.BankIdentifier, specificationId, input);

        return result ? Ok(new { }) : NotFound();
    }

    [HttpGet("api/assessments/workshops/specs/{specificationId}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("retrieveSpecWorkshop")]
    public async Task<ActionResult<SpecWorkshop>> RetrieveSpecAsync(Guid specificationId, Guid? sectionId, Guid? questionId)
    {
        var principal = principalProvider.GetPrincipal();
        var entity = await specificationReader.RetrieveAsync(specificationId);

        if (entity == null
            || !principalProvider.AllowOrganizationAccess(principal, entity.OrganizationIdentifier)
            )
        {
            return NotFound();
        }

        var organization = await organizationService.RetrieveAsync(entity.OrganizationIdentifier) ?? throw new ArgumentNullException("organization");
        var organizationData = organizationAdapter.ToData(organization);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);

        var formWorkshop = await retrieveService.RetrieveAsync(
            entity.BankIdentifier,
            specificationId,
            sectionId,
            questionId,
            timeZone
        );

        return Ok(formWorkshop);
    }

    [HttpGet("api/assessments/workshops/specs/{specificationId}/sets/{setId}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("retrieveSpecWorkshopSet")]
    public async Task<ActionResult<FormWorkshop.Section>> RetrieveSpecSetAsync(Guid specificationId, Guid setId)
    {
        var principal = principalProvider.GetPrincipal();
        var entity = await specificationReader.RetrieveAsync(specificationId);

        if (entity == null
            || !principalProvider.AllowOrganizationAccess(principal, entity.OrganizationIdentifier)
            )
        {
            return NotFound();
        }

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);

        var set = await retrieveService.RetrieveSpecSetAsync(
            entity.BankIdentifier,
            specificationId,
            setId,
            timeZone
        );

        return set != null ? Ok(set) : NotFound();
    }
}
