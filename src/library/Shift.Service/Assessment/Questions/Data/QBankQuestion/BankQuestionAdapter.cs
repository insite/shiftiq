using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankQuestionAdapter : IEntityAdapter
{
    public void Copy(ModifyBankQuestion modify, BankQuestionEntity entity)
    {
        entity.BankIdentifier = modify.BankIdentifier;
        entity.BankIndex = modify.BankIndex;
        entity.QuestionCode = modify.QuestionCode;
        entity.QuestionDifficulty = modify.QuestionDifficulty;
        entity.QuestionLikeItemGroup = modify.QuestionLikeItemGroup;
        entity.QuestionReference = modify.QuestionReference;
        entity.QuestionTag = modify.QuestionTag;
        entity.QuestionTaxonomy = modify.QuestionTaxonomy;
        entity.QuestionText = modify.QuestionText;
        entity.CompetencyIdentifier = modify.CompetencyIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.QuestionCondition = modify.QuestionCondition;
        entity.QuestionFlag = modify.QuestionFlag;
        entity.SetIdentifier = modify.SetIdentifier;
        entity.QuestionFirstPublished = modify.QuestionFirstPublished;
        entity.QuestionSourceIdentifier = modify.QuestionSourceIdentifier;
        entity.QuestionSourceAssetNumber = modify.QuestionSourceAssetNumber;
        entity.QuestionAssetNumber = modify.QuestionAssetNumber;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.ParentQuestionIdentifier = modify.ParentQuestionIdentifier;
        entity.BankSubIndex = modify.BankSubIndex;
        entity.QuestionType = modify.QuestionType;
        entity.QuestionTags = modify.QuestionTags;
        entity.RubricIdentifier = modify.RubricIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public BankQuestionEntity ToEntity(CreateBankQuestion create)
    {
        var entity = new BankQuestionEntity
        {
            BankIdentifier = create.BankIdentifier,
            BankIndex = create.BankIndex,
            QuestionCode = create.QuestionCode,
            QuestionDifficulty = create.QuestionDifficulty,
            QuestionIdentifier = create.QuestionIdentifier,
            QuestionLikeItemGroup = create.QuestionLikeItemGroup,
            QuestionReference = create.QuestionReference,
            QuestionTag = create.QuestionTag,
            QuestionTaxonomy = create.QuestionTaxonomy,
            QuestionText = create.QuestionText,
            CompetencyIdentifier = create.CompetencyIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            QuestionCondition = create.QuestionCondition,
            QuestionFlag = create.QuestionFlag,
            SetIdentifier = create.SetIdentifier,
            QuestionFirstPublished = create.QuestionFirstPublished,
            QuestionSourceIdentifier = create.QuestionSourceIdentifier,
            QuestionSourceAssetNumber = create.QuestionSourceAssetNumber,
            QuestionAssetNumber = create.QuestionAssetNumber,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            ParentQuestionIdentifier = create.ParentQuestionIdentifier,
            BankSubIndex = create.BankSubIndex,
            QuestionType = create.QuestionType,
            QuestionTags = create.QuestionTags,
            RubricIdentifier = create.RubricIdentifier
        };
        return entity;
    }

    public IEnumerable<BankQuestionModel> ToModel(IEnumerable<BankQuestionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public BankQuestionModel ToModel(BankQuestionEntity entity)
    {
        var model = new BankQuestionModel
        {
            BankIdentifier = entity.BankIdentifier,
            BankIndex = entity.BankIndex,
            QuestionCode = entity.QuestionCode,
            QuestionDifficulty = entity.QuestionDifficulty,
            QuestionIdentifier = entity.QuestionIdentifier,
            QuestionLikeItemGroup = entity.QuestionLikeItemGroup,
            QuestionReference = entity.QuestionReference,
            QuestionTag = entity.QuestionTag,
            QuestionTaxonomy = entity.QuestionTaxonomy,
            QuestionText = entity.QuestionText,
            CompetencyIdentifier = entity.CompetencyIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            QuestionCondition = entity.QuestionCondition,
            QuestionFlag = entity.QuestionFlag,
            SetIdentifier = entity.SetIdentifier,
            QuestionFirstPublished = entity.QuestionFirstPublished,
            QuestionSourceIdentifier = entity.QuestionSourceIdentifier,
            QuestionSourceAssetNumber = entity.QuestionSourceAssetNumber,
            QuestionAssetNumber = entity.QuestionAssetNumber,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            ParentQuestionIdentifier = entity.ParentQuestionIdentifier,
            BankSubIndex = entity.BankSubIndex,
            QuestionType = entity.QuestionType,
            QuestionTags = entity.QuestionTags,
            RubricIdentifier = entity.RubricIdentifier
        };

        return model;
    }

    public IEnumerable<BankQuestionMatch> ToMatch(IEnumerable<BankQuestionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public BankQuestionMatch ToMatch(BankQuestionEntity entity)
    {
        var match = new BankQuestionMatch
        {
            QuestionIdentifier = entity.QuestionIdentifier

        };

        return match;
    }
}