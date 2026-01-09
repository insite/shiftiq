using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.UI.Layout.Lobby;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class AccordionPanel : Control, IAttributeAccessor, IAccordionPanel, IHasText
    {
        #region Events

        public event EventHandler SelectedChanged;

        private void OnSelectedChanged()
        {
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        public bool IsSelected
        {
            get { return ViewState[nameof(IsSelected)] != null && (bool)ViewState[nameof(IsSelected)]; }
            set
            {
                var changed = IsSelected != value;

                ViewState[nameof(IsSelected)] = value;

                if (changed)
                    OnSelectedChanged();
            }
        }

        public string NextPageUrl
        {
            get { return (string)ViewState[nameof(NextPageUrl)]; }
            set { ViewState[nameof(NextPageUrl)] = value; }
        }

        public string Title
        {
            get { return (string)ViewState[nameof(Title)]; }
            set { ViewState[nameof(Title)] = value; }
        }

        public int? Count
        {
            get { return (int?)ViewState[nameof(Count)]; }
            set { ViewState[nameof(Count)] = value; }
        }

        public int? TitleSize
        {
            get { return (int?)ViewState[nameof(TitleSize)]; }
            set { ViewState[nameof(TitleSize)] = value; }
        }

        public string Subtitle
        {
            get { return (string)ViewState[nameof(Subtitle)]; }
            set { ViewState[nameof(Subtitle)] = value; }
        }

        public string Icon
        {
            get { return (string)ViewState[nameof(Icon)]; }
            set { ViewState[nameof(Icon)] = value; }
        }

        public bool IsTitleLocalizable
        {
            get { return ViewState[nameof(IsTitleLocalizable)] as bool? ?? true; }
            set { ViewState[nameof(IsTitleLocalizable)] = value; }
        }

        private Dictionary<string, string> Attributes =>
            (Dictionary<string, string>)(ViewState[nameof(Attributes)]
                ?? (ViewState[nameof(Attributes)] = new Dictionary<string, string>()));

        protected LobbyBasePage AppPage => Page as LobbyBasePage;

        public bool? Animate
        {
            get { return (bool?)ViewState[nameof(Animate)]; }
            set { ViewState[nameof(Animate)] = value; }
        }

        string IHasText.Text { get => Title; set => Title = value; }

        #endregion

        #region Fields

        private Accordion _parent = null;

        #endregion

        #region IAttributeAccessor

        string IAttributeAccessor.GetAttribute(string key)
        {
            return Attributes.ContainsKey(key) ? Attributes[key] : null;
        }

        void IAttributeAccessor.SetAttribute(string key, string value)
        {
            if (Attributes.ContainsKey(key))
                Attributes[key] = value;
            else
                Attributes.Add(key, value);
        }

        #endregion

        #region Render

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            if (_parent != null)
                RenderCollapsePanel(_parent, writer);
            else
                RenderAccordionPanel(writer);
        }

        private void RenderAccordionPanel(HtmlTextWriter writer)
        {
            #region div.section-panel

            AddAttributes(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "section-panel");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            #region h1

            var title = GetFullTitle();
            if (!string.IsNullOrEmpty(title))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.H1);
                writer.Write(title);
                writer.RenderEndTag();
            }

            #endregion

            base.Render(writer);

            writer.RenderEndTag();

            #endregion
        }

        private void RenderCollapsePanel(Accordion parent, HtmlTextWriter writer)
        {
            #region div.panel

            var panelId = ClientID + "_panel";

            AddAttributes(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "accordion-item");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            #region h2.accordion-header

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "accordion-header");
            writer.RenderBeginTag(HtmlTextWriterTag.H2);

            #region button.accordion-button

            var buttonCssClass = "accordion-button";

            if (TitleSize == 2)
                buttonCssClass += " fs-2";
            else if (TitleSize == 3)
                buttonCssClass += " fs-3";

            if (!IsSelected)
                buttonCssClass += " collapsed";

            writer.AddAttribute(HtmlTextWriterAttribute.Class, buttonCssClass);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute("data-bs-toggle", "collapse");
            writer.AddAttribute("data-bs-target", "#" + panelId);
            writer.AddAttribute("aria-expanded", IsSelected ? "true" : "false");
            writer.AddAttribute("aria-controls", panelId);
            writer.RenderBeginTag(HtmlTextWriterTag.Button);

            #region i.fa

            if (!string.IsNullOrEmpty(Icon))
            {
                var prefix = "me-2 ";

                writer.AddAttribute(HtmlTextWriterAttribute.Class, prefix + " " + Icon);
                writer.RenderBeginTag(HtmlTextWriterTag.I);
                writer.RenderEndTag();
            }

            #endregion

            var title = GetFullTitle();
            if (!string.IsNullOrEmpty(title))
                writer.Write(title);

            if (!string.IsNullOrEmpty(Subtitle))
            {
                var subtitle = Translate(Subtitle);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-text");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write($" {subtitle}");
                writer.RenderEndTag();
            }

            writer.RenderEndTag();

            #endregion

            writer.RenderEndTag();

            #endregion

            #region div.accordion-collapse

            if ((Animate ?? _parent?.Animate) == false)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "-webkit-transition:none; transition:none;");

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "accordion-collapse collapse" + (IsSelected ? " show" : string.Empty));
            writer.AddAttribute(HtmlTextWriterAttribute.Id, panelId);
            writer.AddAttribute("aria-labelledby", panelId);
            if (!string.IsNullOrEmpty(_parent.DataParentID))
                writer.AddAttribute("data-bs-parent", "#" + _parent.DataParentID);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            #region div.accordion-body

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "accordion-body");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.RenderChildren(writer);

            if (parent.Navigation)
            {
                var prevID = GetPrevPanelID(parent.Panels);
                if (prevID.HasValue())
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-default me-2");
                    writer.AddAttribute("data-bs-toggle", "collapse");
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, $"#{prevID}_panel");
                    writer.AddAttribute("role", "button");
                    writer.AddAttribute("aria-expanded", "false");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write("<i class='far fa-arrow-left me-1'></i>");
                    writer.Write("Previous");
                    writer.RenderEndTag();
                }

                var nextID = GetNextPanelID(parent.Panels);
                if (nextID.HasValue() || NextPageUrl.HasValue())
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-primary");
                    if (NextPageUrl.HasValue())
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, NextPageUrl);
                    }
                    else
                    {
                        writer.AddAttribute("data-bs-toggle", "collapse");
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, $"#{nextID}_panel");
                        writer.AddAttribute("role", "button");
                        writer.AddAttribute("aria-expanded", "false");
                    }
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write("Next");
                    writer.Write("<i class='far fa-arrow-right ms-2'></i>");
                    writer.RenderEndTag();
                }
            }

            writer.RenderEndTag();

            #endregion

            writer.RenderEndTag();

            #endregion

            writer.RenderEndTag();

            #endregion
        }

        private string GetFullTitle()
        {
            if (string.IsNullOrEmpty(Title))
                return Title;

            var translatedTitle = Translate(Title);

            return Count.HasValue
                ? $"{translatedTitle}&nbsp;<span class=\"col-form-label-sm\">({Count:n0})</span>"
                : translatedTitle;
        }

        private string GetNextPanelID(IReadOnlyList<AccordionPanel> panels)
        {
            var index = GetCurrentPanelIndex(panels);

            if (index != -1)
                if (index < panels.Count - 1) return panels[index + 1].ClientID;

            return null;
        }

        private string GetPrevPanelID(IReadOnlyList<AccordionPanel> panels)
        {
            var index = GetCurrentPanelIndex(panels);

            if (index != -1)
                if (index > 0) return panels[index - 1].ClientID;

            return null;
        }

        private int GetCurrentPanelIndex(IReadOnlyList<AccordionPanel> panels)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (ClientID == panels[i].ClientID) return i;
            }

            return -1;
        }

        private void AddAttributes(HtmlTextWriter writer)
        {
            foreach (KeyValuePair<string, string> p in Attributes)
                writer.AddAttribute(p.Key, p.Value);
        }

        internal void SetupParent(Accordion parent)
        {
            if (_parent != null)
                throw ApplicationError.Create("The section collapse is already assigned to this panel: {0}", UniqueID);

            _parent = parent;
        }

        private string Translate(string text)
            => (IsTitleLocalizable && AppPage?.Translator != null)
             ? AppPage.Translator.Translate(text)
            : text;

        #endregion
    }
}