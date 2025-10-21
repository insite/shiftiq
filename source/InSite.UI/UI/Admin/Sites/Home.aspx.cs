using System;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Sites.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Sites
{
    public partial class Home : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
                BindModelToControls();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var insite = CurrentSessionState.Identity.IsOperator;

            var counts = ServiceLocator.SiteSearch.SelectCount(Organization.OrganizationIdentifier);
            var hasCounts = counts.Length > 0;
            var sum = counts.Sum(x => x.Count);

            var list = counts.Select(x => new Counter
            {
                Name = x.Name,
                Value = x.Count,
                Icon = SiteHelper.GetIconCssClass(x.Name),
                Url = SiteHelper.GetSearchUrl(x.Name, true)
            }).ToList();

            foreach (var item in list)
                if (item.Value != 1)
                    item.Name = item.Name.Pluralize();

            SiteRepeater.DataSource = list.Where(x => x.Name.StartsWith("Site"));
            SiteRepeater.DataBind();

            PageRepeater.DataSource = list.Where(x => !x.Name.StartsWith("Site"));
            PageRepeater.DataBind();

            HistoryPanel.Visible = hasCounts;

            RecentPages.LoadData(7);
        }
    }
}