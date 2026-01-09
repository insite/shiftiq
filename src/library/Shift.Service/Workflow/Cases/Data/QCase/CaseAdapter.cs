using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseAdapter : IEntityAdapter
{
    public void Copy(ModifyCase modify, CaseEntity entity)
    {
        entity.CaseClosed = modify.CaseClosed;
        entity.CaseDescription = modify.CaseDescription;
        entity.CaseOpened = modify.CaseOpened;
        entity.CaseSource = modify.CaseSource;
        entity.CaseStatusCategory = modify.CaseStatusCategory;
        entity.CaseType = modify.CaseType;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.AdministratorUserIdentifier = modify.AdministratorUserIdentifier;
        entity.TopicUserIdentifier = modify.TopicUserIdentifier;
        entity.LawyerUserIdentifier = modify.LawyerUserIdentifier;
        entity.AttachmentCount = modify.AttachmentCount;
        entity.CommentCount = modify.CommentCount;
        entity.PersonCount = modify.PersonCount;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.CaseTitle = modify.CaseTitle;
        entity.CaseNumber = modify.CaseNumber;
        entity.EmployerGroupIdentifier = modify.EmployerGroupIdentifier;
        entity.GroupCount = modify.GroupCount;
        entity.CaseReported = modify.CaseReported;
        entity.CaseOpenedBy = modify.CaseOpenedBy;
        entity.CaseClosedBy = modify.CaseClosedBy;
        entity.CaseStatusIdentifier = modify.CaseStatusIdentifier;
        entity.CaseStatusEffective = modify.CaseStatusEffective;
        entity.OwnerUserIdentifier = modify.OwnerUserIdentifier;
        entity.ResponseSessionIdentifier = modify.ResponseSessionIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public CaseEntity ToEntity(CreateCase create)
    {
        var entity = new CaseEntity
        {
            CaseClosed = create.CaseClosed,
            CaseDescription = create.CaseDescription,
            CaseIdentifier = create.CaseIdentifier,
            CaseOpened = create.CaseOpened,
            CaseSource = create.CaseSource,
            CaseStatusCategory = create.CaseStatusCategory,
            CaseType = create.CaseType,
            OrganizationIdentifier = create.OrganizationIdentifier,
            AdministratorUserIdentifier = create.AdministratorUserIdentifier,
            TopicUserIdentifier = create.TopicUserIdentifier,
            LawyerUserIdentifier = create.LawyerUserIdentifier,
            AttachmentCount = create.AttachmentCount,
            CommentCount = create.CommentCount,
            PersonCount = create.PersonCount,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            CaseTitle = create.CaseTitle,
            CaseNumber = create.CaseNumber,
            EmployerGroupIdentifier = create.EmployerGroupIdentifier,
            GroupCount = create.GroupCount,
            CaseReported = create.CaseReported,
            CaseOpenedBy = create.CaseOpenedBy,
            CaseClosedBy = create.CaseClosedBy,
            CaseStatusIdentifier = create.CaseStatusIdentifier,
            CaseStatusEffective = create.CaseStatusEffective,
            OwnerUserIdentifier = create.OwnerUserIdentifier,
            ResponseSessionIdentifier = create.ResponseSessionIdentifier
        };
        return entity;
    }

    public IEnumerable<CaseModel> ToModel(IEnumerable<CaseEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public CaseModel ToModel(CaseEntity entity)
    {
        var model = new CaseModel
        {
            CaseClosed = entity.CaseClosed,
            CaseDescription = entity.CaseDescription,
            CaseIdentifier = entity.CaseIdentifier,
            CaseOpened = entity.CaseOpened,
            CaseSource = entity.CaseSource,
            CaseStatusCategory = entity.CaseStatusCategory,
            CaseType = entity.CaseType,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            AdministratorUserIdentifier = entity.AdministratorUserIdentifier,
            TopicUserIdentifier = entity.TopicUserIdentifier,
            LawyerUserIdentifier = entity.LawyerUserIdentifier,
            AttachmentCount = entity.AttachmentCount,
            CommentCount = entity.CommentCount,
            PersonCount = entity.PersonCount,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            CaseTitle = entity.CaseTitle,
            CaseNumber = entity.CaseNumber,
            EmployerGroupIdentifier = entity.EmployerGroupIdentifier,
            GroupCount = entity.GroupCount,
            CaseReported = entity.CaseReported,
            CaseOpenedBy = entity.CaseOpenedBy,
            CaseClosedBy = entity.CaseClosedBy,
            CaseStatusIdentifier = entity.CaseStatusIdentifier,
            CaseStatusEffective = entity.CaseStatusEffective,
            OwnerUserIdentifier = entity.OwnerUserIdentifier,
            ResponseSessionIdentifier = entity.ResponseSessionIdentifier
        };

        return model;
    }

    public IEnumerable<CaseMatch> ToMatch(IEnumerable<CaseEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public CaseMatch ToMatch(CaseEntity entity)
    {
        var match = new CaseMatch
        {
            CaseIdentifier = entity.CaseIdentifier

        };

        return match;
    }
}