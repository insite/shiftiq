namespace Shift.Service.Content;

using Shift.Contract;

using Shift.Common;

public class TInputAdapter : IEntityAdapter
{
    public void Copy(ModifyInput modify, TInputEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.ContainerIdentifier = modify.ContainerId;
        entity.ContentLabel = modify.ContentLabel;
        entity.ContentLanguage = modify.ContentLanguage;
        entity.ContentSnip = modify.ContentSnip;
        entity.ContentText = modify.ContentText;
        entity.ContentHtml = modify.ContentHtml;
        entity.ContainerType = modify.ContainerType;
        entity.ContentSequence = modify.ContentSequence;
        entity.ReferenceFiles = modify.ReferenceFiles;
        entity.ReferenceCount = modify.ReferenceCount;

    }

    public TInputEntity ToEntity(CreateInput create)
    {
        var entity = new TInputEntity
        {
            OrganizationIdentifier = create.OrganizationId,
            ContainerIdentifier = create.ContainerId,
            ContentLabel = create.ContentLabel,
            ContentLanguage = create.ContentLanguage,
            ContentSnip = create.ContentSnip,
            ContentText = create.ContentText,
            ContentHtml = create.ContentHtml,
            ContainerType = create.ContainerType,
            ContentSequence = create.ContentSequence,
            ContentIdentifier = create.ContentId,
            ReferenceFiles = create.ReferenceFiles,
            ReferenceCount = create.ReferenceCount
        };
        return entity;
    }

    public IEnumerable<InputModel> ToModel(IEnumerable<TInputEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public InputModel ToModel(TInputEntity entity)
    {
        var model = new InputModel
        {
            OrganizationId = entity.OrganizationIdentifier,
            ContainerId = entity.ContainerIdentifier,
            ContentLabel = entity.ContentLabel,
            ContentLanguage = entity.ContentLanguage,
            ContentSnip = entity.ContentSnip,
            ContentText = entity.ContentText,
            ContentHtml = entity.ContentHtml,
            ContainerType = entity.ContainerType,
            ContentSequence = entity.ContentSequence,
            ContentId = entity.ContentIdentifier,
            ReferenceFiles = entity.ReferenceFiles,
            ReferenceCount = entity.ReferenceCount
        };

        return model;
    }

    public IEnumerable<InputMatch> ToMatch(IEnumerable<TInputEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public InputMatch ToMatch(TInputEntity entity)
    {
        var match = new InputMatch
        {
            ContentId = entity.ContentIdentifier

        };

        return match;
    }
}