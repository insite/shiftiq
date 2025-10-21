using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<DepartmentProfileUserFilter>
    {
        public override DepartmentProfileUserFilter Filter
        {
            get
            {
                var filter = new DepartmentProfileUserFilter
                {
                    OrganizationIdentifier = CompanySelector.Value,
                    DepartmentIdentifier = DepartmentSelector.Value,
                    ProfileStandardIdentifier = ProfileSelector.Value,
                    UserIdentifier = Person.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                CompanySelector.Value = value.OrganizationIdentifier;

                InitDepartment();

                DepartmentSelector.Value = value.DepartmentIdentifier;
                ProfileSelector.Value = value.ProfileStandardIdentifier;
                Person.Value = value.UserIdentifier;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CompanySelector.AutoPostBack = true;
            CompanySelector.ValueChanged += (s, a) => InitDepartment();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                InitDepartment();

                Person.Filter.EnableIsArchived = false;
            }
        }

        public override void Clear()
        {
            CompanySelector.Value = null;
            DepartmentSelector.Value = null;
            ProfileSelector.Value = null;
            Person.Value = null;

            InitDepartment();
        }

        private void InitDepartment()
        {
            DepartmentSelector.Filter.OrganizationIdentifier = CompanySelector.Value ?? Guid.Empty;
            DepartmentSelector.Value = null;
        }
    }
}