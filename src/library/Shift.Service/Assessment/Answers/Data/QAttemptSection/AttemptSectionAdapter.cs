using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptSectionAdapter : IEntityAdapter
{
    public void Copy(ModifyAttemptSection modify, AttemptSectionEntity entity)
    {
        entity.SectionIdentifier = modify.SectionIdentifier;
        entity.SectionStarted = modify.SectionStarted;
        entity.SectionCompleted = modify.SectionCompleted;
        entity.SectionDuration = modify.SectionDuration;
        entity.TimeLimit = modify.TimeLimit;
        entity.TimerType = modify.TimerType;
        entity.IsBreakTimer = modify.IsBreakTimer;
        entity.ShowWarningNextTab = modify.ShowWarningNextTab;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AttemptSectionEntity ToEntity(CreateAttemptSection create)
    {
        var entity = new AttemptSectionEntity
        {
            AttemptIdentifier = create.AttemptIdentifier,
            SectionIndex = create.SectionIndex,
            SectionIdentifier = create.SectionIdentifier,
            SectionStarted = create.SectionStarted,
            SectionCompleted = create.SectionCompleted,
            SectionDuration = create.SectionDuration,
            TimeLimit = create.TimeLimit,
            TimerType = create.TimerType,
            IsBreakTimer = create.IsBreakTimer,
            ShowWarningNextTab = create.ShowWarningNextTab
        };
        return entity;
    }

    public IEnumerable<AttemptSectionModel> ToModel(IEnumerable<AttemptSectionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AttemptSectionModel ToModel(AttemptSectionEntity entity)
    {
        var model = new AttemptSectionModel
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            SectionIndex = entity.SectionIndex,
            SectionIdentifier = entity.SectionIdentifier,
            SectionStarted = entity.SectionStarted,
            SectionCompleted = entity.SectionCompleted,
            SectionDuration = entity.SectionDuration,
            TimeLimit = entity.TimeLimit,
            TimerType = entity.TimerType,
            IsBreakTimer = entity.IsBreakTimer,
            ShowWarningNextTab = entity.ShowWarningNextTab
        };

        return model;
    }

    public IEnumerable<AttemptSectionMatch> ToMatch(IEnumerable<AttemptSectionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AttemptSectionMatch ToMatch(AttemptSectionEntity entity)
    {
        var match = new AttemptSectionMatch
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            SectionIndex = entity.SectionIndex

        };

        return match;
    }
}