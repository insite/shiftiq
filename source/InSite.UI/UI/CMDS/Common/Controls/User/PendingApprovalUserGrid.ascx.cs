using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Cmds.Controls.User
{
    public partial class PendingApprovalUserGrid : BaseUserControl
    {
        public void LoadData()
        {
            var users = MembershipSearch.SelectPendingApprovalUsers();
            var hasData = users.Count > 0;

            Repeater.Visible = hasData;
            NoDataMessage.Visible = !hasData;

            Repeater.DataSource = users;
            Repeater.DataBind();
        }
    }
}
