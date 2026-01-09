using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using UserCompetencyRepository = InSite.Persistence.Plugin.CMDS.UserCompetencyRepository;
using UserProfileRepository = InSite.Persistence.Plugin.CMDS.UserProfileRepository;

namespace InSite.Cmds.Actions.Profile.Employee.Profile
{
    public partial class Search : AdminBasePage, ICmdsUserControl
    {
        #region Fields

        private string _firstName;

        #endregion

        #region Overriden methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            DisplaySelector.Visible = FilterSelector.ValueAsInt != EmployeeProfileFilterSelector.All;
        }

        #endregion

        #region Register new profile

        private void RegisterNewProfile()
        {
            AcquireInstruction.Text = string.Empty;
            AcquireInstruction.CssClass = string.Empty;

            if (NewProfile.Value == null)
                return;

            var isPrimary = NewProfileType.Value == "Primary";
            var department = isPrimary ? DepartmentSearch.Select(NewDepartment.Value.Value) : null;

            if (isPrimary && DepartmentProfileUserSearch.SelectFirst(x => x.UserIdentifier == PersonID && x.Department.OrganizationIdentifier == department.OrganizationIdentifier && x.IsPrimary) != null)
            {
                AcquireInstruction.Text = @"<i class='far fa-exclamation-triangle'></i> A user can only be assigned one primary profile, and that profile should reflect the job they are currently doing.";
                AcquireInstruction.CssClass = "text-danger";
            }
            else
            {
                try
                {
                    var isRecommended = string.Equals(NewProfileStatus.Value, "Required for Promotion", StringComparison.OrdinalIgnoreCase);
                    var isInProgress = string.Equals(NewProfileStatus.Value, "In Training", StringComparison.OrdinalIgnoreCase);

                    UserProfileRepository.RegisterNewProfile(
                        isPrimary,
                        NewDepartment.Value.Value,
                        PersonID,
                        NewProfile.Value.Value,
                        isRecommended,
                        isInProgress,
                        IsComplianceRequired.Checked
                        );
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot insert duplicate key row in object"))
                    {
                        AcquireInstruction.Text = @"<i class='far fa-exclamation-triangle'></i> A user can only be assigned one primary profile, and that profile should reflect the job they are currently doing.";
                        AcquireInstruction.CssClass = "text-danger";
                    }
                    else
                    {
                        AcquireInstruction.Text = ex.ToString();
                        AcquireInstruction.CssClass = "text-danger";
                    }
                }
            }

            LoadProfiles();

            EmploymentGrid.LoadData(PersonID);
        }

        #endregion

        #region Security

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            var adminAccess = Identity.IsInRole(CmdsRole.Programmers)
                              || Access.Administrate
                              || Access.Configure;

            AddSection.Visible = adminAccess;
            OtherOrganizationPanel.Visible = adminAccess;

            Grid.Columns.FindByName("DeleteColumn").Visible = adminAccess;

