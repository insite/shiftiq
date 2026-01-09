using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class EmployeeProfileStatusSelector : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("In Training");
            list.Add("Required for Promotion");

            return list;
        }
    }
}
