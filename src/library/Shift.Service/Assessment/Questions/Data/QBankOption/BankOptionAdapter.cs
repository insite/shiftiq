using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankOptionAdapter : IEntityAdapter
{
    public void Copy(ModifyBankOption modify, BankOptionEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.BankIdentifier = modify.BankIdentifier;
        entity.SetIdentifier = modify.SetIdentifier;
        entity.CompetencyIdentifier = modify.CompetencyIdentifier;
        entity.OptionText = modify.OptionText;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public BankOptionEntity ToEntity(CreateBankOption create)
    {
        var entity = new BankOptionEntity
        {
            OrganizationIdentifier = create.OrganizationIdentifier,
            BankIdentifier = create.BankIdentifier,
            SetIdentifier = create.SetIdentifier,
            QuestionIdentifier = create.QuestionIdentifier,
            OptionKey = create.OptionKey,
            CompetencyIdentifier = create.CompetencyIdentifier,
            OptionText = create.OptionText
        };
        return entity;
    }

    public IEnumerable<BankOptionModel> ToModel(IEnumerable<BankOptionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public BankOptionModel ToModel(BankOptionEntity entity)
    {
        var model = new BankOptionModel
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            BankIdentifier = entity.BankIdentifier,
            SetIdentifier = entity.SetIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier,
            OptionKey = entity.OptionKey,
            CompetencyIdentifier = entity.CompetencyIdentifier,
            OptionText = entity.OptionText
        };

        return model;
    }

    public IEnumerable<BankOptionMatch> ToMatch(IEnumerable<BankOptionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public BankOptionMatch ToMatch(BankOptionEntity entity)
    {
        var match = new BankOptionMatch
        {
            OptionKey = entity.OptionKey,
            QuestionIdentifier = entity.QuestionIdentifier

        };

        return match;
    }
}