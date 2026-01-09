namespace Shift.Service.Content;

using Shift.Contract;

using Shift.Common;

public class TInputAdapter : IEntityAdapter
{
    public void Copy(ModifyInput modify, TInputEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.ContainerIdentifier = modify.ContainerIdentifier;
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
            OrganizationIdentifier = create.OrganizationIdentifier,
            ContainerIdentifier = create.ContainerIdentifier,
            ContentLabel = create.ContentLabel,
            ContentLanguage = create.ContentLanguage,
            ContentSnip = create.ContentSnip,
            ContentText = create.ContentText,
            ContentHtml = create.ContentHtml,
            ContainerType = create.ContainerType,
            ContentSequence = create.ContentSequence,
            ContentIdentifier = create.ContentIdentifier,
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
            OrganizationIdentifier = entity.OrganizationIdentifier,
            ContainerIdentifier = entity.ContainerIdentifier,
            ContentLabel = entity.ContentLabel,
            ContentLanguage = entity.ContentLanguage,
            ContentSnip = entity.ContentSnip,
            ContentText = entity.ContentText,
            ContentHtml = entity.ContentHtml,
            ContainerType = entity.ContainerType,
            ContentSequence = entity.ContentSequence,
            ContentIdentifier = entity.ContentIdentifier,
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
            ContentIdentifier = entity.ContentIdentifier

        };

        return match;
    }
}