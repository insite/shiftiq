using System;
using System.Web.UI.WebControls;

using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Sites.Sites;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Common.Contents;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Admin.Sites.Sites.Forms
{
    public partial class Configuration : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SiteId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;

            ConfigurationTextValidator.ServerValidate += ConfigurationTextValidator_ServerValidate;
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

                CancelButton.NavigateUrl = $"/ui/admin/sites/outline?id={SiteId}&panel=configuration";
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

            HttpResponseHelper.Redirect($"/ui/admin/sites/outline?id={SiteId}&panel=configuration");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={SiteId}&panel=configuration"
                : null;
        }

        private SiteState GetEntityValues()
        {
            var site = ServiceLocator.SiteSearch.Select(SiteId);
            return new SiteState()
            {
                Identifier = site.SiteIdentifier
            };
        }

        private SiteState GetInputValues()
        {
            return new SiteState()
            {
                
            };
        }

        private void ConfigurationTextValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                JsonConvert.DeserializeObject<SiteConfiguration>(ConfigurationText.Text);
            }
            catch
            {
                args.IsValid = false;
            }
        }
    }
}