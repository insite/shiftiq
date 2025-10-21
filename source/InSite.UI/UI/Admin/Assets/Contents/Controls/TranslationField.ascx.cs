using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class TranslationField : UserControl
    {
        #region Public methods

        public void SetTranslation(string translation)
        {
            TranslationJson = translation;

            var translations = MultilingualString.Deserialize(translation);
            var list = new List<TranslationItem>();

            foreach (var language in translations.Languages)
                list.Add(new TranslationItem { LanguageCode = language, Text = translations[language] });

            Translations.DataSource = list;
            Translations.DataBind();
        }

        #endregion

        #region Classes

        private class TranslationItem
        {
            public string LanguageCode { get; set; }
            public string Text { get; set; }
            public string Html => string.IsNullOrEmpty(Text) ? string.Empty : Markdown.ToHtml(Text);
        }

        #endregion

        #region Events

        public event EventHandler SaveTranslation;

        private void OnSaveTranslation()
        {
            SaveTranslation?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        public string HelpText
        {
            get => (string)ViewState[nameof(HelpText)];
            set => ViewState[nameof(HelpText)] = value;
        }

        public string LabelText
        {
            get => (string)ViewState[nameof(LabelText)];
            set
            {
                ViewState[nameof(LabelText)] = value;

                RequiredValidator.ErrorMessage = string.IsNullOrEmpty(value)
                    ? string.Empty
                    : $"Required field: {value}";
            }
        }

        public string TranslationJson
        {
            get => (string)ViewState[nameof(TranslationJson)];
            private set => ViewState[nameof(TranslationJson)] = value;
        }

        public bool AllowOrganizationSpecific
        {
            get => TranslationWindow.AllowOrganizationSpecific;
            set => TranslationWindow.AllowOrganizationSpecific = value;
        }

        public string[] IncludeLanguage
        {
            get => TranslationWindow.IncludeLanguage;
            set => TranslationWindow.IncludeLanguage = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Percentage(100));
            set => ViewState[nameof(Width)] = value;
        }

        public bool Required
        {
            get => RequiredValidator.Enabled;
            set
            {
                RequiredValidator.Enabled = value;
                TranslationWindow.Required = value;
            }
        }

        public string ValidationGroup
        {
            get => RequiredValidator.ValidationGroup;
            set => RequiredValidator.ValidationGroup = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RequiredValidator.ServerValidate += RequiredValidator_ServerValidate;

            TranslationWindow.LoadTranslation += TranslationWindow_LoadTranslation;
            TranslationWindow.SaveTranslation += TranslationWindow_SaveTranslation;

            UpdateButton.Click += UpdateButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                EditButton.OnClientClick = TranslationWindow.GetOpenScript(null) + " return false;";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ListPanel.Attributes["style"] = $"width:{Width.ToString()}; padding:0 5px 5px 5px;";
        }

        #endregion

        #region Event handlers

        private void RequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (!string.IsNullOrEmpty(TranslationJson))
            {
                var translations = MultilingualString.Deserialize(TranslationJson);

                args.IsValid = translations.Any(x => !string.IsNullOrWhiteSpace(x.Value));
            }
        }

        private void TranslationWindow_LoadTranslation(object sender, TranslationWindow.LoadEventArgs e)
        {
            e.Translation = TranslationJson;
        }

        private void TranslationWindow_SaveTranslation(object sender, StringValueArgs e)
        {
            SetTranslation(e.Value);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(TranslationEditor),
                "update",
                TranslationWindow.GetCloseScript() + $" $('#{UpdateButton.ClientID}').click();",
                true);
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            OnSaveTranslation();
        }

        #endregion
    }
}