using System.Web.UI;

using InSite.Domain.Organizations;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class OrganizationInfo : UserControl
    {
        public void BindOrganization(OrganizationState organization)
        {
            OrganizationCode.Text = organization.OrganizationCode;
            OrganizationName.Text = organization.CompanyName;
            OrganizationTitle.Text = organization.CompanyDescription.LegalName;
        }
    }
}