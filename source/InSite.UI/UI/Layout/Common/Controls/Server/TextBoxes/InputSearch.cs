using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InSite.Common.Web.UI
{
    public class InputSearch : Control, IButtonControl
    {
        private System.Web.UI.WebControls.LinkButton _button;
        private TextBox _box;

        public event EventHandler Click;
        public event CommandEventHandler Command;

        private void OnClick() => Click?.Invoke(this, EventArgs.Empty);

        private void OnCommand(string name, object args) => Command?.Invoke(this, new CommandEventArgs(name, args));

        public string Text => _box.Text;

        public bool SubmitOnEnter
        {
            get => (bool?)ViewState[nameof(SubmitOnEnter)] ?? false;
            set => ViewState[nameof(SubmitOnEnter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Controls.Add(CreateTextBox());
            Controls.Add(CreateButton());
        }

        private Control CreateTextBox()
        {
            _box = new TextBox
            {
                CssClass = "form-control form-control-sm"
            };
            _box.EmptyMessage = "Filter";
            _box.ID = ID + "_box";
            return _box;
        }

        private Control CreateButton()
        {
            _button = new System.Web.UI.WebControls.LinkButton
            {
                CssClass = "btn btn-sm btn-outline-secondary btn-icon border",
                Text = "<i class=\"fas fa-filter\"></i>"
            };
            _button.Click += (s, a) => OnClick();
            _button.ID = ID + "_button";
            return _button;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (SubmitOnEnter)
                _box.ClientEvents.OnKeyDown = $"if (event.key === 'Enter') __doPostBack('{_button.UniqueID}', '');";

            writer.Write("<div class=\"input-group\">");
            base.Render(writer);
            writer.Write("</div>");
        }

        #region IButtonControl

        string IButtonControl.Text { get => _box.Text; set => _box.Text = value; }
        public bool CausesValidation { get => _button.CausesValidation; set => _button.CausesValidation = value; }
        public string CommandArgument { get => _button.CommandArgument; set => _button.CommandArgument = value; }
        public string CommandName { get => _button.CommandName; set => _button.CommandName = value; }
        public string PostBackUrl { get => _button.PostBackUrl; set => _button.PostBackUrl = value; }
        public string ValidationGroup { get => _button.ValidationGroup; set => _button.ValidationGroup = value; }

        #endregion
    }
}