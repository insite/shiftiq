using System;
using System.Web;
using System.Web.UI;

using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionTextEditor : BaseUserControl
    {
        #region Classes

        [Serializable]
        protected class ControlData
        {
            #region Properties

            public Guid OrganizationIdentifier { get; private set; }
            public Guid BankID { get; private set; }
            public Guid? QuestionID { get; private set; }
            public int BankAsset { get; private set; }

            #endregion

            #region Construction

            public ControlData(BankState bank)
            {
                OrganizationIdentifier = bank.Tenant;
                BankID = bank.Identifier;
                BankAsset = bank.Asset;
            }

            public ControlData(Question question)
                : this(question.Set.Bank)
            {
                QuestionID = question.Identifier;
            }

            #endregion
        }

        #endregion

        #region Properties

        protected ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)];
            private set => ViewState[nameof(CurrentData)] = value;
        }

        public MultilingualString Text
        {
            get => EditorTranslation.Text;
            set => EditorTranslation.Text = value;
        }

        public bool AllowTranslation
        {
            get => EditorTranslation.AllowRequestTranslation;
            set => EditorTranslation.AllowRequestTranslation = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EditorUpload.Custom += EditorUpload_Custom;

            ImageSelectorRefresh.Click += ImageSelectorRefresh_Click;

            CommonScript.ContentKey = typeof(QuestionTextEditor).FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "init_" + ClientID,
                $"questionTextEditor.init(" +
                $"{HttpUtility.JavaScriptStringEncode(ClientID, true)}" +
                $",{HttpUtility.JavaScriptStringEncode(QuestionText.ClientID, true)}" +
                $",{HttpUtility.JavaScriptStringEncode(EditorTranslation.ClientID, true)}" +
                $",{HttpUtility.JavaScriptStringEncode(ImageSelectorWindow.ClientID, true)}" +
                $",{HttpUtility.JavaScriptStringEncode(ImageSelectorRefresh.UniqueID, true)}" +
                $");",
                true);
        }

        #endregion

        #region Event handlers

        private void EditorUpload_Custom(object sender, Common.Web.UI.EditorUpload.CustomEventArgs args)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(args.File.FileName);
            var extension = System.IO.Path.GetExtension(args.File.FileName);
            var filename = StringHelper.ToIdentifier(name) + extension;

            var attachmentEntity = Attachments.Forms.Add.AttachFile(CurrentData.BankID, CurrentData.BankAsset, filename, filename, args.File.InputStream);
            var uploadEntity = UploadSearch.Select(attachmentEntity.Upload);

            QuestionText.SetupCallback(args.Callback, uploadEntity.Name, FileHelper.GetUrl(uploadEntity.NavigateUrl));
        }

        private void ImageSelectorRefresh_Click(object sender, EventArgs e)
        {
            ImageSelectorRepeater.LoadData(CurrentData.BankID);

            var hasImages = !ImageSelectorRepeater.IsEmpty;

            ImageSelectorRepeater.Visible = hasImages;
            ImageSelectorEmptyMessage.Visible = !hasImages;

            if (hasImages)
                ScriptManager.RegisterStartupScript(
                    Page,
                    GetType(),
                    "initimgs_" + ClientID,
                    $"questionTextEditor.initImgSelector(" +
                    $"{HttpUtility.JavaScriptStringEncode(QuestionText.ClientID, true)}" +
                    $",{HttpUtility.JavaScriptStringEncode(ImageSelectorWindow.ClientID, true)}" +
                    $");",
                    true);
        }

        #endregion

        #region Methods

        public void LoadData(BankState bank)
        {
            CurrentData = new ControlData(bank);
        }

        public void LoadData(Question question)
        {
            CurrentData = new ControlData(question);
        }

        #endregion
    }
}