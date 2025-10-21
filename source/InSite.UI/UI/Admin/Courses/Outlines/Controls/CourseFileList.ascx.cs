using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Courses.Courses;

using Shift.Common;
using Shift.Common.File;

namespace InSite.UI.Admin.Courses.Outlines.Controls
{
    public partial class CourseFileList : UserControl
    {
        private Guid CourseId
        {
            get => (Guid)ViewState[nameof(CourseId)];
            set => ViewState[nameof(CourseId)] = value;
        }

        private string RelativePhysicalPath => $@"\courses\{CourseId}\";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UploadButton.Click += UploadButton_Click;
            FileRepeater.ItemCommand += FileRepeater_ItemCommand;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            UploadHelper.SaveUploadedFiles(ServiceLocator.FilePaths, FileUpload.Metadata, RelativePhysicalPath);

            LoadData(CourseId);
        }

        private void FileRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteFile")
                return;

            var folder = OutlineHelper.GetFolderPhysicalPath(CourseId);
            var filePath = Path.Combine(folder, (string)e.CommandArgument);

            try
            {
                File.Delete(filePath);
            }
            catch
            {
                //The file can be locked so we should not crash in this case
            }

            LoadData(CourseId);
        }

        public void LoadData(Guid courseId)
        {
            const int maxUrlLength = 200;

            CourseId = courseId;

            var fileFolderUrl = UploadHelper.GetFileFolderUrl(RelativePhysicalPath);

            FileUpload.ContainerIdentifier = courseId;
            FileUpload.MaxFileNameLength = maxUrlLength - fileFolderUrl.Length;

            var fileUrls = OutlineHelper.GetFileUrls(courseId);

            var data = fileUrls
                .Select(x => new
                {
                    FileName = UrlHelper.GetFileName(x),
                    Url = x,
                    Language = OutlineHelper.GetLinkLanguage(x) ?? "N/A"
                })
                .ToList();

            FileRepeater.Visible = data.Count > 0;
            FileRepeater.DataSource = data;
            FileRepeater.DataBind();
        }
    }
}