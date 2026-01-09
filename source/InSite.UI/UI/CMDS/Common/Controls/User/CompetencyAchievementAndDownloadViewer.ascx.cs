using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Infrastructure;
using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.User.Competencies.Controls
{
    public partial class CompetencyAchievementAndDownloadViewer : UserControl
    {
        #region Delegates

        public delegate void SignedOffHandler(CompetencyAchievementAndDownloadViewer sender, VCmdsCredential credential);

        #endregion

        protected string GetAchievementGroupTitle(object eval)
        {
            var org = CurrentSessionState.Identity.Organization.Code;

            if (eval is string title)
                return title = AchievementTypes.Pluralize(title, org);

            return string.Empty;
        }

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementGroups.ItemCreated += AchievementGroups_ItemCreated;
            AchievementGroups.ItemDataBound += AchievementGroups_ItemDataBound;

            GroupWithDownloads.ItemDataBound += GroupWithDownloads_ItemDataBound;

            IsModuleQuizCompleted.AutoPostBack = true;
            IsModuleQuizCompleted.CheckedChanged += IsModuleQuizCompleted_CheckedChanged;
        }

        #endregion

        #region Refresh data

        private bool RefreshData()
        {
            HasAchievements = LoadAchievements();
            HasDownloads = LoadGroups();

            return HasAchievements || HasDownloads;
        }

        #endregion

        #region AchievementGroupItem class

        protected class AchievementGroupItem
        {
            public string AchievementGroupName { get; set; }
            public IList<AchievementItem> Achievements { get; set; }
        }

        #endregion

        #region AchievementItem class

        protected class AchievementItem
        {
            public DateTime? DateCompleted { get; set; }
            public IList<DownloadItem> Downloads { get; set; }
            public Guid? EmployeeID { get; set; }
            public bool EnableSignOff { get; set; }
            public decimal? GradePercent { get; set; }
            public Guid AchievementIdentifier { get; set; }
            public bool IsShiftCourse { get; set; }
            public string AchievementTitle { get; set; }
            public string ValidationStatus { get; set; }
        }

        #endregion

        #region DownloadItem class

        protected class DownloadItem
        {
            public Guid ContainerGuid { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
        }

        #endregion

        #region GroupItem class

        protected class GroupItem
        {
            public IList<DownloadItem> Downloads { get; set; }
            public string GroupName { get; set; }
            public Guid GroupIdentifier { get; set; }
        }

        #endregion

        #region Events

        public event SignedOffHandler SignedOff;

        private void OnSignedOff(VCmdsCredential credential)
        {
            SignedOff?.Invoke(this, credential);
        }

        public event EventHandler ModuleQuizCompletedChanged;

        private void OnModuleQuizCompletedChanged()
        {
            ModuleQuizCompletedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        private Guid OrganizationIdentifier
        {
            get => (Guid)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        private Guid CompetencyStandardIdentifier
        {
            get => (Guid)ViewState[nameof(CompetencyStandardIdentifier)];
            set => ViewState[nameof(CompetencyStandardIdentifier)] = value;
        }

        private Guid EmployeeID
        {
            get => (Guid)ViewState[nameof(EmployeeID)];
            set => ViewState[nameof(EmployeeID)] = value;
        }

        public bool HasDownloads { get; set; }
        public bool HasAchievements { get; set; }

        #endregion

        #region Event handlers

        private void AchievementGroups_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var achievementsRepeater = (Repeater)e.Item.FindControl("Achievements");

            achievementsRepeater.ItemCommand += Achievements_ItemCommand;
            achievementsRepeater.ItemDataBound += AchievementsRepeater_ItemDataBound;
        }

        private void AchievementGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var achievementGroup = (AchievementGroupItem)e.Item.DataItem;
            var achievementsRepeater = (Repeater)e.Item.FindControl("Achievements");

            achievementsRepeater.ItemCommand += Achievements_ItemCommand;
            achievementsRepeater.ItemDataBound += AchievementsRepeater_ItemDataBound;

            achievementsRepeater.DataSource = achievementGroup.Achievements;
            achievementsRepeater.DataBind();
        }

        private void Achievements_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Sign Off")
            {
                var info = Guid.TryParse((string)e.CommandArgument, out var achievementID)
                    ? EmployeeAchievementHelper.SignOff(EmployeeID, achievementID)
                    : null;

                RefreshData();

                if (info != null)
                    OnSignedOff(info);
            }
        }

        private void AchievementsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var achievement = (AchievementItem)e.Item.DataItem;
            var signOffStatusFlag = (Label)e.Item.FindControl("SignOffStatusFlag");
            var achievementTitle = (ITextControl)e.Item.FindControl("AchievementTitle");
            var achievementScore = (ITextControl)e.Item.FindControl("AchievementScore");
            var achievementDownloadsRepeater = (Repeater)e.Item.FindControl("AchievementDownloads");
            var signOffButton = (IButton)e.Item.FindControl("SignOffButton");

            var isSignedOff = EmployeeAchievementHelper.IsSignedOff(achievement.DateCompleted);

            if (isSignedOff)
            {
                signOffStatusFlag.Text = @"<i class='far fa-flag-checkered text-success'></i>";
                signOffStatusFlag.ToolTip = @"Signed Off";

                signOffButton.Visible = false;
            }
            else
            {
                signOffStatusFlag.Visible = achievement.IsShiftCourse || achievement.EmployeeID.HasValue;
                signOffStatusFlag.Text = @"<i class='fas fa-flag text-danger'></i>";
                signOffStatusFlag.ToolTip = @"Not Signed Off";

                signOffButton.Visible = achievement.EmployeeID.HasValue
                                        && EmployeeAchievementHelper.CanSignOff(achievement.AchievementIdentifier, achievement.EmployeeID, achievement.EnableSignOff, achievement.DateCompleted, CurrentSessionState.Identity.User.UserIdentifier);

                signOffButton.CommandArgument = achievement.AchievementIdentifier.ToString();
            }

            signOffButton.Visible = signOffButton.Visible && !CurrentSessionState.Identity.IsImpersonating;

            if (achievement.IsShiftCourse)
            {
                var courseId = CourseSearch.BindCourseFirst(x => (Guid?)x.CourseIdentifier, x => x.Gradebook.AchievementIdentifier == achievement.AchievementIdentifier);

                if (courseId.HasValue && TGroupPermissionSearch.IsAccessAllowed(courseId.Value, CurrentSessionState.Identity))
                {
                    var link = Custom.CMDS.Portal.Courses.CmdsCourseLink.GetCourseLink(courseId.Value);
                    achievementTitle.Text = $@"<a target='_blank' href='{link}'>{achievement.AchievementTitle}</a>";
                }
                else
                {
                    achievementTitle.Text = achievement.AchievementTitle;
                }
            }
            else
            {
                achievementTitle.Text = achievement.AchievementTitle;
            }

            if (achievement.GradePercent.HasValue && achievement.GradePercent.Value > 0)
                achievementScore.Text = $@" (Score = {achievement.GradePercent:p0})";
            else
                achievementScore.Text = @" &nbsp;";

            achievementDownloadsRepeater.Visible = achievement.Downloads != null;

            if (achievement.Downloads != null)
            {
                achievementDownloadsRepeater.ItemDataBound += Downloads_ItemDataBound;

                achievementDownloadsRepeater.DataSource = achievement.Downloads;
                achievementDownloadsRepeater.DataBind();
            }
        }

        private void GroupWithDownloads_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var group = (GroupItem)e.Item.DataItem;
            var groupDownloadsRepeater = (Repeater)e.Item.FindControl("GroupDownloads");

            groupDownloadsRepeater.ItemDataBound += Downloads_ItemDataBound;

            groupDownloadsRepeater.DataSource = group.Downloads;
            groupDownloadsRepeater.DataBind();
        }

        private void Downloads_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var download = (DownloadItem)e.Item.DataItem;
            var downloadLink = (HyperLink)e.Item.FindControl("DownloadLink");

            downloadLink.NavigateUrl = download.Type == UploadType.CmdsFile
                ? CmdsUploadProvider.GetFileRelativeUrl(download.ContainerGuid, download.Name)
                : download.Name;

            downloadLink.Text = download.Title;
        }

        private void IsModuleQuizCompleted_CheckedChanged(object sender, EventArgs e)
        {
            IsModuleQuizCompleted.Checked = true;

            HasAchievements = LoadAchievements();

            OnModuleQuizCompletedChanged();
        }

        #endregion

        #region Public methods

        public bool LoadData(Guid competencyId, Guid employeeId, Guid organizationId)
        {
            CompetencyStandardIdentifier = competencyId;
            EmployeeID = employeeId;
            OrganizationIdentifier = organizationId;

            return RefreshData();
        }

        public void SetQuizCheckbox(bool isVisible, bool isChecked)
        {
            ModuleQuizCompletedPanel.Visible = isVisible;
            IsModuleQuizCompleted.Enabled = !isChecked;
            IsModuleQuizCompleted.Checked = isChecked;
        }

        public void HideDownloads()
        {
            DownloadsPanel.Visible = false;
        }

        #endregion

        #region Helper methods: achievements

        private bool LoadAchievements()
        {
            var achievements = VCmdsAchievementSearch.SelectCompetencyAchievements(CompetencyStandardIdentifier, EmployeeID, OrganizationIdentifier);

            AchievementGroups.Visible = achievements.Count > 0;

            if (achievements.Count == 0)
                return false;

            var achievementGroupList = CreateAchievementGroupList(achievements);

            AchievementGroups.DataSource = achievementGroupList;
            AchievementGroups.DataBind();

            return true;
        }

        private IList<AchievementGroupItem> CreateAchievementGroupList(List<VCmdsAchievementSearch.CompetencyAchievement> achievements)
        {
            IList<AchievementGroupItem> achievementGroupList = new List<AchievementGroupItem>();

            foreach (var row in achievements)
            {
                var achievementGroup = achievementGroupList.Count == 0 ? null : achievementGroupList[achievementGroupList.Count - 1];
                var achievementGroupName = row.AchievementLabel;

                if (achievementGroup == null || !StringHelper.Equals(achievementGroupName, achievementGroup.AchievementGroupName))
                {
                    achievementGroup = new AchievementGroupItem
                    {
                        AchievementGroupName = achievementGroupName,
                        Achievements = new List<AchievementItem>()
                    };

                    achievementGroupList.Add(achievementGroup);
                }

                AddAchievement(achievementGroup, row);
            }

            return achievementGroupList;
        }

        private void AddAchievement(AchievementGroupItem achievementGroup, VCmdsAchievementSearch.CompetencyAchievement row)
        {
            var achievement = achievementGroup.Achievements.Count == 0 ? null : achievementGroup.Achievements[achievementGroup.Achievements.Count - 1];
            var id = row.AchievementIdentifier;

            if (achievementGroup.Achievements.Count == 0 || achievement != null && achievement.AchievementIdentifier != id)
            {
                var dateCompleted = row.DateCompleted;

                achievement = new AchievementItem
                {
                    AchievementIdentifier = id,
                    AchievementTitle = row.AchievementTitle,
                    DateCompleted = dateCompleted?.UtcDateTime,
                    GradePercent = null,
                    IsShiftCourse = row.AchievementLabel == "Module",
                    EmployeeID = row.UserIdentifier,
                    ValidationStatus = row.ValidationStatus,
                    EnableSignOff = row.EnableSignOff ?? false,
                    Downloads = new List<DownloadItem>()
                };

                achievementGroup.Achievements.Add(achievement);
            }

            AddAchievementDownload(achievement, row);
        }

        private void AddAchievementDownload(AchievementItem achievement, VCmdsAchievementSearch.CompetencyAchievement row)
        {
            if (row.UploadName == null)
                return;

            if (achievement.Downloads == null)
                achievement.Downloads = new List<DownloadItem>();

            var download = new DownloadItem
            {
                ContainerGuid = row.UploadContainerIdentifier.Value,
                Name = row.UploadName,
                Title = row.UploadTitle,
                Type = row.UploadType
            };

            achievement.Downloads.Add(download);
        }

        #endregion

        #region Load groups with downloads

        private bool LoadGroups()
        {
            var table = UploadRepository.SelectCompetencyUploads(CurrentIdentityFactory.ActiveOrganizationIdentifier, CompetencyStandardIdentifier);

            GroupWithDownloads.Visible = table.Rows.Count > 0;

            if (table.Rows.Count == 0)
                return false;

            var groupList = CreateGroupList(table);

            GroupWithDownloads.DataSource = groupList;
            GroupWithDownloads.DataBind();

            return true;
        }

        private IList<GroupItem> CreateGroupList(DataTable table)
        {
            var groupList = new List<GroupItem>();

            foreach (DataRow row in table.Rows)
            {
                var group = groupList.Count == 0 ? null : groupList[groupList.Count - 1];
                var id = (Guid)row["OrganizationIdentifier"];

                if (group == null || group.GroupIdentifier != id)
                {
                    group = new GroupItem
                    {
                        GroupIdentifier = id,
                        GroupName = (string)row["GroupName"],
                        Downloads = new List<DownloadItem>()
                    };

                    groupList.Add(group);
                }

                AddGroupDownload(group, row);
            }

            return groupList;
        }

        private void AddGroupDownload(GroupItem group, DataRow row)
        {
            var download = new DownloadItem
            {
                ContainerGuid = (Guid)row["UploadContainerIdentifier"],
                Name = (string)row["UploadName"],
                Title = (string)row["UploadTitle"],
                Type = (string)row["UploadType"]
            };

            group.Downloads.Add(download);
        }

        #endregion
    }
}