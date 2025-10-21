using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Logs.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Assessments.Reports.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void BindModelToControls(int count)
        {
            MostRecentChange[] changes;
            if (Organization.OrganizationIdentifier == OrganizationIdentifiers.SkilledTradesBC)
                changes = ServiceLocator.BankSearch.GetMostRecentlyChangedBanks(Organization.OrganizationIdentifier, count, "b.LastChangeType != 'BankCommentPosted'");
            else
                changes = ServiceLocator.BankSearch.GetMostRecentlyChangedBanks(Organization.OrganizationIdentifier, count);
            BankRepeater.DataSource = changes;
            BankRepeater.DataBind();
            ItemCount = changes.Length;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BankRepeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (MostRecentChange)e.Item.DataItem;
            var lastChange = (ITextControl)e.Item.FindControl("LastChange");
            lastChange.Text = UserSearch.GetTimestampHtml(item.LastChangeUser, item.LastChangeType, null, item.LastChangeTime);
        }
    }
}