using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class InputFilter : Control, INamingContainer, IHasEmptyMessage
    {
        private HtmlGenericControl _div;
        private HtmlInputText _input;
        private System.Web.UI.HtmlControls.HtmlButton _clearButton;
        private System.Web.UI.HtmlControls.HtmlButton _filterButton;

        public event EventHandler ClearClick;
        public event EventHandler FilterClick;

        public string EmptyMessage
        {
            get
            {
                EnsureChildControls();
                return _input.Attributes["placeholder"];
            }
            set
            {
                EnsureChildControls();
                _input.Attributes["placeholder"] = value;
            }
        }

        public string CssClass
        {
            get
            {
                EnsureChildControls();
                return _div.Attributes["class"];
            }
            set
            {
                EnsureChildControls();
                _div.Attributes["class"] = "input-filter " + (value ?? "");
            }
        }

        public string ClearButtonTooltip
        {
            get
            {
                EnsureChildControls();
                return _clearButton.Attributes["title"];
            }
            set
            {
                EnsureChildControls();
                _clearButton.Attributes["title"] = value;
            }
        }

        public string FilterButtonTooltip
        {
            get
            {
                EnsureChildControls();
                return _filterButton.Attributes["title"];
            }
            set
            {
                EnsureChildControls();
                _filterButton.Attributes["title"] = value;
            }
        }

        public string Text
        {
            get
            {
                EnsureChildControls();
                return _input.Value;
            }
            set
            {
                EnsureChildControls();

                var text = AllowHtml ? value : StringHelper.BreakHtml(value) ?? string.Empty;

                _input.Value = text;
            }
        }

        public Unit Width
        {
            get
            {
                EnsureChildControls();
                return Unit.Parse(_div.Style["width"]);
            }
            set
            {
                EnsureChildControls();
                _div.Style["width"] = value.ToString();
            }
        }

        public bool AllowHtml
        {
            get => ViewState[nameof(AllowHtml)] as bool? ?? false;
            set => ViewState[nameof(AllowHtml)] = value;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _div = new HtmlGenericControl("div") { ID = "GroupDiv" };
            Controls.Add(_div);
            _div.Attributes["class"] = "input-filter";

            _input = new HtmlInputText { ID = "InputText" };
            _div.Controls.Add(_input);
            _input.Attributes["class"] = "form-control";

            _clearButton = new System.Web.UI.HtmlControls.HtmlButton { ID = "ClearButton" };
            _div.Controls.Add(_clearButton);
            _clearButton.Attributes["class"] = "clear-button";
            _clearButton.Attributes["title"] = "Clear";
            _clearButton.InnerHtml = "<i class=\"far fa-times\"></i>";
            _clearButton.ServerClick += (sender, e) => ClearClick?.Invoke(sender, e);

            _filterButton = new System.Web.UI.HtmlControls.HtmlButton { ID = "FilterButton" };
            _div.Controls.Add(_filterButton);
            _filterButton.Attributes["class"] = "search-button";
            _filterButton.Attributes["title"] = "Filter";
            _filterButton.InnerHtml = "<i class=\"far fa-search\"></i>";
            _filterButton.ServerClick += (sender, e) => FilterClick?.Invoke(sender, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            _input.Attributes["onkeypress"] = $"if (event.which == 13) {{ __doPostBack('{_filterButton.UniqueID}', ''); return false; }}";

            var clearReturnResult = ClearClick != null ? "" : "return false;";
            _clearButton.Attributes["onclick"] = $"document.getElementById('{_input.ClientID}').value = ''; {clearReturnResult}";
        }
    }
}