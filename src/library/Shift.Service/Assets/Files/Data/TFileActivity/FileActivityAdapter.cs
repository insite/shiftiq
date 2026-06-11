using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileActivityAdapter : IEntityAdapter
{
    public void Copy(ModifyFileActivity modify, FileActivityEntity entity)
    {
        entity.FileIdentifier = modify.FileId;
        entity.UserIdentifier = modify.UserId;
        entity.ActivityTime = modify.ActivityTime;
        entity.ActivityChanges = modify.ActivityChanges;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FileActivityEntity ToEntity(CreateFileActivity create)
    {
        var entity = new FileActivityEntity
        {
            FileIdentifier = create.FileId,
            UserIdentifier = create.UserId,
            ActivityIdentifier = create.ActivityId,
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
            FileId = entity.FileIdentifier,
            UserId = entity.UserIdentifier,
            ActivityId = entity.ActivityIdentifier,
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
            ActivityId = entity.ActivityIdentifier

        };

        return match;
    }
}