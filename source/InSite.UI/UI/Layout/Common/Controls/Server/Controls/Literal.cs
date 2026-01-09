using System.Web;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class Literal : Control, ITextControl, IHasText
    {
        public string Text
        {
            get => (string)ViewState[nameof(Text)];
            set => ViewState[nameof(Text)] = value;
        }

        public object[] Arguments
        {
            get => (object[])ViewState[nameof(Arguments)];
            set => ViewState[nameof(Arguments)] = value;
        }

        public LiteralMode Mode
        {
            get => (LiteralMode)(ViewState[nameof(Mode)] ?? LiteralMode.PassThrough);
            set => ViewState[nameof(Mode)] = value;
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (obj is LiteralControl lit)
                Text = lit.Text;
            else
                throw ApplicationError.Create("Children controls are not allowed.");
        }

        protected override ControlCollection CreateControlCollection()
        {
            return new EmptyControlCollection(this);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var text = Text;
            if (text.IsEmpty())
                return;

            if (Mode == LiteralMode.Encode)
                HttpUtility.HtmlEncode(FormatText(text), writer);
            else if (Mode == LiteralMode.Markdown)
                writer.Write(FormatText(Markdown.ToHtml(Text)));
            else
                writer.Write(FormatText(text));
        }

        private string FormatText(string value) => Arguments.IsEmpty() ? value : value.Format(Arguments);
    }
}