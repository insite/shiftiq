using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;

using OrganizationHierarchyInfo = InSite.Persistence.OrganizationSearch.OrganizationHierarchyInfo;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class Detail : BaseUserControl
    {
        #region Events and Properties

        public event EventHandler Updated;
        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        protected Guid? OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            DetailOrganization.Updated += (s, a) => OnUpdated();

            base.OnInit(e);
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            if (OrganizationIdentifier.HasValue && OrganizationSearch.Select(OrganizationIdentifier.Value) == null)
                throw new ApplicationError("Organization not found: " + OrganizationIdentifier.Value);

            base.OnPreRender(e);
        }

        #endregion

        #region Setting and getting input values

        public void SetInputValues(OrganizationState organization, string selectedPanel, string selectedTab)
        {
            selectedPanel = selectedPanel.EmptyIfNull().ToLower();
            selectedTab = selectedTab.EmptyIfNull().ToLower();

            OrganizationIdentifier = organization.OrganizationIdentifier;

            DetailOrganization.SetInputValues(organization);

            DetailConfiguration.SetInputValues(organization, selectedPanel, selectedTab);

            BindSubOrganizations();

            CollectionManager.LoadData(organization.Identifier);
            CollectionSection.Visible = true;

            if (selectedPanel.HasValue() && selectedPanel.ToLower() == "configuration")
                ConfigurationSection.IsSelected = true;
            else if (selectedPanel.HasValue() && selectedPanel.ToLower() == "collection")
                CollectionSection.IsSelected = true;

            DivisionGrid.LoadData(organization.Identifier);
            DepartmentGrid.LoadData(organization.Identifier);

            // Fields

            UserProfileFieldsGrid.LoadData(OrganizationIdentifier.Value, nameof(PortalFieldInfo.UserProfile));
            ClassRegistrationFieldsGrid.LoadData(OrganizationIdentifier.Value, nameof(PortalFieldInfo.ClassRegistration));
            LearnerDashboardFieldsGrid.LoadData(OrganizationIdentifier.Value, nameof(PortalFieldInfo.LearnerDashboard));
            InvoiceBillingAddressGrid.LoadData(OrganizationIdentifier.Value, nameof(PortalFieldInfo.InvoiceBillingAddress));
        }

        public void GetInputValues(OrganizationState organization)
        {
            DetailOrganization.GetInputValues(organization);

            DetailConfiguration.GetInputValues(organization);
        }

        public void SaveCollections() => CollectionManager.SaveData();

        #endregion

        #region Methods (subtenant tree view)

        public class SubOrganizationState
        {
            public OrganizationHierarchyInfo[] Data { get; }
            public int Index { get; private set; }
            public OrganizationHierarchyInfo Current => Data[Index];

            public SubOrganizationState(IEnumerable<OrganizationHierarchyInfo> data)
            {
                Data = data.Where(x => !x.AccountClosed.HasValue).OrderBy(x => x.OrganizationName).ToArray();
                Index = 0;
            }

            public bool IsEnd() => Index >= Data.Length;

            public void Next() => Index++;
        }

        public class SubOrganizationTreeViewItem
        {
            public int Depth { get; set; }
            public OrganizationHierarchyInfo DataItem { get; set; }
            public SubOrganizationTreeViewItem NextItem { get; set; }
            public SubOrganizationTreeViewItem PrevItem { get; set; }
        }

        private void BindSubOrganizations()
        {
            var treeItems = new List<SubOrganizationTreeViewItem>();
            var stack = new Stack<SubOrganizationState>();

            var organization = OrganizationSearch.SelectHierarchyDescendents(OrganizationIdentifier.Value)
                ?? new OrganizationHierarchyInfo();
            stack.Push(new SubOrganizationState(organization.Children));

            while (stack.Count > 0)
            {
                var state = stack.Pop();

                while (!state.IsEnd())
                {
                    var current = state.Current;

                    state.Next();

                    {
                        var currentItem = new SubOrganizationTreeViewItem
                        {
                            Depth = stack.Count,
                            DataItem = current
                        };

                        if (treeItems.Count > 0)
                        {
                            currentItem.PrevItem = treeItems[treeItems.Count - 1];
                            currentItem.PrevItem.NextItem = currentItem;
                        }

                        treeItems.Add(currentItem);
                    }

                    if (current.Children.Count > 0)
                    {
                        stack.Push(state);
                        stack.Push(new SubOrganizationState(current.Children));

                        break;
                    }
                }
            }

            SubOrganizationRepeater.DataSource = treeItems;
            SubOrganizationRepeater.DataBind();

            SubOrganizationsSection.Visible = treeItems.Count > 0;
        }

        protected string GetTreeViewPrefix(object obj)
        {
            var dataItem = (SubOrganizationTreeViewItem)obj;

            return dataItem.PrevItem != null && dataItem.PrevItem.Depth < dataItem.Depth
                ? "<ul class='tree-view'><li>"
                : "<li>";
        }

        protected string GetTreeViewPostfix(object obj)
        {
            var dataItem = (SubOrganizationTreeViewItem)obj;

            if (dataItem.NextItem == null)
                return "</li>" + string.Concat(Enumerable.Repeat("</ul></li>", dataItem.Depth));

            if (dataItem.NextItem.Depth < dataItem.Depth)
                return "</li>" + string.Concat(Enumerable.Repeat("</ul></li>", dataItem.Depth - dataItem.NextItem.Depth));

            if (dataItem.NextItem.Depth == dataItem.Depth)
                return "</li>";

            return string.Empty;
        }

        #endregion
    }
}
