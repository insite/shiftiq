using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationTimerAdapter : IEntityAdapter
{
    public void Copy(ModifyRegistrationTimer modify, RegistrationTimerEntity entity)
    {
        entity.RegistrationIdentifier = modify.RegistrationIdentifier;
        entity.TimerDescription = modify.TimerDescription;
        entity.TimerStatus = modify.TimerStatus;
        entity.TriggerTime = modify.TriggerTime;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public RegistrationTimerEntity ToEntity(CreateRegistrationTimer create)
    {
        var entity = new RegistrationTimerEntity
        {
            RegistrationIdentifier = create.RegistrationIdentifier,
            TimerDescription = create.TimerDescription,
            TimerStatus = create.TimerStatus,
            TriggerCommand = create.TriggerCommand,
            TriggerTime = create.TriggerTime,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<RegistrationTimerModel> ToModel(IEnumerable<RegistrationTimerEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public RegistrationTimerModel ToModel(RegistrationTimerEntity entity)
    {
        var model = new RegistrationTimerModel
        {
            RegistrationIdentifier = entity.RegistrationIdentifier,
            TimerDescription = entity.TimerDescription,
            TimerStatus = entity.TimerStatus,
            TriggerCommand = entity.TriggerCommand,
            TriggerTime = entity.TriggerTime,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<RegistrationTimerMatch> ToMatch(IEnumerable<RegistrationTimerEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public RegistrationTimerMatch ToMatch(RegistrationTimerEntity entity)
    {
        var match = new RegistrationTimerMatch
        {
            TriggerCommand = entity.TriggerCommand

        };

        return match;
    }
}