using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankSpecificationAdapter : IEntityAdapter
{
    public void Copy(ModifyBankSpecification modify, BankSpecificationEntity entity)
    {
        entity.BankIdentifier = modify.BankIdentifier;
        entity.CalcDisclosure = modify.CalcDisclosure;
        entity.CalcPassingScore = modify.CalcPassingScore;
        entity.SpecAsset = modify.SpecAsset;
        entity.SpecConsequence = modify.SpecConsequence;
        entity.SpecFormCount = modify.SpecFormCount;
        entity.SpecFormLimit = modify.SpecFormLimit;
        entity.SpecName = modify.SpecName;
        entity.SpecQuestionLimit = modify.SpecQuestionLimit;
        entity.SpecType = modify.SpecType;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.CriterionTagCount = modify.CriterionTagCount;
        entity.CriterionPivotCount = modify.CriterionPivotCount;
        entity.CriterionAllCount = modify.CriterionAllCount;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public BankSpecificationEntity ToEntity(CreateBankSpecification create)
    {
        var entity = new BankSpecificationEntity
        {
            BankIdentifier = create.BankIdentifier,
            CalcDisclosure = create.CalcDisclosure,
            CalcPassingScore = create.CalcPassingScore,
            SpecAsset = create.SpecAsset,
            SpecConsequence = create.SpecConsequence,
            SpecFormCount = create.SpecFormCount,
            SpecFormLimit = create.SpecFormLimit,
            SpecIdentifier = create.SpecIdentifier,
            SpecName = create.SpecName,
            SpecQuestionLimit = create.SpecQuestionLimit,
            SpecType = create.SpecType,
            OrganizationIdentifier = create.OrganizationIdentifier,
            CriterionTagCount = create.CriterionTagCount,
            CriterionPivotCount = create.CriterionPivotCount,
            CriterionAllCount = create.CriterionAllCount
        };
        return entity;
    }

    public IEnumerable<BankSpecificationModel> ToModel(IEnumerable<BankSpecificationEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public BankSpecificationModel ToModel(BankSpecificationEntity entity)
    {
        var model = new BankSpecificationModel
        {
            BankIdentifier = entity.BankIdentifier,
            CalcDisclosure = entity.CalcDisclosure,
            CalcPassingScore = entity.CalcPassingScore,
            SpecAsset = entity.SpecAsset,
            SpecConsequence = entity.SpecConsequence,
            SpecFormCount = entity.SpecFormCount,
            SpecFormLimit = entity.SpecFormLimit,
            SpecIdentifier = entity.SpecIdentifier,
            SpecName = entity.SpecName,
            SpecQuestionLimit = entity.SpecQuestionLimit,
            SpecType = entity.SpecType,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            CriterionTagCount = entity.CriterionTagCount,
            CriterionPivotCount = entity.CriterionPivotCount,
            CriterionAllCount = entity.CriterionAllCount
        };

        return model;
    }

    public IEnumerable<BankSpecificationMatch> ToMatch(IEnumerable<BankSpecificationEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public BankSpecificationMatch ToMatch(BankSpecificationEntity entity)
    {
        var match = new BankSpecificationMatch
        {
            SpecIdentifier = entity.SpecIdentifier

        };

        return match;
    }
}