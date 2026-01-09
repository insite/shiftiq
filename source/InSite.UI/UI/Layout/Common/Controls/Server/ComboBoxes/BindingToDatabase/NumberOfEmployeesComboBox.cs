using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class NumberOfEmployeesComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            list.Add("");
            list.Add("Small (1-99 employees)", "Small (1-99 employees)");
            list.Add("Medium (100-499 employees)", "Medium (100-499 employees)");
            list.Add("Large (500+ employees)", "Large (500+ employees)");
            list.Add("Government", "Government");
            return list;
        }
    }
}