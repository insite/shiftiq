using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;

using Shift.Common;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class SearchDownloadColumnTabs : BaseUserControl
    {
        #region Classes

        public sealed class TypeInfo
        {
            public string Name => _group.TypeName;

            public DownloadColumnState[] Columns { get; }

            private DownloadColumnGroup _group;

            public TypeInfo(DownloadColumnGroup group, DownloadColumnState[] columns)
            {
                _group = group;
                Columns = columns;
            }
        }

        private sealed class TypeTabInfo
        {
            public string GroupName { get; }
            public SearchDownloadColumnRepeater ColumnRepeater { get; }

            public TypeTabInfo(string name, SearchDownloadColumnRepeater repeater)
            {
                GroupName = name;
                ColumnRepeater = repeater;
            }
        }

        #endregion

        #region Properties

        private int TabsCount
        {
            get => (int)(ViewState[nameof(TabsCount)] ?? 0);
            set => ViewState[nameof(TabsCount)] = value;
        }

        private string SingleGroupName
        {
            get => (string)(ViewState[nameof(SingleGroupName)]);
            set => ViewState[nameof(SingleGroupName)] = value;
        }

        private IEnumerable<TypeTabInfo> Tabs
        {
            get
            {
                var navItems = GroupTabs.GetItems();
                if (navItems.Count > 0)
                {
                    foreach (var navItem in navItems)
                        yield return new TypeTabInfo(navItem.Title, (SearchDownloadColumnRepeater)navItem.Controls[0]);
                }
                else if (SingleGroupName.IsNotEmpty())
                {
                    yield return new TypeTabInfo(SingleGroupName, ColumnRepeater);
                }
            }
        }

        private string SelectedGroupName => GroupTabs.ItemsCount > 0 ? GroupTabs.SelectedItem.Title : SingleGroupName;

        #endregion

        #region Loading

        protected override void CreateChildControls()
        {
            if (GroupTabs.ItemsCount == 0 && TabsCount > 1)
            {
                for (var i = 0; i < TabsCount; i++)
                    AddTab(out _, out _);
            }

            base.CreateChildControls();
        }

        public void LoadData(IReadOnlyCollection<DownloadColumnState> columns)
        {
            var types = new List<TypeInfo>();
            {
                DownloadColumnGroup group = null;
                List<DownloadColumnState> list = null;

                foreach (var c in columns)
                {
                    if (group == null || group.TypeName != c.Group.TypeName)
                    {
                        if (group != null)
                            types.Add(new TypeInfo(group, list.ToArray()));

                        group = c.Group;
                        list = new List<DownloadColumnState>();
                    }

                    list.Add(c);
                }

                if (list.Count > 0)
                    types.Add(new TypeInfo(group, list.ToArray()));
            }

            if (TabsCount != types.Count || !UpdateItems(types))
                ReloadItems(types);
        }

        private bool UpdateItems(List<TypeInfo> types)
        {
            if (TabsCount > 1)
            {
                var navItems = GroupTabs.GetItems();
                foreach (var item in navItems)
                {
                    var group = types.SingleOrDefault(x => x.Name == item.Title);
                    if (group == null)
                        return false;

                    var repeater = (SearchDownloadColumnRepeater)item.Controls[0];

                    repeater.LoadData(group.Columns);
                }
            }
            else if (TabsCount == 1)
            {
                var group = types.First();
                if (SingleGroupName == group.Name)
                    ColumnRepeater.LoadData(group.Columns);
                else
                    return false;
            }

            return true;
        }

        private void ReloadItems(List<TypeInfo> types)
        {
            TabsCount = types.Count;
            SingleGroupName = null;

            GroupTabs.ClearItems();
            GroupTabs.Visible = false;

            ColumnRepeater.Visible = false;

            if (types.Count > 1)
            {
                GroupTabs.Visible = true;

                foreach (var group in types)
                {
                    AddTab(out var navItem, out var repeater);

                    navItem.Title = group.Name;

                    repeater.LoadData(group.Columns);
                }

                GroupTabs.SelectedIndex = 0;
            }
            else if (types.Count == 1)
            {
                ColumnRepeater.Visible = true;

                var group = types.First();

                SingleGroupName = group.Name;
                ColumnRepeater.LoadData(group.Columns);
            }
        }

        private void AddTab(out NavItem navItem, out SearchDownloadColumnRepeater repeater)
        {
            GroupTabs.AddItem(navItem = new NavItem());
            navItem.Controls.Add(repeater = (SearchDownloadColumnRepeater)LoadControl("~/UI/Layout/Common/Controls/SearchDownloadColumnRepeater.ascx"));
        }

        internal BaseSearchDownload.JsonColumnState[] GetState()
        {
            if (TabsCount > 1)
            {
                var result = new List<BaseSearchDownload.JsonColumnState>();

                foreach (var tab in Tabs)
                    result.AddRange(tab.ColumnRepeater.State);

                return result.ToArray();
            }
            else
            {
                return ColumnRepeater.State;
            }
        }

        internal void SetState(BaseSearchDownload.JsonColumnState[] state)
        {
            if (TabsCount > 1)
            {
                var index = 0;

                foreach (var tab in Tabs)
                {
                    var repeater = tab.ColumnRepeater;
                    var count = repeater.ItemsCount;

                    var buffer = new BaseSearchDownload.JsonColumnState[count];
                    Array.Copy(state, index, buffer, 0, count);
                    index += count;

                    tab.ColumnRepeater.State = buffer;
                }
            }
            else
            {
                ColumnRepeater.State = state;
            }
        }

        #endregion
    }
}