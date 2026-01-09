using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class AttendanceStatusComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            list.Add("Pending");
            list.Add("Present");
            list.Add("Absent");
            return list;
        }
    }
}