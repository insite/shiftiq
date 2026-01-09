using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class PhotoSection : UserControl
    {
        private class DataItem
        {
            public Guid ID { get; set; }
            public string Type { get; set; }
            public int Sequence { get; set; }
            public long Timestamp { get; set; }
            public string ResourceName { get; set; }
            public string ResourceUrl { get; set; }
            public long ResourceSize { get; set; }
            public string ThumbnailUrl { get; set; }
            public string CaptionText { get; set; }

            [JsonConstructor]
            private DataItem()
            {

            }

            public DataItem(string type)
            {
                ID = Guid.NewGuid();
                Type = type;
                Timestamp = DateTime.UtcNow.Ticks;
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class VideoData
        {
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "provider_name")]
            public string ProviderName { get; set; }

            [JsonProperty(PropertyName = "provider_url")]
            public string ProviderUrl { get; set; }

            [JsonProperty(PropertyName = "author_name")]
            public string AuthorName { get; set; }

            [JsonProperty(PropertyName = "author_url")]
            public string AuthorUrl { get; set; }

            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "thumbnail_url")]
            public string ThumbnailUrl { get; set; }

            [JsonProperty(PropertyName = "thumbnail_width")]
            public int ThumbnailWidth { get; set; }

            [JsonProperty(PropertyName = "thumbnail_height")]
            public int ThumbnailHeight { get; set; }
        }

        protected Guid GroupIdentifier
        {
            get => (Guid)ViewState[nameof(GroupIdentifier)];
            set => ViewState[nameof(GroupIdentifier)] = value;
        }

        private List<Guid> ItemIdentifiers
        {
            get => (List<Guid>)ViewState[nameof(ItemIdentifiers)];
            set => ViewState[nameof(ItemIdentifiers)] = value;
        }

        private string ImageUploadPath => $"/contacts/groups/{GroupIdentifier}/photos/";

        private string DataUploadPath => Path.Combine(ServiceLocator.FilePaths.FileStoragePath, "Tenants", CurrentSessionState.Identity.Organization.OrganizationCode, "Contacts", "Groups", GroupIdentifier.ToString(), "Photos");

        private static readonly object _syncRoot = new object();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(PhotoSection).FullName;
            CommonScript.ContentKey = typeof(PhotoSection).FullName;

            UploadType.AutoPostBack = true;
            UploadType.ValueChanged += UploadType_ValueChanged;

            VideoUrlValidator.ServerValidate += VideoUrlValidator_ServerValidate;

            UploadButton.Click += UploadButton_Click;

            ManageItemRepeater.DataBinding += ManageItemRepeater_DataBinding;
            ManageItemRepeater.ItemDataBound += ManageItemRepeater_ItemDataBound;
            ManageItemRepeater.ItemCommand += ManageItemRepeater_ItemCommand;

            ManageSaveButton.Click += ManageSaveButton_Click;
            ManageCancelButton.Click += ManageCancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            TryDownloadPhoto();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            OnUploadTypeChanged();
        }

        public void LoadData(Guid groupId)
        {
            GroupIdentifier = groupId;

            var directory = new DirectoryInfo(DataUploadPath);
            if (!directory.Exists)
                directory.Create();

            BindDataItems();
        }

        private void UploadType_ValueChanged(object sender, ComboBoxValueChangedEventArgs e) => OnUploadTypeChanged();

        private void OnUploadTypeChanged()
        {
            var value = UploadType.Value;

            ImageUploadContainer.Visible = value == "image";
            VideoUploadContainer.Visible = value == "video";

            ResetInputs();
        }

        private void VideoUrlValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var videoUrl = VideoUrl.Text;
            var videoInfo = VideoInfo.Value;

            args.IsValid = videoUrl.IsNotEmpty()
                && videoInfo.IsNotEmpty()
                && (JsonConvert.DeserializeObject<VideoData>(videoInfo)?.ThumbnailUrl).IsNotEmpty()
                && (
                    UrlHelper.YouTubeLinkPatterns.Any(x => x.IsMatch(videoUrl)) ||
                    UrlHelper.VimeoLinkPatterns.Any(x => x.IsMatch(videoUrl))
                );
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var data = new DataItem(UploadType.Value);

            if (data.Type == "image")
                UploadImage(data);
            else if (data.Type == "video")
                UploadVideo(data);
            else
                throw new NotImplementedException();

            CreateDataFile(data);

            ResetInputs();

            BindDataItems();

            GalleryUpdatePanel.Update();
            ManagerUpdatePanel.Update();
        }

        private void ManageItemRepeater_DataBinding(object sender, EventArgs e)
        {
            ItemIdentifiers = new List<Guid>();
        }

        private void ManageItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            ItemIdentifiers.Add((Guid)DataBinder.Eval(e.Item.DataItem, "ID"));
        }

        private void ManageItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var id = ItemIdentifiers[e.Item.ItemIndex];
                if (DeleteDataItem(id))
                {
                    BindDataItems();

                    GalleryUpdatePanel.Update();
                    ManagerUpdatePanel.Update();
                }
            }
        }

        private void ManageSaveButton_Click(object sender, EventArgs e)
        {
            lock (_syncRoot)
            {
                for (var i = 0; i < ManageItemRepeater.Items.Count; i++)
                {
                    var id = ItemIdentifiers[i];
                    var item = ManageItemRepeater.Items[i];
                    var caption = (ITextBox)item.FindControl("Caption");

                    UpdateDataItem(id, dataItem =>
                    {
                        dataItem.CaptionText = caption.Text.Trim().NullIfEmpty();

                        return true;
                    });
                }
            }

            BindDataItems();

            GalleryUpdatePanel.Update();
        }

        private void ManageCancelButton_Click(object sender, EventArgs e)
        {
            BindDataItems(bindGallery: false);
        }

        private void BindDataItems(bool bindGallery = true, bool bindManager = true)
        {
            var dataItems = GetDataItems();

            if (bindGallery)
            {
                GalleryTab.Visible = dataItems.Length > 0;
                GalleryItemRepeater.DataSource = dataItems.Select(x => new
                {
                    IsVideo = x.Type == "video",
                    HasCaption = x.CaptionText != null,
                    x.ID,
                    x.ResourceUrl,
                    x.ThumbnailUrl,
                    x.CaptionText
                });
                GalleryItemRepeater.DataBind();
            }

            if (bindManager)
            {
                ManageContainer.Visible = dataItems.Length > 0;
                ManageItemRepeater.DataSource = dataItems.Select(x => new
                {
                    IsVideo = x.Type == "video",
                    x.ID,
                    x.ResourceName,
                    x.ResourceUrl,
                    x.ThumbnailUrl,
                    Size = x.ResourceSize != 0 ? x.ResourceSize.Bytes().Humanize() : string.Empty,
                    x.CaptionText,
                });
                ManageItemRepeater.DataBind();
            }
        }

        private void ResetInputs()
        {
            ImageCaption.Text = string.Empty;
            VideoUrl.Text = string.Empty;
            VideoInfo.Value = string.Empty;
            VideoCaption.Text = string.Empty;
        }

        private void GetImageFilePaths(Guid id, string fileExt, out string filePath, out string thumbnailPath)
        {
            var fileName = id.ToString();
            var thumbnailName = fileName + "-thumb";

            filePath = ImageUploadPath + fileName + fileExt;
            thumbnailPath = ImageUploadPath + thumbnailName + fileExt;
        }

        private void UploadImage(DataItem data)
        {
            var organization = CurrentSessionState.Identity.Organization;

            FileModel imageUpload = null;
            FileModel thumbnailUpload = null;

            try
            {
                var file = ImageUpload.PostedFile;

                GetImageFilePaths(data.ID, Path.GetExtension(file.FileName), out var filePath, out var thumbnailPath);

                imageUpload = FileHelper.Provider.Save(organization.Identifier, filePath, file.InputStream, isCheckFileSizeLimits: false);

                using (var imageStream = FileHelper.Provider.Read(imageUpload.OrganizationIdentifier, imageUpload.Path))
                {
                    using (var image = new Bitmap(imageStream))
                    {
                        var scale = Math.Min(480d / image.Width, 360d / image.Height);
                        var scaleWidth = (int)(image.Width * scale);
                        var scaleHeight = (int)(image.Height * scale);

                        using (var thumbnail = new Bitmap(scaleWidth, scaleHeight))
                        {
                            using (var g = Graphics.FromImage(thumbnail))
                            {
                                g.Clear(Color.Transparent);
                                g.DrawImage(image, 0, 0, scaleWidth, scaleHeight);
                            }

                            using (var thumbnailStream = new MemoryStream())
                            {
                                thumbnail.Save(thumbnailStream, ImageFormat.Jpeg);

                                thumbnailStream.Position = 0;

                                thumbnailUpload = FileHelper.Provider.Save(organization.Identifier, thumbnailPath, thumbnailStream, isCheckFileSizeLimits: false);
                            }
                        }
                    }
                }

                data.ResourceName = file.FileName;
                data.CaptionText = ImageCaption.Text.Trim().NullIfEmpty();
                data.ResourceUrl = FileHelper.GetUrl(imageUpload.Path);
                data.ThumbnailUrl = FileHelper.GetUrl(thumbnailUpload.Path);
                data.ResourceSize = imageUpload.ContentSize;
            }
            catch
            {
                if (imageUpload != null)
                    FileHelper.Provider.Delete(imageUpload.OrganizationIdentifier, imageUpload.Path);

                if (thumbnailUpload != null)
                    FileHelper.Provider.Delete(thumbnailUpload.OrganizationIdentifier, thumbnailUpload.Path);

                throw;
            }
        }

        private void UploadVideo(DataItem data)
        {
            var organization = CurrentSessionState.Identity.Organization;

            FileModel thumbnailUpload = null;

            try
            {
                var videoData = JsonConvert.DeserializeObject<VideoData>(VideoInfo.Value);

                data.ResourceName = videoData.Title + " by " + videoData.AuthorName;

                GetImageFilePaths(
                    data.ID,
                    ".bin",
                    out var filePath, out var thumbnailPath);

                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(videoData.ThumbnailUrl))
                    {
                        thumbnailUpload = FileHelper.Provider.Save(organization.Identifier, thumbnailPath, stream, isCheckFileSizeLimits: false);
                    }
                }
            }
            catch
            {
                if (thumbnailUpload != null)
                    FileHelper.Provider.Delete(thumbnailUpload.OrganizationIdentifier, thumbnailUpload.Path);

                throw;
            }

            data.CaptionText = VideoCaption.Text.Trim().NullIfEmpty();
            data.ResourceUrl = VideoUrl.Text;
            data.ThumbnailUrl = FileHelper.GetUrl(thumbnailUpload.Path);
        }

        private void TryDownloadPhoto()
        {
            if (IsPostBack || !Guid.TryParse(Request.QueryString["photo"], out var photoId))
                return;

            var dataItem = GetDataItem(photoId);
            if (dataItem == null || dataItem.Type != "image")
                return;

            var descriptor = FileHelper.GetDescriptor(dataItem.ResourceUrl);
            if (descriptor == null)
                return;

            Response.SendFile(dataItem.ResourceName, (stream) =>
            {
                using (var fs = FileHelper.Provider.Read(descriptor))
                    fs.CopyTo(stream);
            });
        }

        private void CreateDataFile(DataItem data)
        {
            lock (_syncRoot)
            {
                File.WriteAllText(
                    Path.Combine(DataUploadPath, data.ID.ToString() + ".json"),
                    JsonConvert.SerializeObject(data));
            }
        }

        private DataItem GetDataItem(Guid id)
        {
            lock (_syncRoot)
            {
                var directory = new DirectoryInfo(DataUploadPath);

                var files = directory.GetFiles(id + ".json");
                if (files.Length == 1)
                    return ReadDataItem(files[0]);
            }

            return null;
        }

        private void UpdateDataItem(Guid id, Func<DataItem, bool> update)
        {
            lock (_syncRoot)
            {
                var directory = new DirectoryInfo(DataUploadPath);

                var files = directory.GetFiles(id + ".json");
                if (files.Length != 1)
                    return;

                var file = files[0];
                var dataItem = ReadDataItem(files[0]);

                if (update(dataItem))
                    CreateDataFile(dataItem);
            }
        }

        private bool DeleteDataItem(Guid id)
        {
            lock (_syncRoot)
            {
                var directory = new DirectoryInfo(DataUploadPath);

                var files = directory.GetFiles(id + ".json");
                if (files.Length != 1)
                    return false;

                var file = files[0];
                var dataItem = ReadDataItem(file);

                if (dataItem != null)
                {
                    var resourceDescriptor = FileHelper.GetDescriptor(dataItem.ResourceUrl);
                    if (resourceDescriptor != null)
                        FileHelper.Provider.Delete(resourceDescriptor.UploadId);

                    var thumbnailDescriptor = FileHelper.GetDescriptor(dataItem.ThumbnailUrl);
                    if (thumbnailDescriptor != null)
                        FileHelper.Provider.Delete(thumbnailDescriptor.UploadId);
                }

                file.Delete();

                return true;
            }
        }

        private DataItem[] GetDataItems()
        {
            var result = new List<DataItem>();

            lock (_syncRoot)
            {
                var directory = new DirectoryInfo(DataUploadPath);

                var files = directory.GetFiles("*.json");
                for (var i = 0; i < files.Length; i++)
                    result.Add(ReadDataItem(files[i]));
            }

            return result
                .OrderBy(x => x.Sequence == 0 ? int.MaxValue : x.Sequence)
                .ThenBy(x => x.Timestamp)
                .ThenBy(x => x.ID)
                .ToArray();
        }

        private DataItem ReadDataItem(FileInfo file)
        {
            var json = File.ReadAllText(file.FullName);
            return JsonConvert.DeserializeObject<DataItem>(json);
        }

        protected string GetDownloadUrl(object resourceId)
        {
            return string.Format("/ui/admin/contacts/groups/edit?contact={0}&photo={1}", GroupIdentifier, resourceId);
        }
    }
}