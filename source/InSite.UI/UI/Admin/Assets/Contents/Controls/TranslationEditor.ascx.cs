using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class TranslationEditor : UserControl
    {
        #region Classes

        [Serializable]
        private class TranslationItem
        {
            #region Properties

            public int ID { get; }
            public string Language { get; }
            public string Html { get; }

            #endregion

            #region Construction

            public TranslationItem(int id, string language, string text)
            {
                ID = id;
                Language = Shift.Common.Language.GetDisplayName(language);
                Html = string.IsNullOrEmpty(text)
                    ? string.Empty
                    : Markdown.ToHtml(text);
            }

            #endregion

            #region Methods

            public string GetIFrameID(string clientId) => $"{clientId}__frame{ID}";

            #endregion
        }

        #endregion

        #region Public methods

        private void SetTranslation(string json)
        {
            EnsureChildControls();

            TranslationJson = json;

            var translations = MultilingualString.Deserialize(json);
            var selectedLang = _nav.SelectedItem == null
                ? null :
                Language.GetCode(_nav.SelectedItem.Title);

            //var selectedIndex = 0;
            //if (_nav.SelectedItem != null)
            //{
            //    var langCode = Language.GetCode(_nav.SelectedItem.Title);
            //    if (!string.IsNullOrEmpty(langCode))
            //    {
            //        var foundIndex = translations.Languages.Any(x => x == langCode);
            //        if (foundIndex >= 0)
            //            selectedIndex = foundIndex;
            //    }
            //}

            Items.Clear();
            _nav.ClearItems();

            foreach (var lang in translations.Languages)
            {
                var item = new TranslationItem(Items.Count, lang, translations[lang]);
                Items.Add(item);

                AddNavItem(out var navItem, out var literal);

                navItem.Title = item.Language;
                literal.Text = $"<iframe id='{item.GetIFrameID(ClientID)}' style='display:none; width:100%; border:none;' scrolling='no'></iframe>";

                if (lang == selectedLang)
                    navItem.IsSelected = true;
            }
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

        private List<TranslationItem> Items => (List<TranslationItem>)(ViewState[nameof(Items)]
            ?? (ViewState[nameof(Items)] = new List<TranslationItem>()));

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

        #region Fields

        private Nav _nav;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RequiredValidator.ServerValidate += RequiredValidator_ServerValidate;

            TranslationWindow.LoadTranslation += TranslationWindow_LoadTranslation;
            TranslationWindow.SaveTranslation += TranslationWindow_SaveTranslation;

            UpdateButton.Click += UpdateButton_Click;

            CommonScript.ContentKey = typeof(TranslationEditor).ToString();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                EditButton.OnClientClick = TranslationWindow.GetOpenScript(null) + " return false;";
        }

        protected override void CreateChildControls()
        {
            _nav = new Nav
            {
                ItemType = NavItemType.Pills,
                ItemAlignment = NavItemAlignment.Vertical,
                ContentRendererID = TranslationContent.ID,
            };

            if (Items.Count > 0)
            {
                for (var i = 0; i < Items.Count; i++)
                    AddNavItem(out var navItem, out var literal);
            }

            NavContainer.Controls.Add(_nav);

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            var script = new StringBuilder();

            script
                .AppendFormat("translationEditor.init('{0}');", _nav.ClientID)
                .AppendLine();

            foreach (var item in Items)
            {
                script
                    .AppendFormat(
                        "translationEditor.setHtml('{0}',{1});",
                        item.GetIFrameID(ClientID),
                        HttpUtility.JavaScriptStringEncode(item.Html, true))
                    .AppendLine();
            }

            if (script.Length > 0)
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(TranslationEditor),
                    "init_" + ClientID,
                    script.ToString(),
                    true);

            base.OnPreRender(e);
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

        #region Helper methods

        private void AddNavItem(out NavItem navItem, out System.Web.UI.WebControls.Literal literal)
        {
            navItem = new NavItem();

            _nav.AddItem(navItem);

            literal = new System.Web.UI.WebControls.Literal();

            navItem.Controls.Add(literal);
        }

        #endregion
    }
}