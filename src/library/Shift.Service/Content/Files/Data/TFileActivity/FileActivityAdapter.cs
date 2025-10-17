using System.Reflection;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileActivityAdapter : IEntityAdapter
{
    public void Copy(ModifyFileActivity modify, FileActivityEntity entity)
    {
        entity.FileIdentifier = modify.FileIdentifier;
        entity.UserIdentifier = modify.UserIdentifier;
        entity.ActivityTime = modify.ActivityTime;
        entity.ActivityChanges = modify.ActivityChanges;

    }

    public string Serialize(IEnumerable<FileActivityModel> models, string format)
    {
        var content = string.Empty;

        if (format.ToLower() == "csv")
        {
            var csv = new CsvExportHelper(models);

            var properties = typeof(FileActivityModel)
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

    public FileActivityEntity ToEntity(CreateFileActivity create)
    {
        var entity = new FileActivityEntity
        {
            FileIdentifier = create.FileIdentifier,
            UserIdentifier = create.UserIdentifier,
            ActivityIdentifier = create.ActivityIdentifier,
            ActivityTime = create.ActivityTime,
            ActivityChanges = create.ActivityChanges
        };
        return entity;
    }

    public IEnumerable<FileActivityModel> ToModel(IEnumerable<FileActivityEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FileActivityModel ToModel(FileActivityEntity entity)
    {
        var model = new FileActivityModel
        {
            FileIdentifier = entity.FileIdentifier,
            UserIdentifier = entity.UserIdentifier,
            ActivityIdentifier = entity.ActivityIdentifier,
            ActivityTime = entity.ActivityTime,
            ActivityChanges = entity.ActivityChanges
        };

        return model;
    }

    public IEnumerable<FileActivityMatch> ToMatch(IEnumerable<FileActivityEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FileActivityMatch ToMatch(FileActivityEntity entity)
    {
        var match = new FileActivityMatch
        {
            ActivityIdentifier = entity.ActivityIdentifier

        };

        return match;
    }
}