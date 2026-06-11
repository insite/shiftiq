using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Files.Read;
using InSite.UI.Admin.Assessments.Attempts.Utilities.TakerReport;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class TakerReportUpload : AdminBasePage
    {
        private List<PersonRow> People
        {
            get => (List<PersonRow>)ViewState[nameof(People)];
            set => ViewState[nameof(People)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CsvFile.FileUploaded += CsvFile_FileUploaded;
            SaveButton.Click += SaveButton_Click;

            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }

        private void CsvFile_FileUploaded(object sender, EventArgs e)
        {
            if (!CsvFile.HasFile)
                return;

            var file = TakerReportReader.Read(CsvFile.File.FileId);
            if (file.Errors.Count > 0)
            {
                ShowErrors(file.Errors);
                return;
            }

            People = file.People;

            AttemptPanel.Visible = true;
            SaveButton.Visible = true;

            DataRepeater.DataSource = file.People;
            DataRepeater.DataBind();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var now = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, User.TimeZone);
            var fileName = $"CAMLPR_ExamReport_{now:yyyy}-{now:MM}-{now:dd}.csv";

            CsvFile.SaveFile(Organization.Identifier, FileObjectType.Organization, fileName);

            TakerReportUploader.UploadBatch(this, People);

            SaveButton.Visible = false;

            StatusAlert.AddMessage(AlertType.Success, "Reports have been generated and saved");
        }

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var person = (PersonRow)e.Item.DataItem;

            var frameworkStatusRepeater = (Repeater)e.Item.FindControl("FrameworkStatusRepeater");
            frameworkStatusRepeater.DataSource = person.Frameworks;
            frameworkStatusRepeater.DataBind();
        }

        private void ShowErrors(List<string> errors)
        {
            var message = new StringBuilder();
            message.AppendLine("Validation errors: <ul>");

            for (int i = 0; i < errors.Count && i < 10; i++)
                message.AppendLine($"<li>{errors[i]}</li>");

            if (errors.Count > 10)
                message.AppendLine($"<li>...</li>");

            message.AppendLine("</ul>");

            StatusAlert.AddMessage(AlertType.Error, message.ToString());

            People = null;

            AttemptPanel.Visible = false;
        }
    }
}
