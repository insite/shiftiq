using System;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;

using Shift.Common;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfigurationLocationTaxNumber : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TaxNumberLabelValidator.ServerValidate +=
                (s,a) => a.IsValid = TaxNumberValue.Text.IsEmpty() || TaxNumberLabel.Text.IsNotEmpty();
        }

        public void SetInputValues(OrganizationState organization)
        {
            var location = organization.PlatformCustomization.TenantLocation;
            TaxNumberLabel.Text = location.TaxNumberLabel;
            TaxNumberValue.Text = location.TaxNumberValue;
        }

        public void GetInputValues(OrganizationState organization)
        {
            var location = organization.PlatformCustomization.TenantLocation;
            location.TaxNumberLabel = TaxNumberLabel.Text;
            location.TaxNumberValue = TaxNumberValue.Text;
        }
    }
}
