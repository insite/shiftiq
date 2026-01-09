using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankFormQuestionGradeitemAdapter : IEntityAdapter
{
    public void Copy(ModifyBankFormQuestionGradeitem modify, BankFormQuestionGradeitemEntity entity)
    {
        entity.GradeItemIdentifier = modify.GradeItemIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public BankFormQuestionGradeitemEntity ToEntity(CreateBankFormQuestionGradeitem create)
    {
        var entity = new BankFormQuestionGradeitemEntity
        {
            QuestionIdentifier = create.QuestionIdentifier,
            FormIdentifier = create.FormIdentifier,
            GradeItemIdentifier = create.GradeItemIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<BankFormQuestionGradeitemModel> ToModel(IEnumerable<BankFormQuestionGradeitemEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public BankFormQuestionGradeitemModel ToModel(BankFormQuestionGradeitemEntity entity)
    {
        var model = new BankFormQuestionGradeitemModel
        {
            QuestionIdentifier = entity.QuestionIdentifier,
            FormIdentifier = entity.FormIdentifier,
            GradeItemIdentifier = entity.GradeItemIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<BankFormQuestionGradeitemMatch> ToMatch(IEnumerable<BankFormQuestionGradeitemEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public BankFormQuestionGradeitemMatch ToMatch(BankFormQuestionGradeitemEntity entity)
    {
        var match = new BankFormQuestionGradeitemMatch
        {
            FormIdentifier = entity.FormIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier

        };

        return match;
    }
}