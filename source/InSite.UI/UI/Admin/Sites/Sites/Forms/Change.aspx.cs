using System;

using InSite.Admin.Sites.Utilities;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Sites.Sites;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Sites.Forms
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SiteId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var site = ServiceLocator.SiteSearch.Select(SiteId);

                if (site == null || site.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null, site.SiteTitle);

                SiteDetails.BindSite(site);

                TitleInput.Text = site.SiteTitle;
                Domain.Text = site.SiteDomain;

                CancelButton.NavigateUrl = $"/ui/admin/sites/outline?id={SiteId}&panel=setup";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var commands = new SiteCommandGenerator().
                GetDifferenceCommands(
                    GetEntityValues(),
                    GetInputValues()
                );

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/sites/outline?id={SiteId}&panel=setup");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={SiteId}&panel=setup"
                : null;
        }

        private SiteState GetEntityValues()
        {
            var site = ServiceLocator.SiteSearch.Select(SiteId);
            return new SiteState()
            {
                Domain = site.SiteDomain,
                Identifier = site.SiteIdentifier,
                Title = site.SiteTitle
            };
        }

        private SiteState GetInputValues()
        {
            return new SiteState()
            {
                Domain = SiteHelper.SanitizeSiteName(Domain.Text),
                Title = TitleInput.Text
            };
        }
    }
}