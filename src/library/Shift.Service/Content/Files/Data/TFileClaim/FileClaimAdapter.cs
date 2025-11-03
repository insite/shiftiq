using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileClaimAdapter : IEntityAdapter
{
    public void Copy(ModifyFileClaim modify, FileClaimEntity entity)
    {
        entity.FileIdentifier = modify.FileIdentifier;
        entity.ObjectType = modify.ObjectType;
        entity.ObjectIdentifier = modify.ObjectIdentifier;
        entity.ClaimGranted = modify.ClaimGranted;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FileClaimEntity ToEntity(CreateFileClaim create)
    {
        var entity = new FileClaimEntity
        {
            FileIdentifier = create.FileIdentifier,
            ClaimIdentifier = create.ClaimIdentifier,
            ObjectType = create.ObjectType,
            ObjectIdentifier = create.ObjectIdentifier,
            ClaimGranted = create.ClaimGranted
        };
        return entity;
    }

    public IEnumerable<FileClaimModel> ToModel(IEnumerable<FileClaimEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FileClaimModel ToModel(FileClaimEntity entity)
    {
        var model = new FileClaimModel
        {
            FileIdentifier = entity.FileIdentifier,
            ClaimIdentifier = entity.ClaimIdentifier,
            ObjectType = entity.ObjectType,
            ObjectIdentifier = entity.ObjectIdentifier,
            ClaimGranted = entity.ClaimGranted
        };

        return model;
    }

    public IEnumerable<FileClaimMatch> ToMatch(IEnumerable<FileClaimEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FileClaimMatch ToMatch(FileClaimEntity entity)
    {
        var match = new FileClaimMatch
        {
            ClaimIdentifier = entity.ClaimIdentifier

        };

        return match;
    }
}