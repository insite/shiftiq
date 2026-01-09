using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.UI.Layout.Common.Contents;

using Shift.Constant;
using Shift.Common.Events;

namespace InSite.Admin.Sites.Pages.Controls
{
    public abstract class ContentBlocksEditorBase : UserControl
    {
        public event EventHandler Insert;
        protected void OnInsert() => Insert?.Invoke(this, EventArgs.Empty);

        public event EventHandler Delete;
        protected void OnDelete() => Delete?.Invoke(this, EventArgs.Empty);

        public event AlertHandler Alert;
        protected void OnAlert(AlertType type, string message) => OnAlert(new AlertArgs(type, message));
        protected void OnAlert(AlertArgs args) => Alert?.Invoke(this, args);

        public string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        public static void BindControlSelector(ComboBox combobox) =>
            combobox.LoadItems(ControlPath.BlockControlTypes, "Name", "Title");

        protected static bool IsContentItem(RepeaterItemEventArgs e) => ControlHelper.IsContentItem(e.Item);

        protected static bool IsContentItem(RepeaterItem item) => ControlHelper.IsContentItem(item);
    }
}