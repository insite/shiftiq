using InSite.Application.Banks.Write;
using InSite.Domain.Banks;

using Microsoft.AspNetCore.Mvc;

using Shift.Constant;
using Shift.Sdk.Service;
using Shift.Service.Evaluation;

namespace Shift.Api.Evaluation.Workshops;

[ApiController]
[ApiExplorerSettings(GroupName = "Evaluation API: Workshops")]
public class FormWorkshopController(
    IPrincipalProvider principalProvider,
    AssessmentService assessmentService,
    OrganizationService organizationService,
    OrganizationAdapter organizationAdapter,
    IFormWorkshopRetrieveService retrieveService,
    ICommanderAsync commander,
    ITimelineQuery timelineQuery
) : ShiftControllerBase
{
    [HttpGet("api/evaluation/workshops/forms/{formId}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [ProducesResponseType<FormWorkshop>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFormWorkshop")]
    public async Task<IActionResult> RetrieveFormAsync(Guid formId, Guid? sectionId, Guid? questionId)
    {
        var principal = principalProvider.GetPrincipal();
        var model = await assessmentService.RetrieveAsync(formId);

        if (model == null
            || !principalProvider.AllowOrganizationAccess(principal, model.OrganizationIdentifier)
            )
        {
            return NotFound();
        }

        var organization = await organizationService.RetrieveAsync(model.OrganizationIdentifier) ?? throw new ArgumentNullException("organization");
        var organizationData = organizationAdapter.ToData(organization);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);

        var formWorkshop = await retrieveService.RetrieveAsync(
            model.BankIdentifier,
            model.FormIdentifier,
            sectionId,
            questionId,
            model.FormPublicationStatus,
            organizationData.Toolkits.Assessments.EnableQuestionSubCompetencySelection,
            timeZone
        );

        return Ok(formWorkshop);
    }

    [HttpGet("api/evaluation/workshops/forms/{formId}/sections/{sectionId}")]
    [HybridPermission("evaluation/banks", DataAccess.Read)]
    [ProducesResponseType<FormWorkshop.Section>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFormWorkshopSection")]
    public async Task<IActionResult> RetrieveSectionAsync(Guid formId, Guid sectionId)
    {
        var principal = principalProvider.GetPrincipal();
        var model = await assessmentService.RetrieveAsync(formId);

        if (model == null
            || !principalProvider.AllowOrganizationAccess(principal, model.OrganizationIdentifier)
            )
        {
            return NotFound();
        }

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);

        var formWorkshopSection = await retrieveService.RetrieveSectionAsync(
            model.BankIdentifier,
            sectionId,
            timeZone
        );

        return formWorkshopSection != null ? Ok(formWorkshopSection) : NotFound();
    }

    [HttpPut("api/evaluation/workshops/forms/{formId}/third-party-assessment")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointName("modifyFormWorkshop_thirdPartyAssessment")]
    public async Task<IActionResult> ModifyThirdPartyAssessmentAsync(Guid formId, bool enabled)
    {
        var principal = principalProvider.GetPrincipal();
        var model = await assessmentService.RetrieveAsync(formId);

        if (model == null
            || !principalProvider.AllowOrganizationAccess(principal, model.OrganizationIdentifier)
            )
        {
            return NotFound();
        }

        if (enabled)
            await commander.SendCommandsAsync([new EnableThirdPartyAssessment(model.BankIdentifier, model.FormIdentifier)]);
        else
            await commander.SendCommandsAsync([new DisableThirdPartyAssessment(model.BankIdentifier, model.FormIdentifier)]);

        return Ok();
    }

    [HttpPost("api/evaluation/workshops/forms/{formId}/verify-static-question-order")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<FormWorkshop.Questions>(StatusCodes.Status200OK)]
    [EndpointName("modifyFormWorkshop_verifyStaticQuestionOrder")]
    public async Task<IActionResult> VerifyStaticQuestionOrderAsync(Guid formId)
    {
        var principal = principalProvider.GetPrincipal();
        var model = await assessmentService.RetrieveAsync(formId);

        if (model == null
            || !principalProvider.AllowOrganizationAccess(principal, model.OrganizationIdentifier)
            )
        {
            return NotFound();
        }

        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(model.BankIdentifier);
        var form = bank.FindForm(formId);

        if (form.Specification.Type != SpecificationType.Static)
            return BadRequest();

        var questions = form.GetStaticFormQuestionIdentifiersInOrder();

        await commander.SendCommandsAsync([new VerifyAssessmentQuestionOrder(model.BankIdentifier, model.FormIdentifier, questions)]);

        var result = await retrieveService.RetrieveVerifiedQuestionsAsync(model.BankIdentifier, model.FormIdentifier);

        return Ok(result);
    }
}
