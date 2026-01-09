using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Specifications.Controls
{
    public class QBankSpecificationItem : QBankSpecification
    {
        public string BankName { get; set; }
        public string BankEdition { get; set; }
        public bool IsActive { get; set; }
    }

    public partial class SearchResults : SearchResultsGridViewController<QBankSpecificationFilter>
    {
        private static readonly MemoryCache<Guid, QBank> _cache = new MemoryCache<Guid, QBank>();

        protected override int SelectCount(QBankSpecificationFilter filter)
        {
            return ServiceLocator.BankSearch.Count(filter);
        }

        protected override IListSource SelectData(QBankSpecificationFilter filter)
        {
            var list = new List<QBankSpecificationItem>();
            var specs = ServiceLocator.BankSearch.Get(filter);

            foreach (var spec in specs)
            {
                var bank = GetBank(spec.BankIdentifier);
                var item = new QBankSpecificationItem
                {
                    BankEdition = bank.BankEdition,
                    IsActive = bank.IsActive,
                    BankIdentifier = bank.BankIdentifier,
                    BankName = bank.BankName,

                    SpecAsset = spec.SpecAsset,
                    SpecConsequence = spec.SpecConsequence,
                    SpecFormCount = spec.SpecFormCount,
                    SpecFormLimit = spec.SpecFormLimit,
                    SpecIdentifier = spec.SpecIdentifier,
                    SpecName = spec.SpecName,
                    SpecQuestionLimit = spec.SpecQuestionLimit,
                    SpecType = spec.SpecType,

                    CalcPassingScore = spec.CalcPassingScore,
                    CriterionPivotCount = spec.CriterionPivotCount,
                    CriterionTagCount = spec.CriterionTagCount
                };
                list.Add(item);
            }

            return list.ToSearchResult();
        }

        private QBank GetBank(Guid bank)
        {
            if (!_cache.Exists(bank))
                _cache.Add(bank, ServiceLocator.BankSearch.GetBank(bank), 60);
            return _cache[bank];
        }
    }
}