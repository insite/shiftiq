using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [SupportsEventValidation, ValidationProperty(nameof(Value))]
    public abstract class BaseDateTimeSelector<TValue> : BaseControl, IBaseDateTimeSelector, IPostBackDataHandler, IHasEmptyMessage
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class JsonOptions
        {
            [JsonProperty(PropertyName = "id")]
            public string ClientID { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string UniqueID { get; set; }

            [JsonProperty(PropertyName = "class", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string CssClass { get; set; }

            [JsonProperty(PropertyName = "style", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string CssStyle { get; set; }

            [JsonProperty(PropertyName = "placeholder", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string EmptyMessage { get; set; }

            [JsonProperty(PropertyName = "readOnly", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ReadOnly { get; set; }

            [JsonProperty(PropertyName = "disabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool Disabled { get; set; }

            [JsonProperty(PropertyName = "enableDate", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ShowDate { get; set; }

            [JsonProperty(PropertyName = "enableTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ShowTime { get; set; }

            [JsonProperty(PropertyName = "enableTz", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ShowTimeZone { get; set; }

            [JsonProperty(PropertyName = "events")]
            public JsonEvents Events { get; set; }

            [JsonProperty(PropertyName = "data", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public JsonOutput Data { get; set; }

            [JsonProperty(PropertyName = "preset", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Preset { get; set; }

            public bool ShouldSerializeEvents() => Events != null && !Events.IsEmpty;
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class JsonEvents
        {
            [JsonProperty(PropertyName = "blur", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnBlur { get; set; }

            [JsonProperty(PropertyName = "click", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnClick { get; set; }

            [JsonProperty(PropertyName = "focus", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnFocus { get; set; }

            [JsonProperty(PropertyName = "keydown", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnKeyDown { get; set; }

            [JsonProperty(PropertyName = "keypress", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnKeyPress { get; set; }

            [JsonProperty(PropertyName = "keyup", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnKeyUp { get; set; }

            [JsonProperty(PropertyName = "change", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnChange { get; set; }

            public bool IsEmpty =>
                OnBlur == null &&
                OnClick == null &&
                OnFocus == null &&
                OnKeyDown == null &&
                OnKeyPress == null &&
                OnKeyUp == null &&
                OnChange == null;
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected abstract class JsonOutput
        {
            public TValue Value { get; set; }

            [JsonProperty(PropertyName = "fullFormat")]
            public string FullDateFormat { get; set; }

            [JsonProperty(PropertyName = "shortFormat")]
            public string ShortDateFormat { get; set; }

            [JsonProperty(PropertyName = "date")]
            protected abstract string DateInternal { get; }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected abstract class JsonInput
        {
            public abstract TValue Value { get; }

            [JsonProperty(PropertyName = "value")]
            protected string StringValue { get; set; }

            public abstract bool IsChanged(TValue value);
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;
        private void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        #endregion

        #region Properties

        public bool AutoPostBack
        {
            get { return ViewState[nameof(AutoPostBack)] != null && (bool)ViewState[nameof(AutoPostBack)]; }
            set { ViewState[nameof(AutoPostBack)] = value; }
        }

        public bool Enabled
        {
            get { return ViewState[nameof(Enabled)] == null || (bool)ViewState[nameof(Enabled)]; }
            set { ViewState[nameof(Enabled)] = value; }
        }

        public Unit Width
        {
            get { return (Unit)(ViewState[nameof(Width)] ?? Unit.Empty); }
            set { ViewState[nameof(Width)] = value; }
        }

        public Unit Height
        {
            get { return (Unit)(ViewState[nameof(Height)] ?? Unit.Empty); }
            set { ViewState[nameof(Height)] = value; }
        }

        public bool CausesValidation
        {
            get { return ViewState[nameof(CausesValidation)] != null && (bool)ViewState[nameof(CausesValidation)]; }
            set { ViewState[nameof(CausesValidation)] = value; }
        }

        public string ValidationGroup
        {
            get { return (string)(ViewState[nameof(ValidationGroup)] ?? string.Empty); }
            set { ViewState[nameof(ValidationGroup)] = value; }
        }

        public bool ReadOnly
        {
            get { return ViewState[nameof(ReadOnly)] != null && (bool)ViewState[nameof(ReadOnly)]; }
            set { ViewState[nameof(ReadOnly)] = value; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InputClientEvents ClientEvents => _clientEvents;

        public string EmptyMessage
        {
            get { return (string)ViewState[nameof(EmptyMessage)] ?? string.Empty; }
            set { ViewState[nameof(EmptyMessage)] = value; }
        }

        public abstract TValue Value { get; set; }

        public abstract bool HasValue { get; }

        #endregion

        #region Fields

        private InputClientEvents _clientEvents;
        private static readonly HashSet<string> _redundantAttributes = new HashSet<string>(new[]
        {
            "id",
            "class",
            "type",
            "name",
            "disabled",
            "readonly",
            "width",
            "height",
            "placeholder",

            "onblur",
            "onfocus",
            "onchange",
            "onclick",
            "onkeydown",
            "onkeypress",
            "onkeyup"
        }, StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Construction

        public BaseDateTimeSelector()
        {
            _clientEvents = new InputClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region Loading

        protected override void OnAttributeSet(AttributeEventArgs e)
        {
            if (!e.Cancel)
                e.Cancel = _redundantAttributes.Contains(e.Name);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!AutoPostBack)
                Page.ClientScript.RegisterForEventValidation(UniqueID);

            var options = GetJsonOptions();
            var optionsJson = JsonHelper.SerializeJsObject(options);

            PageFooterContentRenderer.RegisterScript(
                typeof(DateTimeOffsetSelector),
                "init_" + ClientID,
                $"inSite.common.dateTimeOffsetSelector.init({optionsJson});");

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return LoadPostData(postDataKey, postCollection);
        }

        private bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            var isChanged = false;

            var inputData = GetInputData(postCollection[postDataKey]);
            if (Visible && !ReadOnly && inputData != null)
            {
                isChanged = inputData.IsChanged(Value);
                if (isChanged)
                    Value = inputData.Value;
            }

            return isChanged;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            RaisePostDataChangedEvent();
        }

        private void RaisePostDataChangedEvent()
        {
            if (AutoPostBack && !Page.IsPostBackEventControlRegistered)
            {
                Page.AutoPostBackControl = this;

                if (CausesValidation)
                    Page.Validate(ValidationGroup);
            }

            OnValueChanged(EventArgs.Empty);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        #endregion

        #region Methods

        protected abstract JsonInput GetInputData(string json);

        protected abstract JsonOutput GetOutputData();

        protected virtual JsonOptions GetJsonOptions()
        {
            var outputData = GetOutputData();

            outputData.Value = Value;

            return new JsonOptions
            {
                ClientID = ClientID,
                UniqueID = UniqueID,
                CssClass = CssClass.NullIfEmpty(),
                CssStyle = GetCssStyle(),
                EmptyMessage = EmptyMessage.NullIfEmpty(),
                ReadOnly = ReadOnly,
                Disabled = !Enabled,
                Events = new JsonEvents
                {
                    OnBlur = ClientEvents.OnBlur.NullIfEmpty(),
                    OnClick = ClientEvents.OnClick.NullIfEmpty(),
                    OnFocus = ClientEvents.OnFocus.NullIfEmpty(),
                    OnKeyDown = ClientEvents.OnKeyDown.NullIfEmpty(),
                    OnKeyPress = ClientEvents.OnKeyPress.NullIfEmpty(),
                    OnKeyUp = ClientEvents.OnKeyUp.NullIfEmpty(),
                    OnChange = GetOnChangeScript()
                },
                Data = outputData,
            };
        }

        private string GetCssStyle()
        {
            var styles = Shift.Sdk.UI.CssStyleCollection.Parse(Attributes["style"]);

            if (!Width.IsEmpty)
                styles["width"] = Width.ToString();

            if (!Height.IsEmpty)
                styles["height"] = Height.ToString();

            return styles.IsEmpty ? null : styles.ToString();
        }

        private string GetOnChangeScript()
        {
            var onChangeScript = new StringBuilder();

            if (ClientEvents.OnChange.IsNotEmpty())
                ControlHelper.AddScript(onChangeScript, ClientEvents.OnChange);

            if (AutoPostBack)
            {
                var postBackOptions = GetPostBackOptions();
                var postBackEventReference = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);

                if (postBackEventReference.IsNotEmpty())
                {
                    ControlHelper.AddScript(onChangeScript, postBackEventReference);
                    if (postBackOptions.ClientSubmit)
                        ControlHelper.AddScript(onChangeScript, "return false;");
                }
            }

            return onChangeScript.Length == 0 ? null : onChangeScript.ToString();
        }

        private PostBackOptions GetPostBackOptions()
        {
            var result = new PostBackOptions(this, string.Empty)
            {
                AutoPostBack = AutoPostBack
            };

            if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
            {
                result.PerformValidation = true;
                result.ValidationGroup = ValidationGroup;
            }

            return result;
        }

        #endregion
    }
}