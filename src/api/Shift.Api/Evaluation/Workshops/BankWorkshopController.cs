using InSite.Application.Banks.Write;
using InSite.Domain.Banks;

using Microsoft.AspNetCore.Mvc;

using Shift.Constant;

using Shift.Sdk.Service;
using Shift.Service.Assessment;
using Shift.Service.Competency;

namespace Shift.Api.Evaluation.Workshops;

[ApiController]
[ApiExplorerSettings(GroupName = "Evaluation API: Workshops")]
public class BankWorkshopController(IPrincipalProvider principalProvider, ITimelineQuery timelineQuery) : ShiftControllerBase
{
    public class QuestionInput
    {
        public WorkshopQuestionField Field { get; set; }
        public int? ColumnIndex { get; set; }
        public string? Value { get; set; }
    }

    public class OptionInput
    {
        public WorkshopQuestionOptionField Field { get; set; }
        public int? ColumnIndex { get; set; }
        public string? Value { get; set; }
    }

    public class FieldCommentInput
    {
        public string AuthorType { get; set; } = default!;
        public FlagType Flag { get; set; }
        public string Text { get; set; } = default!;
    }

    public class QuestionChangeDate
    {
        public Guid QuestionId { get; set; }
        public DateTimeOffset LastChangeTime { get; set; }
    }

    public class Result
    {
        public string? Html { get; set; }
    }

    [HttpGet("api/evaluation/workshops/banks/{bankId}/images")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<WorkshopImage[]>(StatusCodes.Status200OK)]
    [EndpointName("collectWorkshopImages")]
    public async Task<IActionResult> CollectWorkshopImagesAsync(BankReader bankReader, IWorkshopImageListService imageListService, Guid bankId)
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await bankReader.RetrieveAsync(bankId);

        if (bank == null || !principalProvider.AllowOrganizationAccess(principal, bank.OrganizationIdentifier))
            return NotFound();

        var result = await imageListService.CollectImagesAsync(principal, bankId);

