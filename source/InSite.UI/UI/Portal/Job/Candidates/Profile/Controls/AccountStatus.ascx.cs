using System.Web.UI;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class AccountStatus : UserControl
    {
        public void SetStatus(bool isApproved)
        {
            AccountStatusApproved.Visible = isApproved;
            AccountStatusNotApproved.Visible = !isApproved;
        }
    }
}
