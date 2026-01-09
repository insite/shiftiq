using System;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Standards.Profiles
{
    public partial class PersonProfile : AdminBasePage, ICmdsUserControl
    {
        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;
        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo
            ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        public override void ApplyAccessControl()
        {
            if (!Identity.IsGranted("cmds/users/assign-profiles"))
                CreateAccessDeniedException();

            FinderSecurityInfo.LoadPermissions();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssignCompanyProfilesToEmployee.Alert += (s, a) => ScreenStatus.AddMessage(a);
            AssignSelectedProfilesToEmployees.Alert += (s, a) => ScreenStatus.AddMessage(a);
            AssignProfileToSelectedEmployees.Alert += (s, a) => ScreenStatus.AddMessage(a);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this, qualifier: Organization.CompanyName);

            AssignCompanyProfilesToEmployee.LoadData(FinderSecurityInfo);
            AssignSelectedProfilesToEmployees.LoadData(FinderSecurityInfo);
            AssignProfileToSelectedEmployees.LoadData(FinderSecurityInfo);
        }
    }
}