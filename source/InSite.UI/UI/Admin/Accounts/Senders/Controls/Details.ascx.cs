using System;
using System.Linq;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.Admin.Accounts.Senders.Controls
{
    public partial class Details : UserControl
    {
        public Guid? OrganizationIdentifier
        {
            get => OrganizationSelector.Value;
            set
            {
                OrganizationSelector.Value = value;
                OnOrganizationChanged();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OrganizationSelector.AutoPostBack = true;
            OrganizationSelector.ValueChanged += (s, a) => OnOrganizationChanged();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            OrganizationSelector.Filter.IsClosed = false;
            OrganizationSelector.Filter.IncludeOrganizationCode = !CurrentSessionState.Identity.IsOperator
                ? CurrentSessionState.Identity.Organizations.Select(x => x.OrganizationCode).ToArray()
                : null;
        }

        private void OnOrganizationChanged()
        {
            var organizationId = OrganizationSelector.Value;
            var organization = organizationId.HasValue
                ? ServiceLocator.OrganizationSearch.GetModel(organizationId.Value)
                : null;
            if (organization == null)
                return;

            var location = organization.PlatformCustomization.TenantLocation;

            CompanyAddress.Text = location.Street;
            CompanyCity.Text = location.City;
            CompanyPostalCode.Text = location.PostalCode;
            CompanyCountry.Text = location.Country;

            CompanyUpdatePanel.Update();
        }

        public void SetInputValues(TSender entity)
        {
            OrganizationField.Visible = false;
            SenderIdentifierField.Visible = true;

            SenderType.SelectedValue = entity.SenderType;
            SenderNickname.Text = entity.SenderNickname;
            SenderName.Text = entity.SenderName;
            SenderEmail.Text = entity.SenderEmail;
            SystemMailbox.Text = entity.SystemMailbox;

            SenderEnabled.ValueAsBoolean = entity.SenderEnabled;

            CompanyAddress.Text = entity.CompanyAddress;
            CompanyPostalCode.Text = entity.CompanyPostalCode;
            CompanyCountry.Text = entity.CompanyCountry;
            CompanyCity.Text = entity.CompanyCity;
        }

        public void GetInputValues(TSender entity)
        {
            entity.SenderType = SenderType.SelectedValue;
            entity.SenderNickname = SenderNickname.Text;
            entity.SenderName = SenderName.Text;
            entity.SenderEmail = SenderEmail.Text;
            entity.SystemMailbox = SystemMailbox.Text;
            entity.SenderEnabled = SenderEnabled.ValueAsBoolean.Value;
            entity.CompanyAddress = CompanyAddress.Text;
            entity.CompanyPostalCode = CompanyPostalCode.Text;
            entity.CompanyCountry = CompanyCountry.Text;
            entity.CompanyCity = CompanyCity.Text;
        }
    }
}