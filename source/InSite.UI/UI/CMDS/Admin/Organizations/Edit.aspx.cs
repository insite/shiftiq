using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Humanizer;

using InSite.Application.Standards.Read;
using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.UI;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Admin.Organizations.Forms
{
    public partial class Edit : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const int TempFolderLiveTime = 24 * 60; // in minutes

        private const string SearchUrl = "/ui/cmds/admin/organizations/search";
        private const string CreateUrl = "/ui/cmds/admin/organizations/create";

        #endregion

        #region Properties

        private Guid OrganizationIdentifier
        {
            get => (Guid)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        private OrganizationState _organization;

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _organization = Guid.TryParse(Request.QueryString["id"], out var organizationId)
                ? OrganizationSearch.Select(organizationId)
                : null;

            OrganizationIdentifier = organizationId;

            ProfileEditor.InitDelegates(SelectProfiles, SelectProfilesToAdd, DeleteProfiles, InsertProfiles, InitProfileCopy, EditorStatus, "organization");

            AchievementEditor.InitDelegates(
                OrganizationIdentifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "organization");

            AchievementEditor.ShowVisibilityCriteria();

            LoadRedundantFileButton.Click += LoadRedundantFileButton_Click;
            DeleteRedundantFileButton.Click += DeleteRedundantFileButton_Click;

            SaveButton.Click += SaveButton_Click;
            ArchiveButton.Click += (s, a) => Archive();
            UnarchiveButton.Click += (s, a) => Unarchive();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();

            CancelButton.NavigateUrl = SearchUrl;

            // Sep 23, 2019 - Daniel Miller: Before we reenable this we need to ensure the file cleanup 
            // functionality does not delete uploads for ContactExperience records.

            // var isProgrammer = CurrentUser.IsInGroup("CMDS Programmers");
            FileCleanupTab.Visible = false;
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            ArchiveButton.Visible = Access.Delete;
            UnarchiveButton.Visible = Access.Delete;
            SaveButton.Visible = Access.Write;
        }

        private List<RedundantFile> GetRedundantFiles()
        {
            var infos = UploadRepository.SelectRedundant(UploadType.CmdsFile);
            var list = new List<RedundantFile>();

            foreach (var info in infos)
            {
                var organization = OrganizationSearch.Select(info.OrganizationIdentifier);

                var item = new RedundantFile
                {
                    Name = info.Name,
                    Size = (info.ContentSize ?? 0).Bytes().Humanize("0"),
                    Organization = organization.CompanyName,
                    Uploaded = info.Uploaded,
                    Uploader = info.Uploader,
                    Url = $"https://{organization.OrganizationCode}.{ServiceLocator.AppSettings.Security.Domain}/cmds/uploads/{info.ContainerIdentifier}/{Uri.EscapeDataString(info.Name)}"
                };

                list.Add(item);
            }

            return list.OrderBy(x => x.Name).ToList();
        }

        #endregion

        #region Event handlers

        private void LoadRedundantFileButton_Click(object sender, EventArgs e)
        {
            LoadRedundantFileButton.Visible = false;
            RedundantFilePanel.Visible = true;

            FileCleanupRepeater.DataSource = GetRedundantFiles();
            FileCleanupRepeater.DataBind();
            FileCleanupRepeater.Visible = FileCleanupRepeater.Items.Count > 0;

            var count = UploadRepository.CountRedundant(UploadType.CmdsFile);
            FileCleanupCount.Text = "unreferenced file".ToQuantity(count, "n0") + " found";
        }

        private void DeleteRedundantFileButton_Click(object sender, EventArgs e)
        {
            var validUntil = DateTime.UtcNow.AddMinutes(-TempFolderLiveTime);
            var infos = UploadRepository.SelectRedundant(UploadType.CmdsFile);
            var count = 0;

            using (var fileStorage = CmdsUploadProvider.Current.CreateFileStorage())
            {
                foreach (var info in infos)
                    if (info.Uploaded.UtcDateTime < validUntil)
                    {
                        CmdsUploadProvider.Delete(info.ContainerIdentifier, info.Name, fileStorage);
                        count++;
                    }

                fileStorage.Commit();
            }

            ResultLiteral.Text = "File".ToQuantity(count) + " deleted";
            DeleteRedundantFileButton.Visible = false;
            FileCleanupRepeater.Visible = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        #endregion

        #region Load & Save

        private void Open()
        {
            var organization = _organization;

            if (organization == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var organizationId = _organization.Identifier;

            if (Access.Delete)
            {
                ArchiveButton.Visible = !organization.AccountClosed.HasValue;
                UnarchiveButton.Visible = organization.AccountClosed.HasValue;
            }

            PageHelper.AutoBindHeader(
                this,
                new BreadcrumbItem("Add New Organization", CreateUrl, null, null),
                organization.CompanyDescription.LegalName);

            CompanyDetail.SetInputValues(organization);

            var enableDivisions = OrganizationHelper.EnableDivisions(organization.CompanyDescription.CompanySize);

            var departmentCount = DepartmentGrid.LoadData(organizationId, enableDivisions);
            var districtCount = DivisionGrid.LoadData(organizationId);
            var profileCount = ProfileEditor.LoadProfiles(organizationId, null);

            CompanyTimeSensitiveCompetencies.SetInputValues(organization);

            CompanySkillEditorLink.NavigateUrl = $"/ui/cmds/admin/departments/profile-competencies/build?id={organizationId}";

            var competencyCount = VCmdsCompetencyOrganizationRepository.CountByOrganization(organizationId);
            CompanySkillEditorLink.Visible = competencyCount > 0;

            AchievementEditor.SetEditable(CanEdit, CanEdit);
            AchievementEditor.LoadAchievements(GroupByEnum.Type);

            var personFilter = new PersonFilter
            {
                OrganizationIdentifier = organizationId,
                EnableIsArchived = false,
                RoleType = new[] { MembershipType.Organization }
            };

            PersonGrid.SetVisibleColumns(new[] { "Name", "City", "Province", "EmailWork", "ToolTipWithLinks" });
            PersonGrid.LoadData(personFilter);
            PersonGrid.ShowFilterPanel();

            var categoryCount = CategoryGrid.LoadData(organizationId);

            if (!IsPostBack)
                CheckForUnusedProfiles(organizationId);

            DivisionTab.Visible = enableDivisions;
        }

        private void CheckForUnusedProfiles(Guid organizationId)
        {
            var table = VCmdsProfileOrganizationRepository.SelectUnusedProfiles(organizationId);
            if (table.Rows.Count == 0)
                return;

            var message = new StringBuilder("The following profiles are not being used by any of the departments in this organization, and should therefore be removed from the organization:<br/>");

            message.Append("<ul>");

            foreach (DataRow row in table.Rows)
                message.AppendFormat("<li>{0}: {1}</li>", row["ProfileNumber"], row["ProfileTitle"]);

            message.Append("</ul>");

            EditorStatus.AddMessage(AlertType.Warning, message.ToString());
        }

        private void Save()
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifier)
                ?? throw new ApplicationError("Organization not found: " + OrganizationIdentifier);

            CompanyDetail.GetInputValues(organization);
            CompanyTimeSensitiveCompetencies.GetInputValues(organization);

            OrganizationStore.Update(organization);
        }

        private void Archive()
        {
            OrganizationStore.Close(OrganizationIdentifier);

            Open();
        }

        private void Unarchive()
        {
            OrganizationStore.Open(OrganizationIdentifier);

            Open();
        }

        #endregion

        #region Profile Editor

        private DataTable SelectProfiles()
        {
            return ProfileRepository.SelectOrganizationProfilesOnly(OrganizationIdentifier);
        }

        private DataTable SelectProfilesToAdd(string searchText)
        {
            return ProfileRepository.SelectNewOrganizationProfiles(OrganizationIdentifier, searchText);
        }

        private bool DeleteProfiles(IList<Guid> profiles)
        {
            StandardOrganizationStore.Delete(OrganizationIdentifier, profiles);
            VCmdsCompetencyOrganizationRepository.DeleteUnusedByOrganizationIdentifier(OrganizationIdentifier);

            return true;
        }

        private int InsertProfiles(IList<Guid> profiles)
        {
            var profilesForCompetencies = new List<Guid>();

            foreach (var profileStandardIdentifier in profiles)
                if (!StandardOrganizationSearch.Exists(x => x.OrganizationIdentifier == OrganizationIdentifier && x.StandardIdentifier == profileStandardIdentifier))
                    profilesForCompetencies.Add(profileStandardIdentifier);

            StandardOrganizationStore.Insert(OrganizationIdentifier, profilesForCompetencies);

            foreach (var profileStandardIdentifier in profilesForCompetencies)
                VCmdsCompetencyOrganizationRepository.InsertProfileCompetencies(OrganizationIdentifier, profileStandardIdentifier);

            return profilesForCompetencies.Count;
        }

        private void InitProfileCopy(QStandard profile)
        {
            profile.OrganizationIdentifier = OrganizationIdentifier;
        }

        #endregion

        #region Achievement Editor

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var organizationId = OrganizationIdentifier;

            var assignedAchievementIds = TAchievementOrganizationSearch
                .Bind(x => x.AchievementIdentifier, x => x.OrganizationIdentifier == organizationId);

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            var list = TAchievementOrganizationSearch.Select(x => x.OrganizationIdentifier == OrganizationIdentifier && achievements.Contains(x.AchievementIdentifier));

            TAchievementOrganizationStore.Delete(list);
        }

        private int InsertAchievements(IEnumerable<Guid> achievements)
        {
            var count = 0;

            foreach (var achievementID in achievements)
            {
                if (!TAchievementOrganizationSearch.Exists(x => x.OrganizationIdentifier == OrganizationIdentifier && x.AchievementIdentifier == achievementID))
                {
                    TAchievementOrganizationStore.InsertOrganizationAchievement(OrganizationIdentifier, achievementID);
                    count++;
                }
            }

            return count;
        }

        #endregion
    }
}