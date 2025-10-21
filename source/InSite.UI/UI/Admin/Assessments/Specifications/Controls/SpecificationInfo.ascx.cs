using InSite.Domain.Banks;

using Shift.Constant;

namespace InSite.Admin.Assessments.Specifications.Controls
{
    public partial class SpecificationInfo : System.Web.UI.UserControl
    {
        public void BindSpec(Specification spec, bool showName = true, bool showConf = true, bool showCalc = true)
        {
            SpecificationName.Text = spec.Name;
            SpecificationType.Text = spec.Type.GetName();
            NameDiv.Visible = TypeDiv.Visible = showName;

            SpecificationFormLimit.Text = spec.FormLimit.ToString("n0");
            SpecificationQuestionLimit.Text = spec.QuestionLimit.ToString("n0");
            ConfDiv.Visible = showConf;
            SpecificationCalculationDisclosure.Text = $"{spec.Calculation.Disclosure.GetName()}";
            SpecificationCalculationPassingScore.Text = $"{spec.Calculation.PassingScore:p0}";
            CalcDiv.Visible = showCalc;
        }
    }
}