using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class SectionTabs : SectionBase
    {
        #region Classes

        [Serializable]
        private class TabInfo
        {
            public string ID { get; set; }
            public string Path { get; set; }
        }

        private class TabControls
        {
            public NavItem NavItem { get; }
            public SectionBase Content { get; }

            public TabControls(NavItem navItem, SectionBase content)
            {
                NavItem = navItem;
                Content = content;
            }
        }

        #endregion

        #region Properties

        private Dictionary<string, int> Identifiers
        {
            get => (Dictionary<string, int>)(ViewState[nameof(Identifiers)]
                ?? (ViewState[nameof(Identifiers)] = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)));
            set => ViewState[nameof(Identifiers)] = value;
        }

        private List<TabInfo> NavItems
        {
            get => (List<TabInfo>)(ViewState[nameof(NavItems)]
                ?? (ViewState[nameof(NavItems)] = new List<TabInfo>()));
            set => ViewState[nameof(NavItems)] = value;
        }

        #endregion

        #region Fields

        private List<TabControls> _controls = new List<TabControls>();

        #endregion

        #region Initialization

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            if (TabsNav.ItemsCount == 0 && NavItems.Count > 0)
            {
                for (var i = 0; i < NavItems.Count; i++)
                    AddTab(NavItems[i].Path, out _, out _);
            }

            base.CreateChildControls();
        }

        #endregion

        #region Methods (set input values)

        public override void SetOptions(LayoutContentSection options)
        {
            if (options is LayoutContentSection.TabList tabList)
            {
                foreach (var tab in tabList.Tabs)
                {
                    AddTab(tab.Id, tab.ControlPath, out var navItem, out var tabCtrl);

                    navItem.Title = tab.Title;
                    tabCtrl.SetOptions(tab);
                }
            }
            else
            {
                throw new NotImplementedException("Section type: " + options.GetType().FullName);
            }
        }

        public override void SetValidationGroup(string groupName)
        {
            foreach (var tab in _controls)
                tab.Content.SetValidationGroup(groupName);
        }

        public override void SetLanguage(string lang)
        {
            foreach (var ctrl in _controls)
                ctrl.Content.SetLanguage(lang);
        }

        #endregion

        #region Methods (get input values)

        public override MultilingualString GetValue() =>
            throw new NotImplementedException();

        public override MultilingualString GetValue(string id)
        {
            if (!string.IsNullOrEmpty(id) && Identifiers.TryGetValue(id, out var index))
                return _controls[index].Content.GetValue();
            else
                throw ApplicationError.Create("Section not found: " + id);
        }

        public override IEnumerable<MultilingualString> GetValues()
        {
            foreach (var tab in _controls)
                yield return tab.Content.GetValue();
        }

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        #endregion

        #region Methods (tab management)

        private void AddTab(string id, string tabPath, out NavItem navItem, out SectionBase tab)
        {
            if (Identifiers.ContainsKey(id))
                throw ApplicationError.Create("Invalid tab ID: " + id);

            AddTab(tabPath, out navItem, out tab);

            Identifiers.Add(id, NavItems.Count);
            NavItems.Add(new TabInfo
            {
                ID = id,
                Path = tabPath
            });
        }

        private void AddTab(string tabPath, out NavItem navItem, out SectionBase tab)
        {
            TabsNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(tab = (SectionBase)LoadControl(tabPath));

            _controls.Add(new TabControls(navItem, tab));
        }

        public override void OpenTab(string id)
        {
            if (!string.IsNullOrEmpty(id) && Identifiers.TryGetValue(id, out var index))
                _controls[index].NavItem.IsSelected = true;
        }

        public string GetCurrentTab()
        {
            if (TabsNav.ItemsCount == 0)
                return null;

            var index = TabsNav.SelectedIndex;

            return Identifiers.Where(kv => kv.Value == index).Select(x => x.Key).FirstOrDefault();
        }

        #endregion
    }
}