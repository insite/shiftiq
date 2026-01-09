using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Reports.Dashboards
{
    public partial class Home : AdminBasePage
    {
        private const string DashboardUrl = "/ui/admin/reports/dashboards";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);

            SpecificationPanel.Visible = Identity.IsOperator;
            DeleteButton.Visible = DashboardContainer.HasModel;
            DownloadButton.Visible = DashboardContainer.HasModel;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            DashboardContainer.DownloadModel();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!UploadFile.HasFile)
            {
                BodyStatus.AddMessage(AlertType.Error, "The file is required");
                return;
            }

            int index;

            try
            {
                using (var fileStream = UploadFile.OpenFile())
                    index = DashboardContainer.UploadModel(fileStream);
            }
            catch (ApplicationError apperr)
            {
                BodyStatus.AddMessage(AlertType.Error, apperr.Message);
                return;
            }

            var url = DashboardUrl;
            if (index >= 0)
                url += $"?d={index}";

            HttpResponseHelper.Redirect(url);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DashboardContainer.DeleteModel();

            HttpResponseHelper.Redirect(DashboardUrl);
        }
    }
}