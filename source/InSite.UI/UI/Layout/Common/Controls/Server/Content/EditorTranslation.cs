using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    [ValidationProperty(nameof(DefaultText))]
    public class EditorTranslation : BaseControl
    {
        #region Constants

        private const string DefaultLanguage = "en";
        protected static readonly string LanguagesJson = JsonHelper.SerializeJsObject(
            Shift.Common.Language.GetAllInfo().ToDictionary(x => x.Code, x => x.Name));

        #endregion

        #region Events

        public class RequestEventArgs : EventArgs
        {
            #region Properties

            public string FromLanguage { get; }

            public string[] ToLanguages { get; }

            #endregion

            #region Construction

            public RequestEventArgs(string fromLanguage, string[] toLanguages)
            {
                FromLanguage = fromLanguage;
                ToLanguages = toLanguages;
            }

            #endregion
        }

        public delegate void RequestEventHandler(object sender, RequestEventArgs args);

        public event RequestEventHandler Requested;

        private void OnRequested(string fromLang, string[] toLangs) =>
            Requested?.Invoke(this, new RequestEventArgs(fromLang, toLangs));

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsSettings
        {
            [JsonProperty(PropertyName = "id")]
            public string ControlID { get; }

            [JsonProperty(PropertyName = "output")]
            public string OutputID { get; set; }

            [JsonProperty(PropertyName = "container")]
            public string ContainerID { get; set; }

            [JsonProperty(PropertyName = "state")]
            public StateInfo State { get; set; }

            [JsonProperty(PropertyName = "isMd")]
            public bool IsMarkdown { get; set; }

            [JsonProperty(PropertyName = "changed")]
            public bool IsChanged { get; set; }

            [JsonProperty(PropertyName = "onSetText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnSetText { get; set; }

            [JsonProperty(PropertyName = "onGetText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnGetText { get; set; }

            public JsSettings(string clientId)
            {
                ControlID = clientId;
            }
        }

        [Serializable]
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class StateInfo
        {
            #region Properties

            [JsonProperty(PropertyName = "lang")]
            public string Language
            {
                get => _lang;
                set => _lang = value.IsNotEmpty() && Shift.Common.Language.CodeExists(value) ? value : DefaultLanguage;
            }

            [JsonProperty(PropertyName = "data")]
            public MultilingualString Text
            {
                get => _text;
                set => _text = EnsureOrganizationLanguages(value);
            }

            #endregion

            #region Fields

            private string _lang;
            private MultilingualString _text;

            #endregion

            #region Construction

            public StateInfo()
            {
                Language = CurrentSessionState.Identity.Language;
                Text = null;
            }

            #endregion

            #region Methods

            public StateInfo Clone()
            {
                return new StateInfo
                {
                    _text = _text.Clone(),
                    _lang = _lang
                };
            }

            public bool IsEqual(StateInfo other)
            {
                return this._lang.Equals(other._lang, StringComparison.OrdinalIgnoreCase)
                    && this._text.IsEqual(other._text);
            }

            private static MultilingualString EnsureOrganizationLanguages(MultilingualString value)
            {
                var result = value != null ? value.Clone() : new MultilingualString();

                foreach (var lang in CurrentSessionState.Identity.Organization.Languages.Select(x => x.Name))
                {
                    if (!result.Languages.Contains(lang))
                        result[lang] = string.Empty;
                }

                return result;
            }

            #endregion
        }

        #endregion

        #region Properties

        public string TableContainerID
        {
            get => (string)ViewState[nameof(TableContainerID)];
            set => ViewState[nameof(TableContainerID)] = value;
        }

        public bool EnableMarkdownConverter
        {
            get => (bool)(ViewState[nameof(EnableMarkdownConverter)] ?? false);
            set => ViewState[nameof(EnableMarkdownConverter)] = value;
        }

        public bool TranslateDisabled
        {
            get => (bool)(ViewState[nameof(TranslateDisabled)] ?? false);
            set => ViewState[nameof(TranslateDisabled)] = value;
        }

        public bool AllowRequestTranslation
        {
            get => !TranslateDisabled && (bool)(ViewState[nameof(AllowRequestTranslation)] ?? true);
            set => ViewState[nameof(AllowRequestTranslation)] = value;
        }

        private StateInfo ClientState
        {
            get
            {
                if (_clientState == null)
                    LoadClientState(null);

                return _clientState;
            }
        }

        public MultilingualString Text
        {
            get => ClientState.Text;
            set => ClientState.Text = value;
        }

        public string DefaultText
        {
            get => ClientState.Text.Default;
        }

        public string Language
        {
            get => ClientState.Language;
            set => ClientState.Language = value;
        }

        protected CultureInfo[] TranslateToLanguages
        {
            get
            {
                var value = (CultureInfo[])Context.Items[_translateToLanguagesKey];

                if (value == null)
                    Context.Items[_translateToLanguagesKey] = value = CurrentSessionState.Identity.Organization.Languages.Where(x => x.Name != DefaultLanguage).ToArray();

                return value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EditorTranslationClientEvents ClientEvents => _clientEvents;

        private string OutputId => ClientID + "_Output";

        private string InternalTableContainerId => ClientID + "_Table";

        #endregion

        #region Fields

        private static readonly string _translateToLanguagesKey = typeof(EditorTranslation).FullName + "." + nameof(TranslateToLanguages);

        private Button _requestButton;

        private bool _isAllowTranslate = false;
        private bool _isInternalTableContainer = false;
        private string _translationLanguages;
        private StateInfo _clientState = null;
        private StateInfo _clientStateOnLoad = null;

        private EditorTranslationClientEvents _clientEvents;

        #endregion

        #region Construction

        public EditorTranslation()
        {
            _clientEvents = new EditorTranslationClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region ViewState

        protected override void LoadViewState(object savedState)
        {
            var list = (object[])savedState;
            var savedClientState = list != null ? (StateInfo)list[0] : null;
            var otherSavedState = list != null ? list[1] : null;

            if (_clientState == null)
                LoadClientState(savedClientState);

            base.LoadViewState(otherSavedState);
        }

        private void LoadClientState(StateInfo savedClientState)
        {
            var formValue = Page.Request.Form[UniqueID];

            _clientState = formValue.IsNotEmpty()
                ? JsonConvert.DeserializeObject<StateInfo>(formValue)
                : savedClientState;

            if (_clientState == null)
                _clientState = new StateInfo();
            else if (savedClientState != null)
                _clientStateOnLoad = _clientState.Clone();
        }

        protected override object SaveViewState()
        {
            return new object[]
            {
                _clientState,
                base.SaveViewState()
            };
        }

        #endregion

        #region Initialization

        protected override void CreateChildControls()
        {
            Controls.Add(_requestButton = new Button
            {
                ButtonStyle = ButtonStyle.Default,
                Size = ButtonSize.ExtraSmall,
                ToolTip = "Request Google Translation",
                Icon = "fas fa-globe",
                Text = "Translate"
            });
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();

            _requestButton.Visible = TranslateToLanguages.Length > 0;
            _requestButton.Click += RequestButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            _translationLanguages = string.Join(", ", TranslateToLanguages.Select(x => x.DisplayName));
            _requestButton.OnClientClick =
                $"if (!confirm('Are you sure you want to translate this content from English to {_translationLanguages}?')) return false;";

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            var settings = new JsSettings(ClientID)
            {
                OutputID = OutputId,
                IsMarkdown = EnableMarkdownConverter,
                State = ClientState,
                IsChanged = _clientStateOnLoad == null || !ClientState.IsEqual(_clientStateOnLoad),
                OnSetText = ClientEvents.OnSetText.NullIfEmpty(),
                OnGetText = ClientEvents.OnGetText.NullIfEmpty()
            };

            _isAllowTranslate = !TranslateDisabled && TranslateToLanguages.Length > 0;
            _isInternalTableContainer = TableContainerID.IsEmpty();

            _requestButton.Visible = AllowRequestTranslation && _isAllowTranslate;

            settings.ContainerID = !_isAllowTranslate
                ? null
                : _isInternalTableContainer
                    ? InternalTableContainerId
                    : (NamingContainer.FindControl(TableContainerID)?.ClientID)
                        ?? throw ApplicationError.Create("Table container not found: {0}", TableContainerID);

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "setup",
                $"inSite.common.editorTranslation.setup({LanguagesJson});",
                true);

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "refresh_" + ClientID,
                $"inSite.common.editorTranslation.init({JsonHelper.SerializeJsObject(settings)});",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void RequestButton_Click(object sender, EventArgs e)
        {
            var fromLang = DefaultLanguage;

            if (Text[fromLang].IsNotEmpty())
            {
                var toLangs = TranslateToLanguages.Select(x => x.Name).ToArray();
                foreach (var toLang in toLangs)
                    ((IHasTranslator)Page).Translator.Translate(fromLang, toLang, Text);

                OnRequested(fromLang, toLangs);
            }
            else
            {
                ScriptManager.RegisterStartupScript(
                    Page,
                    GetType(),
                    "request_message",
                    $"alert({HttpUtility.JavaScriptStringEncode("Error: the source language has no content.", true)});",
                    true);
            }
        }

        #endregion

        #region Rendering

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);

            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            if (_isAllowTranslate)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, OutputId);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-default btn-xs lang-out");
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, $"alert('Supported Languages: {_translationLanguages}');");

                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.RenderEndTag();

                writer.Write(" ");
            }

            _requestButton.RenderControl(writer);

            if (_isInternalTableContainer && _isAllowTranslate)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, InternalTableContainerId);

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderEndTag();
            }
        }

        #endregion
    }
}