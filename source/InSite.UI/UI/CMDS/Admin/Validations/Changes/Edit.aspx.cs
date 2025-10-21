using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/cmds/admin/validations/changes/search";

        private Guid Identifier => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Delete())
                HttpResponseHelper.Redirect(SearchUrl);
        }

        private void Open()
        {
            var entity = StandardValidationChangeSearch.SelectFirst(x => x.ChangeIdentifier == Identifier, x => x.Standard);
            if (entity == null || !Detail.SetInputValues(entity))
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, null, $"{entity.Standard.ContentTitle} <span class=form-text>#{entity.Standard.Code}</span>");
        }

        private bool Save()
        {
            var entity = ServiceLocator.StandardValidationSearch.GetStandardValidationLog(Identifier);
            if (entity == null)
                return false;

            Detail.GetInputValues(entity);

            StandardValidationChangeStore.Update(entity);

            return true;
        }

        private bool Delete()
        {
            var entity = ServiceLocator.StandardValidationSearch.GetStandardValidationLog(Identifier);
            if (entity == null)
                return false;

            StandardValidationChangeStore.Delete(entity.StandardValidationIdentifier, entity.LogIdentifier);

            return true;
        }
    }
}
