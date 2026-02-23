using System;
using System.Collections.Generic;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;

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
                throw new KeyNotFoundException("Organization not found: " + OrganizationIdentifier.Value);

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
    }
}
