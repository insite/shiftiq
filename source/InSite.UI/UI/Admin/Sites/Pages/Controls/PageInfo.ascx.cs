using InSite.Application.Sites.Read;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class PageInfo : System.Web.UI.UserControl
    {
        public void BindPage(QPage page)
        {
            PageLink.HRef = $"/ui/admin/sites/pages/outline?id={page.PageIdentifier}";
            PageTitle.Text = page.Title ?? page.PageSlug ?? "Untitled";
            PageType.Text = string.IsNullOrEmpty(page.PageType)? "None" : page.PageType;
            PageSlug.Text = string.IsNullOrEmpty(page.PageSlug)? "None" : page.PageSlug;
        }
    }
}