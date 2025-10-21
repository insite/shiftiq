using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

namespace InSite.Custom.CMDS.Admin.Standards.DepartmentProfileUsers.Controls
{
    public partial class Detail : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DuplicateValidator.ServerValidate += DuplicateValidator_ServerValidate;

            CompanySelector.AutoPostBack = true;
            CompanySelector.ValueChanged += CompanySelector_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                UserIdentifier.Filter.EnableIsArchived = false;
        }

        public void SetDefaultInputValues(Guid organization)
        {
            CompanySelector.Value = organization;

            InitDepartment();
        }

        public bool SetInputValues(DepartmentProfileUser entity)
        {
            CompanySelector.Value = entity.Department.OrganizationIdentifier;

            InitDepartment();

            DepartmentIdentifier.Value = entity.DepartmentIdentifier;
            ProfileStandardIdentifier.Value = entity.ProfileStandardIdentifier;
            UserIdentifier.Value = entity.UserIdentifier;

            IsPrimary.Checked = entity.IsPrimary;
            IsRequired.Checked = entity.IsRequired;
            IsRecommended.Checked = entity.IsRecommended;
            IsInProgress.Checked = entity.IsInProgress;

            CompanySelector.Enabled = false;
            DepartmentIdentifier.Enabled = false;
            ProfileStandardIdentifier.Enabled = false;
            UserIdentifier.Enabled = false;

            return true;
        }

        public void GetInputValues(DepartmentProfileUser entity)
        {
            entity.DepartmentIdentifier = DepartmentIdentifier.Value.Value;
            entity.ProfileStandardIdentifier = ProfileStandardIdentifier.Value.Value;
            entity.UserIdentifier = UserIdentifier.Value.Value;

            entity.IsPrimary = IsPrimary.Checked;
            entity.IsRequired = IsRequired.Checked;
            entity.IsRecommended = IsRecommended.Checked;
            entity.IsInProgress = IsInProgress.Checked;
        }

        private void DuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (DepartmentIdentifier.Enabled && DepartmentIdentifier.HasValue && ProfileStandardIdentifier.HasValue && UserIdentifier.HasValue)
            {
                var departmentKey = DepartmentIdentifier.Value.Value;
                var profileStandardIdentifier = ProfileStandardIdentifier.Value.Value;
                var userIdentifier = UserIdentifier.Value.Value;
                var existing = DepartmentProfileUserSearch.SelectFirst(x => x.DepartmentIdentifier == departmentKey && x.ProfileStandardIdentifier == profileStandardIdentifier && x.UserIdentifier == userIdentifier);

                args.IsValid = existing == null;
            }
        }

        private void CompanySelector_ValueChanged(object sender, EventArgs e)
        {
            InitDepartment();
        }

        private void InitDepartment()
        {
            DepartmentIdentifier.Filter.OrganizationIdentifier = CompanySelector.Value ?? Guid.Empty;
            DepartmentIdentifier.Value = null;
        }
    }
}