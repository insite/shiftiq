using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.StandardValidations.Write;
using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Actions.BulkTool.Validate
{
    public partial class Competency : AdminBasePage, ICmdsUserControl
    {
        #region Fields

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;
        private DataTable _validatorCompetencies;

        #endregion

        #region Security

        public override void ApplyAccessControlForCmds()
        {
            FinderSecurityInfo.LoadPermissions();
        }

        #endregion

        #region Properties

        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo
            ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private EmployeeCompetencyFilter EmployeeFilter
        {
            get
            {
                var filter = new EmployeeCompetencyFilter
                {
                    OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                    UserIdentifier = Employee.Value,
                    ProfileStandardIdentifier = CurrentProfile.Value,
                    Statuses = new[] { ValidationStatuses.SubmittedForValidation },
                    SelfAssessmentStatus = SelfAssessmentStatus.SelectedValue
                };

                return filter;
            }
        }

        private EmployeeCompetencyFilter ValidatorFilter
        {
            get
            {
                var filter = new EmployeeCompetencyFilter
                {
                    OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                    UserIdentifier = User.UserIdentifier,
                    Statuses = new[] { ValidationStatuses.Validated, ValidationStatuses.Expired },
                    ValidationDateMustBeSet = true
                };

                return filter;
            }
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Employee.AutoPostBack = true;
            Employee.ValueChanged += Employee_ValueChanged;

            CurrentProfile.AutoPostBack = true;
            CurrentProfile.ValueChanged += CurrentProfile_ValueChanged;

            SelfAssessmentStatus.AutoPostBack = true;
            SelfAssessmentStatus.SelectedIndexChanged += SelfAssessmentStatus_SelectedIndexChanged;

            Competencies.ItemDataBound += Competencies_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            YesButton.ButtonStyle = ValidatorSelection.SelectedIndex == 0
                ? ButtonStyle.Success
                : ButtonStyle.Default;

            NoButton.ButtonStyle = ValidatorSelection.SelectedIndex == 1
                ? ButtonStyle.Success
                : ButtonStyle.Default;

            if (Identity.IsImpersonating)
            {
                YesButton.Enabled = false;
                NoButton.Enabled = false;
            }

            SelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", CompetenciesPanel.ClientID);
            UnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", CompetenciesPanel.ClientID);
        }

        #endregion

        #region Event handlers

        private void Employee_ValueChanged(object sender, EventArgs e)
        {
            LoadProfiles();
        }

        private void CurrentProfile_ValueChanged(object sender, EventArgs e)
        {
            LoadProfileInfo();
        }

        private void SelfAssessmentStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProfileInfo();
        }

        private void Competencies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;

            bool canValidate;

            if (Identity.IsInRole(CmdsRole.SuperValidators) || Identity.IsInRole(CmdsRole.Programmers))
            {
                canValidate = true;
            }
            else
            {
                var filter = $"CompetencyStandardIdentifier = '{row["CompetencyStandardIdentifier"]}'";
                canValidate = _validatorCompetencies.Select(filter).Length > 0;
            }

            var isSelectedCheckBox = (ICheckBox)e.Item.FindControl("IsSelected");
            isSelectedCheckBox.Enabled = canValidate;
            isSelectedCheckBox.Text = $"{row["Number"]} - {row["Summary"]}";

            var cannotValidateLabel = e.Item.FindControl("CannotValidate");
            cannotValidateLabel.Visible = !canValidate;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var count = SaveData();

            if (count == 0)
            {
                EditorStatus.AddMessage(AlertType.Error,
                    "There are no competencies selected for validation.<br/>Please select one or more competencies.");
                return;
            }

            EditorStatus.AddMessage(AlertType.Success, string.Format("{0} have been updated.", "competency".ToQuantity(count)));

            LoadProfileInfo();
        }

        #endregion

        #region Load data

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            InitPredefinedComments();

            Employee.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            Employee.Filter.ParentUserIdentifier = User.UserIdentifier;
            Employee.Filter.RelationWithParent = new[] { RelationCategory.Validator };
            Employee.Filter.ExcludeUserIdentifier = User.UserIdentifier;

            if (!FinderSecurityInfo.CanSeeAllDepartments && !Identity.HasAccessToAllCompanies)
                Employee.Filter.DepartmentsForParentId = User.UserIdentifier;

            SelfAssessmentStatus.Items.Clear();
            SelfAssessmentStatus.Items.Add(new System.Web.UI.WebControls.ListItem("All", string.Empty));
            SelfAssessmentStatus.Items.Add(new System.Web.UI.WebControls.ListItem(SelfAssessedStatuses.SelfAssessed, SelfAssessedStatuses.SelfAssessed));
            SelfAssessmentStatus.Items.Add(new System.Web.UI.WebControls.ListItem(SelfAssessedStatuses.NotApplicable, SelfAssessedStatuses.NotApplicable));
            SelfAssessmentStatus.SelectedIndex = 0;

            LoadProfiles();
        }

        private void LoadProfiles()
        {
            CurrentProfile.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            CurrentProfile.Filter.ProfileUserIdentifier = Employee.Value ?? Guid.Empty;
            CurrentProfile.Value = null;

            LoadProfileInfo();
        }

        private void LoadProfileInfo()
        {
            ValidationPanel.Visible = Employee.Value.HasValue;

            if (!ValidationPanel.Visible)
                return;

            var table = UserCompetencyRepository.SelectSearchResults(EmployeeFilter, null, null);

            if (table.Rows.Count == 0)
            {
                ValidationPanel.Visible = false;
                EditorStatus.AddMessage(AlertType.Warning, "The selected person has no competencies submitted for validation");
                return;
            }

            _validatorCompetencies = UserCompetencyRepository.SelectSearchResults(ValidatorFilter, null, null);

            var view = table.DefaultView;
            view.Sort = "Number";

            Competencies.DataSource = view;
            Competencies.DataBind();
        }

        private void InitPredefinedComments()
        {
            PredefinedComments.DataSource = Comments.Validator;
            PredefinedComments.DataBind();
        }

        #endregion

        #region Save data

        private int SaveData()
        {
            if (!Employee.Value.HasValue)
                return 0;

            var employeeId = Employee.Value.Value;
            var isValidated = StringHelper.Equals(ValidatorSelection.SelectedValue, "Yes");
            var comment = ValidatorComment.Text;
            var count = 0;

            foreach (RepeaterItem item in Competencies.Items)
            {
                var checkbox = (ICheckBoxControl)item.FindControl("IsSelected");
                if (!checkbox.Checked)
                    continue;

                var standardId = Guid.Parse(((ITextControl)item.FindControl("CompetencyStandardIdentifier")).Text);
                var entity = StandardValidationSearch.SelectFirst(x => x.UserIdentifier == employeeId && x.StandardIdentifier == standardId);
                if (entity == null)
                    continue;

                var status = !isValidated
                    ? ValidationStatuses.NeedsTraining
                    : entity.SelfAssessmentStatus == SelfAssessedStatuses.NotApplicable
                        ? ValidationStatuses.NotApplicable
                        : ValidationStatuses.Validated;

                ServiceLocator.SendCommand(new ValidateStandardValidation(entity.ValidationIdentifier, UniqueIdentifier.Create(), isValidated, status, comment));

                count++;
            }

            return count;
        }

        #endregion
    }
}
