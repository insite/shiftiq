using InSite.Application.Sites.Read;

namespace InSite.Admin.Sites.Sites.Controls
{
    public partial class SiteInfo : System.Web.UI.UserControl
    {
        public void BindSite(QSite site)
        {
            Sitelink.HRef = $"/ui/admin/sites/outline?id={site.SiteIdentifier}";
            SiteTitle.Text = site.SiteTitle ?? site.SiteDomain ?? "Untitled";
            Domain.Text = site.SiteDomain;
        }
    }
}