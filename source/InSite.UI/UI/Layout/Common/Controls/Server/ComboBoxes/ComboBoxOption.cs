using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public sealed class ComboBoxOption : ComboBoxItem, IComboBoxOption
    {
        #region Properties

        public override string Text { get; set; }

        public string Value { get; set; }

        public bool Selected
        {
            get => AllowSelect && _selected;
            set
            {
                if (!AllowSelect || _selected == value)
                    return;

                _selected = value;

                if (ComboBox != null)
                    ((IComboBoxItemOwner)ComboBox).ItemSelected(this);
            }
        }

        public string Title { get; set; }

        public string SubText { get; set; }

        public string CssClass { get; set; }

        public string Style { get; set; }

        public string Icon { get; set; }

        public string Html { get; set; }

        public bool Enabled { get; set; } = true;

        [TypeConverter(typeof(StringArrayConverter))]
        public string[] Keywords { get; set; }

        public bool AllowSelect => Visible && Enabled;

        #endregion

        #region Fields

        private bool _selected = false;

        #endregion

        #region Construction

        public ComboBoxOption()
            : base()
        {

        }

        public ComboBoxOption(string text, string value)
            : this()
        {
            Text = text;
            Value = value;
        }

        #endregion

        #region ComboBoxItem

        protected override bool SetOwner(BaseComboBox owner, bool isTrackingViewState)
        {
            var isAssigned = base.SetOwner(owner, isTrackingViewState);

            if (isAssigned && Selected)
                ((IComboBoxItemOwner)ComboBox).ItemSelected(this);

            return isAssigned;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Title.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Title, Title);

            if (SubText.IsNotEmpty())
                writer.AddAttribute("data-subtext", SubText);

            if (Value.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Value, Value);

            if (CssClass.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);

            if (Style.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Style, Style);

            if (Icon.IsNotEmpty())
                writer.AddAttribute("data-icon", Icon);

            if (Html.IsNotEmpty())
                writer.AddAttribute("data-content", Html);

            if (!Enabled)
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

            if (Keywords.IsNotEmpty())
                writer.AddAttribute("data-tokens", string.Join(" ", Keywords));

            if (Selected)
                writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");

            writer.RenderBeginTag(HtmlTextWriterTag.Option);
            writer.Write(Text);
            writer.RenderEndTag();
        }

        protected override void LoadState(IStateReader reader)
        {
            base.LoadState(reader);

            reader.Get<bool>(x => Enabled = x);
            reader.Get<string>(x => Text = x);
            reader.Get<string>(x => Value = x);
            reader.Get<bool>(x => Selected = x);
            reader.Get<string>(x => Title = x);
            reader.Get<string>(x => SubText = x);
            reader.Get<string>(x => CssClass = x);
            reader.Get<string>(x => Style = x);
            reader.Get<string>(x => Icon = x);
            reader.Get<string>(x => Html = x);
            reader.Get<string[]>(x => Keywords = x);
        }

        protected override void SaveState(IStateWriter writer)
        {
            base.SaveState(writer);

            writer.Add(Enabled);
            writer.Add(Text);
            writer.Add(Value);
            writer.Add(Selected);
            writer.Add(Title);
            writer.Add(SubText);
            writer.Add(CssClass);
            writer.Add(Style);
            writer.Add(Icon);
            writer.Add(Html);
            writer.Add(Keywords, (a, b) => a == b || a != null && b != null && a.SequenceEqual(b));
        }

        #endregion
    }
}