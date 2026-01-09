using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class EventUserAdapter : IEntityAdapter
{
    public void Copy(ModifyEventUser modify, EventUserEntity entity)
    {
        entity.AttendeeRole = modify.AttendeeRole;
        entity.Assigned = modify.Assigned;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize(IEnumerable<EventUserModel> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public EventUserEntity ToEntity(CreateEventUser create)
    {
        var entity = new EventUserEntity
        {
            EventIdentifier = create.EventIdentifier,
            UserIdentifier = create.UserIdentifier,
            AttendeeRole = create.AttendeeRole,
            Assigned = create.Assigned,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<EventUserModel> ToModel(IEnumerable<EventUserEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public EventUserModel ToModel(EventUserEntity entity)
    {
        var model = new EventUserModel
        {
            EventIdentifier = entity.EventIdentifier,
            UserIdentifier = entity.UserIdentifier,
            AttendeeRole = entity.AttendeeRole,
            Assigned = entity.Assigned,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<EventUserMatch> ToMatch(IEnumerable<EventUserEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public EventUserMatch ToMatch(EventUserEntity entity)
    {
        var match = new EventUserMatch
        {
            EventId = entity.EventIdentifier,
            EventTitle = entity.Event?.EventTitle,

            UserId = entity.UserIdentifier,
            UserName = entity.User?.FullName,

            RelationCreated = entity.Assigned,
            RelationRole = entity.AttendeeRole
        };

        return match;
    }
}