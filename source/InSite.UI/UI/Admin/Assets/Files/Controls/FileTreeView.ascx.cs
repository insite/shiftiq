using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

using InSite.Common.Web;

using Shift.Common;
using Shift.Common.File;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assets.Files.Controls
{
    public partial class FileTreeView : UserControl
    {
        private class FileItem
        {
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string Url { get; set; }
            public int IsImage { get; set; }

            public string FileNameJS => HttpUtility.HtmlEncode(FileName).Replace("'", "\\'");
        }

        private static readonly string[] Extensions = new[] { ".csv", ".css", ".docx", ".html", ".gif", ".jpg", ".jpeg", ".js", ".mp4", ".pdf", ".png", ".sql", ".txt", ".xlsx", ".xml" };
        private static readonly string[] ImageExtensions = new[] { ".gif", ".jpg", ".jpeg", ".png" };
        private static readonly string RelativePath = "/Library";

        private const int OneMB = 1024 * 1024;
        private const int MaxFileAllowedMB = 10;

        protected override void OnInit(EventArgs e)
        {
            if (HandleAjaxRequest())
                return;

            base.OnInit(e);

            ShowFolderButton.Click += ShowFolderButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            FileUpload.MaxFileSize = MaxFileAllowedMB * OneMB;
            FileUpload.AllowedExtensions = Extensions;
        }

        private void ShowFolderButton_Click(object sender, EventArgs e)
        {
            ShowFolder();
        }

        public void LoadData()
        {
            var path = Server.MapPath(RelativePath);
            var folders = GetFolders(path);

            var root = new List<FileTreeItemModel>
            {
                new FileTreeItemModel
                {
                    FileName = Path.GetFileName(RelativePath),
                    IsCollapsed = false,
                    IsSelected = true,
                    IsRoot = true,
                    Children = folders
                }
            };

            Files.LoadData(root);

            FolderPath.Value = RelativePath;

            ShowFolder();

            MaxFileAllowedLiteral.Text = $"{MaxFileAllowedMB:n0} MB";
            FileExtensionsAllowedLiteral.Text = string.Join(", ", Extensions);
        }

        private List<FileTreeItemModel> GetFolders(string path)
        {
            var result = new List<FileTreeItemModel>();

            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                var children = GetFolders(folder);

                result.Add(new FileTreeItemModel
                {
                    FileName = Path.GetFileName(folder),
                    IsCollapsed = true,
                    Children = children
                });
            }

            result.Sort((a, b) => a.FileName.CompareTo(b.FileName));

            return result;
        }

        private void ShowFolder()
        {
            var files = GetFiles();

            NoFiles.Visible = files.Count == 0;
            FileRepeater.Visible = files.Count > 0;

            FileRepeater.DataSource = files;
            FileRepeater.DataBind();
        }

        private List<FileItem> GetFiles()
        {
            var result = new List<FileItem>();

            var relativePath = FolderPath.Value;
            var path = Server.MapPath(relativePath);
            var files = Directory.GetFiles(path);
            foreach (var filePath in files)
            {
                if (!StringHelper.EndsWithAny(filePath, Extensions))
                    continue;

                var sizeKB = new FileInfo(filePath).Length / 1024;
                var fileName = Path.GetFileName(filePath);

                result.Add(new FileItem
                {
                    FileName = fileName,
                    FileSize = $"{sizeKB:n0} KB",
                    Url = $"{relativePath}/{fileName}".ToLower(),
                    IsImage = StringHelper.EndsWithAny(fileName, ImageExtensions) ? 1 : 0
                });
            }

            result.Sort((a, b) => a.FileName.CompareTo(b.FileName));

            return result;
        }

        private bool HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["isPageAjax"], out var isAjax) || !isAjax)
                return false;

            var action = Page.Request.Form["action"];
            switch (action)
            {
                case "createFolder":
                    CreateFolder(Page.Request.Form["folderPath"], Page.Request.Form["name"]);
                    break;
                case "renameFolder":
                    RenameFolder(Page.Request.Form["folderPath"], Page.Request.Form["name"]);
                    break;
                case "deleteFolder":
                    DeleteFolder(Page.Request.Form["folderPath"]);
                    break;
                case "uploadFile":
                    UploadFile(Page.Request.Form["folderPath"], Page.Request.Form["files"]);
                    break;
                case "renameFile":
                    RenameFile(Page.Request.Form["folderPath"], Page.Request.Form["oldName"], Page.Request.Form["newName"]);
                    break;
                case "deleteFile":
                    DeleteFile(Page.Request.Form["folderPath"], Page.Request.Form["name"]);
                    break;
                default:
                    throw new ArgumentException($"Unsupported action: {action}");
            }

            return true;
        }

        private void CreateFolder(string folderPath, string name)
        {
            var parentPath = Server.MapPath(folderPath);
            var path = Path.Combine(parentPath, name);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            SendFileTreeViewNode(parentPath);
        }

        private void DeleteFolder(string folderPath)
        {
            var path = Server.MapPath(folderPath);
            var parentPath = Path.GetDirectoryName(path);

            if (!StringHelper.Equals(folderPath, RelativePath) && Directory.Exists(path))
                Directory.Delete(path, true);

            SendFileTreeViewNode(parentPath);
        }

        private void RenameFolder(string folderPath, string name)
        {
            var sourcePath = Server.MapPath(folderPath);
            var destPath = Path.Combine(Path.GetDirectoryName(sourcePath), name);

            if (!StringHelper.Equals(folderPath, RelativePath)
                && Directory.Exists(sourcePath)
                && !Directory.Exists(destPath)
                )
            {
                Directory.Move(sourcePath, destPath);
            }

            var result = name + "\n" + HttpUtility.HtmlEncode(name);

            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        private void UploadFile(string folderPath, string files)
        {
            var path = Server.MapPath(folderPath);

            if (Directory.Exists(path))
            {
                var metadata = ServiceLocator.Serializer.Deserialize<UploadMetadata>(files);
                if (metadata == null)
                    throw new ArgumentNullException(nameof(metadata));

                if (string.IsNullOrEmpty(metadata.FilePath))
                    throw new ArgumentNullException(nameof(metadata));

                var dstFilePath = Path.Combine(path, Path.GetFileName(metadata.FilePath));
                if (File.Exists(dstFilePath))
                    File.Delete(dstFilePath);

                File.Move(metadata.FilePath, dstFilePath);
            }

            Response.Clear();
            Response.End();
        }

        private void RenameFile(string folderPath, string oldName, string newName)
        {
            var path = Server.MapPath(folderPath);
            var sourcePath = Path.Combine(path, oldName);
            var destPath = Path.Combine(path, newName);

            if (File.Exists(sourcePath) && !File.Exists(destPath))
                File.Move(sourcePath, destPath);

            var result = newName + "\n" + HttpUtility.HtmlEncode(newName);

            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        private void DeleteFile(string folderPath, string name)
        {
            var path = Server.MapPath(folderPath);
            var filePath = Path.Combine(path, name);

            if (File.Exists(filePath))
                File.Delete(filePath);

            Response.Clear();
            Response.End();
        }

        private void SendFileTreeViewNode(string path)
        {
            var folders = GetFolders(path);
            if (folders.Count == 0)
            {
                Response.Clear();
                Response.Write(string.Empty);
                Response.End();
                return;
            }

            var commentRepeater = (FileTreeViewNode)LoadControl("~/UI/Admin/Assets/Files/Controls/FileTreeViewNode.ascx");
            commentRepeater.LoadData(folders);

            var result = new StringBuilder();
            using (var writer = new StringWriter(result))
            {
                var htmlWriter = new HtmlTextWriter(writer);
                commentRepeater.RenderControl(htmlWriter);
            }

            Response.Clear();
            Response.Write(result.ToString());
            Response.End();
        }
    }
}