using InSite.Domain.Banks;

using Shift.Contract;
using Shift.Service.Competency;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class SpecWorkshopStandardCreator(StandardReader standardReader)
{
    public async Task<WorkshopStandard[]> CreateAsync(Specification spec)
    {
        if (spec.Bank.Standard == Guid.Empty)
            return [];

        var creator = new WorkshopStandardCreator(standardReader);
        var standards = new List<WorkshopStandard>();

        await creator.CreateAsync([spec.Bank.Standard], standards);
        if (standards.Count == 0)
            return [];

        standards[0].ParentId = null;

        var areaIds = spec.Criteria
            .SelectMany(x => x.Sets.Where(y => y.Standard != Guid.Empty).Select(y => y.Standard))
            .Distinct()
            .ToArray();

        await creator.CreateAsync(areaIds, standards);
        await creator.CreateByParentAsync(areaIds, standards);

        return standards.ToArray();
    }
}