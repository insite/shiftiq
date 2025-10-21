using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [PersistChildren(true), ParseChildren(typeof(AccordionPanel), ChildrenAsProperties = false)]
    public class Accordion : Control, INamingContainer, IPostBackDataHandler, IAccordion
    {
        #region Events

        public event EventHandler SelectedIndexChanged;

        private void OnSelectedIndexChanged()
        {
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        public IReadOnlyList<AccordionPanel> Panels => _panels;

        public bool Navigation
        {
            get { return ViewState[nameof(Navigation)] != null && (bool)ViewState[nameof(Navigation)]; }
            set { ViewState[nameof(Navigation)] = value; }
        }

        public bool Animate
        {
            get { return (bool?)ViewState[nameof(Animate)] ?? false; }
            set { ViewState[nameof(Animate)] = value; }
        }

        public int SelectedIndex
        {
            get { return _isLoaded ? GetSelectedIndex() : _selectedIndex; }
            set
            {
                if (_isLoaded)
                {
                    var currentIndex = GetSelectedIndex();

                    if (currentIndex != value && (currentIndex != -1 || value != 0))
                        SetSelectedIndex(value);
                }
                else
                    _selectedIndex = value;
            }
        }

        private bool IsInited
        {
            get { return ViewState[nameof(IsInited)] != null && (bool)ViewState[nameof(IsInited)]; }
            set { ViewState[nameof(IsInited)] = value; }
        }

        public bool IsFlush
        {
            get { return ViewState[nameof(IsFlush)] != null && (bool)ViewState[nameof(IsFlush)]; }
            set { ViewState[nameof(IsFlush)] = value; }
        }

        public bool EnabledAutoScroll
        {
            get { return (bool?)ViewState[nameof(EnabledAutoScroll)] ?? false; }
            set { ViewState[nameof(EnabledAutoScroll)] = value; }
        }

        internal string DataParentID => ClientID;

        #endregion

        #region Fields

        private List<AccordionPanel> _panels = new List<AccordionPanel>();
        private int _selectedIndex = 0;
        private bool _isLoaded = false;
        private bool _isSelectedChangedLocked = false;

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _isLoaded = true;
        }

        #endregion

        #region Overriden methods

        protected override void AddedControl(Control control, int index)
        {
            base.AddedControl(control, index);

            if (control is AccordionPanel)
                AddPanel((AccordionPanel)control);
            else if (control is UserControl)
                AddPanel(control.Controls);
        }

        protected override void RemovedControl(Control control)
        {
            base.RemovedControl(control);

            if (control is AccordionPanel)
                RemovePanel((AccordionPanel)control);
            else if (control is UserControl)
                RemovePanel(control.Controls);
        }

        #endregion

        #region Event handlers

        private void Panel_SelectedChanged(object sender, EventArgs e)
        {
            if (_isSelectedChangedLocked)
                return;

            _isSelectedChangedLocked = true;

            foreach (var panel in Panels)
            {
                if (panel != sender)
                    panel.IsSelected = false;
            }

            _isSelectedChangedLocked = false;
        }

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return LoadPostData(postDataKey, postCollection);
        }

        private bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            var value = postCollection[postDataKey];
            var before = SelectedIndex;

            foreach (var panel in Panels)
                panel.IsSelected = panel.ClientID == value;

            var after = SelectedIndex;

            return before != after;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            OnSelectedIndexChanged();
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsInited)
            {
                if (SelectedIndex == -1)
                {
                    _isSelectedChangedLocked = true;

                    var isFound = false;
                    for (var i = 0; i < Panels.Count; i++)
                    {
                        var panel = Panels[i];

                        if (!isFound && i >= _selectedIndex && panel.Visible)
                        {
                            panel.IsSelected = true;
                            isFound = true;
                        }
                        else
                            panel.IsSelected = false;
                    }

                    _isSelectedChangedLocked = false;
                }

                IsInited = true;
            }

            Page.RegisterRequiresPostBack(this);

            ScriptManager.RegisterClientScriptBlock(
                Page,
                typeof(Accordion),
                "init",
                string.Format(@"
function {0}_onShow() {{ $('#{0}_state').val(this.id); }}
function {0}_onHide() {{ $('#{0}_state').val(''); }}", ClientID),
                true);

            if (Panels.Count > 0)
            {
                var initScript = new StringBuilder();

                foreach (AccordionPanel panel in Panels)
                {
                    initScript.AppendFormat(@"
$('#{0}').on('shown.bs.collapse', {1}_onShow);
$('#{0}').on('hide.bs.collapse', {1}_onHide);",
                        panel.ClientID,
                        ClientID);
                }

                ScriptManager.RegisterStartupScript(Page, typeof(Accordion), "init", initScript.ToString(), true);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);

            var cssClass = "accordion";

            if (IsFlush)
                cssClass += " accordion-flush";

            if (EnabledAutoScroll)
                cssClass += " accordion-auto-scroll";

            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.Render(writer);

            writer.RenderEndTag();
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            AccordionPanel selectedPanel = null;
            foreach (AccordionPanel panel in Panels)
            {
                panel.RenderControl(writer);
                if (panel.IsSelected)
                    selectedPanel = panel;
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_state");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, selectedPanel != null ? selectedPanel.ClientID : string.Empty);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        #endregion

        #region Helper methods

        public void AddPanel(AccordionPanel panel)
        {
            _panels.Add(panel);

            panel.SetupParent(this);
            panel.SelectedChanged += Panel_SelectedChanged;
        }

        private void AddPanel(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is AccordionPanel)
                    AddPanel((AccordionPanel)control);
                else if (control.HasControls())
                    AddPanel(control.Controls);
            }
        }

        private void RemovePanel(AccordionPanel panel)
        {
            _panels.Remove(panel);
        }

        private void RemovePanel(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is AccordionPanel)
                    AddPanel((AccordionPanel)control);
                else if (control.HasControls())
                    RemovePanel(control.Controls);
            }
        }
        int IAccordion.GetIndex(IAccordionPanel panel) => GetIndexBySection((AccordionPanel)panel);

        public int GetIndexBySection(AccordionPanel section)
        {
            for (var i = 0; i < Panels.Count; i++)
            {
                if (Panels[i] == section)
                    return i;
            }

            return -1;
        }

        private int GetSelectedIndex()
        {
            for (var i = 0; i < Panels.Count; i++)
                if (Panels[i].IsSelected)
                    return i;

            return -1;
        }

        private void SetSelectedIndex(int index)
        {
            if (index >= 0 && (Panels.Count == 0 || index >= Panels.Count))
                throw new ArgumentOutOfRangeException("index");

            _isSelectedChangedLocked = true;

            foreach (var panel in Panels)
                panel.IsSelected = false;

            if (index >= 0)
                Panels[index].IsSelected = true;

            _isSelectedChangedLocked = false;

            IsInited = true;
        }

        #endregion
    }
}