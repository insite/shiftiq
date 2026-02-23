using System;
using System.Linq;

using InSite.Domain.Banks;

using Shift.Constant;

namespace InSite.Persistence
{
    public class SpecCalculator
    {
        private BankState _bank { get; set; }

        public SpecCalculator(BankState bank)
        {
            _bank = bank;
        }

        public int CountPivots(Guid specId)
        {
            var spec = _bank.Specifications.Single(x => x.Identifier == specId);
            return spec.Criteria.Count(x => x.FilterType == CriterionFilterType.Pivot);
        }

        public int CountCriteria(Guid specId)
        {
            var spec = _bank.Specifications.Single(x => x.Identifier == specId);
            var pools = _bank.Sets.SelectMany(x => x.Criteria);
            return spec.Criteria.Intersect(pools).Count();
        }

        public int CountTags(Guid specId)
        {
            var spec = _bank.Specifications.Single(x => x.Identifier == specId);
            return spec.Criteria.Count(x => x.FilterType == CriterionFilterType.Tag);
        }
    }
}
