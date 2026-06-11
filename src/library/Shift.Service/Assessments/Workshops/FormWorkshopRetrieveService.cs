using InSite.Domain.Banks;

using Shift.Contract;
using Shift.Sdk.Service;
using Shift.Service.Competency;
using Shift.Service.Content;
using Shift.Service.Security;
using Shift.Service.Timeline;
using Shift.Service.Utility;
using Shift.Service.Evaluation.Workshops.Creators;
using InSite.Application.Files.Read;

namespace Shift.Service.Evaluation.Workshops;

public class FormWorkshopRetrieveService(
    ITimelineQuery timelineQuery,
    StandardReader standardReader,
    UserReader userReader,
    UploadReader uploadReader,
    FileReader fileReader,
    ChangeReader changeReader,
    CollectionItemReader collectionItemReader,
    IStorageServiceAsync storageService
) : IFormWorkshopRetrieveService
{
    public async Task<FormWorkshop> RetrieveAsync(
        Guid bankId,
        Guid formId,
        Guid? sectionId,
        Guid? questionId,
        string? publicationStatus,
        bool enableQuestionSubCompetencySelection,
        TimeZoneInfo timeZone
        )
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var form = bank.FindForm(formId);

        var commentCreator = new WorkshopCommentCreator(bank, timeZone, userReader);

        var comments = bank.Comments.Where(x => x.Subject == bank.Identifier || x.Subject == form.Identifier).ToArray();

        var result = new FormWorkshop();
        result.BankId = bankId;
        result.Details = await new FormWorkshopDetailsCreator(standardReader).CreateAsync(bank, form, publicationStatus);
        result.Statistics = await new FormWorkshopStatisticsCreator(standardReader).CreateAsync(bank, form.GetQuestions(), enableQuestionSubCompetencySelection);
        result.Comments = await commentCreator.CreateAsync(comments, true);
        result.Attachments = await new WorkshopAttachmentCreator(userReader, uploadReader, fileReader, changeReader, storageService).CreateAsync(bank);
        result.ProblemQuestions = WorkshopProblemQuestionCreator.Create(form.GetQuestions());
        result.QuestionData = await new WorkshopQuestionCreator(bank, standardReader, collectionItemReader).CreateInitDataAsync(form, sectionId, questionId, commentCreator);

        return result;
    }

    public async Task<FormWorkshop.Section?> RetrieveSectionAsync(Guid bankId, Guid sectionId, TimeZoneInfo timeZone)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var section = bank.FindSection(sectionId);
        if (section == null)
            return null;

        var commentCreator = new WorkshopCommentCreator(bank, timeZone, userReader);

        return await new WorkshopQuestionCreator(bank, standardReader, collectionItemReader).CreateSectionDataAsync(section, commentCreator);
    }

    public async Task<FormWorkshop.Questions> RetrieveVerifiedQuestionsAsync(Guid bankId, Guid formId)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var form = bank.FindForm(formId);
        
        var (isQuestionOrderMatch, verifiedQuestions) = FormWorkshopDetailsCreator.GetVerifiedQuestions(bank, form);

        return new FormWorkshop.Questions
        {
            IsQuestionOrderMatch = isQuestionOrderMatch,
            StaticQuestionOrderVerified = form.StaticQuestionOrderVerified,
            VerifiedQuestions = verifiedQuestions
        };
    }

    public async Task<WorkshopQuestionComments> RetrieveQuestionCommentsAsync(Guid bankId, Guid fieldId, TimeZoneInfo timeZone)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var field = bank.FindField(fieldId);

        var commentCreator = new WorkshopCommentCreator(bank, timeZone, userReader);

        return await WorkshopQuestionCreator.CreateQuestionCommentsAsync(field.Question, commentCreator);
    }
}
