using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Accounts.Senders.Controls
{
    public partial class SenderOrganizationList : BaseUserControl
    {
        #region Classes

        private class DataRow
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }
            public string OrganizationCode { get; set; }
        }

        #endregion

        #region Properties

        private Guid SenderIdentifier
        {
            get => (Guid)ViewState[nameof(SenderIdentifier)];
            set => ViewState[nameof(SenderIdentifier)] = value;
        }

        public int ItemsCount
        {
            get => (int)(ViewState[nameof(ItemsCount)] ?? 0);
            set => ViewState[nameof(ItemsCount)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemCommand += Repeater_ItemCommand;

            OrganizationSelector.AutoPostBack = true;
            OrganizationSelector.ValueChanged += OrganizationSelector_ValueChanged;
        }

        #endregion

        #region Event handlers

        private void OrganizationSelector_ValueChanged(object sender, EventArgs e)
        {
            if (OrganizationSelector.HasValue)
            {
                var organizationId = OrganizationSelector.Value.Value;

                if (!TSenderOrganizationSearch.Exists(SenderIdentifier, organizationId))
                    TSenderOrganizationStore.Insert(organizationId, SenderIdentifier);

                LoadData(SenderIdentifier);
            }

            OrganizationSelector.Value = null;
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var organizationId = Guid.Parse((string)e.CommandArgument);

                TSenderOrganizationStore.Delete(organizationId, SenderIdentifier);

                LoadData(SenderIdentifier);
            }
        }

        #endregion

        #region Public methods

        public void LoadData(Guid sender)
        {
            SenderIdentifier = sender;

            var organizations = TSenderOrganizationSearch.Bind(
                x => new DataRow
                {
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    CompanyName = x.Organization.CompanyName,
                    OrganizationCode = x.Organization.OrganizationCode
                },
                new TSenderOrganizationFilter
                {
                    SenderIdentifier = SenderIdentifier
                },
                "CompanyName");

            if (!CurrentSessionState.Identity.IsOperator)
            {
                var availableCodes = CurrentSessionState.Identity.Organizations.Select(x => x.OrganizationCode).ToHashSet(StringComparer.OrdinalIgnoreCase);

                organizations = organizations.Where(x => availableCodes.Contains(x.OrganizationCode)).ToArray();

                OrganizationSelector.Filter.IncludeOrganizationCode = availableCodes.ToArray();
            }
            else
            {
                OrganizationSelector.Filter.IncludeOrganizationCode = null;
            }

            var hasData = organizations.Count > 0;

            Repeater.Visible = hasData;
            Repeater.DataSource = organizations;
            Repeater.DataBind();

            ItemsCount = organizations.Count;

            OrganizationSelector.Value = null;
            OrganizationSelector.Filter.IsClosed = false;

            if (hasData)
                OrganizationSelector.Filter.ExcludeOrganizationCode = organizations.Select(x => x.OrganizationCode).ToArray();
        }

        #endregion
    }
}