        return Ok(result);
    }

    [HttpGet("api/evaluation/workshops/banks/{bankId}/question-change-dates")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<QuestionChangeDate[]>(StatusCodes.Status200OK)]
    [EndpointName("collectWorkshopQuestionChangeDates")]
    public async Task<IActionResult> CollectQuestionChangeDatesAsync(BankReader bankReader, BankQuestionReader questionReader, Guid bankId)
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await bankReader.RetrieveAsync(bankId);

        if (bank == null || !principalProvider.AllowOrganizationAccess(principal, bank.OrganizationIdentifier))
            return NotFound();

        var questions = await questionReader.CollectByBankIdAsync(bankId);

        var result = questions
            .Where(x => x.LastChangeTime != null)
            .Select(x => new QuestionChangeDate
            {
                QuestionId = x.QuestionIdentifier,
                LastChangeTime = x.LastChangeTime!.Value
            })
            .ToArray();

        return Ok(result);
    }

    [HttpPut("api/evaluation/workshops/banks/{bankId}/questions/{questionId}")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    [EndpointName("modifyWorkshopQuestion")]
    public async Task<IActionResult> ModifyWorkshopQuestionAsync(
        IWorkshopModifyQuestionService modifyQuestionService,
        Guid bankId,
        Guid questionId,
        QuestionInput input
    )
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);

        if (bank == null || !principalProvider.AllowOrganizationAccess(principal, bank.Tenant))
            return NotFound();

        var result = await modifyQuestionService.ModifyAsync(bank, bankId, questionId, input.Field, input.ColumnIndex, input.Value);

        return Ok(new Result { Html = result });
    }

    [HttpPut("api/evaluation/workshops/banks/{bankId}/questions/{questionId}/options/{optionNumber}")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<Result>(StatusCodes.Status200OK)]
    [EndpointName("modifyWorkshopOption")]
    public async Task<IActionResult> ModifyWorkshopOptionAsync(
        IWorkshopModifyOptionService modifyOptionService,
        Guid bankId,
        Guid questionId,
        int optionNumber,
        OptionInput input
    )
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);

        if (bank == null || !principalProvider.AllowOrganizationAccess(principal, bank.Tenant))
            return NotFound();

        var result = await modifyOptionService.ModifyAsync(bank, bankId, questionId, optionNumber, input.Field, input.ColumnIndex, input.Value);

        return Ok(new Result { Html = result });
    }

    [HttpPost("api/evaluation/workshops/banks/{bankId}/questions/{questionId}/comments")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<WorkshopQuestionComments>(StatusCodes.Status200OK)]
    [EndpointName("postWorkshopQuestionComment")]
    public async Task<IActionResult> PostQuestionCommentAsync(
        ISpecWorkshopRetrieveService retrieveService,
        ICommanderAsync commander,
        Guid bankId,
        Guid questionId,
        FieldCommentInput input
    )
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);

        if (bank == null || bank.FindQuestion(questionId) == null || !principalProvider.AllowOrganizationAccess(principal, bank.Tenant))
            return NotFound();

        var command = new PostComment(
            bankId,
            UniqueIdentifier.Create(),
            input.Flag,
            CommentType.Question,
            questionId,
            principal.UserId,
            input.AuthorType,
            null,
            input.Text,
            null, null, null,
            DateTimeOffset.UtcNow
        );

        await commander.SendCommandsAsync([command]);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);
        var result = await retrieveService.RetrieveQuestionCommentsAsync(bankId, questionId, timeZone);

        return Ok(result);
    }

    [HttpPost("api/evaluation/workshops/banks/{bankId}/fields/{fieldId}/comments")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<WorkshopQuestionComments>(StatusCodes.Status200OK)]
    [EndpointName("postWorkshopFieldComment")]
    public async Task<IActionResult> PostFieldCommentAsync(
        IFormWorkshopRetrieveService retrieveService,
        ICommanderAsync commander,
        Guid bankId,
        Guid fieldId,
        FieldCommentInput input
    )
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);

        if (bank == null || bank.FindField(fieldId) == null || !principalProvider.AllowOrganizationAccess(principal, bank.Tenant))
            return NotFound();

        var command = new PostComment(
            bankId,
            UniqueIdentifier.Create(),
            input.Flag,
            CommentType.Field,
            fieldId,
            principal.UserId,
            input.AuthorType,
            null,
            input.Text,
            null, null, null,
            DateTimeOffset.UtcNow
        );

        await commander.SendCommandsAsync([command]);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);
        var result = await retrieveService.RetrieveQuestionCommentsAsync(bankId, fieldId, timeZone);

        return Ok(result);
    }

    [HttpPost("api/evaluation/workshops/banks/{bankId}/fields/{fieldId}/replace")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<FormWorkshop.Section>(StatusCodes.Status200OK)]
    [EndpointName("replaceWorkshopField")]
    public async Task<IActionResult> ReplaceFieldQuestionAsync(
        IFormWorkshopRetrieveService retrieveService,
        IWorkshopModifyQuestionService modifyService,
        Guid bankId,
        Guid fieldId,
        WorkshopReplaceCommand command
    )
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var field = bank?.FindField(fieldId);

        if (field == null || !principalProvider.AllowOrganizationAccess(principal, bank!.Tenant))
            return NotFound();

        await modifyService.ReplaceQuestionAsync(bank, bankId, fieldId, command);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);
        var sectionData = await retrieveService.RetrieveSectionAsync(bankId, field.Section.Identifier, timeZone);

        return Ok(sectionData);
    }

    [HttpPost("api/evaluation/workshops/banks/{bankId}/sets/{setId}/questions")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType<WorkshopQuestion>(StatusCodes.Status200OK)]
    [EndpointName("addWorkshopQuestion")]
    public async Task<IActionResult> AddQuestionAsync(
        ISpecWorkshopRetrieveService retrieveService,
        IWorkshopModifyQuestionService modifyService,
        StandardReader standardReader,
        Guid bankId,
        Guid setId,
        Guid? standardId,
        WorkshopNewQuestionCommand command
    )
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var set = bank?.FindSet(setId);

        if (set == null || !principalProvider.AllowOrganizationAccess(principal, bank!.Tenant))
            return NotFound();

        if (standardId.HasValue)
        {
            var standard = await standardReader.RetrieveAsync(standardId.Value);
            if (standard == null || standard.ParentStandardIdentifier != set.Standard)
                return NotFound();
        }

        var questionId = await modifyService.AddQuestionAsync(bank.Tenant, bankId, setId, standardId, command);

        var question = await retrieveService.RetrieveQuestionWithoutCommentsAsync(bankId, questionId);
        if (question == null)
            throw ApplicationError.Create($"Question is null: {questionId}");

        return Ok(question);
    }

    [HttpPut("api/evaluation/workshops/banks/{bankId}/comments/{commentId}/showhide")]
    [HybridPermission("evaluation/banks", DataAccess.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointName("showHideWorkshopComment")]
    public async Task<IActionResult> HideShowCommentAsync(
        IFormWorkshopRetrieveService retrieveService,
        ICommanderAsync commander,
        Guid bankId,
        Guid commentId,
        bool hidden
    )
    {
        var principal = principalProvider.GetPrincipal();
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var comment = bank?.FindComment(commentId);

        if (comment == null || !principalProvider.AllowOrganizationAccess(principal, bank!.Tenant))
            return NotFound();

        var command = new ChangeCommentVisibility(bankId, commentId, hidden);

        await commander.SendCommandsAsync([command]);

        return Ok(new {});
    }
}
