using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Jobs.Applications.Forms
{
    public partial class Create : AdminBasePage
    {
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

                CancelButton.NavigateUrl = "/ui/admin/jobs/applications/search";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = new TApplication
            {
                ApplicationIdentifier = UniqueIdentifier.Create()
            };

            Detail.GetInputValues(entity);

            entity.WhenCreated = DateTime.UtcNow;
            entity.WhenModified = DateTime.UtcNow;

            TApplicationStore.Insert(entity);

            var url = $"/ui/admin/jobs/applications/edit?id={entity.ApplicationIdentifier}";

            HttpResponseHelper.Redirect(url);
        }
    }
}