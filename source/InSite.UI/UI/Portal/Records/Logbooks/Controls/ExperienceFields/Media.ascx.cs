using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Files.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;

using AspLiteral = System.Web.UI.WebControls.Literal;

namespace InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields
{
    public partial class Media : UserControl, IExperienceMediaField
    {
        #region Constants

        private static readonly Guid TempObjectId = Guid.Parse("1B011246-17CB-4A8E-95A9-800D6D45E787");

        #endregion

        #region Classes

        private class ResultData : IExperienceMediaFieldData
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public Guid? FileIdentifier { get; set; }
        }

        public class OutputControl : System.Web.UI.Control
        {
            public string Type
            {
                get => (string)ViewState[nameof(Type)];
                set => ViewState[nameof(Type)] = value;
            }

            public string Title
            {
                get => (string)ViewState[nameof(Title)];
                set => ViewState[nameof(Title)] = value;
            }

            public string MediaURL
            {
                get => (string)ViewState[nameof(MediaURL)];
                set => ViewState[nameof(MediaURL)] = value;
            }

            private AspLiteral _literal;
            private OutputAudio _audio;
            private OutputVideo _video;

            protected override void OnLoad(EventArgs e)
            {
                EnsureChildControls();

                base.OnLoad(e);
            }

            protected override void CreateChildControls()
            {
                base.CreateChildControls();

                _literal = new AspLiteral { ViewStateMode = ViewStateMode.Disabled };
                _audio = new OutputAudio { Visible = false, ViewStateMode = ViewStateMode.Disabled };
                _video = new OutputVideo { Visible = false, ViewStateMode = ViewStateMode.Disabled };

                Controls.Add(_literal);
                Controls.Add(_audio);
                Controls.Add(_video);
            }

            protected override void OnPreRender(EventArgs e)
            {
                base.OnPreRender(e);

                SetupControls();
            }

            private void SetupControls()
            {
                if (Type == null)
                {
                    _literal.Text = "<i>None</i>";
                    return;
                }

                var title = Title.IsNotEmpty() ? HttpUtility.HtmlEncode(Title) : "<i>No Name</i>";
                _literal.Text = $"<div class='mb-2'>{title}</div>";

                if (Type == "audio")
                {
                    _audio.Visible = true;
                    _audio.AudioURL = MediaURL;
                }
                else if (Type == "video")
                {
                    _video.Visible = true;
                    _video.VideoURL = MediaURL;
                }
            }
        }

        #endregion

        #region Properties

        public string Title
        {
            get => FieldTitle.Text;
            set => FieldTitle.Text = value;
        }

        public string Help
        {
            get => HelpText.Text;
            set => HelpText.Text = value;
        }

        public bool IsRequired
        {
            get => (bool)(ViewState[nameof(IsRequired)] ?? false);
            set => ViewState[nameof(IsRequired)] = value;
        }

        public string ValidationGroup
        {
            get => FieldValidator.ValidationGroup;
            set => FieldValidator.ValidationGroup = value;
        }

        private bool IsInitialFileRemoved
        {
            get => (bool)(ViewState[nameof(IsInitialFileRemoved)] ?? false);
            set => ViewState[nameof(IsInitialFileRemoved)] = value;
        }

        private FileStorageModel InitialFile
        {
            get => (FileStorageModel)ViewState[nameof(InitialFile)];
            set => ViewState[nameof(InitialFile)] = value;
        }

        private FileStorageModel UploadedFile
        {
            get => (FileStorageModel)ViewState[nameof(UploadedFile)];
            set => ViewState[nameof(UploadedFile)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MediaType.AutoPostBack = true;
            MediaType.ValueChanged += (x, y) => OnMediaTypeChanged();

            MediaVideoInput.MediaCaptured += MediaVideoInput_MediaCaptured;
            MediaVideoInput.MediaCaptureFailed += MediaInput_MediaCaptureFailed;

            MediaAudioInput.MediaCaptured += MediaAudioInput_MediaCaptured;
            MediaAudioInput.MediaCaptureFailed += MediaInput_MediaCaptureFailed;

            FieldValidator.ServerValidate += FieldValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack && FileUrl.Value.IsEmpty())
            {
                if (!IsInitialFileRemoved && InitialFile != null)
                    IsInitialFileRemoved = true;
                else if (UploadedFile != null)
                    DeleteUploadedFile();
            }

            base.OnLoad(e);
        }

        private void SetMediaVisibility(string type)
        {
            var isVideo = type == "video";
            var isAudio = type == "audio";

            MediaVideoInput.Visible = isVideo;
            MediaAudioInput.Visible = isAudio;
            MediaVideoOutput.Visible = isVideo;
            MediaAudioOutput.Visible = isAudio;
        }

        #endregion

        #region Set/Get

