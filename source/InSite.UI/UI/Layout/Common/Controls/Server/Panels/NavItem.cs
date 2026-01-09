using System;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class NavItem : Control, INavItem
    {
        #region Script

        void INavItem.RegisterScript(string onShowHandler, string onHideHandler)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(NavItem),
                "init_" + ClientID,
                $@"
$('#{ClientID}_tab').on('shown.bs.tab', {onShowHandler});
$('#{ClientID}_tab').on('hide.bs.tab', {onHideHandler});",
                true);
        }

        #endregion

        #region Events

        public event EventHandler SelectedChanged;
        private void OnSelectedChanged() => SelectedChanged?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        public bool IsSelected
        {
            get => ViewState[nameof(IsSelected)] != null && (bool)ViewState[nameof(IsSelected)];
            set
            {
                var changed = IsSelected != value;

                ViewState[nameof(IsSelected)] = value;

                if (changed)
                    OnSelectedChanged();
            }
        }

        public string Title
        {
            get => (string)ViewState[nameof(Title)];
            set => ViewState[nameof(Title)] = value;
        }

        public string Icon
        {
            get => (string)ViewState[nameof(Icon)];
            set => ViewState[nameof(Icon)] = value;
        }

        public IconPositionType IconPosition
        {
            get => (IconPositionType)(ViewState[nameof(IconPosition)] ?? IconPositionType.BeforeText);
            set => ViewState[nameof(IconPosition)] = value;
        }

        public bool IsTitleLocalizable
        {
            get => ViewState[nameof(IsTitleLocalizable)] == null || (bool)ViewState[nameof(IsTitleLocalizable)];
            set => ViewState[nameof(IsTitleLocalizable)] = value;
        }

        public string Identifier
        {
            get => (string)ViewState[nameof(Identifier)] ?? ClientID;
            set => ViewState[nameof(Identifier)] = value;
        }

        #endregion

        #region Render

        protected override void Render(HtmlTextWriter writer)
        {
            // Render nothing 
        }

        void INavItem.RenderTab(HtmlTextWriter writer, NavItemType type)
        {
            if (!Visible)
                return;

            var isSelected = IsSelected;

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "nav-item");
            writer.AddAttribute("role", "presentation");
            writer.RenderBeginTag(HtmlTextWriterTag.Li);

            #region BUTTON

            writer.AddAttribute(HtmlTextWriterAttribute.Class, isSelected ? "nav-link active" : "nav-link");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_tab");
            writer.AddAttribute("data-identifier", Identifier);

            if (type == NavItemType.Tabs)
                writer.AddAttribute("data-bs-toggle", "tab");
            else if (type == NavItemType.Pills)
                writer.AddAttribute("data-bs-toggle", "pill");
            else
                throw new NotImplementedException();

            writer.AddAttribute("data-bs-target", "#" + ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute("role", "tab");
            writer.AddAttribute("aria-controls", ClientID);
            writer.AddAttribute("aria-selected", isSelected ? "true" : "false");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);

            RenderItemContent(writer);

            writer.RenderEndTag();

            #endregion

            writer.RenderEndTag();
        }

        private void RenderItemContent(HtmlTextWriter writer)
        {
            var hasTitle = Title.IsNotEmpty();

            if (IconPosition == IconPositionType.BeforeText)
                RenderIcon(writer, hasTitle, true);

            if (hasTitle)
            {
                var translatedTitle = IsTitleLocalizable && (Page is IHasTranslator translator)
                    ? translator.Translator.Translate(Title)
                    : Title;

                writer.Write(translatedTitle);
            }

            if (IconPosition == IconPositionType.AfterText)
                RenderIcon(writer, hasTitle, false);
        }

        private void RenderIcon(HtmlTextWriter writer, bool hasTitle, bool isStart)
        {
            if (Icon.IsEmpty())
                return;

            var cssClass = Icon;

            if (hasTitle)
                cssClass += isStart ? " me-2" : " ms-2";

            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.RenderEndTag();
        }

        void INavItem.RenderContent(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            var tabClass = "tab-pane";

            if (IsSelected)
                tabClass += " active";

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, tabClass);
            writer.AddAttribute("role", "tabpanel");
            writer.AddAttribute("aria-labelledby", ClientID + "_tab");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderChildren(writer);

            writer.RenderEndTag();
        }

        #endregion
    }
}