using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class EventFormatComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource() => EventExamFormat.GetDataSource();
    }
}