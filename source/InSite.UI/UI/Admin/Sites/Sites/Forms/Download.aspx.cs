using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Sites.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? WebSiteID => Guid.TryParse(Request.QueryString["id"], out var value) ? value : (Guid?)null;

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
                var webSite = WebSiteID.HasValue ? ServiceLocator.SiteSearch.Select(WebSiteID.Value) : null;
                if (webSite == null || webSite.OrganizationIdentifier != Organization.OrganizationIdentifier)
                {
                    HttpResponseHelper.Redirect("/ui/admin/sites/sites/search");
                    return;
                }

                var title =
                    $"{webSite.SiteTitle ?? webSite.SiteDomain ?? "Untitled"}";

                PageHelper.AutoBindHeader(this, null, title);

                SiteDetails.BindSite(webSite);

                SetupDownloadSection();

                CancelLink.NavigateUrl = $"/ui/admin/sites/outline?id={WebSiteID}";
            }
        }

        private void SetupDownloadSection()
        {
            FileName.Text = string.Format("website-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);
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
            var data = ServiceLocator.PageSearch.SerializeSite(WebSiteID.Value);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", data);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={WebSiteID}"
                : null;
        }
    }
}