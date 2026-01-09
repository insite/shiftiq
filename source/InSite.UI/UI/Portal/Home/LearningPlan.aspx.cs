using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web.Cmds;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Home
{
    public class LearningItem
    {
        public IList<VCmdsCredentialAndExperience> Credentials { get; set; }

        public string AchievementType { get; set; }

        public string AchievementTypeDisplay
        {
            get
            {
                var organization = CurrentSessionState.Identity.Organization.Code;
                return AchievementTypes.Pluralize(AchievementType, organization);
            }
        }

        public string AchievementTypeCode
            => StringHelper.Sanitize(AchievementType, '-');
    }

    public partial class LearningPlan : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowWhat.AutoPostBack = true;
            ShowWhat.SelectedIndexChanged += (x, y) => BindAchievements();

            AchievementTypes.ItemCreated += AchievementTypes_ItemCreated;
            AchievementTypes.ItemDataBound += AchievementTypes_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                BindAchievements();
        }

        private void BindAchievements()
        {
            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);

            var list = VCmdsCredentialSearch.SelectForTrainingPlan(User.Identifier, Organization.OrganizationIdentifier, null);

            if (!StringHelper.Equals(ShowWhat.SelectedValue, "All"))
                list = list.Where(x => x.CredentialStatus == ShowWhat.SelectedValue).ToList();

            if (list.Count > 0)
            {
                AchievementTypes.Visible = true;
                NoAchievements.Visible = false;

                AchievementTypes.DataSource = CreateList(list);
                AchievementTypes.DataBind();
            }
            else
            {
                AchievementTypes.Visible = false;
                NoAchievements.Visible = true;

                if (StringHelper.Equals(ShowWhat.SelectedValue, "All"))
                    NoAchievements.InnerHtml = $"There are no achievements assigned to your learning plan.";
                else
                    NoAchievements.InnerHtml = $"There are no <strong>{ShowWhat.SelectedValue}</strong> achievements assigned to your learning plan.";
            }
        }

        private static IList<LearningItem> CreateList(List<VCmdsCredentialAndExperience> table)
        {
            var achievements = new List<LearningItem>();

            foreach (var row in table)
            {
                var achievementType = row.AchievementLabel ?? "N/A";
                var item = FindLearningItem(achievements, achievementType);
                item.Credentials.Add(row);
            }

            return achievements;
        }

        private static LearningItem FindLearningItem(List<LearningItem> achievementTypes, string type)
        {
            foreach (var item in achievementTypes)
                if (item.AchievementType == type)
                    return item;

            var achievementType = new LearningItem();
            achievementType.Credentials = new List<VCmdsCredentialAndExperience>();
            achievementType.AchievementType = type;

            achievementTypes.Add(achievementType);

            return achievementType;
        }

        private void AchievementTypes_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var achievements = (Repeater)e.Item.FindControl("AchievementItems");
            achievements.ItemDataBound += AchievementItems_ItemDataBound;
        }

        private static void AchievementTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = (LearningItem)e.Item.DataItem;

            var achievements = (Repeater)e.Item.FindControl("AchievementItems");
            achievements.DataSource = item.Credentials;
            achievements.DataBind();
        }

        private void AchievementItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var row = (VCmdsCredentialAndExperience)e.Item.DataItem;
        }

        protected string GetDeclarationHtml(object o)
        {
            var item = (VCmdsCredentialAndExperience)o;

            if (!item.CredentialGranted.HasValue)
                return string.Empty;

            var achievement = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == item.UserIdentifier && x.AchievementIdentifier == item.AchievementIdentifier);

            var isSignOffApplicable = EmployeeAchievementHelper.TypeAllowsSignOff(item.AchievementLabel)
                    && achievement.AuthorityType == "Self"
                    && item.UserIdentifier == User.UserIdentifier;

            var isSignOffComplete = EmployeeAchievementHelper.IsSignedOff(achievement);

            if (!isSignOffApplicable)
                return string.Empty;

            // SignOffPanel.Visible = isSignOffApplicable && !isSignOffComplete;
            // RenewSignOffPanel.Visible = isSignOffApplicable && isSignOffComplete;

            var html = $@"
<div class='alert alert-info mt-2 mb-0'>
  <strong>Declaration:</strong>
  {item.UserFirstName} {item.UserLastName} has read the document and has signed off on it, 
  indicating a full understanding of the content and acceptance of any requirements, 
  effective {TimeZones.FormatDateOnly(item.CredentialGranted.Value, User.TimeZone)}.
</div>
";

            return html;
        }

        protected string GetExpiryHtml(object o)
        {
            var item = (VCmdsCredentialAndExperience)o;
            if (!(item.CredentialIsTimeSensitive ?? false) || !item.CredentialExpirationExpected.HasValue)
                return string.Empty;

            var indicator = "info";
            var verb = "Expires";
            var icon = "alarm-clock";

            if (item.CredentialExpirationExpected.Value < DateTimeOffset.Now)
            {
                indicator = "danger";
                verb = "Expired";
                icon = "alarm-exclamation";
            }

            return $"<span class='badge bg-{indicator}'><i class='fas fa-{icon} me-1'></i>{verb} {TimeZones.FormatDateOnly(item.CredentialExpirationExpected.Value, User.TimeZone)}</span>";
        }

        protected string GetFlagHtml(object o)
        {
            var item = (VCmdsCredentialAndExperience)o;

            var icon = "flag";
            var indicator = "danger";
            if (item.CredentialStatus == "Valid")
            {
                indicator = "success";
                icon = "flag-checkered";
            }

            return $"<i class='fas fa-{icon} text-{indicator} me-1'></i>";
        }

        protected string GetGrantedHtml(object o)
        {
            var item = (VCmdsCredentialAndExperience)o;
            if (item.CredentialStatus != "Valid" || !item.CredentialGranted.HasValue)
                return "-";

            return TimeZones.FormatDateOnly(item.CredentialGranted.Value, User.TimeZone);
        }

        protected string GetLifetimeHtml(object o)
        {
            var item = (VCmdsCredentialAndExperience)o;
            if (!(item.CredentialIsTimeSensitive ?? false) || item.LifetimeMonths == null)
                return "-";

            return $"Valid for {Shift.Common.Humanizer.ToQuantity(item.LifetimeMonths.Value, "month")}";
        }

        protected string GetStatusHtml(object o)
        {
            var item = (VCmdsCredentialAndExperience)o;
            var html = new StringBuilder();

            switch (item.CredentialStatus)
            {
                case "Valid":
                    html.AppendLine($"<span class='text-success'><i class='fas fa-badge-check me-1'></i>{GetDisplayText("Valid")}</span>");
                    break;
                case "Pending":
                    html.AppendLine($"<span class='text-warning'><i class='fas fa-hourglass-start me-1'></i>{GetDisplayText("Pending")}</span>");
                    break;
                case "Expired":
                    html.Append($"<span class='text-danger'><i class='fas fa-alarm-clock me-1'></i>{GetDisplayText("Expired")}</span>");
                    break;
            }
            return html.ToString();
        }
    }
}