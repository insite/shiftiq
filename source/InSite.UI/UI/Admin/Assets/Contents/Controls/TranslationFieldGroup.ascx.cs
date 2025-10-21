using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class TranslationFieldGroup : BaseUserControl
    {
        #region Classes

        [Serializable]
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class StateInfo
        {
            [JsonProperty(PropertyName = "lang")]
            public string Language { get; set; }

            [JsonProperty(PropertyName = "data")]
            public MultilingualString Data { get; set; }

            public StateInfo()
            {
                Language = CurrentSessionState.Identity.Language;
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsInitSettings
        {
            [JsonProperty(PropertyName = "langId")]
            public string LanguageOutputID { get; set; }

            [JsonProperty(PropertyName = "textId")]
            public string TranslationTextID { get; set; }

            [JsonProperty(PropertyName = "stateId")]
            public string StateInputID { get; set; }

            [JsonProperty(PropertyName = "state")]
            public string CurrentState { get; set; }
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

        private StateInfo ClientState
        {
            get => (StateInfo)(ViewState[nameof(ClientState)] ?? (ViewState[nameof(ClientState)] = new StateInfo()));
            set => ViewState[nameof(ClientState)] = value;
        }

        public string InputLanguage
        {
            get => ClientState.Language;
            set => ClientState.Language = value;
        }

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

                ClientStateRequiredValidator.ErrorMessage = string.IsNullOrEmpty(value)
                    ? string.Empty
                    : $"Required field: {value}";
            }
        }

        public MultilingualString Translation
        {
            get => ClientState.Data;
            set => ClientState.Data = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string TranslationJson
        {
            get => ClientState.Data.Serialize();
            set => ClientState.Data = MultilingualString.Deserialize(value);
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
            get => ClientStateRequiredValidator.Enabled;
            set
            {
                ClientStateRequiredValidator.Enabled = value;
                TranslationWindow.Required = value;
            }
        }

        public int InputRows
        {
            get => (int)(ViewState[nameof(InputRows)] ?? 3);
            set => ViewState[nameof(InputRows)] = value;
        }

        public Shift.Constant.TextBoxMode InputTextMode
        {
            get => (Shift.Constant.TextBoxMode)(base.ViewState[nameof(InputTextMode)] ?? Shift.Constant.TextBoxMode.MultiLine);
            set => ViewState[nameof(InputTextMode)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = GetType().FullName;
            CommonScript.ContentKey = GetType().FullName;

            EditButton.OnClientClick = TranslationWindow.GetOpenScript(null) + " return false;";

            ClientStateRequiredValidator.ServerValidate += ClientStateRequiredValidator_ServerValidate;

            TranslationWindow.LoadTranslation += TranslationWindow_LoadTranslation;
            TranslationWindow.SaveTranslation += TranslationWindow_SaveTranslation;

            UpdateButton.Click += UpdateButton_Click;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            ClientStateRequiredValidator.ValidationGroup = groupName;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!string.IsNullOrEmpty(StateInput.Value))
                ClientState = JsonConvert.DeserializeObject<StateInfo>(StateInput.Value);

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            StateInput.Value = JsonConvert.SerializeObject(ClientState);

            TranslationText.Rows = InputRows;
            TranslationText.TextMode = InputTextMode;
            TranslationWindow.TextMode = InputTextMode;

            var options = new JsInitSettings
            {
                LanguageOutputID = LanguageOutput.ClientID,
                TranslationTextID = TranslationText.ClientID,
                StateInputID = StateInput.ClientID
            };

            if (HttpRequestHelper.IsAjaxRequest)
                options.CurrentState = StateInput.Value;

            ScriptManager.RegisterStartupScript(Page, GetType(), "refresh_" + ClientID, $"translationField.init({JsonHelper.SerializeJsObject(options)});", true);

            base.OnPreRender(e);

            InputWrapper.Attributes["style"] = $"width:{Width}; padding: 0px;";
        }

        #endregion

        #region Event handlers

        private void ClientStateRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ClientState.Data.Any(x => !string.IsNullOrWhiteSpace(x.Value));
        }

        private void TranslationWindow_LoadTranslation(object sender, TranslationWindow.LoadEventArgs e)
        {
            TranslationWindow.DefaultLanguage = ClientState.Language.ToLower();
            e.Translation = TranslationJson;
        }

        private void TranslationWindow_SaveTranslation(object sender, StringValueArgs e)
        {
            TranslationJson = e.Value;

            var script = TranslationWindow.GetCloseScript();

            if (SaveTranslation != null)
                script += $" setTimeout(function () {{ $('#{UpdateButton.ClientID}').click(); }}, 0);";

            ScriptManager.RegisterStartupScript(Page, GetType(), "update_" + ClientID, script, true);
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            OnSaveTranslation();
        }

        #endregion
    }
}