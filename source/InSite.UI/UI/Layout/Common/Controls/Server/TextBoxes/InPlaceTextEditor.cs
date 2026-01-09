using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(ChildrenAsProperties = true, DefaultProperty = "Text")]
    public class InPlaceTextEditor : Control, IHasEmptyMessage
    {
        #region Properties

        public TextRenderModeType RenderMode
        {
            get { return (TextRenderModeType?)ViewState[nameof(RenderMode)] ?? TextRenderModeType.Div; }
            set { ViewState[nameof(RenderMode)] = value; }
        }

        public TextModeType InputTextMode
        {
            get { return (TextModeType?)ViewState[nameof(InputTextMode)] ?? TextModeType.SingleLine; }
            set { ViewState[nameof(InputTextMode)] = value; }
        }

        public bool EnableTrimText
        {
            get { return ViewState[nameof(EnableTrimText)] == null || (bool)ViewState[nameof(EnableTrimText)]; }
            set { ViewState[nameof(EnableTrimText)] = value; }
        }

        public string Text
        {
            get { return (string)ViewState[nameof(Text)]; }
            set
            {
                var textValue = value ?? string.Empty;

                if (EnableTrimText && textValue.Length > 0)
                    textValue = textValue.Trim();

                if (InputMaxLength > 0 && textValue.Length > InputMaxLength)
                    textValue = textValue.Substring(0, InputMaxLength);

                ViewState[nameof(Text)] = textValue;
            }
        }

        public string CssClass
        {
            get { return (string)ViewState[nameof(CssClass)]; }
            set { ViewState[nameof(CssClass)] = value; }
        }

        public string EmptyMessage
        {
            get { return (string)ViewState[nameof(EmptyMessage)]; }
            set { ViewState[nameof(EmptyMessage)] = value; }
        }

        public Unit Height
        {
            get { return (Unit?)ViewState[nameof(Height)] ?? Unit.Empty; }
            set { ViewState[nameof(Height)] = value; }
        }

        public Unit Width
        {
            get { return (Unit?)ViewState[nameof(Width)] ?? Unit.Empty; }
            set { ViewState[nameof(Width)] = value; }
        }

        public Unit InputHeight
        {
            get { return (Unit?)ViewState[nameof(InputHeight)] ?? Unit.Empty; }
            set { ViewState[nameof(InputHeight)] = value; }
        }

        public Unit InputWidth
        {
            get { return (Unit?)ViewState[nameof(InputWidth)] ?? Unit.Empty; }
            set { ViewState[nameof(InputWidth)] = value; }
        }

        public int InputRows
        {
            get { return (int?)ViewState[nameof(InputRows)] ?? 2; }
            set
            {
                if (value < 0)
                    throw ApplicationError.Create("Rows must be greater than -1.");

                ViewState[nameof(InputRows)] = value;
            }
        }

        public int InputColumns
        {
            get { return (int?)ViewState[nameof(InputColumns)] ?? 20; }
            set
            {
                if (value < 0)
                    throw ApplicationError.Create("Columns must be greater than -1.");

                ViewState[nameof(InputColumns)] = value;
            }
        }

        public int InputMaxLength
        {
            get { return (int)(ViewState[nameof(InputMaxLength)] ?? 0); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(InputMaxLength));

                ViewState[nameof(InputMaxLength)] = value;
            }
        }

        public string InputCssClass
        {
            get { return (string)ViewState[nameof(InputCssClass)]; }
            set { ViewState[nameof(InputCssClass)] = value; }
        }

        public string SaveButtonText
        {
            get { return (string)ViewState[nameof(SaveButtonText)] ?? "Save"; }
            set { ViewState[nameof(SaveButtonText)] = value; }
        }

        public string SaveButtonCssClass
        {
            get { return (string)ViewState[nameof(SaveButtonCssClass)] ?? "btn btn-primary"; }
            set { ViewState[nameof(SaveButtonCssClass)] = value; }
        }

        public string CancelButtonText
        {
            get { return (string)ViewState[nameof(CancelButtonText)] ?? "Cancel"; }
            set { ViewState[nameof(CancelButtonText)] = value; }
        }

        public string CancelButtonCssClass
        {
            get { return (string)ViewState[nameof(CancelButtonCssClass)] ?? "btn btn-primary"; }
            set { ViewState[nameof(CancelButtonCssClass)] = value; }
        }

        public string OnClientSave
        {
            get { return (string)ViewState[nameof(OnClientSave)]; }
            set { ViewState[nameof(OnClientSave)] = value; }
        }

        public string CallbackData
        {
            get { return (string)ViewState[nameof(CallbackData)]; }
            set { ViewState[nameof(CallbackData)] = value; }
        }

        public bool Enabled
        {
            get { return ViewState[nameof(Enabled)] == null || (bool)ViewState[nameof(Enabled)]; }
            set { ViewState[nameof(Enabled)] = value; }
        }

        public bool IsMarkdown
        {
            get { return ViewState[nameof(IsMarkdown)] == null || (bool)ViewState[nameof(IsMarkdown)]; }
            set { ViewState[nameof(IsMarkdown)] = value; }
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            if (!ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
                ScriptManager.RegisterClientScriptResource(Page, typeof(InPlaceTextEditor), "InSite.Content.Scripts.InPlaceTextEditor.js");

            if (Enabled)
            {
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(InPlaceTextEditor),
                    "register_" + UniqueID,
                    string.Format(
                        "inPlaceTextEditor.register('{0}',{1},{2},{3});",
                        ClientID,
                        string.IsNullOrEmpty(CallbackData) ? "null" : "'" + CallbackData.Replace("'", "\\'") + "'",
                        string.IsNullOrEmpty(EmptyMessage) ? "null" : "'" + EmptyMessage.Replace("'", "\\'") + "'",
                        string.IsNullOrEmpty(OnClientSave) ? "null" : "'" + OnClientSave.Replace("'", "\\'") + "'"),

                    true
                );

                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(InPlaceTextEditor),
                    "init",
                    "$(document).ready(inPlaceTextEditor.__init);",
                    true
                );
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(ClientID))
                return;

            var isEmpty = string.IsNullOrEmpty(Text);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);

            if (!IsMarkdown)
                writer.AddAttribute("data-ismarkdown", "no");

            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses(isEmpty ? "ipte-empty" : null, CssClass));

            var style = new StringBuilder();

            if (!Width.IsEmpty)
                style.AppendFormat("width:{0};", Width);

            if (!Height.IsEmpty)
                style.AppendFormat("height:{0};", Height);

            if (style.Length > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style.ToString());

            writer.RenderBeginTag(GetTagName(RenderMode));

            if (!isEmpty)
                writer.Write(IsMarkdown ? Markdown.ToHtml(Text) : Text);
            else if (!string.IsNullOrEmpty(EmptyMessage))
                writer.Write(EmptyMessage);

            writer.RenderEndTag();

            RenderEditor(writer);
        }

        private void RenderEditor(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_editor");
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none; margin-bottom: 8px;");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            var style = new StringBuilder();

            if (!InputWidth.IsEmpty)
                style.AppendFormat("width:{0};", InputWidth);

            if (!InputHeight.IsEmpty)
                style.AppendFormat("height:{0};", InputHeight);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_input");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("insite-text form-control", InputCssClass));

            if (InputMaxLength > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, InputMaxLength.ToString());

            if (InputTextMode == TextModeType.SingleLine)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                writer.AddAttribute(HtmlTextWriterAttribute.Size, InputColumns.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Text);

                writer.RenderBeginTag(HtmlTextWriterTag.Input);

                writer.RenderEndTag();
            }
            else if (InputTextMode == TextModeType.MultiLine)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Cols, InputColumns.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Rows, InputRows.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style.ToString());

                writer.RenderBeginTag(HtmlTextWriterTag.Textarea);

                HttpUtility.HtmlEncode(Text, writer);

                writer.RenderEndTag();
            }
            else
                throw ApplicationError.Create("Unknown text mode type: {0}", InputTextMode.GetName());

            RenderEditorCommandButtons(writer);

            writer.RenderEndTag();
        }

        private void RenderEditorCommandButtons(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "padding-top:8px;");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.Write(
                "<a class='{3}' id='{0}_save' href='#'>{1}</a> <a class='{4}' id='{0}_cancel' href='#'>{2}</a>",
                ClientID, SaveButtonText, CancelButtonText, SaveButtonCssClass, CancelButtonCssClass);

            writer.RenderEndTag();
        }

        #endregion

        #region Helper methods

        private static HtmlTextWriterTag GetTagName(TextRenderModeType mode)
        {
            if (mode == TextRenderModeType.Div)
                return HtmlTextWriterTag.Div;
            else if (mode == TextRenderModeType.Paragraph)
                return HtmlTextWriterTag.P;
            else if (mode == TextRenderModeType.Span)
                return HtmlTextWriterTag.Span;

            throw ApplicationError.Create("Unknown render mode: {0}", mode.GetName());
        }

        #endregion
    }
}