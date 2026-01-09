using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class AchievementVisibilitySelector : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        public bool IsGlobalItemVisible
        {
            get => ViewState[nameof(IsGlobalItemVisible)] == null || (bool)ViewState[nameof(IsGlobalItemVisible)];
            set => ViewState[nameof(IsGlobalItemVisible)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            if (IsGlobalItemVisible)
                list.Add(AccountScopes.Enterprise);

            list.Add(AccountScopes.Organization);

            return list;
        }
    }
}