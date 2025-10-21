using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

using InSite.Application.Files.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Programs.Tasks.Controls
{
    public partial class TaskPresentationSetup : System.Web.UI.UserControl
    {
        private Guid ProgramIdentifier
        {
            get => (Guid)ViewState[nameof(ProgramIdentifier)];
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }

        private Guid TaskIdentifier
        {
            get => (Guid)ViewState[nameof(TaskIdentifier)];
            set => ViewState[nameof(TaskIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteImage.Click += DeleteImage_Click;
        }

        public void LoadData(Guid taskId)
        {
            var task = TaskSearch.SelectFirst(x => x.TaskIdentifier == taskId);
            if (task == null)
                return;

            LoadData(task);
        }

        public void LoadData(TTask task)
        {
            ProgramIdentifier = task.ProgramIdentifier;
            TaskIdentifier = task.TaskIdentifier;

            BindModelToControls(task);
        }

        public void BindModelToControls(TTask task)
        {
            TaskImage.ImageUrl = task.TaskImage;
            TaskImageField.Visible = !string.IsNullOrEmpty(task.TaskImage);
            TaskImageUrl.Text = task.TaskImage;
        }

        private void DeleteImage_Click(object sender, EventArgs e)
        {
            var task = TaskSearch.SelectFirst(x => x.TaskIdentifier == TaskIdentifier);
            if (task == null)
                return;

            if (task.TaskImage == null)
                return;

            var (fileIdentifier, _) = ServiceLocator.StorageService.ParseFileUrl(task.TaskImage);
            if (fileIdentifier != null)
                ServiceLocator.StorageService.Delete(fileIdentifier.Value);

            task.TaskImage = null;

            TaskStore.Update(task);
            Response.StatusCode = (int)HttpStatusCode.OK;
            HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "tab", "presentation"));
        }

        public bool SaveTaskImageV2()
        {
            var task = TaskSearch.SelectFirst(x => x.TaskIdentifier == TaskIdentifier);
            if (task == null || !TaskImageUploadV2.HasFile)
                return false;

            var oldUrl = task.TaskImage;

            var newUrl = TaskImageUploadV2.AdjustImageSaveAndGetUrl(
                task.TaskIdentifier,
                FileObjectType.ProgramTask,
                300,
                200);

            if (string.IsNullOrWhiteSpace(newUrl))
                return false;

            if (!string.IsNullOrWhiteSpace(oldUrl) && !string.Equals(oldUrl, newUrl, StringComparison.OrdinalIgnoreCase))
                FileUploadV2.DeleteFileByUrl(oldUrl);

            task.TaskImage = newUrl;
            TaskStore.Update(task);
            return true;
        }
    }
}