using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankAdapter : IEntityAdapter
{
    public void Copy(ModifyBank modify, BankEntity entity)
    {
        entity.AssetNumber = modify.AssetNumber;
        entity.AttachmentCount = modify.AttachmentCount;
        entity.BankLevel = modify.BankLevel;
        entity.BankName = modify.BankName;
        entity.BankSize = modify.BankSize;
        entity.BankStatus = modify.BankStatus;
        entity.BankTitle = modify.BankTitle;
        entity.BankType = modify.BankType;
        entity.BankEdition = modify.BankEdition;
        entity.CommentCount = modify.CommentCount;
        entity.FormCount = modify.FormCount;
        entity.OptionCount = modify.OptionCount;
        entity.QuestionCount = modify.QuestionCount;
        entity.SetCount = modify.SetCount;
        entity.SpecCount = modify.SpecCount;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.FrameworkIdentifier = modify.FrameworkIdentifier;
        entity.DepartmentIdentifier = modify.DepartmentIdentifier;
        entity.IsActive = modify.IsActive;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public BankEntity ToEntity(CreateBank create)
    {
        var entity = new BankEntity
        {
            AssetNumber = create.AssetNumber,
            AttachmentCount = create.AttachmentCount,
            BankIdentifier = create.BankIdentifier,
            BankLevel = create.BankLevel,
            BankName = create.BankName,
            BankSize = create.BankSize,
            BankStatus = create.BankStatus,
            BankTitle = create.BankTitle,
            BankType = create.BankType,
            BankEdition = create.BankEdition,
            CommentCount = create.CommentCount,
            FormCount = create.FormCount,
            OptionCount = create.OptionCount,
            QuestionCount = create.QuestionCount,
            SetCount = create.SetCount,
            SpecCount = create.SpecCount,
            OrganizationIdentifier = create.OrganizationIdentifier,
            FrameworkIdentifier = create.FrameworkIdentifier,
            DepartmentIdentifier = create.DepartmentIdentifier,
            IsActive = create.IsActive,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser
        };
        return entity;
    }

    public IEnumerable<BankModel> ToModel(IEnumerable<BankEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public BankModel ToModel(BankEntity entity)
    {
        var model = new BankModel
        {
            AssetNumber = entity.AssetNumber,
            AttachmentCount = entity.AttachmentCount,
            BankIdentifier = entity.BankIdentifier,
            BankLevel = entity.BankLevel,
            BankName = entity.BankName,
            BankSize = entity.BankSize,
            BankStatus = entity.BankStatus,
            BankTitle = entity.BankTitle,
            BankType = entity.BankType,
            BankEdition = entity.BankEdition,
            CommentCount = entity.CommentCount,
            FormCount = entity.FormCount,
            OptionCount = entity.OptionCount,
            QuestionCount = entity.QuestionCount,
            SetCount = entity.SetCount,
            SpecCount = entity.SpecCount,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            FrameworkIdentifier = entity.FrameworkIdentifier,
            DepartmentIdentifier = entity.DepartmentIdentifier,
            IsActive = entity.IsActive,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser
        };

        return model;
    }

    public IEnumerable<BankMatch> ToMatch(IEnumerable<BankEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public BankMatch ToMatch(BankEntity entity)
    {
        var match = new BankMatch
        {
            BankIdentifier = entity.BankIdentifier

        };

        return match;
    }
}