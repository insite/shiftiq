using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class ProfileVisibilitySelector : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(AccountScopes.Enterprise, "Global");

            list.Add(AccountScopes.Organization, "Organization-Specific");

            return list;
        }
    }
}
