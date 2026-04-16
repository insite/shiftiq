using Shift.Contract;
using Shift.Service.Competency;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class WorkshopStandardCreator(StandardReader standardReader)
{
    public async Task CreateAsync(Guid[] standardIds, List<WorkshopStandard> list)
    {
        if (standardIds.Length == 0)
            return;

        var criteria = new SearchStandards { StandardIds = standardIds };
        criteria.Filter.Page = 0;

        var entities = await standardReader.SearchAsync(criteria);
        foreach (var entity in entities)
            list.Add(CreateModel(entity));
    }

    public async Task CreateByParentAsync(Guid[] parentStandardIds, List<WorkshopStandard> list)
    {
        if (parentStandardIds.Length == 0)
            return;

        var criteria = new SearchStandards { ParentStandardIds = parentStandardIds };
        criteria.Filter.Page = 0;

        var entities = await standardReader.SearchAsync(criteria);
        foreach (var entity in entities)
            list.Add(CreateModel(entity));
    }

    private static WorkshopStandard CreateModel(StandardMatch entity)
    {
        return new WorkshopStandard
        {
            StandardId = entity.Id,
            ParentId = entity.ParentId,
            AssetNumber = entity.AssetNumber,
            Sequence = entity.Sequence,
            Code = entity.Code ?? "",
            Label = entity.Label ?? entity.Type,
            Title = entity.Title,
        };
    }
}
