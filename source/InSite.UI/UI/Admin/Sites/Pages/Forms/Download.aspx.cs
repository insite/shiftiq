using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Pages.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? WebPageID => Guid.TryParse(Request.QueryString["id"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var webPage = WebPageID.HasValue ? ServiceLocator.PageSearch.Select(WebPageID.Value, x => x.Site) : null;
                if (webPage == null || webPage.OrganizationIdentifier != Organization.OrganizationIdentifier)
                {
                    HttpResponseHelper.Redirect("/ui/admin/sites/pages/search");
                    return;
                }

                var title = $"{webPage.Title ?? webPage.PageSlug ?? "Untitled"}";

                PageHelper.AutoBindHeader(this, null, title);

                PageDetails.BindPage(webPage);

                SetupDownloadSection();

                CancelLink.NavigateUrl = $"/ui/admin/sites/pages/outline?id={WebPageID}";
            }
        }

        private void SetupDownloadSection()
        {
            FileName.Text = string.Format("webpage-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;
            if (fileFormat == "JSON")
                SendJson();
        }

        private void SendJson()
        {
            var data = ServiceLocator.PageSearch.SerializePage(WebPageID.Value);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", data);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={WebPageID}"
                : null;
        }
    }
}