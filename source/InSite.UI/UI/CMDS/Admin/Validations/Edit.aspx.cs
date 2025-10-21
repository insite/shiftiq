using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Custom.CMDS.Admin.Standards.Validations.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/cmds/admin/validations/search";

        private Guid ValidationIdentifier => Guid.TryParse(Request["id"], out var key) ? key : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var entity = StandardValidationSearch.SelectFirst(x => x.ValidationIdentifier == ValidationIdentifier);
            if (entity != null)
                StandardValidationStore.Delete(entity.ValidationIdentifier);

            HttpResponseHelper.Redirect(SearchUrl);
        }

        private void Open()
        {
            var entity = StandardValidationSearch.SelectFirst(x => x.ValidationIdentifier == ValidationIdentifier, x => x.User);
            if (entity == null || !Detail.SetInputValues(entity))
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, null, entity.ValidationIdentifier.ToString());
        }

        private void Save()
        {
            var entity = ServiceLocator.StandardValidationSearch.GetStandardValidation(ValidationIdentifier);
            if (entity == null)
                return;

            Detail.GetInputValues(entity);

            StandardValidationStore.Update(entity);
        }
    }
}
