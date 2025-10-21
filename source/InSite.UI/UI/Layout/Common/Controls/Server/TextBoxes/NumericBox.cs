using System;
using System.Collections.Specialized;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true, "ValueAsText"), ValidationProperty("ValueAsText")]
    public class NumericBox : BaseInputBox, INumericBox
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ClientSideSettings
        {
            [JsonProperty(PropertyName = "min", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public decimal? MinValue => _input.MinValue;

            [JsonProperty(PropertyName = "max", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public decimal? MaxValue => _input.MaxValue;

            [JsonProperty(PropertyName = "decimals", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int? DecimalPlaces => _input.NumericMode == NumericBoxMode.Float ? _input.DecimalPlaces : (int?)null;

            [JsonProperty(PropertyName = "mode", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string NumericMode => _input.NumericMode == NumericBoxMode.Float ? null : _input.NumericMode.GetName().ToLower();

            [JsonProperty(PropertyName = "group", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool DigitGrouping => _input.DigitGrouping;

            private NumericBox _input;

            public ClientSideSettings(NumericBox input)
            {
                _input = input;
            }
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;

        private void OnValueChanged() => ValueChanged?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string ValueAsText
        {
            get => DecimalValueToText(ValueAsDecimal);
            set => ValueAsDecimal = TextValueToDecimalValue(value);
        }

        public int? ValueAsInt
        {
            get => (int?)ValueAsDecimal;
            set => ValueAsDecimal = value;
        }

        public decimal? ValueAsDecimal
        {
            get => (decimal?)ViewState[nameof(ValueAsDecimal)];
            set => ViewState[nameof(ValueAsDecimal)] = ValidateDecimalValue(value);
        }

        public decimal? MinValue
        {
            get
            {
                return (decimal?)ViewState[nameof(MinValue)];
            }
            set
            {
                if (value.HasValue)
                {
                    if (MaxValue.HasValue && MaxValue.Value < value)
                        throw ApplicationError.Create("MinValue must be less than MaxValue.");

                    if (ValueAsDecimal.HasValue && value.Value > ValueAsDecimal.Value)
                        ValueAsDecimal = value;
                }

                ViewState[nameof(MinValue)] = value;
            }
        }

        public decimal? MaxValue
        {
            get
            {
                return (decimal?)ViewState[nameof(MaxValue)];
            }
            set
            {
                if (value.HasValue)
                {
                    if (MinValue.HasValue && MinValue.Value > value)
                        throw ApplicationError.Create("MaxValue must be greater than MinValue.");

                    if (ValueAsDecimal.HasValue && value.Value < ValueAsDecimal.Value)
                        ValueAsDecimal = value;
                }

                ViewState[nameof(MaxValue)] = value;
            }
        }

        public NumericBoxMode NumericMode
        {
            get { return (NumericBoxMode)(ViewState[nameof(NumericMode)] ?? NumericBoxMode.Float); }
            set { ViewState[nameof(NumericMode)] = value; }
        }

        public int DecimalPlaces
        {
            get { return (int)(ViewState[nameof(DecimalPlaces)] ?? 2); }
            set
            {
                if (value < 1)
                    throw new ArgumentException("The value must be greater than zero", nameof(value));

                if (value > 10)
                    throw new ArgumentException("The value must be less than or equal to 10", nameof(value));

                ViewState[nameof(DecimalPlaces)] = value;
            }
        }

        public bool DigitGrouping
        {
            get => (bool)(ViewState[nameof(DigitGrouping)] ?? true);
            set => ViewState[nameof(DigitGrouping)] = value;
        }

        public override bool HasValue => ValueAsDecimal.HasValue;

        #endregion

        #region Loading

        protected override void AddParsedSubObject(object obj)
        {
            ValueAsText = (obj is LiteralControl literal)
                ? literal.Text
                : throw new ApplicationException("Unexpected child type: " + obj.GetType().Name);
        }

        #endregion

        #region Methods

        private decimal? ValidateDecimalValue(decimal? value)
        {
            if (value.HasValue)
            {
                if (MinValue.HasValue && MinValue.Value > value)
                    return MinValue.Value;

                if (MaxValue.HasValue && MaxValue.Value < value)
                    return MaxValue.Value;
            }

            return value;
        }

        private string DecimalValueToText(decimal? value)
        {
            if (!value.HasValue)
                return string.Empty;

            var format = DigitGrouping ? "#,##0" : "0";

            if (NumericMode == NumericBoxMode.Float)
                format += "." + new string('0', DecimalPlaces);
            else if (NumericMode != NumericBoxMode.Integer)
                throw ApplicationError.Create("Unexpected numeric mode: {0}", NumericMode.GetName());

            return value.Value.ToString(format);
        }

        private decimal? TextValueToDecimalValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            else if (decimal.TryParse(text, out var dValue))
                return dValue;
            else
                throw ApplicationError.Create("Invalid numeric value: " + text);
        }

        #endregion

        #region IPostBackDataHandler

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            var isChanged = false;

            if (Visible && !ReadOnly)
            {
                var value = TextValueToDecimalValue(postCollection[postDataKey]);
                value = ValidateDecimalValue(value);

                isChanged = value != ValueAsDecimal;

                if (isChanged)
                    ValueAsDecimal = value;
            }

            return isChanged;
        }

        protected override void RaisePostDataChangedEvent()
        {
            if (AutoPostBack && !Page.IsPostBackEventControlRegistered)
            {
                Page.AutoPostBackControl = this;

                if (CausesValidation)
                    Page.Validate(ValidationGroup);
            }

            OnValueChanged();
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("insite-numeric form-control", CssClass));
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");

            var jsonSettings = new ClientSideSettings(this);
            writer.AddAttribute("data-settings", JsonConvert.SerializeObject(jsonSettings));

            if (EmptyMessage.Length > 0)
                writer.AddAttribute("placeholder", EmptyMessage);

            if (ReadOnly)
                writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");

            if (!Enabled)
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

            if (TabIndex != 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString());

            AddStyleAttributes(writer);
            AddClientEventAttributes(writer);
            AddAttributesToRender(writer);

            if (ValueAsDecimal.HasValue)
                writer.AddAttribute(HtmlTextWriterAttribute.Value, ValueAsText);

            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        #endregion
    }
}