using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Identities.Departments.Controls
{
    public partial class Details : BaseUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                DivisionIdentifier.OrganizationIdentifier = Organization.Key;
                DivisionIdentifier.RefreshData();
            }
        }

        public void SetDefaultInputValues()
        {
            OrganizationIdentifier.Value = Organization.Key;
        }

        public void SetInputValues(Department entity)
        {
            OrganizationIdentifier.Value = entity.OrganizationIdentifier;
            DepartmentName.Text = entity.DepartmentName;
            DepartmentCode.Text = entity.DepartmentCode;
            DepartmentDescription.Text = entity.DepartmentDescription;

            DivisionIdentifier.ValueAsGuid = entity.DivisionIdentifier;
        }

        public void GetInputValues(Department entity)
        {
            entity.OrganizationIdentifier = OrganizationIdentifier.Value.Value;
            entity.DepartmentName = DepartmentName.Text;
            entity.DepartmentCode = DepartmentCode.Text;
            entity.DepartmentDescription = DepartmentDescription.Text;

            entity.DivisionIdentifier = DivisionIdentifier.ValueAsGuid;
        }
    }
}