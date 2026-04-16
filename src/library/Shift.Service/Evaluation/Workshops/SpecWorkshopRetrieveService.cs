using InSite.Application.Files.Read;
using InSite.Domain.Banks;

using Shift.Contract;

using Shift.Sdk.Service;
using Shift.Service.Competency;
using Shift.Service.Content;
using Shift.Service.Evaluation.Workshops.Creators;
using Shift.Service.Security;
using Shift.Service.Timeline;
using Shift.Service.Utility;

namespace Shift.Service.Evaluation.Workshops;

public class SpecWorkshopRetrieveService(
    ITimelineQuery timelineQuery,
    StandardReader standardReader,
    UserReader userReader,
    UploadReader uploadReader,
    FileReader fileReader,
    ChangeReader changeReader,
    CollectionItemReader collectionItemReader,
    IStorageServiceAsync storageService
) : ISpecWorkshopRetrieveService
{
    public async Task<SpecWorkshop> RetrieveAsync(
        Guid bankId,
        Guid specificationId,
        Guid? setId,
        Guid? questionId,
        TimeZoneInfo timeZone
        )
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var spec = bank.FindSpecification(specificationId);

        var commentCreator = new WorkshopCommentCreator(bank, timeZone, userReader);
        var comments = bank.Comments.Where(x => x.Subject == bank.Identifier || x.Subject == spec.Identifier).ToArray();

        var standards = await new SpecWorkshopStandardCreator(standardReader).CreateAsync(spec);

        var result = new SpecWorkshop();
        result.BankId = bankId;
        result.Standards = standards;
        result.Details = new SpecWorkshopDetailsCreator().CreateAsync(spec, standards);
        result.Comments = await commentCreator.CreateAsync(comments, true);
        result.Attachments = await new WorkshopAttachmentCreator(userReader, uploadReader, fileReader, changeReader, storageService).CreateAsync(bank);
        result.ProblemQuestions = WorkshopProblemQuestionCreator.Create(spec.Bank.Sets.SelectMany(x => x.Questions));
        result.QuestionData = await new WorkshopQuestionCreator(bank, standardReader, collectionItemReader).CreateInitDataAsync(spec, setId, questionId, commentCreator);

        return result;
    }

    public async Task<SpecWorkshop.Set?> RetrieveSpecSetAsync(Guid bankId, Guid specificationId, Guid setId, TimeZoneInfo timeZone)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var spec = bank.FindSpecification(specificationId);
        if (spec == null)
            return null;

        var set = spec.Criteria.SelectMany(x => x.Sets).FirstOrDefault(x => x.Identifier == setId);
        if (set == null)
            return null;

        var commentCreator = new WorkshopCommentCreator(bank, timeZone, userReader);

        return await new WorkshopQuestionCreator(bank, standardReader, collectionItemReader).CreateSetDataAsync(set, commentCreator);
    }

    public async Task<WorkshopQuestion?> RetrieveQuestionWithoutCommentsAsync(Guid bankId, Guid questionId)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var question = bank.FindQuestion(questionId);

        return question != null
            ? WorkshopQuestionCreator.CreateQuestion(question, null, null)
            : null;
    }

    public async Task<WorkshopQuestionComments> RetrieveQuestionCommentsAsync(Guid bankId, Guid questionId, TimeZoneInfo timeZone)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var question = bank.FindQuestion(questionId);

        var commentCreator = new WorkshopCommentCreator(bank, timeZone, userReader);

        return await WorkshopQuestionCreator.CreateQuestionCommentsAsync(question, commentCreator);
    }
}
