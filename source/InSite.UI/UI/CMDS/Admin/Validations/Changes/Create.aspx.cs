using System;

using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/cmds/admin/validations/changes/edit";
        private const string SearchUrl = "/ui/cmds/admin/validations/changes/search";

        public override void ApplyAccessControlForCmds() { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);
                CancelButton.NavigateUrl = SearchUrl;
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
            var entity = new QStandardValidationLog();

            Detail.GetInputValues(entity);

            entity.LogIdentifier = UniqueIdentifier.Create();

            StandardValidationChangeStore.Insert(entity);

            HttpResponseHelper.Redirect($"{EditUrl}?id={entity.LogIdentifier}&status=saved");
        }
    }
}
