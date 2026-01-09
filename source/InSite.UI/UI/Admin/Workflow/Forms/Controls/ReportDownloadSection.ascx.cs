using System;
using System.Web.UI;

using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDownloadSection : BaseUserControl
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) => Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        public Guid? SurveyID
        {
            get => (Guid?)ViewState[nameof(SurveyID)];
            set => ViewState[nameof(SurveyID)] = value;
        }

        #endregion

        #region Methods (initialization and loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        #endregion

        #region Methods (event handling)

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!SurveyID.HasValue)
                return;

            var (surveyName, report) = new ReportDownloader().Create(
                SurveyID.Value,
                EncodingSelector.SelectedValue,
                IncludeAdditionalFiles.Checked,
                OptionFormat.SelectedValue
            );

            if (report == null)
                return;

            var fileNameZip = $"Export_{surveyName}_{DateTime.Now.ToString("yyyyMMddhhmmss")}";

            Page.Response.SendFile(fileNameZip, "zip", report);
        }

        #endregion
    }
}