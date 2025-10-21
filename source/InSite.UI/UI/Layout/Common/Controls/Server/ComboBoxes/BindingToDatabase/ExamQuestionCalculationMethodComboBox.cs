using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class ExamQuestionCalculationMethodComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        public QuestionCalculationMethod? EnumValue
        {
            get => base.Value.IsEmpty() ? (QuestionCalculationMethod?)null : base.Value.ToEnum<QuestionCalculationMethod>();
            set => base.Value = value?.GetName();
        }

        protected override ListItemArray CreateDataSource() => ComboBoxHelper.CreateDataSource(
            QuestionCalculationMethod.AllOrNothing,
            QuestionCalculationMethod.EquallyWeighted,
            QuestionCalculationMethod.CorrectMinusIncorrect,
            QuestionCalculationMethod.LimitedCorrect);
    }
}