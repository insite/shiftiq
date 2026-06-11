using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class PendingPersonAdapter : IEntityAdapter
{
    public void Copy(ModifyPendingPerson modify, PendingPersonEntity entity)
    {
        entity.UserFirstName = modify.UserFirstName;
        entity.UserLastName = modify.UserLastName;
        entity.UserEmail = modify.UserEmail;
        entity.UserId = modify.UserId;
        entity.PersonCode = modify.PersonCode;
        entity.PersonId = modify.PersonId;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public PendingPersonEntity ToEntity(CreatePendingPerson create)
    {
        var entity = new PendingPersonEntity
        {
            PendingId = create.PendingId,
            UserFirstName = create.UserFirstName,
            UserLastName = create.UserLastName,
            UserEmail = create.UserEmail,
            UserId = create.UserId,
            PersonCode = create.PersonCode,
            PersonId = create.PersonId
        };
        return entity;
    }

    public IEnumerable<PendingPersonModel> ToModel(IEnumerable<PendingPersonEntity> entities, TimeZoneInfo? timezone)
    {
        return entities.Select(e => ToModel(e, timezone));
    }

    public PendingPersonModel ToModel(PendingPersonEntity entity, TimeZoneInfo? timezone)
    {
        var model = new PendingPersonModel
        {
            OrganizationId = entity.OrganizationId,
            SubmittedAt = entity.SubmittedAt,
            SubmittedBy = entity.SubmittedBy,
            PendingId = entity.PendingId,
            UserFirstName = entity.UserFirstName,
            UserLastName = entity.UserLastName,
            UserEmail = entity.UserEmail,
            UserId = entity.UserId,
            PersonCode = entity.PersonCode,
            PersonId = entity.PersonId
        };

        return model;
    }
}
