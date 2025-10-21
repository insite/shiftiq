using Shift.Common;


namespace InSite.Common.Web.UI
{
    public class MyCoursesComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("Enrolled", "Enrolled");
            list.Add("Completed", "Completed");

            return list;
        }
    }
}