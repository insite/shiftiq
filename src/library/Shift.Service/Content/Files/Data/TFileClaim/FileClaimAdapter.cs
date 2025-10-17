using System.Reflection;

using Newtonsoft.Json;

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

    public string Serialize(IEnumerable<FileClaimModel> models, string format)
    {
        var content = string.Empty;

        if (format.ToLower() == "csv")
        {
            var csv = new CsvExportHelper(models);

            var properties = typeof(FileClaimModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name);

            foreach (var property in properties)
                csv.AddMapping(property, property);

            content = csv.GetString();
        }
        else // The default export file format is JSON.
        {
            content = JsonConvert.SerializeObject(models, Formatting.Indented);
        }

        return content;
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