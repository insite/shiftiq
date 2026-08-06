using System.Web.UI;

namespace InSite.UI.Portal.Learning
{
    public partial class CatalogRedirect : Page
    {
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            var query = Request.Url.Query;
            var target = "/ui/portal/learning/catalogue" + query;

            Response.RedirectPermanent(target, true);
        }
    }
}
