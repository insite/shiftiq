using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class BooleanComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        public string TrueText 
        {
            get => (string)ViewState[nameof(TrueText)];
            set => ViewState[nameof(TrueText)] = value;
        }

        public string FalseText
        {
            get => (string)ViewState[nameof(FalseText)];
            set => ViewState[nameof(FalseText)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(bool.TrueString, string.IsNullOrEmpty(TrueText) ? "Yes" : TrueText);
            list.Add(bool.FalseString, string.IsNullOrEmpty(FalseText) ? "No" : FalseText);

            return list;
        }
    }
}