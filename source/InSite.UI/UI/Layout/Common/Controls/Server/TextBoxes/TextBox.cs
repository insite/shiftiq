using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true, "Text"), ValidationProperty("Text")]
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public class TextBox : BaseInputBox, ITextBox
    {
        #region Events

        public event EventHandler TextChanged;
        private void OnTextChanged(EventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        #endregion

        #region Properties

        public bool AllowHtml
        {
            get => (bool)(ViewState[nameof(AllowHtml)] ?? false);
            set => ViewState[nameof(AllowHtml)] = value;
        }

        public bool EnableTrimText
        {
            get => (bool)(ViewState[nameof(EnableTrimText)] ?? true);
            set => ViewState[nameof(EnableTrimText)] = value;
        }

        [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string Text
        {
            get { return (string)(ViewState[nameof(Text)] ?? string.Empty); }
            set
            {
                var textValue = NormalizeNewLine(value);

                if (EnableTrimText && textValue.Length > 0)
                    textValue = textValue.Trim();

                if (!AllowHtml)
                    textValue = StringHelper.BreakHtml(textValue) ?? string.Empty;

                if (MaxLength > 0 && textValue.Length > MaxLength)
                    textValue = textValue.Substring(0, MaxLength);

                ViewState[nameof(Text)] = textValue;
            }
        }

        private string MaskedText
        {
            get => (string)ViewState[nameof(MaskedText)] ?? string.Empty;
            set => ViewState[nameof(MaskedText)] = value;
        }

        public TextBoxMode TextMode
        {
            get { return (TextBoxMode)(base.ViewState[nameof(TextMode)] ?? System.Web.UI.WebControls.TextBoxMode.SingleLine); }
            set { ViewState[nameof(TextMode)] = value; }
        }

        public int Columns
        {
            get { return (int)(ViewState[nameof(Columns)] ?? 20); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Columns));

                ViewState[nameof(Columns)] = value;
            }
        }

        public int Rows
        {
            get { return (int)(ViewState[nameof(Rows)] ?? 2); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Rows));

                ViewState[nameof(Rows)] = value;
            }
        }

        public int MaxLength
        {
            get { return (int)(ViewState[nameof(MaxLength)] ?? 0); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(MaxLength));

                ViewState[nameof(MaxLength)] = value;
            }
        }

        public string TranslationControl
        {
            get => (string)ViewState[nameof(TranslationControl)];
            set => ViewState[nameof(TranslationControl)] = value;
        }

        private Random Random => (Random)(Context.Items[_randomKey] ?? (Context.Items[_randomKey] = new Random()));
        private static readonly string _randomKey = typeof(ITextBox).FullName + "." + nameof(Random);

        public override bool HasValue => Text.IsNotEmpty();

        #endregion

        #region Fields

        private static readonly Regex _emailPattern = new Regex(Pattern.ValidEmail, RegexOptions.Compiled);

        #endregion

        #region Initialization and Loading

        protected override void AddParsedSubObject(object obj)
        {
            Text = obj is LiteralControl literal
                ? literal.Text
                : throw new ApplicationException("Unexpected child type: " + obj.GetType().Name);
        }

        protected override object SaveViewState()
        {
            if (TextMode == TextBoxMode.Password)
                base.ViewState.SetItemDirty(nameof(Text), false);

            return base.SaveViewState();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (TextMode == TextBoxMode.Masked)
            {
                var textLength = Text.Length;
                if (textLength != MaskedText.Length)
                    MaskedText = textLength > 0 ? RandomStringGenerator.Create(Random, RandomStringType.AlphanumericCaseSensitiveAndSymbols, textLength) : null;
            }
        }

        #endregion

        #region IPostBackDataHandler

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            var text = NormalizeNewLine(postCollection[postDataKey]);

            if (TextMode == TextBoxMode.Masked)
            {
                if (text == MaskedText)
                    text = Text;
            }
            else if (TextMode == TextBoxMode.Email)
            {
                if (text.IsNotEmpty() && !_emailPattern.IsMatch(text))
                    text = null;
            }

            var isChanged = Visible && !ReadOnly && !Text.Equals(text, StringComparison.Ordinal);
            if (isChanged)
                Text = text;

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

            OnTextChanged(EventArgs.Empty);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("insite-text form-control", CssClass));

            if (EmptyMessage.Length > 0)
                writer.AddAttribute("placeholder", EmptyMessage);

            if (ReadOnly)
                writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");

            if (!Enabled)
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

            if (!AllowHtml)
                writer.AddAttribute("data-break-html", "1");

            if (TabIndex != 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString());

            if (TranslationControl.IsNotEmpty())
            {
                var translationControl = (EditorTranslation)(NamingContainer is DynamicControl
                    ? NamingContainer.NamingContainer.FindControl(TranslationControl)
                    : NamingContainer.FindControl(TranslationControl));
                if (translationControl != null)
                    writer.AddAttribute("data-translation", translationControl.ClientID);
            }

            if (TextMode == TextBoxMode.MultiLine)
                AddMultiLineAttributes(writer);
            else
                AddSingleLineAttributes(writer);

            AddStyleAttributes(writer);

            AddClientEventAttributes(writer);

            AddAttributesToRender(writer);

            if (TextMode == TextBoxMode.MultiLine)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Textarea);

                HttpUtility.HtmlEncode(Text, writer);
            }
            else
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
            }

            writer.RenderEndTag();
        }

        private void AddMultiLineAttributes(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Rows, Rows.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Cols, Columns.ToString());

            if (MaxLength > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, MaxLength.ToString());
        }

        private void AddSingleLineAttributes(HtmlTextWriter writer)
        {
            if (TextMode == TextBoxMode.SingleLine)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");

                if (Text.Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, Text);
            }
            else if (TextMode == TextBoxMode.Email)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "email");

                if (Text.Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, Text);
            }
            else if (TextMode == TextBoxMode.Masked)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "password");
                writer.AddAttribute("data-masked", "1");

                if (MaskedText.Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, MaskedText);
            }
            else if (TextMode == TextBoxMode.Password)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "password");
            }
            else
                throw ApplicationError.Create("Unexpected TextMode: " + TextMode.GetName());

            if (MaxLength > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, MaxLength.ToString());
        }

        #endregion

        #region Methods (helpers)

        private static string NormalizeNewLine(string value)
        {
            return value.IsEmpty()
                ? string.Empty
                : value.Replace("\r", string.Empty).Replace("\n", System.Environment.NewLine);
        }

        #endregion
    }
}