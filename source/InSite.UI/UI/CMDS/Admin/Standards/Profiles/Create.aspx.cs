using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Standards.Profiles
{
    public partial class Create : AdminBasePage, ICmdsUserControl
    {
        private const string SearchUrl = "/ui/cmds/admin/standards/profiles/search";
        private const string EditUrl = "/ui/cmds/admin/standards/profiles/edit";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var asset = StandardFactory.Create("Profile");
            asset.StandardIdentifier = UniqueIdentifier.Create();
            asset.CertificationHoursPercentCore = CertificationHoursPercentCore.ValueAsDecimal;
            asset.CertificationHoursPercentNonCore = CertificationHoursPercentNonCore.ValueAsDecimal;
            asset.ContentTitle = TitleInput.Text;
            asset.ContentDescription = Description.Text;

            ProfileHierarchy.GetInputValues(asset);

            asset.Code = ProfileHelper.InitNumber(asset, User.Email);

            StandardStore.Insert(asset);

            HttpResponseHelper.Redirect($"{EditUrl}?id={asset.StandardIdentifier}&status=saved");
        }
    }
}