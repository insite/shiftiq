using System;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Talent.Employee.Competency.Validation
{
    public partial class Summary : AdminBasePage, ICmdsUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SummaryGrid.DataBinding += Summary_DataBinding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            int rowCount = UserCompetencyRepository.SelectItemCountForValidator(User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, null, null).Rows.Count;

            SummaryGrid.Visible = rowCount > 0;
            NoItems.Visible = rowCount == 0;

            SummaryGrid.VirtualItemCount = rowCount;
        }

        private void Summary_DataBinding(object sender, EventArgs e)
        {
            var table = UserCompetencyRepository.SelectItemCountForValidator(User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, SummaryGrid.PageIndex, SummaryGrid.PageSize);

            SummaryGrid.DataSource = table;
        }

        protected static string GetCompetencyLink(Object personID)
        {
            return string.Format("/ui/cmds/design/validations/competencies/search?userID={0}", personID);
        }

    }
}
