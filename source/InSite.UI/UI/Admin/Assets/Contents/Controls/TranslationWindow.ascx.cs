using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common.Events;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class TranslationWindow : UserControl
    {
        #region Load Event

        public class LoadEventArgs : EventArgs
        {
            public string Key { get; set; }
            public string Translation { get; set; }
            public string Language { get; set; }
        }

        public delegate void LoadHandler(object sender, LoadEventArgs e);

        public event LoadHandler LoadTranslation;
        private LoadEventArgs OnLoadTranslation(string key)
        {
            var args = new LoadEventArgs { Key = key };

            LoadTranslation?.Invoke(this, args);

            return args;
        }

        #endregion

        #region Save Event

        public event StringValueHandler SaveTranslation;

        private void OnSaveTranslation(string translation) =>
            SaveTranslation?.Invoke(this, new StringValueArgs(translation));

        #endregion

        #region Properties

        public bool AllowOrganizationSpecific
        {
            get { return TranslationControl.AllowOrganizationSpecific; }
            set { TranslationControl.AllowOrganizationSpecific = value; }
        }

        public string[] IncludeLanguage
        {
            get { return TranslationControl.IncludeLanguage; }
            set { TranslationControl.IncludeLanguage = value; }
        }

        public bool Required
        {
            get => TranslationControl.Required;
            set
            {
                TranslationControl.Required = value;
                SaveButton.CausesValidation = value;
            }
        }

        public string DefaultLanguage
        {
            get => TranslationControl.DefaultLanguage;
            set => TranslationControl.DefaultLanguage = value;
        }

        public Shift.Constant.TextBoxMode TextMode
        {
            get => TranslationControl.TextMode;
            set => TranslationControl.TextMode = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UpdatePanel.Request += UpdatePanel_Request;

            SaveButton.Click += SaveButton_Click;

            CommonScript.ContentKey = typeof(TranslationWindow).ToString();
        }

        protected override void OnPreRender(EventArgs e)
        {
            Window.MinHeight = TextMode == Shift.Constant.TextBoxMode.MultiLine ? Unit.Pixel(500) : Unit.Pixel(215);

            base.OnPreRender(e);

            CloseButton.OnClientClick = GetCloseScript();
        }

        #endregion

        #region Event handlers

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            var translation = OnLoadTranslation(e.Value);

            TranslationControl.SetTranslation(translation.Translation, translation.Language);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Required && !Page.IsValid)
                return;

            OnSaveTranslation(TranslationControl.GetTranslation());
        }

        #endregion

        #region Public methods

        public string GetOpenScript(string key) => $"translationWindow.open('{Window.ClientID}','{key}');";

        public string GetCloseScript() => $"translationWindow.close('{Window.ClientID}');";

        #endregion
    }
}