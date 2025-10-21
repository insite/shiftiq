using System;
using System.Linq;

using Humanizer;

using InSite.Application.Achievements.Write;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Constant;

using BreadcrumbItem = Shift.Contract.BreadcrumbItem;

namespace InSite.UI.Admin.Records.Achievements
{
    public partial class Delete : AdminBasePage
    {
        private const string HomeUrl = "/ui/admin/records/home";
        private const string SearchUrl = "/ui/admin/records/achievements/search";

        private Guid? AchievementIdentifier => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        private string DefaultParameters => $"id={AchievementIdentifier}";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                BindModelToControls();
        }

        private void BindModelToControls()
        {
            var achievement = AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value) : null;

            if (achievement == null || achievement.OrganizationIdentifier != CurrentSessionState.Identity.Organization.Identifier)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            if (!achievement.AchievementIsEnabled)
            {
                DeleteButton.Visible = false;
                Status.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
            }

            PageHelper.BindHeader(this, new BreadcrumbItem[]
            {
                new BreadcrumbItem("Records", HomeUrl),
                new BreadcrumbItem("Achievements", SearchUrl),
                new BreadcrumbItem("Outline", GetParentUrl(DefaultParameters)),
                new BreadcrumbItem("Delete", null, null, "active"),
            }, null, achievement.AchievementTitle);

            AchievementDetails.BindAchievement(achievement, User.TimeZone);

            var eventCount = ServiceLocator.EventSearch.CountEvents(new QEventFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                AchievementIdentifier = AchievementIdentifier.Value
            });

            var gradbookCount = ServiceLocator.RecordSearch.CountGradebooks(new QGradebookFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                AchievementIdentifier = AchievementIdentifier.Value
            });

            var filter = new VCredentialFilter();
            filter.AchievementIdentifier = AchievementIdentifier.Value;
            var credentials = ServiceLocator.AchievementSearch.GetCredentials(filter);

            EventCount.Text = $"{eventCount:n0}";
            CredentialCount.Text = $"{credentials.Count:n0}";
            GradebookCount.Text = $"{gradbookCount:n0}";

            var gradebookUsage = GetGradebookUsage();
            if (gradebookUsage != null)
                GradebookUsage.Text = $"It has been assigned to {gradebookUsage}.";

            var credentialUsage = "This achievement has been assigned to "
                + "user".ToQuantity(credentials.Count)
                + ". Therefore, if you delete the achievement then you must also delete all of the credentials associated with it.";
            if (credentials.Count > 0)
                CredentialUsage.Text = credentialUsage;

            if (credentials.Count > 0)
                DeleteConfirmationCheckbox.Text = "Delete Achievement and Credentials";

            var allowVoid = achievement.AchievementIsEnabled && gradebookUsage == null;

            DeleteError.Visible = !allowVoid;
            LockedMessage.Visible = !achievement.AchievementIsEnabled;
            GradebookUsage.Visible = achievement.AchievementIsEnabled && gradebookUsage != null;
            DeleteButton.Visible = allowVoid;

            CancelButton.Visible = allowVoid;
            CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);

            CloseButton.Visible = !allowVoid;
            CloseButton.NavigateUrl = GetParentUrl(DefaultParameters);

            DeleteConfirmationPanel.Visible = allowVoid;
        }

        private string GetGradebookUsage()
        {
            var gradebooks = ServiceLocator
                .RecordSearch
                .GetGradebooks(new QGradebookFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    AchievementIdentifier = AchievementIdentifier.Value
                }, null, null, x => x.Enrollments)
                .Where(x => x.Enrollments.Count > 0)
                .ToList();

            if (gradebooks.Count > 0)
                return "gradebook".ToQuantity(gradebooks.Count)
                    + " containing "
                    + "student".ToQuantity(gradebooks.Sum(x => x.Enrollments.Count));

            return null;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value);

            if (achievement != null)
            {
                if (!achievement.AchievementIsEnabled)
                {
                    Status.AddMessage(AlertType.Error, "This achievement is locked. You must unlock this achievement before deleting it.");
                    return;
                }

                var events = ServiceLocator.EventSearch.GetEvents(new QEventFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    AchievementIdentifier = AchievementIdentifier.Value
                });

                var gradebooks = ServiceLocator.RecordSearch.GetGradebooks(new QGradebookFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    AchievementIdentifier = AchievementIdentifier.Value
                });

                var gradebookItems = ServiceLocator.RecordSearch.GetGradeItems(new QGradeItemFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    AchievementIdentifier = AchievementIdentifier.Value
                }, null, null, x => x.Gradebook);

                foreach (var @event in events)
                    ServiceLocator.SendCommand(new ChangeEventAchievement(@event.EventIdentifier, null));

                foreach (var gradebook in gradebooks)
                {
                    var isLocked = gradebook.IsLocked;

                    if (isLocked) ServiceLocator.SendCommand(new UnlockGradebook(gradebook.GradebookIdentifier));
                    ServiceLocator.SendCommand(new ChangeGradebookAchievement(gradebook.GradebookIdentifier, null));
                    if (isLocked) ServiceLocator.SendCommand(new LockGradebook(gradebook.GradebookIdentifier));
                }

                foreach (var gradebookItem in gradebookItems)
                {
                    var isLocked = gradebookItem.Gradebook.IsLocked;

                    if (isLocked) ServiceLocator.SendCommand(new UnlockGradebook(gradebookItem.GradebookIdentifier));
                    ServiceLocator.SendCommand(new ChangeGradeItemAchievement(gradebookItem.GradebookIdentifier, gradebookItem.GradeItemIdentifier, null));
                    if (isLocked) ServiceLocator.SendCommand(new LockGradebook(gradebookItem.GradebookIdentifier));
                }

                ServiceLocator.SendCommand(new DeleteAchievement(AchievementIdentifier.Value, DeleteConfirmationCheckbox.Checked));
            }

            HttpResponseHelper.Redirect(SearchUrl);
        }
    }
}