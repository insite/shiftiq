using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Contract;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class Breadcrumbs : UserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            LoadBreadcrumbs();
        }

        private void LoadBreadcrumbs()
        {
            var crumbs = GetCrumbs();

            var html = new StringBuilder();
            foreach (var crumb in crumbs)
            {
                var status = crumb == crumbs.Last() ? "active" : null;
                if (crumb.Href != null && status == null)
                    html.Append($"<li class='breadcrumb-item'><a href='{crumb.Href}'>{crumb.Text}</a></li>");
                else
                    html.Append($"<li class='breadcrumb-item {status}'>{crumb.Text}</li>");
            }

            BreadcrumbList.InnerHtml = html.ToString();
        }

        public void Add(BreadcrumbItem item)
            => Items.Add(item);

        public List<BreadcrumbItem> Items
            => GetCrumbs();

        private List<BreadcrumbItem> GetCrumbs()
        {
            List<BreadcrumbItem> crumbs = null;
            if (Page is AdminBasePage)
                crumbs = ((AdminBasePage)Page).BreadcrumbsItems;
            if (crumbs == null && Page is PortalBasePage)
                crumbs = ((PortalBasePage)Page).BreadcrumbsItems;
            if (crumbs == null)
                return new List<BreadcrumbItem>();
            return crumbs;
        }

        public HtmlGenericControl ListControl
            => BreadcrumbList;
    }
}