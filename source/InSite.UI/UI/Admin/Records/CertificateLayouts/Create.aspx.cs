using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Records.AchievementLayouts
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/records/achievement-layouts/edit";
        private const string SearchUrl = "/ui/admin/records/achievement-layouts/search";

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
                PageHelper.AutoBindHeader(this);

                CancelLink.NavigateUrl = SearchUrl;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var entity = new TCertificateLayout
            {
                CertificateLayoutIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = Organization.Identifier
            };

            if (!Details.GetInputValues(entity))
                return;

            TCertificateLayoutStore.Save(entity);

            HttpResponseHelper.Redirect(EditUrl + $"?id={entity.CertificateLayoutCode}");
        }
    }
}
