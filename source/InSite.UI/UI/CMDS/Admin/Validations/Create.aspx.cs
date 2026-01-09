using System;

using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Custom.CMDS.Admin.Standards.Validations.Forms
{
    public partial class Create : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                Detail.SetDefaultInputValues();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var entity = new QStandardValidation();

            entity.StandardValidationIdentifier = UniqueIdentifier.Create();

            Detail.GetInputValues(entity);

            StandardValidationStore.Insert(entity);

            var editUrl = $"/ui/cmds/admin/validations/edit?id={entity.StandardValidationIdentifier}&status=saved";

            HttpResponseHelper.Redirect(editUrl);
        }
    }
}
