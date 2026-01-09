using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class TranslationControl : BaseUserControl
    {
        #region Constants

        public const string CustomLabel = "Custom Translation for <strong>{0}</strong>";

        #endregion

        #region Classes

        private class TranslationItem
        {
            public string LanguageCode { get; set; }
            public string Text { get; set; }
            public string Html => string.IsNullOrEmpty(Text) ? string.Empty : Markdown.ToHtml(Text);
        }

        #endregion

        #region Properties

        public string DefaultLanguage
        {
            get => (string)(ViewState[nameof(DefaultLanguage)] ?? (ViewState[nameof(DefaultLanguage)] = "en"));
            set
            {
                var lang = value;

                if (!string.IsNullOrEmpty(lang))
                {
                    lang = lang.Split(new[] { ':' })[0];

                    if (!Language.CodeExists(lang))
                        throw new ApplicationError($"Invalid language code: {value}");
                }

                ViewState[nameof(DefaultLanguage)] = lang;
            }
        }

        public bool AllowOrganizationSpecific
        {
            get => (bool?)ViewState[nameof(AllowOrganizationSpecific)] ?? true;
            set => ViewState[nameof(AllowOrganizationSpecific)] = value;
        }

        public string[] IncludeLanguage
        {
            get => (string[])ViewState[nameof(IncludeLanguage)];
            set => ViewState[nameof(IncludeLanguage)] = value;
        }

        private MultilingualString Translations
        {
            get => (MultilingualString)ViewState[nameof(Translations)];
            set => ViewState[nameof(Translations)] = value;
        }

        public bool Required
        {
            get => RequiredValidator.Enabled;
            set => RequiredValidator.Enabled = value;
        }

        public Shift.Constant.TextBoxMode TextMode
        {
            get => (Shift.Constant.TextBoxMode)(base.ViewState[nameof(TextMode)] ?? Shift.Constant.TextBoxMode.MultiLine);
            set => ViewState[nameof(TextMode)] = value;
        }

        private string SelectedLanguage => TranslatedLanguageSelector.Value;

        public bool ShowExcludedLanguage
        {
            get => (bool?)ViewState[nameof(ShowExcludedLanguage)] ?? false;
            set => ViewState[nameof(ShowExcludedLanguage)] = value;
        }

        private string SelectedOrganizationCode
        {
            get
            {
                var organization = CurrentSessionState.Identity.Organization;
                if (!AllowOrganizationSpecific || organization.OrganizationIdentifier == OrganizationIdentifiers.Global)
                    return null;
                return organization.Code;
            }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            TranslatedLanguageSelector.AutoPostBack = true;
            TranslatedLanguageSelector.ValueChanged += TranslatedLanguageSelector_ValueChanged;

            RequestGoogleTranslation.Click += GoogleTranslationRequested;

            SelectButton.Click += SelectButton_Click;

            OtherTranslations.ItemDataBound += OtherTranslations_ItemDataBound;
            OtherTranslations.ItemCommand += OtherTranslations_ItemCommand;

            CommonScript.ContentKey = typeof(TranslationControl).ToString();

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            IsOrganizationSpecific.Text = string.Format(CustomLabel, Organization.CompanyName);
            IsOrganizationSpecific.Visible = Organization.OrganizationIdentifier != OrganizationIdentifiers.Global;

            DefaultText.Visible = !AllowOrganizationSpecific || Organization.OrganizationIdentifier == OrganizationIdentifiers.Global;
            DefaultLiteral.Visible = !DefaultText.Visible;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            OrganizationSpecificRow.Visible = AllowOrganizationSpecific;

            DefaultText.TextMode = TranslatedText.TextMode = TextMode;

            if (TextMode == Shift.Constant.TextBoxMode.MultiLine)
            {
                DefaultText.Height = TranslatedText.Height = Unit.Pixel(100);

                ScriptManager.RegisterStartupScript(
                    base.Page,
                    typeof(TranslationControl),
                    "init_" + base.ClientID,
                    $@"
translationControl.initEditor('{DefaultText.ClientID}');
translationControl.initEditor('{TranslatedText.ClientID}');",
                    true);
            }
            else
            {
                DefaultText.Height = TranslatedText.Height = Unit.Empty;
            }
        }

        #endregion

        #region Methods (event handling)

        private void GoogleTranslationRequested(object sender, EventArgs e)
        {
            Translations[SelectedLanguage, SelectedOrganizationCode] = Translate(DefaultLanguage, SelectedLanguage, DefaultText.Text);
            TranslatedText.Text = Translations[SelectedLanguage, SelectedOrganizationCode];
        }

        private void TranslatedLanguageSelector_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            LanguageSelected(e.OldValue, e.NewValue);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            var oldLanguage = TranslatedLanguageSelector.Value;
            var oldOrganization = SelectedOrganizationCode;

            var newValues = SelectedLanguageCode.Value.Split(new[] { ':' });
            var newLanguage = newValues[0];
            var newOrganization = newValues.Length == 2 ? newValues[1] : null;

            LanguageSelected(oldLanguage, oldOrganization, newLanguage, newOrganization);
        }

        private void OtherTranslations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (TranslationItem)e.Item.DataItem;

            var selectButton = (LinkButton)e.Item.FindControl("SelectLinkButton");
            var selectLiteral = (System.Web.UI.WebControls.Literal)e.Item.FindControl("SelectLiteral");
            var deleteButton = (IconButton)e.Item.FindControl("DeleteTranslationCommand");

            var global = Organization.Identifier == OrganizationIdentifiers.Global;
            var supported = Organization.Languages.Any(x => item.LanguageCode.StartsWith(x.TwoLetterISOLanguageName));

            if (!AllowOrganizationSpecific || global)
                selectButton.Visible = item.LanguageCode.Length == 2;
            else
                selectButton.Visible = supported && item.LanguageCode.EndsWith(":" + Organization.Code);

            selectButton.OnClientClick = string.Format($"$get('{SelectedLanguageCode.ClientID}').value = '{item.LanguageCode}'; __doPostBack('{SelectButton.UniqueID}', ''); return false;");

            selectLiteral.Visible = !selectButton.Visible;

            if (!AllowOrganizationSpecific || global)
                deleteButton.Visible = item.LanguageCode.Length == 2;
            else
                deleteButton.Visible = supported && item.LanguageCode.EndsWith(":" + Organization.Code);
        }

        private void OtherTranslations_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteTranslation")
            {
                var languageCode = (string)e.CommandArgument;

                var strs = languageCode.Split(':');
                var language = strs[0];

                if (strs.Length == 2)
                {
                    var organization = strs[1];

                    Translations[language, organization] = null;
                }
                else
                {
                    Translations[language] = null;
                }

                ShowOtherTranslations();
            }
        }

        #endregion

        #region Methods (data binding)

        public string GetTranslation()
        {
            var global = Organization.Identifier == OrganizationIdentifiers.Global;

            if (!AllowOrganizationSpecific || global)
            {
                Translations[DefaultLanguage] = DefaultText.Text;

                if (DefaultLanguage != SelectedLanguage)
                    Translations[SelectedLanguage] = TranslatedText.Text;
            }
            else
            {
                Translations[SelectedLanguage, SelectedOrganizationCode] = TranslatedText.Text;
            }

            return Translations.Serialize();
        }

        public void SetDefault() => SetTranslation(string.Empty, null);

        public void SetTranslation(string translation, string language)
        {
            Translations = MultilingualString.Deserialize(translation);

            DefaultLanguageSelector.Value = DefaultLanguage;

            BindTranslatedLanguage(SelectedOrganizationCode);

            DefaultText.Text = Translations[DefaultLanguage];
            DefaultLiteral.InnerHtml = Markdown.ToHtml(Translations[DefaultLanguage]);

            LanguageSelected(null, null, language ?? SelectedLanguage, SelectedOrganizationCode);

            ShowOtherTranslations();
        }

        private void ShowOtherTranslations()
        {
            var selectedKey = string.IsNullOrEmpty(SelectedLanguage) ? null : MultilingualString.GetKey(SelectedLanguage, SelectedOrganizationCode);
            var list = new List<TranslationItem>();

            foreach (var language in Translations.Languages)
            {
                if (language != DefaultLanguage && (selectedKey == null || language != selectedKey))
                    list.Add(new TranslationItem { LanguageCode = language, Text = Translations[language] });
            }

            OtherTranslations.DataSource = list;
            OtherTranslations.DataBind();
        }

        private void BindTranslatedLanguage(string organization)
        {
            if (organization.IsEmpty())
                organization = Organization.Code;

            var value = TranslatedLanguageSelector.Value;

            if (!ShowExcludedLanguage)
                TranslatedLanguageSelector.Settings.ExcludeLanguage = DefaultLanguageSelector.Value.IsNotEmpty()
                    ? new[] { DefaultLanguageSelector.Value }
                    : new string[0];

            TranslatedLanguageSelector.Settings.IncludeLanguage = IncludeLanguage ?? (!string.IsNullOrEmpty(organization)
                ? OrganizationSearch.Select(organization).Languages.Select(x => x.TwoLetterISOLanguageName).ToArray()
                : new string[0]);

            TranslatedLanguageSelector.RefreshData();

            if (!string.IsNullOrEmpty(value) && (TranslatedLanguageSelector.Settings.ExcludeLanguage == null || TranslatedLanguageSelector.Settings.ExcludeLanguage.Contains(DefaultLanguage)))
                TranslatedLanguageSelector.Value = value;
        }

        private void LanguageSelected(string oldValue, string newValue)
        {
            LanguageSelected(oldValue, SelectedOrganizationCode, newValue, SelectedOrganizationCode);
        }

        private void LanguageSelected(string oldValue, string oldOrganization, string newValue, string newOrganization)
        {
            if (Translations == null)
                Translations = new MultilingualString();

            if (!string.IsNullOrEmpty(oldValue))
                Translations[oldValue, oldOrganization] = TranslatedText.Text;

            BindTranslatedLanguage(newOrganization);

            if (TranslatedLanguageSelector.Settings.IncludeLanguage.IsNotEmpty() && !TranslatedLanguageSelector.Settings.IncludeLanguage.Contains(newValue))
                newValue = TranslatedLanguageSelector.Settings.IncludeLanguage.First();

            if (TranslatedLanguageSelector.Settings.ExcludeLanguage.IsNotEmpty() && TranslatedLanguageSelector.Settings.ExcludeLanguage.Contains(newValue))
                newValue = null;

            var translationEnabled = newValue != DefaultLanguage && !string.IsNullOrEmpty(newValue);

            RequestGoogleTranslation.Enabled = translationEnabled;
            RequestGoogleTranslation.Visible = RequestGoogleTranslation.Enabled;

            if (TranslatedLanguageSelector.Items.Count > 0)
            {
                TranslationColumn.Visible = true;

                TranslatedLanguageSelector.Enabled = true;
                TranslatedLanguageSelector.Value = newValue;

                if (newValue.IsNotEmpty())
                {
                    TranslatedText.Text = Translations[newValue, newOrganization];
                    TranslatedTextRow.Visible = true;
                }
                else
                {
                    TranslatedText.Text = string.Empty;
                    TranslatedTextRow.Visible = false;
                }

                IsOrganizationSpecific.Checked = SelectedOrganizationCode != null;
                IsOrganizationSpecific.Text = string.Format(CustomLabel, CurrentSessionState.Identity.Organization.CompanyName);
            }
            else
            {
                TranslationColumn.Visible = false;
            }

            ShowOtherTranslations();
        }

        #endregion
    }
}