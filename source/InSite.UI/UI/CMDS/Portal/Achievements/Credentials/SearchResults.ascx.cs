using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.CMDS.Portal.Achievements.Credentials
{
    public partial class SearchResults : UserControl
    {
        private List<AchievementTypeGroup> _groups;
        private List<AttachmentItem> _attachments;

        public bool IsCMDSTraining { get; set; }

        public VCmdsCredentialFilter Filter
        {
            get { return ViewState[nameof(Filter)] as VCmdsCredentialFilter; }
            set { ViewState[nameof(Filter)] = value; }
        }

        public int GroupsCount { get; set; }

        private string GroupBy
        {
            get => ViewState[nameof(GroupBy)] as string;
            set => ViewState[nameof(GroupBy)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementTypeLinkRepeater.ItemDataBound += AchievementTypeLinkRepeater_ItemDataBound;
            AchievementTypeBodyRepeater.ItemDataBound += AchievementTypeBodyRepeater_ItemDataBound;
        }

        public void LoadData(VCmdsCredentialFilter filter, string groupBy)
        {
            Filter = filter;
            GroupBy = groupBy;

            SelectGroups();
            SelectAttachments();

            GroupsCount = _groups.Count;

            AchievementTypeLinkRepeater.DataSource = _groups;
            AchievementTypeLinkRepeater.DataBind();

            AchievementTypeBodyRepeater.DataSource = _groups;
            AchievementTypeBodyRepeater.DataBind();
        }

        private void AchievementTypeLinkRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var group = (AchievementTypeGroup)e.Item.DataItem;
            var achievementType = group.AchievementType;

            var count = group.AchievementItemsCount;
            var name = !string.IsNullOrEmpty(achievementType)
                ? AchievementTypes.Pluralize(achievementType, CurrentSessionState.Identity.Organization.Code)
                : AchievementTypes.OtherAchievement;

            var titleLiteral = (System.Web.UI.WebControls.Literal)e.Item.FindControl("Title");

            titleLiteral.Text = string.Format("{0} <span class=\"form-text\">({1})</span>", name, count);
        }

        private void AchievementTypeBodyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var group = (AchievementTypeGroup)e.Item.DataItem;
            var achievementType = group.AchievementType;

            var achievementCategoryRepeater = (Repeater)e.Item.FindControl("AchievementCategoryRepeater");
            achievementCategoryRepeater.ItemDataBound += AchievementCategoryRepeater_ItemDataBound;
            achievementCategoryRepeater.DataSource = group.AchievementCategoryGroups;
            achievementCategoryRepeater.DataBind();
        }

        private void AchievementCategoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var group = (AchievementCategoryGroup)e.Item.DataItem;

            var achievementCategoryTitle = (System.Web.UI.WebControls.Literal)e.Item.FindControl("AchievementCategoryTitle");
            if (GroupBy == "Type")
            {
                achievementCategoryTitle.Visible = false;
            }
            else
            {
                achievementCategoryTitle.Text = $"<h4>{group.AchievementCategory}</h4>";
                achievementCategoryTitle.Visible = true;
            }

            var repeater = (Repeater)e.Item.FindControl("Repeater");
            repeater.ItemDataBound += Grid_RowDataBound;
            repeater.DataSource = group.AchievementItems;
            repeater.DataBind();
        }

        private void Grid_RowDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            BindDataItem(e.Item, (AchievementItem)e.Item.DataItem);
            BindAttachments(e.Item, (AchievementItem)e.Item.DataItem);
        }

        private void SelectGroups()
        {
            var filter = Filter.Copy();
            filter.IsCompetencyTraining = IsCMDSTraining;
            filter.IsReportingDisabled = false;

            var organization = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            var table = VCmdsCredentialSearch.SelectSearchResults(filter, organization, true).ToList();
            var groups = new List<AchievementTypeGroup>();

            foreach (var row in table)
            {
                var achievement = GetAchievementItem(row, false);

                var group = groups.FirstOrDefault(x => x.AchievementType == achievement.AchievementType);
                if (group == null)
                {
                    group = new AchievementTypeGroup { AchievementType = achievement.AchievementType };
                    groups.Add(group);
                }

                if (GroupBy == "Type")
                {
                    if (group.AchievementCategoryGroups.Count == 0)
                        group.AchievementCategoryGroups.Add(new AchievementCategoryGroup());

                    group.AchievementCategoryGroups[0].AchievementItems.Add(achievement);
                }
                else
                {
                    var categoryName = GetAchievementCategoryName(achievement.AchievementIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier);

                    var achievementCategoryGroup = group.AchievementCategoryGroups.FirstOrDefault(x => x.AchievementCategory == categoryName);
                    if (achievementCategoryGroup == null)
                    {
                        achievementCategoryGroup = new AchievementCategoryGroup { AchievementCategory = categoryName };
                        group.AchievementCategoryGroups.Add(achievementCategoryGroup);
                    }

                    achievementCategoryGroup.AchievementItems.Add(achievement);
                }
            }

            _groups = groups;
        }

        private void SelectAttachments()
        {
            var containers = new List<Guid>();
            foreach (var group in _groups)
            {
                var items = group.AchievementCategoryGroups.SelectMany(x => x.AchievementItems).ToList();
                foreach (var item in items)
                {
                    var guid = IsCMDSTraining
                        ? item.CredentialIdentifier
                        : item.ExperienceIdentifier;
                    if (guid.HasValue)
                        containers.Add(guid.Value);
                }

            }
            _attachments = UploadSearch.Bind(
                x => new AttachmentItem { Identifier = x.ContainerIdentifier, Name = x.Name, Title = x.Title },
                x => containers.Any(container => x.ContainerIdentifier == container),
                "Title").ToList();
        }

        public static void BindDataItem(System.Web.UI.Control control, VCmdsCredentialAndExperience row, bool isSkillsPassport)
        {
            var item = GetAchievementItem(row, isSkillsPassport);

            BindDataItem(control, item);
        }

        public static void BindDataItem(Control control, AchievementItem item)
        {
            var flagIcon = (Icon)control.FindControl("FlagIcon");

            var p = new VCmdsCredentialFilter2
            {
                AchievementLabel = item.AchievementType,
                CredentialStatus = item.ValidationStatus,
                CredentialGranted = item.DateCompleted,
                IsSuccess = item.IsSuccess == true
            };

            var isExpired = item.DateExpired != null &&
                            item.DateExpired < DateTimeOffset.UtcNow;

            flagIcon.Type = IconType.Solid;
            flagIcon.Name = "flag";

            if (VCmdsAchievementHelper.IsCompleted(p) && !isExpired)
            {
                flagIcon.Color = Indicator.Success;
                flagIcon.Name = "flag-checkered";
            }
            else if (isExpired)
            {
                flagIcon.Color = Indicator.Warning;
            }
            else
            {
                flagIcon.Color = Indicator.Danger;
            }

            var lifetimeMonths = item.LifetimeMonths ?? 0;

            var achievementLabels = (System.Web.UI.WebControls.Literal)control.FindControl("AchievementLabels");
            achievementLabels.Text = TimeSensitivityHelper.GetLabelsHtml(item.IsRequired, item.IsInTrainingPlan, lifetimeMonths);

            var isRequired = item.IsRequired != null && item.IsRequired.Value;
            var isPlanned = item.IsInTrainingPlan != null && item.IsInTrainingPlan.Value;
            var title = item.AchievementTitle;
            var experienceKey = item.ExperienceIdentifier;
            var achievement = item.AchievementIdentifier;

            if (string.IsNullOrEmpty(title))
                title = "N/A";

            var t = (ITextControl)control.FindControl("AchievementTitle");
            if (t != null)
            {
                var url = achievement.HasValue && achievement != Guid.Empty
                    ? $"/ui/cmds/portal/achievements/credentials/edit?user={item.UserIdentifier}&achievement={achievement}"
                    : $"/ui/cmds/portal/achievements/experiences/edit?id={experienceKey}";

                t.Text = $@"<a href='{url}'>{title}</a>";
            }
        }

        private void BindAttachments(System.Web.UI.Control control, AchievementItem item)
        {
            var guid = IsCMDSTraining
                ? item.CredentialIdentifier
                : item.ExperienceIdentifier;

            var attachmentsRepeater = (Repeater)control.FindControl("Attachments");
            attachmentsRepeater.DataSource = _attachments.Where(x => x.Identifier == guid).OrderBy(x => x.Title);
            attachmentsRepeater.DataBind();
        }

        private static string GetScore(decimal? granted, decimal? revoked)
        {
            if (granted == null && revoked == null)
                return "&nbsp;";

            var p = granted ?? revoked;
            return p > 0 ? string.Format("({0:p0})", p) : "&nbsp;";
        }

        protected static string GetYear(object date)
        {
            return date == null ? null : ((DateTime)date).Year.ToString();
        }

        private static AchievementItem GetAchievementItem(VCmdsCredentialAndExperience row, bool isSkillsPassport)
        {
            var item = new AchievementItem();

            item.ExperienceIdentifier = row.ExperienceIdentifier;
            item.UserIdentifier = row.UserIdentifier;
            item.UserIdentifier = row.UserIdentifier;
            item.AchievementIdentifier = row.AchievementIdentifier;
            item.CredentialIdentifier = row.CredentialIdentifier;
            item.AuthorityLocation = row.AuthorityLocation;
            item.AccreditorName = row.AuthorityName;
            item.Comment = row.CredentialDescription;
            item.DateCompleted = row.CredentialGranted;
            item.DateExpired = row.CredentialExpirationExpected;
            item.Hours = row.CreditHours;
            item.IsRequired = row.CredentialIsMandatory;
            item.IsTimeSensitive = row.CredentialIsTimeSensitive;
            item.Number = null;
            item.Title = row.AchievementTitle;
            item.Score = GetScore(row.CredentialGrantedScore, row.CredentialRevokedScore);
            item.ValidationStatus = row.CredentialStatus;
            item.LifetimeMonths = row.LifetimeMonths;
            item.IsInTrainingPlan = row.IsInTrainingPlan;
            item.ProgramTitle = row.ProgramTitle;
            item.AchievementTitle = row.AchievementTitle;
            item.AchievementDescription = row.AchievementDescription;
            item.AchievementType = row.AchievementLabel;
            item.EmployeeLastFirstName = row.UserLastName;
            item.EmployeeFirstLastName = row.UserFirstName;
            item.IsSuccess = row.IsSuccess;
            item.IsSkillsPassport = isSkillsPassport;

            return item;
        }

        private string GetAchievementCategoryName(Guid? achievementIdentifier, Guid? organizationIdentifier)
        {
            if (achievementIdentifier.HasValue && organizationIdentifier.HasValue)
            {
                var category = VAchievementCategorySearch.Select(achievementIdentifier.Value, organizationIdentifier.Value);
                if (category != null)
                    return category.CategoryName;
            }

            return "No Category";
        }
    }
}