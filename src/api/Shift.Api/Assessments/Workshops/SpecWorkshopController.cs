using Microsoft.AspNetCore.Mvc;

using Shift.Service.Assessment;

namespace Shift.Api.Assessments.Workshops;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class SpecWorkshopController(
    IPrincipalProvider principalProvider,
    BankSpecificationReader specificationReader,
    ISpecWorkshopRetrieveService retrieveService,
    ISpecWorkshopModifyService modifyService
) : ShiftControllerBase
{
    [HttpPut("api/assessments/workshops/specs/{specificationId}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("modifySpecWorkshop")]
    public async Task<ActionResult<SpecWorkshopPartial>> ModifySpecAsync(Guid specificationId, SpecWorkshop.Input input)
    {
        var principal = principalProvider.GetPrincipal();
        var entity = await specificationReader.RetrieveAsync(specificationId);

        if (entity == null || entity.OrganizationIdentifier != principal.OrganizationId)
            return NotFound();

        var result = await modifyService.ModifyAsync(entity.BankIdentifier, specificationId, input);

        return result
            ? await retrieveService.RetrievePartialAsync(entity.BankIdentifier, entity.SpecIdentifier)
            : NotFound();
    }

    [HttpGet("api/assessments/workshops/specs/{specificationId}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [EndpointName("retrieveSpecWorkshop")]
    public async Task<ActionResult<SpecWorkshop>> RetrieveSpecAsync(Guid specificationId, Guid? sectionId, Guid? questionId)
    {
        var principal = principalProvider.GetPrincipal();
        var entity = await specificationReader.RetrieveAsync(specificationId);

        if (entity == null || entity.OrganizationIdentifier != principal.OrganizationId)
            return NotFound();

        var timeZone = principal.User.TimeZone;

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

        if (entity == null || entity.OrganizationIdentifier != principal.OrganizationId)
            return NotFound();

        var timeZone = principal.User.TimeZone;

        var set = await retrieveService.RetrieveSpecSetAsync(
            entity.BankIdentifier,
            specificationId,
            setId,
            timeZone
        );

        return set != null ? Ok(set) : NotFound();
    }
}
