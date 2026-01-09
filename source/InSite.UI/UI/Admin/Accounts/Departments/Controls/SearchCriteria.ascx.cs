using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Identities.Departments.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<DepartmentFilter>
    {
        public override DepartmentFilter Filter
        {
            get
            {
                var filter = new DepartmentFilter
                {
                    DepartmentName = DepartmentName.Text,
                    DepartmentCode = DepartmentCode.Text,
                    DivisionIdentifier = DivisionIdentifier.ValueAsGuid,
                    CreatedSince = CreatedSince.Value,
                    CreatedBefore = CreatedBefore.Value,
                    CompanyName = CompanyName.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DepartmentName.Text = value.DepartmentName;
                DepartmentCode.Text = value.DepartmentCode;
                DivisionIdentifier.ValueAsGuid = value.DivisionIdentifier;
                CreatedSince.Value = value.CreatedSince;
                CreatedBefore.Value = value.CreatedBefore;
                CompanyName.Text = value.CompanyName;
            }
        }

        public override void Clear()
        {
            DepartmentName.Text = null;
            DepartmentCode.Text = null;
            DivisionIdentifier.Value = null;
            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            CompanyName.Text = string.Empty;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                DivisionIdentifier.OrganizationIdentifier = Organization.Identifier;
                DivisionIdentifier.RefreshData();
            }

            base.OnLoad(e);
        }
    }
}