using InSite.Domain.Banks;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class SpecWorkshopDetailsCreator
{
    private enum FilterKeysType { CompetencyTaxonomy, TaxonomyCompetency, Empty };

    private class BankViewCompetency
    {
        public int? QuestionCount { get; set; }
        public int Tax1CountActual { get; set; }
        public int Tax2CountActual { get; set; }
        public int Tax3CountActual { get; set; }
        public int? UnassignedCount { get; set; }
    }

    private Dictionary<Guid, List<WorkshopStandard>> _areaCompetencies = default!;
    private Dictionary<Guid, WorkshopStandard> _standards = default!;

    public SpecWorkshop.SpecDetails CreateAsync(Specification spec, WorkshopStandard[] standards)
    {
        if (standards.Length == 0)
        {
            _areaCompetencies = new Dictionary<Guid, List<WorkshopStandard>>();
            _standards = new Dictionary<Guid, WorkshopStandard>();
        }
        else
        {
            var frameworkId = standards[0].StandardId;
            var areaIds = standards.Where(x => x.ParentId == frameworkId).Select(x => x.StandardId).ToList();

            _areaCompetencies = areaIds
                .Select(areaId => new
                {
                    AreaId = areaId,
                    Standards = standards.Where(y => y.ParentId == areaId).ToList()
                })
                .ToDictionary(x => x.AreaId, x => x.Standards);

            _standards = standards.ToDictionary(x => x.StandardId);
        }

        return new SpecWorkshop.SpecDetails
        {
            SpecName = spec.Name,
            AssetNumber = spec.Asset,
            FrameworkId = spec.Bank.Standard != Guid.Empty ? spec.Bank.Standard : null,
            FormLimit = spec.FormLimit,
            QuestionLimit = spec.QuestionLimit,
            Criteria = CreateCriteria(spec)
        };
    }

    private SpecWorkshop.SpecDetails.DetailsCriterion[] CreateCriteria(Specification spec)
    {
        var result = new List<SpecWorkshop.SpecDetails.DetailsCriterion>();

        foreach (var criterion in spec.Criteria)
        {
            result.Add(new SpecWorkshop.SpecDetails.DetailsCriterion
            {
                CriterionId = criterion.Identifier,
                Title = criterion.Title,
                Weight = (int)(criterion.SetWeight * 10000),
                StandardIds = criterion.Sets.Where(y => y.Standard != Guid.Empty).Select(y => y.Standard).Distinct().ToArray(),
                Competencies = CreateCompetencies(criterion)
            });
        }

        return result.ToArray();
    }

    private SpecWorkshop.SpecDetails.DetailsCompetency[] CreateCompetencies(Criterion criterion)
    {
        var result = new List<SpecWorkshop.SpecDetails.DetailsCompetency>();

        foreach (var set in criterion.Sets)
        {
            if (!_areaCompetencies.TryGetValue(set.Standard, out var competencies))
                continue;

            foreach (var competency in competencies)
                result.Add(CreateCompetency(criterion, set, competency));
        }

        result.Sort((a, b) =>
        {
            var standardA = _standards[a.StandardId];
            var standardB = _standards[b.StandardId];
            var cmp = standardA.Sequence.CompareTo(standardB.Sequence);
            if (cmp != 0)
                return cmp;

            var titleA = $"{_standards[standardA.ParentId!.Value].Label} {standardA.Label}. {standardA.Title}";
            var titleB = $"{_standards[standardB.ParentId!.Value].Label} {standardB.Label}. {standardB.Title}";

            return titleA.CompareTo(titleB);
        });

        return result.ToArray();
    }

    private static SpecWorkshop.SpecDetails.DetailsCompetency CreateCompetency(Criterion criterion, Set set, WorkshopStandard competency)
    {
        var bankViewCompetency = CreateBankViewCompetency(set, competency.StandardId);

        var hasFilterDimensions = criterion.PivotFilter?.IsEmpty == false;
        FilterKeysType filterKeys;

        if (hasFilterDimensions && criterion.PivotFilter!.RowDimensions[0].Name == "Competency" && criterion.PivotFilter.ColumnDimensions[0].Name == "Taxonomy")
            filterKeys = FilterKeysType.CompetencyTaxonomy;
        else if (hasFilterDimensions && criterion.PivotFilter!.RowDimensions[0].Name == "Taxonomy" && criterion.PivotFilter.ColumnDimensions[0].Name == "Competency")
            filterKeys = FilterKeysType.TaxonomyCompetency;
        else
            filterKeys = FilterKeysType.Empty;

        return new SpecWorkshop.SpecDetails.DetailsCompetency
        {
            StandardId = competency.StandardId,
            Tax1Count = GetPivotTableValue(criterion, filterKeys, competency.AssetNumber, 1),
            Tax2Count = GetPivotTableValue(criterion, filterKeys, competency.AssetNumber, 2),
            Tax3Count = GetPivotTableValue(criterion, filterKeys, competency.AssetNumber, 3),
            QuestionCount = bankViewCompetency.QuestionCount,
            Tax1CountActual = bankViewCompetency.Tax1CountActual,
            Tax2CountActual = bankViewCompetency.Tax2CountActual,
            Tax3CountActual = bankViewCompetency.Tax3CountActual,
            UnassignedCount = bankViewCompetency.UnassignedCount,
        };
    }

    private static int? GetPivotTableValue(Criterion criterion, FilterKeysType filterKeys, int? competencyNumber, int taxonomyNumber)
    {
        if (competencyNumber == null || criterion.PivotFilter == null || criterion.PivotFilter.IsEmpty)
            return null;

        MultiKey<string>? key1, key2;
        switch (filterKeys)
        {
            case FilterKeysType.CompetencyTaxonomy:
                (key1, key2) = (new MultiKey<string>(competencyNumber.Value.ToString()), new MultiKey<string>(taxonomyNumber.ToString()));
                break;
            case FilterKeysType.TaxonomyCompetency:
                (key1, key2) = (new MultiKey<string>(taxonomyNumber.ToString()), new MultiKey<string>(competencyNumber.Value.ToString()));
                break;
            default:
                (key1, key2) = (null, null);
                break;
        }

        if (!criterion.PivotFilter.RowDimensions.IsValidKey(key1) || !criterion.PivotFilter.ColumnDimensions.IsValidKey(key2))
            return null;

        var value = criterion.PivotFilter.GetCellValue(key1, key2);

        return value == 0 ? null : value;
    }

    private static BankViewCompetency CreateBankViewCompetency(Set set, Guid standardId)
    {
        int total = 0, tax1 = 0, tax2 = 0, tax3 = 0, unassigned = 0;

        foreach (var q in set.Questions.Where(y => y.Standard == standardId))
        {
            if (q.Condition == "New" || q.Condition == "Edit" || q.Condition == "Copy")
            {
                total++;

                if (q.Classification != null)
                {
                    if (q.Classification.Taxonomy == 1)
                        tax1++;

                    if (q.Classification.Taxonomy == 2)
                        tax2++;

                    if (q.Classification.Taxonomy == 3)
                        tax3++;
                }
            }
            else if (q.Condition == "Unassigned")
            {
                unassigned++;
            }
        }

        return new BankViewCompetency
        {
            QuestionCount = total == 0 ? (int?)null : total,
            Tax1CountActual = tax1,
            Tax2CountActual = tax2,
            Tax3CountActual = tax3,
            UnassignedCount = unassigned == 0 ? (int?)null : unassigned
        };
    }
}
