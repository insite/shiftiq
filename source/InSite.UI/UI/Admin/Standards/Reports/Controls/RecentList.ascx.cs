using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Standards.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(int count)
        {
            var data = StandardSearch.SearchStandardsRecent("Modified DESC", Paging.SetSkipTake(0, count), Organization.OrganizationIdentifier);
            ItemCount = data.GetList().Count;

            StandardRepeater.DataSource = data;
            StandardRepeater.DataBind();
        }

        protected static string GetTimestampHtml(Guid assetId)
        {
            var asset = StandardSearch.Select(assetId);
            return UserSearch.GetTimestampHtml(asset.ModifiedBy, "standard", "changed", asset.Modified);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            StandardRepeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = e.Item.DataItem;

            var name = (string)DataBinder.Eval(item, "Name");
            var title = (string)DataBinder.Eval(item, "Title");

            var assetName = (ITextControl)e.Item.FindControl("AssetName");
            assetName.Text = name ?? title;

            var assetTitle = (HtmlControl)e.Item.FindControl("AssetTitle");
            assetTitle.Visible = name != null && name != title;
        }
    }
}