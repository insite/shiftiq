using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchBankSpecifications : Query<IEnumerable<BankSpecificationMatch>>, IBankSpecificationCriteria
    {
        public Guid? BankIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string CalcDisclosure { get; set; }
        public string SpecConsequence { get; set; }
        public string SpecName { get; set; }
        public string SpecType { get; set; }

        public int? CriterionAllCount { get; set; }
        public int? CriterionPivotCount { get; set; }
        public int? CriterionTagCount { get; set; }
        public int? SpecAsset { get; set; }
        public int? SpecFormCount { get; set; }
        public int? SpecFormLimit { get; set; }
        public int? SpecQuestionLimit { get; set; }

        public decimal? CalcPassingScore { get; set; }
    }
}