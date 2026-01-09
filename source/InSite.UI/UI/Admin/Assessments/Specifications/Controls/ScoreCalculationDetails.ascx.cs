using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Specifications.Controls
{
    public partial class ScoreCalculationDetails : BaseUserControl
    {
        public void SetDefaultInputValues()
        {
            var calc = new ScoreCalculation
            {
                Disclosure = Shift.Constant.DisclosureType.Full,
                PassingScore = 0.5M
            };

            SetInputValues(calc);
        }

        public void SetInputValues(ScoreCalculation calc)
        {
            DisclosureType.SelectedValue = calc.Disclosure.GetName();
            PassingScore.ValueAsDecimal = Number.CheckRange(calc.PassingScore, 0, 1) * 100;
        }

        public void GetInputValues(ScoreCalculation calc)
        {
            calc.Disclosure = DisclosureType.SelectedValue.ToEnum<DisclosureType>();
            calc.PassingScore = Number.CheckRange(PassingScore.ValueAsDecimal.Value / 100, 0, 1);
            calc.SuccessWeight = 1M;
            calc.FailureWeight = 1M;
        }
    }
}