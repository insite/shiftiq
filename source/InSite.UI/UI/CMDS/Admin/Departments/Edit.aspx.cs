using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Sdk.UI;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Admin.Departments.Forms
{
    public partial class Edit : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/cmds/admin/organizations/search";
        private const string ParentUrl = "/ui/cmds/admin/organizations/edit";
        private const string PrioritiesUrl = "/ui/cmds/admin/departments/prioritize-competencies";

        #endregion

        #region Properties

        private Guid? OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        private Guid DepartmentIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProfileEditor.InitDelegates(SelectProfiles, SelectProfilesToAdd, DeleteProfiles, InsertProfiles, null, ScreenStatus, "department");

            AchievementEditor.InitDelegates(
                Organization.Identifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "department");

            RemoveReferencesButton.Click += RemoveReferencesButton_Click;

            SaveButton.Click += SaveButton_Click;

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();

            CancelButton.NavigateUrl = GetParentUrl();

            Page.ClientScript.RegisterStartupScript(GetType(), "Init", "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(DepartmentEditor_endRequest);", true);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"id={OrganizationIdentifier}&panel=departments"
                : null;
        }

        private string GetParentUrl() =>
            ParentUrl + $"?id={OrganizationIdentifier}";

        #endregion

        #region Security

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            RemoveReferencesButton.Visible = Identity.IsInRole(CmdsRole.Programmers);
            SaveButton.Visible = Access.Write;
            DeleteButton.Visible = Access.Delete;
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!Save())
                return;

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Delete())
                HttpResponseHelper.Redirect(GetParentUrl());
        }

        private void RemoveReferencesButton_Click(object sender, EventArgs e)
        {
            ContactRepository2.DeleteDepartmentReferences(DepartmentIdentifier);
            DepartmentProfileUserStore.DeleteByDepartmentIdentifier(DepartmentIdentifier, User.UserIdentifier, Organization.Identifier);

            Open();

            ScreenStatus.AddMessage(AlertType.Success, "References to this department have been removed.");
        }

        #endregion

        #region Load & Save

        private void Open()
        {
            var department = DepartmentSearch.Select(DepartmentIdentifier);
            if (department == null)
                HttpResponseHelper.Redirect(SearchUrl);

            OrganizationIdentifier = department.OrganizationIdentifier;

            Details.SetInputValues(department, department.DivisionIdentifier);

            var organization = OrganizationSearch.Select(department.OrganizationIdentifier);

            PageHelper.AutoBindHeader(this, qualifier: $"{organization.CompanyName} ({department.DepartmentName})");

            AchievementEditor.SetEditable(CanEdit, CanEdit);
            AchievementEditor.LoadAchievements();

            var personFilter = new PersonFilter
            {
                DepartmentIdentifier = department.DepartmentIdentifier,
                RoleType = new[] { MembershipType.Department },
                EnableIsArchived = false
            };

            PersonGrid.SetVisibleColumns(new[] { "Name", "City", "Province", "EmailWork", "ToolTipWithLinks" });
            PersonGrid.LoadData(personFilter);
            PersonGrid.ShowFilterPanel();

            DepartmentSkillEditorLink.NavigateUrl = PrioritiesUrl + $"?id={department.DepartmentIdentifier}";
        }

        private bool Save()
        {
            var department = DepartmentSearch.Select(DepartmentIdentifier);
            if (department == null)
                HttpResponseHelper.Redirect(SearchUrl);

            Details.GetInputValues(department);

            var commands = new List<Command>();
            commands.Add(new RenameGroup(department.DepartmentIdentifier, "Department", department.DepartmentName));
            commands.Add(new DescribeGroup(department.DepartmentIdentifier, null, department.DepartmentCode, department.DepartmentDescription, department.DepartmentLabel));
            commands.Add(new ChangeGroupParent(department.DepartmentIdentifier, department.DivisionIdentifier));

            ServiceLocator.SendCommands(commands);

            return true;
        }

        private bool Delete()
        {
            if (!CheckDeleteAllowed())
                return false;

            GroupHelper.Delete(new Commander(), ServiceLocator.GroupSearch, ServiceLocator.RegistrationSearch, ServiceLocator.PersonSearch, DepartmentIdentifier);

            return true;
        }

        #endregion

        #region Profile Editor

        private DataTable SelectProfiles()
        {
            return ProfileRepository.SelectDepartmentProfilesOnly(DepartmentIdentifier);
        }

        private DataTable SelectProfilesToAdd(string searchText)
        {
            return ProfileRepository.SelectNewDepartmentProfiles(DepartmentIdentifier, searchText);
        }

        private bool DeleteProfiles(IList<Guid> profiles)
        {
            if (!CheckProfileDeleteAllowed(profiles))
                return false;

            TDepartmentStandardStore.DeleteByDepartment(DepartmentIdentifier, profiles);
            DepartmentProfileCompetencyRepository2.DeleteUnusedByDepartmentIdentifier(DepartmentIdentifier);

            return true;
        }

        private int InsertProfiles(IList<Guid> profiles)
        {
            var newProfiles = new List<Guid>();

            foreach (var profileStandardIdentifier in profiles)
            {
                if (!TDepartmentStandardSearch.Exists(x => x.DepartmentIdentifier == DepartmentIdentifier && x.StandardIdentifier == profileStandardIdentifier))
                    newProfiles.Add(profileStandardIdentifier);
            }

            TDepartmentStandardStore.InsertPermissions(DepartmentIdentifier, profiles);

            foreach (Guid profileStandardIdentifier in newProfiles)
                DepartmentProfileCompetencyRepository2.InsertProfileCompetencies(DepartmentIdentifier, profileStandardIdentifier);

            return newProfiles.Count;
        }

        #endregion

        #region Achievement Editor

        private List<AchievementListGridItem> SelectAchievements(Guid enterpriseId, Guid organizationId, string scope, string keyword)
        {
            return VCmdsAchievementSearch.SelectDepartmentAchievements(DepartmentIdentifier);
        }

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var groupId = DepartmentIdentifier;

            var groupAchievementIds = TAchievementDepartmentSearch
                .Bind(x => x.AchievementIdentifier, x => x.DepartmentIdentifier == groupId);

            var assignedAchievementIds = groupAchievementIds.Select(x => x).ToList();

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            var list = TAchievementDepartmentSearch.Select(x => x.DepartmentIdentifier == DepartmentIdentifier && achievements.Contains(x.AchievementIdentifier));
            TAchievementDepartmentStore.Delete(list);
        }

        private int InsertAchievements(IEnumerable<Guid> achievements)
        {
            var list = new List<TAchievementDepartment>();

            foreach (var achievementID in achievements)
            {
                if (!TAchievementDepartmentSearch.Exists(x => x.DepartmentIdentifier == DepartmentIdentifier && x.AchievementIdentifier == achievementID))
                    list.Add(new TAchievementDepartment { DepartmentIdentifier = DepartmentIdentifier, AchievementIdentifier = achievementID });
            }

            TAchievementDepartmentStore.Insert(list);

            return list.Count;
        }

        #endregion

        #region Check permissions

        private bool CheckProfileDeleteAllowed(IList<Guid> profiles)
        {
            foreach (var profileStandardIdentifier in profiles)
            {
                var employees = UserProfileRepository.SelectEmployees(DepartmentIdentifier, profileStandardIdentifier);

                if (employees.Rows.Count == 0)
                    continue;

                var profile = StandardSearch.Select(profileStandardIdentifier);

                var message = new StringBuilder();

                message.AppendFormat("Profile <strong>{0}: {1}</strong> is acquired by the following employees in this department, and therefore cannot be removed from the department:<br/>",
                    profile.Code, profile.ContentTitle);

                message.Append("<ul>");

                foreach (DataRow row in employees.Rows) message.AppendFormat("<li>{0}</li>", row["FullName"]);

                message.Append("</ul>");

                ScreenStatus.AddMessage(AlertType.Error, message.ToString());

                NeedMoveTop.Value = "true";

                return false;
            }

            return true;
        }

        private bool CheckDeleteAllowed()
        {
            var profiles = ProfileRepository.SelectDepartmentProfilesOnly(DepartmentIdentifier);

            var personFilter = new PersonFilter { DepartmentIdentifier = DepartmentIdentifier, IsArchived = null };
            var employees = ContactRepository3.SelectPersons(personFilter, Organization.Identifier);

            var achievementFilter = new VCmdsAchievementFilter { DepartmentIdentifier = DepartmentIdentifier };
            var achievements = VCmdsAchievementSearch.SelectByFilter(achievementFilter);

            if (employees.Rows.Count == 0 && profiles.Rows.Count == 0 && achievements.Count == 0)
                return true;

            var department = DepartmentSearch.Select(DepartmentIdentifier);

            var names = new List<string>();

            if (employees.Rows.Count > 0)
                names.Add("workers");

            if (profiles.Rows.Count > 0)
                names.Add("profiles");

            if (achievements.Count > 0)
                names.Add("achievements");

            var nameList = BuildNameList(names);

            var message = new StringBuilder();
            message.AppendFormat("You can't delete this department ({0}) because it is referenced by the following {1}:<br/>", department.DepartmentName, nameList);

            var showHeader = names.Count > 1;

            if (showHeader)
                message.Append("<br/>");

            if (employees.Rows.Count > 0)
                AddRelatedEmployees(message, employees, showHeader);

            if (profiles.Rows.Count > 0)
                AddRelatedProfiles(message, profiles, showHeader);

            if (achievements.Count > 0)
                AddRelatedAchievements(message, achievements, showHeader);

            message.AppendFormat("You must remove all references to the department (from {0}) before you can delete the department.", nameList);

            if (Identity.IsInRole(CmdsRole.SystemAdministrators) || Identity.IsInRole(CmdsRole.Programmers))
                message.AppendFormat("<br/><br/><a href=\"javascript:if(confirm('Are you sure you want to remove all references to this department?'))__doPostBack('{0}', '');\">Remove All References</a>", RemoveReferencesButton.UniqueID);

            ScreenStatus.AddMessage(AlertType.Error, message.ToString());

            return false;
        }

        private static string BuildNameList(IList<string> names)
        {
            var nameList = new StringBuilder();

            for (var i = 0; i < names.Count; i++)
            {
                if (i > 0)
                    nameList.Append(i == names.Count - 1 ? " and " : ", ");

                nameList.Append(names[i]);
            }

            return nameList.ToString();
        }

        private static void AddRelatedEmployees(StringBuilder message, DataTable employees, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Workers</strong>");

            message.Append("<ul>");

            foreach (DataRow row in employees.Rows)
            {
                message.AppendFormat("<li>{0}</li>", row["FullName"]);
            }

            message.Append("</ul>");
        }

        private static void AddRelatedProfiles(StringBuilder message, DataTable profiles, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Profiles</strong>");

            message.Append("<ul>");

            foreach (DataRow row in profiles.Rows)
            {
                var number = row["ProfileNumber"] as string;
                var title = (string)row["ProfileTitle"];
                var text = string.IsNullOrEmpty(number) ? title : $"{number}: {title}";

                message.AppendFormat("<li>{0}</li>", text);
            }

            message.Append("</ul>");
        }

        private static void AddRelatedAchievements(StringBuilder message, List<VCmdsAchievement> achievements, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Achievements</strong>");

            message.Append("<ul>");

            foreach (var row in achievements)
            {
                var text = string.IsNullOrEmpty(row.AchievementLabel) ? row.AchievementTitle : $"{row.AchievementLabel}: {row.AchievementTitle}";

                message.AppendFormat("<li>{0}</li>", text);
            }

            message.Append("</ul>");
        }

        #endregion
    }
}