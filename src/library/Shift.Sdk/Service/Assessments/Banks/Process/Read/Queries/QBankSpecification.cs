using System;
using System.Collections.Generic;
using System.Text;

using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Application.Banks.Read
{
    public class QBankSpecification
    {
        public Guid BankIdentifier { get; set; }
        public string CalcDisclosure { get; set; }
        public decimal CalcPassingScore { get; set; }
        public int? CriterionAllCount { get; set; }
        public int? CriterionTagCount { get; set; }
        public int? CriterionPivotCount { get; set; }
        public int SpecAsset { get; set; }
        public string SpecConsequence { get; set; }
        public int SpecFormCount { get; set; }
        public int SpecFormLimit { get; set; }
        public Guid SpecIdentifier { get; set; }
        public string SpecName { get; set; }
        public int SpecQuestionLimit { get; set; }
        public string SpecType { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual QBank Bank { get; set; }

        public virtual ICollection<QBankForm> Forms { get; set; } = new HashSet<QBankForm>();

        public string CalcDisclosureHtml
        {
            get
            {
                if (CalcDisclosure == "None")
                    return "No Disclosure";

                return CalcDisclosure + " Disclosure";
            }
        }

        public string SetCountHtml
        {
            get
            {
                var sb = new StringBuilder();

                if (CriterionAllCount.HasValue)
                    sb.Append(ShiftHumanizer.ToQuantity(CriterionAllCount.Value, "criterion"));

                return sb.ToString();
            }
        }

        public string CriterionCountHtml
        {
            get
            {
                if (CriterionTagCount.HasValue)
                    return ShiftHumanizer.ToQuantity(CriterionTagCount.Value, "Tag");

                else if (CriterionPivotCount.HasValue)
                    return ShiftHumanizer.ToQuantity(CriterionPivotCount.Value, "Pivot");

                return null;
            }
        }

        public string SpecFormLimitHtml
        {
            get
            {
                return ShiftHumanizer.ToQuantity(SpecFormLimit, "Form");
            }
        }
    }
}
