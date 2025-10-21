using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Records.AchievementLayouts
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/records/achievement-layouts/search";
        private const string CreateUrl = "/ui/admin/records/achievement-layouts/create";

        private Guid CertificateLayoutIdentifier => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

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
                Open();

                CancelLink.NavigateUrl = SearchUrl;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void Open()
        {
            var entity = SelectEntity();
            if (entity == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            PageHelper.AutoBindHeader(
                this,
                new BreadcrumbItem("Add New Achievement Layout", CreateUrl),
                entity.CertificateLayoutCode
            );

            Details.SetInputValues(entity);
        }

        private bool Save()
        {
            var entity = SelectEntity();

            if (!Details.GetInputValues(entity))
                return false;

            TCertificateLayoutStore.Save(entity);

            return true;
        }

        private TCertificateLayout SelectEntity() =>
            TCertificateLayoutSearch.Select(CertificateLayoutIdentifier);
    }
}
