using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ExamTypeComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource() => EventExamType.GetDataSource();

        public string SelectedText
        {
            get
            {
                return Value == EventExamType.IndividualA.Value
                    ? EventExamType.IndividualA.Text
                    : Value == EventExamType.IndividualN.Value
                        ? EventExamType.IndividualN.Text
                        : Value;
            }
        }

        public static string GetDescription(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value == EventExamType.Class.Value)
                    return "One or more classes scheduled as they occur or predictively from Technical Training Session.";

                if (value == EventExamType.Sitting.Value)
                    return "Pre-scheduled (fixed time and place) at specific venues for multiple classes or candidates.";

                if (value.StartsWith("Individual"))
                    return "A single candidate in any location for one exam (e.g. challenger, rewrite).";
            }

            return "(Please provide a brief description for this exam event type)";
        }
    }
}