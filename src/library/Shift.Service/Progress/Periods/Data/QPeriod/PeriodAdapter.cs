using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class PeriodAdapter : IEntityAdapter
{
    public void Copy(ModifyPeriod modify, PeriodEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.PeriodName = modify.PeriodName;
        entity.PeriodStart = modify.PeriodStart;
        entity.PeriodEnd = modify.PeriodEnd;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public PeriodEntity ToEntity(CreatePeriod create)
    {
        var entity = new PeriodEntity
        {
            PeriodIdentifier = create.PeriodIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            PeriodName = create.PeriodName,
            PeriodStart = create.PeriodStart,
            PeriodEnd = create.PeriodEnd
        };
        return entity;
    }

    public IEnumerable<PeriodModel> ToModel(IEnumerable<PeriodEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public PeriodModel ToModel(PeriodEntity entity)
    {
        var model = new PeriodModel
        {
            PeriodId = entity.PeriodIdentifier,
            PeriodName = entity.PeriodName,
            PeriodStart = entity.PeriodStart,
            PeriodEnd = entity.PeriodEnd
        };

        return model;
    }

    public IEnumerable<PeriodMatch> ToMatch(IEnumerable<PeriodEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public PeriodMatch ToMatch(PeriodEntity entity)
    {
        var match = new PeriodMatch
        {
            Id = entity.PeriodIdentifier,
            Name = entity.PeriodName
        };

        return match;
    }
}