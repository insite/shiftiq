using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    internal interface INavItem : INamingContainer
    {
        event EventHandler SelectedChanged;
        string ClientID { get; }
        string Identifier { get; set; }
        bool Visible { get; }
        bool IsSelected { get; set; }

        void RenderTab(HtmlTextWriter writer, NavItemType type);
        void RenderContent(HtmlTextWriter writer);
        void RegisterScript(string onShowHandler, string onHideHandler);
    }

    [PersistChildren(true), ParseChildren(typeof(NavItem), ChildrenAsProperties = false)]
    public class Nav : Control, INamingContainer, IPostBackDataHandler, IAttributeAccessor
    {
        #region Events

        public event EventHandler SelectedIndexChanged;

        private void OnSelectedIndexChanged()
        {
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        public NavItemType ItemType
        {
            get { return (NavItemType?)ViewState[nameof(ItemType)] ?? NavItemType.Tabs; }
            set { ViewState[nameof(ItemType)] = value; }
        }

        public NavItemAlignment ItemAlignment
        {
            get { return (NavItemAlignment?)ViewState[nameof(ItemAlignment)] ?? NavItemAlignment.Horizontal; }
            set { ViewState[nameof(ItemAlignment)] = value; }
        }

        public string ContentRendererID
        {
            get { return (string)ViewState[nameof(ContentRendererID)]; }
            set { ViewState[nameof(ContentRendererID)] = value; }
        }

        public int ItemsCount => _items.Count;

        public int SelectedIndex
        {
            get { return _isLoaded ? GetSelectedIndex() : _selectedIndex ?? -1; }
            set
            {
                if (_isLoaded)
                    SetSelectedIndex(value);
                else
                    _selectedIndex = value;
            }
        }

        public NavItem SelectedItem => SelectedIndex >= 0 && SelectedIndex < _items.Count
            ? (NavItem)_items[SelectedIndex]
            : null;

        public string CssClass
        {
            get { return (string)ViewState[nameof(CssClass)]; }
            set { ViewState[nameof(CssClass)] = value; }
        }

        private Dictionary<string, string> Attributes => (Dictionary<string, string>)(ViewState[nameof(Attributes)]
            ?? (ViewState[nameof(Attributes)] = new Dictionary<string, string>()));

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public NavListProperties List => _listProps;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public NavContentProperties Content => _contentProps;

        #endregion

        #region Fields

        private int? _selectedIndex = null;
        private bool _isLoaded = false;
        private bool _hasContentRenderer = false;
        private bool _isSelectedChangedLocked = false;
        private List<INavItem> _items = new List<INavItem>();

        private readonly NavListProperties _listProps;
        private readonly NavContentProperties _contentProps;

        #endregion

        #region Construction

        public Nav()
        {
            _listProps = new NavListProperties(nameof(List), ViewState);
            _contentProps = new NavContentProperties(nameof(Content), ViewState);
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _isLoaded = true;

            if (_selectedIndex.HasValue)
                SetSelectedIndex(_selectedIndex.Value);
        }

        #endregion

        #region Item Managing

        protected override void AddedControl(Control control, int index)
        {
            if (control is INavItem item)
            {
                base.AddedControl(control, index);

                item.Identifier = _items.Count.ToString();
                item.SelectedChanged += Item_SelectedChanged;

                _items.Add(item);
            }
            else
            {
                Controls.RemoveAt(index);
            }
        }

        protected override void RemovedControl(Control control)
        {
            base.RemovedControl(control);

            if (control is INavItem item)
                _items.Remove(item);
        }

        public void AddItem(NavItem item) => Controls.Add(item);

        public void ClearItems() => Controls.Clear();

        public IReadOnlyList<NavItem> GetItems() => Array.AsReadOnly(_items.Cast<NavItem>().ToArray());

        public int GetIndex(NavItem item) => _items.IndexOf(item);

        #endregion

        #region Event handlers

        private void Item_SelectedChanged(object sender, EventArgs e)
        {
            if (_isSelectedChangedLocked)
                return;

            _isSelectedChangedLocked = true;

            foreach (var item in _items)
            {
                if (item != sender)
                    item.IsSelected = false;
            }

            _isSelectedChangedLocked = false;
        }

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

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return LoadPostData(postDataKey, postCollection);
        }

        private bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            var value = postCollection[postDataKey];
            var before = SelectedIndex;

            foreach (var item in _items)
                item.IsSelected = value != null && value.Equals(item.Identifier, StringComparison.OrdinalIgnoreCase);

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
            if (_items.Count > 0 && SelectedIndex == -1 || SelectedIndex >= 0 && !_items[SelectedIndex].Visible)
            {
                _isSelectedChangedLocked = true;

                INavItem beforeItem = null, afterItem = null;

                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];

                    item.Identifier = i.ToString();
                    item.IsSelected = false;

                    if (item.Visible)
                    {
                        if (i < SelectedIndex)
                            beforeItem = item;
                        else if (afterItem == null && i > SelectedIndex)
                            afterItem = item;
                    }
                }

                if (afterItem != null)
                    afterItem.IsSelected = true;
                else if (beforeItem != null)
                    beforeItem.IsSelected = true;

                _isSelectedChangedLocked = false;
            }

            Page.RegisterRequiresPostBack(this);

            var onShowHandler = $"{ClientID}_onShow";
            var onHideHandler = $"{ClientID}_onHide";

            ScriptManager.RegisterClientScriptBlock(
                Page,
                typeof(Nav),
                "common_" + ClientID,
                string.Format(@"
function {1}() {{ $('#{0}_state').val($(this).data('identifier')); }}
function {2}() {{ $('#{0}_state').val(''); }}", ClientID, onShowHandler, onHideHandler),
                true);

            foreach (INavItem item in _items)
                item.RegisterScript(onShowHandler, onHideHandler);

            if (!string.IsNullOrEmpty(ContentRendererID))
            {
                if (!(NamingContainer.FindControl(ContentRendererID) is NavContent contentRenderer))
                    throw new ApplicationError("Content renderer not found: " + ContentRendererID);

                contentRenderer.Register(this);

                _hasContentRenderer = true;
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            #region DIV

            AddAttributes(writer);

            if (CssClass.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            #region UL.nav

            {
                var cssClass = "nav";

                if (ItemType == NavItemType.Tabs)
                    cssClass += " nav-tabs";
                else if (ItemType == NavItemType.Pills)
                    cssClass += " nav-pills";
                else
                    throw new NotImplementedException();

                if (ItemAlignment == NavItemAlignment.Vertical)
                {
                    cssClass += " flex-column";

                    writer.AddAttribute("aria-orientation", "vertical");
                }

                if (List.CssClass.IsNotEmpty())
                    cssClass = ControlHelper.MergeCssClasses(cssClass, List.CssClass);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
            }

            if (List.Style.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Style, List.Style);

            writer.AddAttribute("role", "tablist");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (var item in _items)
                item.RenderTab(writer, ItemType);

            writer.RenderEndTag();

            #endregion

            #region DIV.tab-content

            if (!_hasContentRenderer)
                RenderTabContent(writer);

            #endregion

            var selectedItem = _items.FirstOrDefault(x => x.IsSelected);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_state");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, selectedItem?.Identifier ?? string.Empty);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            writer.RenderEndTag();

            #endregion
        }

        internal void RenderTabContent(HtmlTextWriter writer)
        {
            {
                var cssClass = "tab-content";

                if (Content.CssClass.IsNotEmpty())
                    cssClass = ControlHelper.MergeCssClasses(cssClass, Content.CssClass);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
            }

            if (List.Style.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Style, List.Style);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach (var item in _items)
                item.RenderContent(writer);

            writer.RenderEndTag();
        }

        #endregion

        #region Helper methods

        private void AddAttributes(HtmlTextWriter writer)
        {
            foreach (KeyValuePair<string, string> p in Attributes)
                writer.AddAttribute(p.Key, p.Value);
        }

        private int GetSelectedIndex()
        {
            for (var i = 0; i < _items.Count; i++)
                if (_items[i].IsSelected)
                    return i;

            return -1;
        }

        private void SetSelectedIndex(int index)
        {
            if (index >= 0 && (_items.Count == 0 || index >= _items.Count))
                throw new ArgumentOutOfRangeException("index");

            _isSelectedChangedLocked = true;

            foreach (var panel in _items)
                panel.IsSelected = false;

            if (index >= 0)
                _items[index].IsSelected = true;

            _isSelectedChangedLocked = false;
        }

        #endregion
    }
}