        public void SetData(QExperience entity)
        {
            MediaName.Text = entity.MediaEvidenceName;
            MediaType.Value = entity.MediaEvidenceType;

            IsInitialFileRemoved = false;
            InitialFile = entity.MediaEvidenceFileIdentifier.HasValue
                ? ServiceLocator.StorageService.GetFile(entity.MediaEvidenceFileIdentifier.Value)
                : null;

            SetFileUrl(InitialFile);

            OnMediaTypeChanged();
        }

        public IExperienceMediaFieldData GetData(QExperience experience)
        {
            Guid? fileId = null;

            if (InitialFile != null)
            {
                if (IsInitialFileRemoved)
                    DeleteInitialFile();
                else
                    fileId = InitialFile.FileIdentifier;
            }

            if (UploadedFile != null)
            {
                if (!fileId.HasValue)
                {
                    IsInitialFileRemoved = false;
                    InitialFile = FileUploadV2.SaveFile(UploadedFile, experience.ExperienceIdentifier, FileObjectType.LogbookExperience, UploadedFile.FileName);
                    UploadedFile = null;

                    fileId = InitialFile.FileIdentifier;

                    SetFileUrl(InitialFile);
                }
                else
                    DeleteUploadedFile();
            }

            var result = new ResultData();
            if (!fileId.HasValue)
                return result;

            result.Name = MediaName.Text;
            result.Type = MediaType.Value;
            result.FileIdentifier = fileId.Value;

            return result;
        }

        #endregion

        #region Event handlers

        private void MediaVideoInput_MediaCaptured(object sender, EventArgs e)
        {
            DeleteUploadedFile();

            IsInitialFileRemoved = true;
            UploadedFile = MediaVideoInput.VideoCapture.Save(TempObjectId, FileObjectType.Temporary);

            SetFileUrl(UploadedFile);
        }

        private void MediaAudioInput_MediaCaptured(object sender, EventArgs e)
        {
            DeleteUploadedFile();

            IsInitialFileRemoved = true;
            UploadedFile = MediaAudioInput.AudioCapture.Save(TempObjectId, FileObjectType.Temporary);

            SetFileUrl(UploadedFile);
        }

        private void MediaInput_MediaCaptureFailed(object sender, StringValueArgs e)
        {
            var message = "An error occurred during uploading the media file: " + e.Value.IfNullOrEmpty("Unknown Error");

            var envName = ServiceLocator.AppSettings.Environment.Name;
            if (envName == EnvironmentName.Local || envName == EnvironmentName.Development)
            {
                var fileUrl = TryGetMediaDebugUrl(sender);
                if (fileUrl != null)
                    message += "\nFile temporary URL: " + PathHelper.ToAbsoluteUrl(fileUrl);
            }

            ShowClientMessage(message);
        }

        private void OnMediaTypeChanged()
        {
            DeleteUploadedFile();
            SetMediaVisibility(MediaType.Value);
        }

        private void FieldValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var isRequired = IsRequired;
            var hasFile = InitialFile != null && !IsInitialFileRemoved || UploadedFile != null;
            var isNameFailed = MediaName.Text.IsEmpty() && (isRequired || hasFile);
            var isFileFailed = !hasFile && isRequired;

            if (args.IsValid = !isNameFailed && !isFileFailed)
                return;

            var text = string.Empty;

            if (isNameFailed)
            {
                text += "Name";

                if (isFileFailed)
                    text += " and ";
            }

            if (isFileFailed)
            {
                if (MediaVideoInput.Visible)
                    text += "Video";
                else
                    text += "Audio";
            }

            FieldValidator.ErrorMessage = "Required field: Evidence " + text;
        }

        #endregion

        #region Helper methods

        private void ShowClientMessage(string message)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Media),
                "show_message",
                $"alert({HttpUtility.JavaScriptStringEncode(message, true)});",
                true);
        }

        private void SetFileUrl(FileStorageModel model)
        {
            FileUrl.Value = model != null
                ? ServiceLocator.StorageService.GetFileUrl(model, false)
                : null;
        }

        private void DeleteInitialFile()
        {
            if (InitialFile == null)
                return;

            ServiceLocator.StorageService.Delete(InitialFile.FileIdentifier);
            InitialFile = null;
        }

        private void DeleteUploadedFile()
        {
            if (UploadedFile == null)
                return;

            ServiceLocator.StorageService.Delete(UploadedFile.FileIdentifier);
            UploadedFile = null;
        }

        private static string TryGetMediaDebugUrl(object sender)
        {
            FileStorageModel file;

            try
            {
                file = sender is InputVideo video
                    ? video.VideoCapture.Save(TempObjectId, FileObjectType.Temporary, checkFileValid: false)
                    : sender is InputAudio audio
                        ? audio.AudioCapture.Save(TempObjectId, FileObjectType.Temporary, checkFileValid: false)
                        : null;
            }
            catch
            {
                // Since we know the capture is invalid, the Save method may throw an exception. Ignore it.

                file = null;
            }

            return file != null
                ? ServiceLocator.StorageService.GetFileUrl(file, true)
                : null;
        }

        #endregion
    }
}