            EmploymentSection.Visible = EmploymentGrid.ApplySecurityPermissions();
        }

        #endregion

        #region Private properties

        private int CurrentFilter
        {
            get
            {
                if (FilterSelector.ValueAsInt == null)
                    throw new NullReferenceException("Filter cannot be null.");

                return FilterSelector.ValueAsInt.Value;
            }
        }

        private string DisplayFormat
        {
            get => string.IsNullOrEmpty(CurrentSessionState.EmployeeProfileGridDisplayFormat)
                ? "DisplayAsScore"
                : CurrentSessionState.EmployeeProfileGridDisplayFormat;
            set => CurrentSessionState.EmployeeProfileGridDisplayFormat = value;
        }

        protected Guid PersonID
        {
            get
            {
                Guid personID;

                return Guid.TryParse(Request["userID"], out personID)
                    ? personID
                    : User.UserIdentifier;
            }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddPosition.Click += AddPosition_Click;

            FilterSelector.ValueChanged += FilterSelector_ValueChanged;
            FilterSelector.AutoPostBack = true;

            DisplaySelector.SelectedIndexChanged += DisplaySelector_SelectedIndexChanged;
            DisplaySelector.AutoPostBack = true;

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowDataBound += Grid_RowDataBound;
            Grid.RowCommand += Grid_RowCommand;
            Grid.RowDeleting += (x, y) => { };

            NewDepartment.AutoPostBack = true;
            NewDepartment.ValueChanged += NewDepartment_ValueChanged;
            NewDepartment.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            EmploymentGrid.Deleted += EmploymentGrid_Deleted;

            NewProfileType.AutoPostBack = true;
            NewProfileType.ValueChanged += NewProfileType_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_firstName == null)
            {
                var person = UserSearch.Select(PersonID);
                _firstName = person.FirstName;
            }

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            UserCompetencyRepository.UpdateEmployeeCompetencies(PersonID);

            LoadData();

            if (Request.QueryString["panel"] == "primary-profile")
            {
                AddSection.IsSelected = true;
                IsComplianceRequired.Checked = true;
                IsComplianceRequired.Enabled = false;
                NewProfileType.Value = "Primary";
            }
            else if (Request.QueryString["panel"] == "organization-profiles")
            {
                CompanyProfilesSection.IsSelected = true;
            }
        }

        #endregion

        #region Event handlers

        private void NewProfileType_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            var isPrimary = NewProfileType.Value == "Primary";
            IsComplianceRequired.Checked = isPrimary;
            IsComplianceRequired.Enabled = !isPrimary;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var grid = (Grid)sender;
                var row = GridViewExtensions.GetRow(e);
                var departmentIdentifier = grid.GetDataKey<Guid>(row, "DepartmentIdentifier");
                var profileStandardIdentifier = grid.GetDataKey<Guid>(row, "ProfileStandardIdentifier");

                var entities = DepartmentProfileUserSearch.Select(x => x.DepartmentIdentifier == departmentIdentifier && x.UserIdentifier == PersonID && x.ProfileStandardIdentifier == profileStandardIdentifier);
                DepartmentProfileUserStore.Delete(entities, User.UserIdentifier, Organization.OrganizationIdentifier);

                Grid.DataBind();

                EmploymentGrid.LoadData(PersonID);
            }
        }

        private void Grid_DataBinding(object source, EventArgs e)
        {
            var table = UserProfileRepository.SelectGridWithCompetencyCount(PersonID, CurrentIdentityFactory.ActiveOrganizationIdentifier, false, Grid.PageIndex, Grid.PageSize);

            Grid.DataSource = table;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var row = (DataRowView)e.Row.DataItem;
            var isPrimary = (bool)row["IsPrimary"];

            var message = isPrimary
                ? string.Format("{0} is {1}'s primary profile. Are you sure you want to remove it?", row["ProfileNumber"], _firstName)
                : string.Format("Are you sure you want to remove {0} from {1}'s list of acquired profiles?", row["ProfileNumber"], _firstName);

            message = message.Replace("'", "\\'");

            var deleteButton = (Common.Web.UI.IconButton)e.Row.FindControl("DeleteButton");
            deleteButton.OnClientClick = string.Format("return confirm('{0}')", message);

            var statusText = row["StatusText"] as string;
            var flag = (Icon)e.Row.FindControl("Flag");

            if (StringHelper.Equals(statusText, "In Training") || StringHelper.Equals(statusText, "Required for Promotion"))
            {
                flag.Name = "wrench";
                flag.ToolTip = flag.ToolTip = statusText;
            }
            else
            {
                flag.Visible = false;
            }
        }

        protected void AddPosition_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            RegisterNewProfile();

            NewProfile.Value = null;
            NewProfileType.ClearSelection();
            NewProfileStatus.ClearSelection();
            IsComplianceRequired.Checked = false;

            Grid.DataBind();
        }

        protected void FilterSelector_ValueChanged(object sender, EventArgs e)
        {
            DisplayFormat = DisplaySelector.SelectedValue;
            Grid.DataBind();
        }

        protected void DisplaySelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            Grid.DataBind();
        }

        private void NewDepartment_ValueChanged(object sender, EventArgs e)
        {
            NewProfile.Filter.DepartmentIdentifier = NewDepartment.Value ?? Guid.Empty;
            NewProfile.Value = null;
        }

        private void EmploymentGrid_Deleted(object sender, EventArgs e)
        {
            Grid.DataBind();
        }

        #endregion

        #region Protected properties & methods

        protected string GetCriticalValue(object dataItem)
        {
            var row = ((DataRowView)dataItem).Row;

            int value;

            switch (CurrentFilter)
            {
                case EmployeeProfileFilterSelector.SelfAssessed:
                    value = (int)row["CriticalCompetencies_SelfAssessed"];
                    break;
                case EmployeeProfileFilterSelector.Submitted:
                    value = (int)row["CriticalCompetencies_Submitted"];
                    break;
                case EmployeeProfileFilterSelector.Validated:
                    value = (int)row["CriticalCompetencies_Validated"];
                    break;
                default:
                    return row["CriticalCompetencies"].ToString();
            }

            return GetValue(value, (int)row["CriticalCompetencies"]);
        }

        protected string GetNonCriticalValue(object dataItem)
        {
            var row = ((DataRowView)dataItem).Row;

            int value;

            switch (CurrentFilter)
            {
                case EmployeeProfileFilterSelector.SelfAssessed:
                    value = (int)row["NonCriticalCompetencies_SelfAssessed"];
                    break;
                case EmployeeProfileFilterSelector.Submitted:
                    value = (int)row["NonCriticalCompetencies_Submitted"];
                    break;
                case EmployeeProfileFilterSelector.Validated:
                    value = (int)row["NonCriticalCompetencies_Validated"];
                    break;
                default:
                    return row["NonCriticalCompetencies"].ToString();
            }

            return GetValue(value, (int)row["NonCriticalCompetencies"]);
        }

        protected string GetTotalValue(object dataItem)
        {
            var row = ((DataRowView)dataItem).Row;

            int value;

            switch (CurrentFilter)
            {
                case EmployeeProfileFilterSelector.SelfAssessed:
                    value = (int)row["CriticalCompetencies_SelfAssessed"] + (int)row["NonCriticalCompetencies_SelfAssessed"];
                    break;
                case EmployeeProfileFilterSelector.Submitted:
                    value = (int)row["CriticalCompetencies_Submitted"] + (int)row["NonCriticalCompetencies_Submitted"];
                    break;
                case EmployeeProfileFilterSelector.Validated:
                    value = (int)row["CriticalCompetencies_Validated"] + (int)row["NonCriticalCompetencies_Validated"];
                    break;
                default:
                    return ((int)row["CriticalCompetencies"] + (int)row["NonCriticalCompetencies"]).ToString();
            }

            return GetValue(value, (int)row["CriticalCompetencies"] + (int)row["NonCriticalCompetencies"]);
        }

        private string GetValue(int value, int total)
        {
            if (total == 0)
                return "0";

            switch (DisplaySelector.SelectedValue)
            {
                case "DisplayAsPercentage":
                    return string.Format("{0:#0.##}%", 100.0 * value / total);
                default:
                    return string.Format("{0} / {1}", value, total);
            }
        }

        #endregion

        #region Load data

        private void LoadData()
        {
            var person = UserSearch.Select(PersonID);

            PageHelper.AutoBindHeader(this, null, person.FullName);

            ActiveOrganizationName.InnerText = Organization.CompanyName;

            FilterSelector.EnsureDataBound();

            NewProfile.Filter.DepartmentIdentifier = Guid.Empty;
            NewProfile.Filter.ExcludeUserIdentifier = PersonID;

            DisplaySelector.SelectedValue = DisplayFormat;

            LoadProfiles();

            EmploymentGrid.LoadData(PersonID);

            CheckForOtherProfiles(person.FirstName);

            PersonEditorLink.NavigateUrl = string.Format("/ui/cmds/admin/users/edit?userID={0}", person.UserIdentifier);
        }

        private void CheckForOtherProfiles(string firstName)
        {
            if (!OtherOrganizationPanel.Visible)
                return;

            OtherGrid.Visible = false;
            OtherOrganizationHelp.InnerText = $"{firstName} has no active profiles in other organizations.";

            var hasOtherProfiles = UserProfileRepository.HasOtherProfiles(CurrentIdentityFactory.ActiveOrganizationIdentifier, PersonID);

            if (hasOtherProfiles)
            {
                var table = UserProfileRepository.SelectOtherProfiles(CurrentIdentityFactory.ActiveOrganizationIdentifier, PersonID);

                if (table != null)
                {
                    OtherGrid.EnablePaging = false;

                    OtherGrid.DataSource = table;
                    OtherGrid.DataBind();
                }

                OtherGrid.Visible = table != null && table.Rows.Count > 0;

                OtherOrganizationHelp.InnerText = $"{firstName} has {"profile".ToQuantity(table.Rows.Count)} in other organizations:";
                OtherProfilesButton.OnClientClick = string.Format("showOtherProfiles({0}); return false;", PersonID);
            }
        }

        private void LoadProfiles()
        {
            var profileCount = UserProfileRepository.SelectGridWithCompetencyCount(PersonID, CurrentIdentityFactory.ActiveOrganizationIdentifier, false, null, null).Rows.Count;

            ActiveOrganizationHelp.Visible = profileCount == 0;
            ActiveOrganizationHelp.InnerText = $"{_firstName} has no active profiles in this organization.";

            GridPanel.Visible = profileCount > 0;

            Grid.PageIndex = 0;
            Grid.VirtualItemCount = profileCount;
            Grid.DataBind();
        }

        #endregion
    }
}
