using System;
using System.IO;
using System.Web.WebPages;

using InSite.UI.Layout.Admin;
using InSite.Web.Integration;

using Shift.Common.Scorm;
using Shift.Constant;

namespace InSite.UI.Admin.Assets.Scorm
{
    public partial class Upload : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UploadButton.Click += UploadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            var options = Organization.Integrations.ScormCloud;

            if (options == null || options.UserName.IsEmpty() || options.Password.IsEmpty())
            {
                SCORMNotConfigured();
                return;
            }

            try
            {
                var scorm = new ScormIntegrator(Organization, User, Guid.Empty);
                var courses = scorm.GetCourses();

                if (courses == null)
                {
                    SCORMNotConfigured();
                    return;
                }

                ScormCourseRepeater.DataSource = courses;
                ScormCourseRepeater.DataBind();
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("SCORM Cloud integration settings are missing"))
                    throw;

                SCORMNotConfigured();
            }
        }

        private void SCORMNotConfigured()
        {
            UploadStatus.AddMessage(AlertType.Warning, $"{Organization.Name} is not configured to integrate with SCORM Cloud. Please contact your Shift iQ Account Representative for assistance.");
            SCORMForm.Visible = false;
            UploadButton.Enabled = false;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (!CourseFile.HasFile)
            {
                UploadStatus.AddMessage(AlertType.Error, "The file is required");
                return;
            }

            if (UploadFile())
            {
                CourseSlug.Text = null;
                CourseVersion.ValueAsInt = null;
                CourseLanguage.Text = null;
            }
        }

        private bool UploadFile()
        {
            var courseId = $"{CourseSlug.Text}.v{CourseVersion.ValueAsInt}-{CourseLanguage.Text}";

            var scormCloud = new ScormIntegrator(Organization, User, Guid.Empty);

            var (jobId, jobStatus) = CreateJob(courseId, scormCloud);

            if (jobId == null)
            {
                return false;
            }

            var (result, waitStatus) = WaitingForJobCompletion(jobId, scormCloud);

            return result != null && ProcessResult(result);
        }

        private (string jobId, string status) CreateJob(string courseId, ScormIntegrator scormCloud)
        {
            if (!ProgressCallback("Uploading (Create Job)"))
                return (null, "Cancelled");

            try
            {
                using (Stream stream = CourseFile.OpenFile())
                {
                    var jobId = scormCloud.StartCourseImport(courseId, true, null, null, null, stream);
                    return (jobId, null);
                }
            }
            catch (Exception e)
            {
                UploadStatus.AddMessage(AlertType.Error, $"Error while trying to upload: {e.Message}");
                return (null, "Error");
            }
        }

        private (CourseImport, string) WaitingForJobCompletion(string jobId, ScormIntegrator courseApi)
        {
            const int timeoutInMinutes = 10;

            CourseImport import;
            var timeout = DateTime.Now.AddMinutes(timeoutInMinutes);

            do
            {
                try
                {
                    import = courseApi.GetCourseImportStatus(jobId);
                }
                catch (Exception e)
                {
                    UploadStatus.AddMessage(AlertType.Error, $"Error while trying to upload: {e.Message}");
                    return (null, "Error");
                }

                if (!ProgressCallback($"Uploading ({import.Message})"))
                    return (null, "Cancelled");
            }
            while (import.IsRunning && timeout > DateTime.Now);

            string status;
            if (import.IsRunning)
                status = "Timedout";
            else if (import.IsError)
                status = "Error";
            else
                status = import.IsComplete ? "Complete" : "Unknown";

            return (import, status);
        }

        private bool ProcessResult(CourseImport result)
        {
            if (result.IsRunning)
            {
                UploadStatus.AddMessage(AlertType.Error, "The request is timed out");
                return false;
            }

            if (result.IsError)
            {
                UploadStatus.AddMessage(AlertType.Error, result?.Message ?? "Unknown upload error");
                return false;
            }

            if (result.IsComplete)
            {
                UploadStatus.AddMessage(AlertType.Success, $"The file has been successfully exported to SCORM with CourseId = {result.CourseId}");
                return true;
            }

            return true;
        }

        private bool ProgressCallback(string status)
        {
            if (UploadProgress.IsRequestCancelled)
            {
                UploadStatus.AddMessage(AlertType.Error, "The request has been cancelled");
                return false;
            }

            UploadProgress.UpdateContext(context => context.Variables["status"] = status);

            return true;
        }
    }
}