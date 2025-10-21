using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.Cmds.Controls.Contacts.Departments
{
    public partial class DepartmentDetails : UserControl
    {
        public void SetDefaultInputValues(Guid organizationId)
        {
            SetupOrganization(organizationId);
        }

        public void SetInputValues(Department department, Guid? division)
        {
            DepartmentName.Text = department.DepartmentName;
            Description.Text = department.DepartmentDescription;

            SetupOrganization(department.OrganizationIdentifier);

            DivisionIdentifier.ValueAsGuid = division ?? department.DivisionIdentifier;
        }

        public void GetInputValues(Department department)
        {
            department.DepartmentName = DepartmentName.Text.Trim();
            department.DepartmentCode = null;
            department.DepartmentDescription = Description.Text;
            department.DivisionIdentifier = DivisionIdentifier.ValueAsGuid;
        }

        private void SetupOrganization(Guid organizationId)
        {
            DivisionIdentifier.OrganizationIdentifier = organizationId;
            DivisionIdentifier.RefreshData();

            var organization = OrganizationSearch.Select(organizationId);

            DivisionPanel.Visible = OrganizationHelper.EnableDivisions(organization.CompanyDescription.CompanySize);
        }
    }
}