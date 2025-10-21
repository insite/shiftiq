using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Web.UI;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Achievements.Achievements.Controls
{
    public partial class CredentialGrid : SearchResultsGridViewController<VCredentialFilter>
    {
        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(DownloadDropDown);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (Filter.AchievementIdentifier.HasValue)
                Filter.UserFullName = FilterText.Text;
            else if (Filter.UserIdentifier.HasValue)
                Filter.AchievementTitle = FilterText.Text;

            RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(CredentialGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterText.ClientID}', true);",
                true);
        }

        public void LoadDataByAchievementID(Guid achievementIdentifier, bool showEditPanel, string returnUrl)
        {
            Filter = new VCredentialFilter
            {
                AchievementIdentifier = achievementIdentifier
            };

            Grid.Columns.FindByHeaderText("Achievement").Visible = false;
            Grid.Columns.FindByName("EditPanel").Visible = showEditPanel;

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(achievementIdentifier);
            var returnUrlParam = !string.IsNullOrEmpty(returnUrl) ? "&return=" + HttpUtility.UrlEncode(returnUrl) : "";

            AddButton.NavigateUrl = $"/ui/admin/records/credentials/assign?achievement={achievementIdentifier}" + returnUrlParam;
            AddButton.Visible = achievement.AchievementIsEnabled;
            if (achievement.HasBadgeImage == true)
                AddButton.Text = "Issue Badge";

            Search(Filter);

            MultiView.SetActiveView(GridView);
        }

        public void LoadDataByUserID(Guid userIdentifier)
        {
            Filter = new VCredentialFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifier = userIdentifier
            };

            Grid.Columns.FindByHeaderText("Learner").Visible = false;

            AddButton.Visible = false;

            Search(Filter);

            if (RowCount == 0)
                MultiView.SetActiveView(NoRecordsView);
            else
                MultiView.SetActiveView(GridView);
        }

        protected override int SelectCount(VCredentialFilter filter)
        {
            return ServiceLocator.AchievementSearch.CountCredentials(filter);
        }

        protected override IListSource SelectData(VCredentialFilter filter)
        {
            if (filter == null)
                return null;

            return ServiceLocator.AchievementSearch
                .GetCredentials(filter)
                .ToSearchResult();
        }

        protected override IEnumerable<DownloadColumn> GetDownloadColumns(IList dataList)
        {
            return new[]
            {
                new DownloadColumn("EmployerGroupName", "Employer"),
                new DownloadColumn("UserFullName", "LearnerName"),
                new DownloadColumn("UserEmail", "LearnerEmail"),
                new DownloadColumn("AchievementTitle", "Achievement"),
                new DownloadColumn("CredentialStatus", "Status", null, 15),
                new DownloadColumn("CredentialPriority", "Priority", null, 15),
                new DownloadColumn("CredentialNecessity", "Necessity", null, 15),
                new DownloadColumn("CredentialAssigned", "Assigned","MMM dd, yyyy", 15),
                new DownloadColumn("CredentialGranted", "Granted","MMM dd, yyyy", 15),
                new DownloadColumn("CredentialExpirationExpected", "Expiry","MMM dd, yyyy", 15),
                new DownloadColumn("AchievementDescription", "Achievement Description")
            };
        }

        protected string GetLocalDate(object item)
        {
            var when = (DateTimeOffset?)item;
            if (when == null)
                return string.Empty;
            return TimeZones.FormatDateOnly(when.Value, User.TimeZone);
        }

        protected string GetCredentialExpiry(object item)
        {
            if (item == null)
                return null;
            return GetCredentialExpiry((VCredential)item);
        }

        public static string GetCredentialExpiryDate(VCredential credential, string dateTimeFormat = null)
        {
            var expected = credential.CredentialExpirationExpected;
            var actual = credential.CredentialExpired;
            var now = DateTimeOffset.UtcNow;
            var userTimeZone = User.TimeZone;

            DateTimeOffset? targetDate = null;

            if (expected.HasValue && expected.Value > now)
                targetDate = expected.Value;
            else if (actual.HasValue || (expected.HasValue && expected.Value < now))
                targetDate = actual ?? expected.Value;

            if (!targetDate.HasValue)
                return null;

            var adjustedDate = targetDate.Value.ToOffset(userTimeZone.GetUtcOffset(targetDate.Value));

            return string.IsNullOrEmpty(dateTimeFormat)
                ? TimeZones.FormatDateOnly(adjustedDate, userTimeZone)
                : adjustedDate.ToString(dateTimeFormat);
        }

        public static string GetCredentialExpiry(VCredential credential)
        {
            var age = GetCredentialExpiryDate(credential);
            var now = DateTimeOffset.UtcNow;

            if (credential.CredentialExpirationExpected.HasValue && credential.CredentialExpirationExpected.Value > now)
            {
                var span = credential.CredentialExpirationExpected.Value - now;
                var days = span.TotalDays;

                if (days > 90)
                    return $"<div>{age}</div><div class='badge bg-success'>{span.Humanize(1, true, CultureInfo.CurrentCulture, TimeUnit.Month)} from now</div>";
                else
                    return $"<div>{age}</div><div class='badge bg-warning'>{credential.CredentialExpirationExpected.Value.Humanize()}</div>";
            }
            else if (credential.CredentialExpired.HasValue || (credential.CredentialExpirationExpected.HasValue && credential.CredentialExpirationExpected.Value < now))
            {
                var expired = credential.CredentialExpired ?? credential.CredentialExpirationExpected.Value;
                return $"<div>{age}</div><div class='badge bg-danger'>{expired.Humanize()}</div>";
            }

            return null;
        }

        public static string GetExpiration(object item)
        {
            var i = (VCredential)item;

            var credentialExpiration = new InSite.Domain.Records.Expiration(
                i.CredentialExpirationType, i.CredentialExpirationFixedDate,
                i.CredentialExpirationLifetimeQuantity, i.CredentialExpirationLifetimeUnit);

            var achievementExpiration = new InSite.Domain.Records.Expiration(
                i.AchievementExpirationType, i.AchievementExpirationFixedDate,
                i.AchievementExpirationLifetimeQuantity, i.AchievementExpirationLifetimeUnit);

            var credential = credentialExpiration.ToString(User.TimeZone);
            var achievement = achievementExpiration.ToString(User.TimeZone);

            var warning = string.Empty;
            if (!credential.Equals(achievement))
            {
                var icon = "<i class='fas fa-exclamation-triangle text-warning' style='padding-right:5px;'></i>";
                warning = $"<span class='pull-left' title='Default expiration is {achievement}'>{icon}</span>";
            }

            return warning + credential;
        }
    }
}
