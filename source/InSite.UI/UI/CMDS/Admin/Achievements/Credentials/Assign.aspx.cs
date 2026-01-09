using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Actions.BulkTool.Assign
{
    public class PersonFinderSecurityInfoWrapper
    {
        private readonly StateBag _viewState;

        public PersonFinderSecurityInfoWrapper(StateBag viewState)
        {
            _viewState = viewState;
        }

        public bool CanSeeAllCompanies
        {
            get => (bool)(_viewState[nameof(CanSeeAllCompanies)] ?? true);
            set => _viewState[nameof(CanSeeAllCompanies)] = value;
        }

        public bool CanSeeAllCompanyPeople
        {
            get => (bool)(_viewState[nameof(CanSeeAllCompanyPeople)] ?? true);
            set => _viewState[nameof(CanSeeAllCompanyPeople)] = value;
        }

        public bool CanSeeAllDepartments
        {
            get => (bool)(_viewState[nameof(CanSeeAllDepartments)] ?? true);
            set => _viewState[nameof(CanSeeAllDepartments)] = value;
        }

        public void LoadPermissions()
        {
            var permissionName = PermissionNames.Custom_CMDS_Workers;

            var identity = CurrentSessionState.Identity;
            if (identity.IsGranted(permissionName, PermissionOperation.Configure))
                return;

            CanSeeAllCompanies = false;
            CanSeeAllCompanyPeople = identity.IsGranted(permissionName, PermissionOperation.Delete);
            CanSeeAllDepartments = false;
        }
    }

    public partial class Education : AdminBasePage
    {
        #region Fields

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;

        #endregion

        #region Helper methods

        private string[] GetRoleTypes()
        {
            if (!Department.HasValue)
                return null;

            var list = new List<string>();

            if (OrganizationEmployment.Checked)
                list.Add("Organization");

            if (DepartmentEmployment.Checked)
                list.Add("Department");

            if (DataAccess.Checked)
                list.Add("Administration");

            return list.ToArray();
        }

        #endregion

        #region Load employees

        private void LoadEmployees()
        {
            if (Department.HasValue || JobDivision.HasValue)
            {
                var filter = new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    JobDivision = JobDivision.Value,
                    DepartmentIdentifier = Department.Value,
                    KeyeraRoles = new[] { CmdsRole.Workers }
                };

                if (!FinderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies)
                    filter.ParentUserIdentifier = User.UserIdentifier;

                if (!FinderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies &&
                    Department.Value == null)
                    filter.DepartmentsForParentId = User.UserIdentifier;

                filter.RoleType = GetRoleTypes();

                var table = ContactRepository3.SelectPersonsWithPolicyInfo(filter, SelectedAchievements, Organization.Identifier);

                Employees.DataSource = table;
                Employees.DataBind();

                EmployeesTable.Visible = table.Rows.Count > 0;
                Employees.Visible = EmployeesTable.Visible;

                NoCriteria.Visible = table.Rows.Count == 0;
            }
            else
            {
                Employees.DataSource = null;
                Employees.DataBind();

                EmployeesTable.Visible = false;
                Employees.Visible = EmployeesTable.Visible;

                NoCriteria.Visible = true;
            }
        }

        #endregion

        #region Security

        public override void ApplyAccessControl()
        {
            if (!Identity.IsGranted("cmds/users/assign-achievements"))
                CreateAccessDeniedException();

            FinderSecurityInfo.LoadPermissions();

            var canSeeDataAccess = Identity.IsInRole(CmdsRole.Programmers)
                                   || Identity.IsInRole(CmdsRole.SystemAdministrators)
                                   || Identity.IsInRole(CmdsRole.OfficeAdministrators);

            var canSeeCompanyEmployment = canSeeDataAccess
                                          || Identity.IsInRole(CmdsRole.FieldAdministrators)
                                          || Identity.IsInRole(CmdsRole.Validators)
                                          || Identity.IsInRole(CmdsRole.Managers)
                                          || Identity.IsInRole(CmdsRole.Supervisors);

            var canSeeDepartmentEmployment = canSeeCompanyEmployment;

            DepartmentEmployment.Visible = canSeeDepartmentEmployment;
            OrganizationEmployment.Visible = canSeeCompanyEmployment;
            DataAccess.Visible = canSeeDataAccess;
        }

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/admin/tools";

        private PersonFinderSecurityInfoWrapper FinderSecurityInfo =>
            _finderSecurityInfo ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private bool AchievementListHasValue
        {
            get
            {
                foreach (System.Web.UI.WebControls.ListItem item in AchievementList.Items)
                    if (item.Selected)
                        return true;

                return false;
            }
        }

        private Guid[] SelectedAchievements
        {
            get
            {
                var list = new List<Guid>();

                foreach (System.Web.UI.WebControls.ListItem item in AchievementList.Items)
                    if (item.Selected)
                    {
                        var id = Guid.Parse(item.Value);
                        list.Add(id);
                    }

                return list.ToArray();
            }
        }

        private Guid[] SelectedUsers
        {
            get
            {
                var list = new List<Guid>();

                foreach (RepeaterItem item in Employees.Items)
                {
                    var userId = (Common.Web.UI.CheckBox)item.FindControl("UserIdentifier");

                    var id = Guid.Parse(userId.Value);

                    if (userId.Checked)
                        list.Add(id);
                }

                return list.ToArray();
            }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementType.AutoPostBack = true;
            AchievementType.ValueChanged += SubType_ValueChanged;

            AchievementVisibility.AutoPostBack = true;
            AchievementVisibility.ValueChanged += AchievementVisibility_ValueChanged;

            Category.AutoPostBack = true;
            Category.ValueChanged += Category_ValueChanged;

            AchievementCategory.AutoPostBack = true;
            AchievementCategory.ValueChanged += Category_ValueChanged;

            AchievementList.AutoPostBack = true;
            AchievementList.SelectedIndexChanged += AchievementList_SelectedIndexChanged;

            JobDivision.AutoPostBack = true;
            JobDivision.ValueChanged += JobDivision_ValueChanged;

            Department.AutoPostBack = true;
            Department.ValueChanged += Department_ValueChanged;

            OrganizationEmployment.AutoPostBack = true;
            OrganizationEmployment.CheckedChanged += (x, y) => LoadEmployees();

            DepartmentEmployment.AutoPostBack = true;
            DepartmentEmployment.CheckedChanged += (x, y) => LoadEmployees();

            DataAccess.AutoPostBack = true;
            DataAccess.CheckedChanged += (x, y) => LoadEmployees();

            EmployeeRequired.ServerValidate += EmployeeRequired_ServerValidate;
            AchievementListRequired.ServerValidate += AchievementListRequired_ServerValidate;

            SelectAllAchievementsButton.Click += SelectAllAchievementsButton_Click;
            UnselectAllAchievementsButton.Click += UnselectAllAchievementsButton_Click;

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                AchievementType.ExcludeSubType = AchievementTypes.TimeSensitiveSafetyCertificate;
                AchievementType.RefreshData();

                SetCategoryVisibility();

                Category.ListFilter.OrganizationIdentifier = OrganizationSearch.Select(CurrentIdentityFactory.ActiveOrganizationIdentifier).OrganizationIdentifier;
                Category.ListFilter.AchievementLabel = AchievementType.Value;
                Category.RefreshData();

                LoadAchievements();

                Open();

                PageHelper.AutoBindHeader(this);

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", EmployeesTable.ClientID);
            UnselectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", EmployeesTable.ClientID);

            trValidFor.Style["display"] = IsTimeSensitive.Checked ? "" : "none";

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void SubType_ValueChanged(object sender, EventArgs e)
        {
            if (AchievementType.Value != AchievementTypes.Module)
                AchievementVisibility.Value = "Organization-Specific Achievements";

            SetCategoryVisibility();

            Category.ListFilter.AchievementLabel = AchievementType.Value;
            Category.RefreshData();

            LoadAchievements();
        }

        private void AchievementVisibility_ValueChanged(object sender, EventArgs e)
        {
            SetCategoryVisibility();

            AchievementCategory.OrganizationIdentifier = AchievementVisibility.Value == "Global Achievements"
                ? OrganizationIdentifiers.CMDS
                : Organization.OrganizationIdentifier;
            AchievementCategory.RefreshData();

            LoadAchievements();
        }

        private void Category_ValueChanged(object sender, EventArgs e)
        {
            LoadAchievements();
        }

        private void AchievementList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AchievementListHasValue)
                return;

            CheckAllAchievementsTimeSensitive();

            LoadEmployees();
        }

        private void JobDivision_ValueChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void Department_ValueChanged(object sender, EventArgs e)
        {
            RolesDiv.Visible = Department.HasValue;

            LoadEmployees();
        }

        private void EmployeeRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            foreach (RepeaterItem item in Employees.Items)
            {
                var userId = (Common.Web.UI.CheckBox)item.FindControl("UserIdentifier");

                if (userId.Checked)
                {
                    args.IsValid = true;
                    break;
                }
            }
        }

        private void AchievementListRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = AchievementListHasValue;
        }

        private void SelectAllAchievementsButton_Click(object sender, EventArgs e)
        {
            foreach (System.Web.UI.WebControls.ListItem item in AchievementList.Items)
                item.Selected = true;

            if (!AchievementListHasValue)
                return;

            CheckAllAchievementsTimeSensitive();

            LoadEmployees();
        }

        private void UnselectAllAchievementsButton_Click(object sender, EventArgs e)
        {
            foreach (System.Web.UI.WebControls.ListItem item in AchievementList.Items)
                item.Selected = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            Open();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Delete();
        }

        #endregion

        #region Load & Save

        private void Open()
        {
            if (!IsPostBack)
            {
                Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

                if (!FinderSecurityInfo.CanSeeAllDepartments && !Identity.HasAccessToAllCompanies)
                    Department.Filter.UserIdentifier = User.UserIdentifier;

                Department.Value = null;
            }

            LoadEmployees();
        }

        private bool Save()
        {
            if (!ValidateAchievements())
                return false;

            if (!ValidatePeople())
                return false;

            var credentialsCreated = new Dictionary<Guid, int>();
            var credentialsModified = new Dictionary<Guid, int>();
            var learnerPlansModified = new Dictionary<Guid, int>();

            var userIds = SelectedUsers;
            var achievementIds = SelectedAchievements;

            var credentials = VCmdsCredentialSearch
                .Bind(
                    x => x,
                    x => SelectedAchievements.Contains(x.AchievementIdentifier) && SelectedUsers.Contains(x.UserIdentifier))
                .ToDictionary(x => new MultiKey<Guid, Guid>(x.UserIdentifier, x.AchievementIdentifier));

            var necessity = IsMandatory.Checked ? "Mandatory" : "Optional";
            var priority = IsPlanned.Checked ? "Planned" : "Unplanned";

            Func<Expiration> expirationFactory;
            if (IsTimeSensitive.Checked && ValidForCount.ValueAsInt.HasValue)
            {
                var validForCount = ValidForCount.ValueAsInt.Value;

                expirationFactory = () => new Expiration
                {
                    Type = ExpirationType.Relative,
                    Lifetime = new Lifetime
                    {
                        Quantity = validForCount,
                        Unit = "Month"
                    }
                };
            }
            else
            {
                expirationFactory = () => null;
            }

            var commands = new List<Command>();
            var newCredentials = new List<MultiKey<Guid, Guid>>();

            foreach (var userId in userIds)
            {
                foreach (var achievementId in achievementIds)
                {
                    var key = new MultiKey<Guid, Guid>(userId, achievementId);

                    if (credentials.TryGetValue(key, out var credential))
                    {
                        var expiration = expirationFactory();

                        if (credential.CredentialIsMandatory != IsMandatory.Checked
                         || credential.IsInTrainingPlan != IsPlanned.Checked
                         || credential.CredentialExpirationLifetimeUnit != expiration?.Lifetime?.Unit
                         || credential.CredentialExpirationLifetimeQuantity != expiration?.Lifetime?.Quantity)
                        {
                            commands.Add(new ChangeCredentialExpiration(credential.CredentialIdentifier, expiration));
                            commands.Add(new TagCredential(credential.CredentialIdentifier, necessity, priority));

                            if (!credentialsModified.ContainsKey(achievementId))
                                credentialsModified.Add(achievementId, 1);

                            credentialsModified[achievementId]++;

                            if (!learnerPlansModified.ContainsKey(userId))
                                learnerPlansModified.Add(userId, 1);

                            learnerPlansModified[userId]++;
                        }
                    }
                    else
                    {
                        newCredentials.Add(key);
                    }
                }
            }

            if (newCredentials.Count > 0)
            {
                var achievements = ServiceLocator.AchievementSearch
                    .GetAchievements(newCredentials.Select(x => x.Key2).Distinct())
                    .ToDictionary(x => x.AchievementIdentifier);

                Dictionary<MultiKey<Guid, Guid>, QCredentialHistory> tombstones;
                {
                    var tombstoneFilter = newCredentials
                        .GroupBy(x => x.Key1)
                        .ToDictionary(x => x.Key, x => x.Select(y => y.Key2).ToArray());
                    tombstones = ServiceLocator.AchievementSearch
                        .GetCredentialHistory(tombstoneFilter.Keys, c => tombstoneFilter[c])
                        .GroupBy(x => new MultiKey<Guid, Guid>(x.UserIdentifier, x.AchievementIdentifier))
                        .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.ChangeTime).First());
                }

                for (var i = 0; i < newCredentials.Count; i++)
                {
                    var key = newCredentials[i];
                    var userId = key.Key1;
                    var achievementId = key.Key2;
                    var credentialId = ServiceLocator.AchievementSearch.GetCredentialIdentifier(null, tombstones.GetOrDefault(key));

                    var expiration = expirationFactory();

                    var label = achievements[achievementId].AchievementLabel;
                    var authorityType = InSite.Common.Web.Cmds.EmployeeAchievementHelper.TypeAllowsSignOff(label)
                        ? "Self" : null;

                    commands.Add(new CreateCredential(credentialId, Organization.OrganizationIdentifier, achievementId, userId, DateTimeOffset.Now));
                    commands.Add(new ChangeCredentialExpiration(credentialId, expiration));
                    commands.Add(new TagCredential(credentialId, necessity, priority));
                    commands.Add(new ChangeCredentialAuthority(credentialId, null, null, authorityType, null, null, null));

                    if (!credentialsCreated.ContainsKey(achievementId))
                        credentialsCreated.Add(achievementId, 1);

                    credentialsCreated[achievementId]++;

                    if (!learnerPlansModified.ContainsKey(userId))
                        learnerPlansModified.Add(userId, 1);

                    learnerPlansModified[userId]++;
                }
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            LoadEmployees();

            var message =
                "Updated " +
                Shift.Common.Humanizer.ToQuantity(learnerPlansModified.Count, "training plan") +
                " based on the selection of " +
                Shift.Common.Humanizer.ToQuantity(achievementIds.Length, "achievement") +
                ". " +
                Shift.Common.Humanizer.ToQuantity(credentialsCreated.Count, "new learner achievement") +
                " assigned, and " +
                Shift.Common.Humanizer.ToQuantity(credentialsCreated.Count, "previously assigned achievement") +
                " modified.";

            EditorStatus.AddMessage(AlertType.Success, message);

            return true;
        }

        private bool ValidateAchievements()
        {
            if (SelectedAchievements.Length == 0)
            {
                EditorStatus.AddMessage(AlertType.Error, "Please select one or more Achievements to bulk-assign.");
                return false;
            }

            if (IsTimeSensitive.Checked && !ValidForCount.ValueAsInt.HasValue)
            {
                EditorStatus.AddMessage(AlertType.Error,
                    "You have checked the Time-Sensitive box. Please input a <strong>Valid For</strong> interval.");
                return false;
            }

            return true;
        }

        private bool ValidatePeople()
        {
            foreach (RepeaterItem item in Employees.Items)
            {
                var userId = (Common.Web.UI.CheckBox)item.FindControl("UserIdentifier");

                if (userId.Checked)
                    return true;
            }

            EditorStatus.AddMessage(AlertType.Error, "Please select one or more People to bulk-assign or bulk-delete.");

            return false;
        }

        private void Delete()
        {
            if (!ValidatePeople())
                return;

            var achievements = SelectedAchievements;
            var commands = new List<Command>();

            foreach (var achievement in achievements)
            {
                foreach (RepeaterItem item in Employees.Items)
                {
                    var userId = (Common.Web.UI.CheckBox)item.FindControl("UserIdentifier");

                    if (!userId.Checked)
                        continue;

                    var id = Guid.Parse(userId.Value);

                    var credential = ServiceLocator.AchievementSearch.GetCredential(achievement, id);

                    if (credential != null)
                        commands.Add(new DeleteCredential(credential.CredentialIdentifier));
                }
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            LoadEmployees();

            EditorStatus.AddMessage(AlertType.Success, "Your changes have been saved.");
        }

        #endregion

        #region Load achievements

        private void LoadAchievements()
        {
            var filter = new VCmdsAchievementFilter { AchievementType = AchievementType.Value };

            if (AchievementVisibility.Value == "Organization-Specific Achievements")
            {
                filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
                filter.CategoryIdentifier = Category.ValueAsGuid;
                filter.AchievementVisibility = AccountScopes.Organization;
            }
            else if (AchievementVisibility.Value == "Global Achievements")
            {
                filter.AchievementVisibility = AccountScopes.Enterprise;

                if (AchievementType.Value == AchievementTypes.Module && AchievementCategory.ValueAsGuid.HasValue)
                    filter.AchievementCategory = AchievementCategory.GetSelectedOption().Text;
            }
            else
            {
                filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
                filter.GlobalOrCompanySpecific = true;
            }

            filter.OrganizationCode = Organization.Code;
            filter.ExcludeHidden = true;

            var table = VCmdsAchievementSearch.SelectForSelector(filter, false);

            AchievementList.Items.Clear();

            foreach (DataRow row in table.Rows)
            {
                var value = (Guid)row["Value"];
                var text = (string)row["Text"];

                var item = new System.Web.UI.WebControls.ListItem(text, value.ToString());

                AchievementList.Items.Add(item);
            }

            Policies.Visible = table.Rows.Count > 0;
        }

        private void CheckAllAchievementsTimeSensitive()
        {
            var isFound = false;
            int? validForCount = null;
            string validForUnit = null;

            foreach (System.Web.UI.WebControls.ListItem item in AchievementList.Items)
            {
                if (!item.Selected)
                    continue;

                var achievementIdentifier = Guid.Parse(item.Value);
                var achievement = VCmdsAchievementSearch.Select(achievementIdentifier);

                if (achievement.ValidForCount == null)
                    return;

                if (!isFound)
                {
                    isFound = true;
                    validForCount = achievement.ValidForCount;
                    validForUnit = achievement.ValidForUnit;
                }
                else if (validForCount != achievement.ValidForCount || validForUnit != achievement.ValidForUnit)
                {
                    return;
                }
            }

            if (!isFound)
                return;

            SetTimeSensitiveValues(AchievementType.Value, validForCount, validForUnit);
        }

        private void SetTimeSensitiveValues(string label, int? validForCount, string validForUnit)
        {
            IsTimeSensitive.Checked = true;
            ValidForCount.ValueAsInt = string.Equals(validForUnit, ValidForUnits.Years, StringComparison.OrdinalIgnoreCase)
                ? 12 * validForCount
                : validForCount;

            IsTimeSensitive.Enabled = label != "Orientation";
            ValidForCount.Enabled = label != "Orientation";
        }

        private void SetCategoryVisibility()
        {
            Categories.Visible = AchievementVisibility.Value == "Organization-Specific Achievements"
                              || AchievementVisibility.Value == "Global Achievements"
                              && AchievementType.Value == AchievementTypes.Module;

            Category.Visible = AchievementVisibility.Value == "Organization-Specific Achievements";

            AchievementCategory.Visible = AchievementVisibility.Value != "Organization-Specific Achievements";
        }

        #endregion
    }
}
