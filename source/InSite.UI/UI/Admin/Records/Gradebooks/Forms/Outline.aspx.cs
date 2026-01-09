using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Admin.Records.Gradebooks.Controls;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class Outline : AdminBasePage
    {
        private Guid? GradebookID => Guid.TryParse(Request["id"], out var gradebookID) ? gradebookID : (Guid?)null;

        private string Status => Request["status"];

        private Guid? ScoreItem => Guid.TryParse(Request["scoreItem"], out var value) ? value : (Guid?)null;

        public static List<string> UploadWarnings
        {
            get => (List<string>)HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Forms.Outline.UploadWarnings"];
            set => HttpContext.Current.Session["InSite.Admin.Grades.Gradebooks.Forms.Outline.UploadWarnings"] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LockButton.Click += LockButton_Click;
            UnlockButton.Click += UnlockButton_Click;

            ProgressList.Calculated += ProgressList_Calculated;
            ProgressList.Released += ProgressList_Released;
            ProgressList.Alert += ProgressList_Alert;
            ProgressList.ScoresCreated += ProgressList_ScoresCreated;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (Status == "copied")
                {
                    StatusAlert.AddMessage(AlertType.Information, $"A copy of this gradebook has been made. This is the copy.");
                }
                else if (Status == "uploaded")
                {
                    if (UploadWarnings.IsNotEmpty())
                    {
                        var warnings = string.Join("", UploadWarnings.Select(x => $"<li>{x}</li>"));
                        StatusAlert.AddMessage(AlertType.Warning, $"<ul>The gradebook has been created from JSON file with these warnings:{warnings}</ul>");

                        UploadWarnings = null;
                    }
                    else
                    {
                        StatusAlert.AddMessage(AlertType.Success, "The gradebook has been created from JSON file.");
                    }
                }

                LoadData();

                if (Request["panel"] == "scores")
                    ProgressPanel.IsSelected = true;
                else if (Request["panel"] == "credentials")
                    CredentialPanel.IsSelected = true;
                else if (Request["panel"] == "config")
                    ConfigurationSection.IsSelected = true;
                else if (Request["panel"] == "gradebook")
                    GradebookPanel.IsSelected = true;
            }
        }

        private void LockButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new LockGradebook(GradebookID.Value));

            LoadData();

            StatusAlert.AddMessage(AlertType.Success, "Gradebook is locked");
        }

        private void UnlockButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new UnlockGradebook(GradebookID.Value));

            LoadData();

            StatusAlert.AddMessage(AlertType.Success, "Gradebook is unlocked");
        }

        private void ProgressList_Calculated(object sender, EventArgs e)
        {
            StatusAlert.AddMessage(AlertType.Success, "Scores have been calculated");

            LoadData();
        }

        private void ProgressList_Released(object sender, EventArgs e)
        {
            StatusAlert.AddMessage(AlertType.Success, "Scores have been released");

            LoadData();

            CredentialPanel.IsSelected = true;
        }

        private void ProgressList_Alert(object sender, AlertArgs args)
        {
            StatusAlert.AddMessage(args.Type, args.Text);
            StatusUpdatePanel.Update();
        }

        private void ProgressList_ScoresCreated(object sender, IntValueArgs e)
        {
            StatusAlert.AddMessage(AlertType.Success, $"<b>{e.Value}</b> scores have been created");

            LoadData();
        }

        private void LoadData()
        {
            if (GradebookID == null)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");
                return;
            }

            var queryGradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookID.Value,
                x => x.Event,
                x => x.Achievement,
                x => x.Period,
                x => x.GradebookEvents.Select(y => y.Event)
            );

            if (queryGradebook == null || queryGradebook.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");
                return;
            }

            DownloadLink.NavigateUrl = $"/ui/admin/records/gradebooks/download?gradebook={GradebookID}";

            var dataGradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookID.Value);

            var title = queryGradebook.GradebookTitle;

            if (queryGradebook.Event != null)
                title += $" <span class='form-text'> for {queryGradebook.Event.EventTitle} ({GetLocalTime(queryGradebook.Event.EventScheduledStart)} - {GetLocalTime(queryGradebook.Event.EventScheduledEnd)})</span>";

            PageHelper.AutoBindHeader(this, null, title);

            var status = dataGradebook.IsLocked ? " <i class='fas fa-lock text-danger' style='font-size:14px;' title='Locked'></i>" : "";

            GradebookPanel.Title = $"Gradebook Setup {status}";

            GradebookStatus.Text = dataGradebook.IsLocked ? "<span class='badge bg-danger'><i class='far fa-lock'></i> Locked</span>" :
                "<span class='badge bg-success'><i class='far fa-lock-open'></i> Unlocked</span>";

            LockButton.Visible = !dataGradebook.IsLocked;
            UnlockButton.Visible = dataGradebook.IsLocked;

            GradebookTitle.Text = queryGradebook.GradebookTitle;

            BindEvents(queryGradebook);

            AchievementTitle.Text = queryGradebook.Achievement?.AchievementTitle != ""
                ? $"<a href=\"/ui/admin/records/achievements/outline?id={queryGradebook.Achievement?.AchievementIdentifier}\">{queryGradebook.Achievement?.AchievementTitle} </a>"
                : "None";

            Reference.Text = queryGradebook.Reference ?? "None";

            var gradebookType = queryGradebook.GradebookType.ToEnum<GradebookType>();
            Scores.Checked = gradebookType == GradebookType.Scores || gradebookType == GradebookType.ScoresAndStandards;
            Standards.Checked = gradebookType == GradebookType.Standards || gradebookType == GradebookType.ScoresAndStandards;

            StandardField.Visible = gradebookType == GradebookType.Standards || gradebookType == GradebookType.ScoresAndStandards;
            FrameworkTitle.Text = queryGradebook.FrameworkIdentifier.HasValue ?
                $"<a href=\"/ui/admin/standards/edit?id={queryGradebook.FrameworkIdentifier}\">{StandardSearch.Select(queryGradebook.FrameworkIdentifier.Value)?.ContentTitle} </a>" : "None";

            PeriodName.Text = queryGradebook.Period?.PeriodName ?? "None";

            GradeItemsGrid.LoadData(GradebookID.Value, dataGradebook);

            ProgressList.LoadData(dataGradebook, ScoreItem, null);

            var credentialCounts = CredentialGrid.LoadData(GradebookID.Value);
            CredentialPanel.Visible = credentialCounts.CountInUsers > 0;

            ScormEventGrid.LoadData(GradebookID.Value);
            ScormRegistrationGrid.LoadData(GradebookID.Value);

            var useScormCloud = Organization.Integrations?.ScormCloud?.UserName != null
                && Organization.Integrations?.ScormCloud?.Password != null;

            ScormPanel.Visible = useScormCloud && ScormRegistrationGrid.RowCount > 0;

            LearningMasteryGrid.LoadData(GradebookID);
            LearningMasteryPanel.Visible = queryGradebook.GradebookType == GradebookType.ScoresAndStandards.ToString();

            GradeItemModifyPanel.Visible = !dataGradebook.IsLocked;

            var canChangeGradebook = !dataGradebook.IsLocked;

            Rename.Visible = !dataGradebook.IsLocked;
            DeleteLink.Visible = !dataGradebook.IsLocked;
            ChangeClass.Visible = canChangeGradebook;
            ChangeClass2.Visible = canChangeGradebook;
            ChangeAchievement.Visible = canChangeGradebook;
            ChangeCheckboxes.Visible = canChangeGradebook;
            ChangeFramework.Visible = canChangeGradebook;
            ChangePeriod.Visible = canChangeGradebook;

            ProgressPanel.Visible = dataGradebook.Type == GradebookType.Scores || dataGradebook.Type == GradebookType.ScoresAndStandards;

            if (dataGradebook.Type == GradebookType.Standards)
                GradebookPanel.IsSelected = true;

            AddCategoryButton.NavigateUrl = $"/ui/admin/records/items/add-category?gradebook={GradebookID}";
            AddScoreButton.NavigateUrl = $"/ui/admin/records/items/add-score?gradebook={GradebookID}";
            AddCalculationButton.NavigateUrl = $"/ui/admin/records/items/add-calculation?gradebook={GradebookID}";

            CopyLink.NavigateUrl = $"/ui/admin/records/gradebooks/open?action=duplicate&gradebook={GradebookID}";
            DeleteLink.NavigateUrl = $"/admin/records/gradebooks/delete?gradebook={GradebookID}";
            Rename.NavigateUrl = $"/ui/admin/records/gradebooks/rename?gradebook={GradebookID}";
            ChangeClass.NavigateUrl = $"/ui/admin/records/gradebooks/change?gradebook={GradebookID}";
            ChangeClass2.NavigateUrl = ChangeClass.NavigateUrl;
            ChangeAchievement.NavigateUrl = $"/ui/admin/records/gradebooks/change?gradebook={GradebookID}";
            ChangeCheckboxes.NavigateUrl = $"/ui/admin/records/gradebooks/change?gradebook={GradebookID}";
            ChangeFramework.NavigateUrl = $"/ui/admin/records/gradebooks/change?gradebook={GradebookID}";
            ChangePeriod.NavigateUrl = $"/ui/admin/records/gradebooks/change?gradebook={GradebookID}";
            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(GradebookID.Value, $"/ui/admin/records/gradebooks/outline?id={GradebookID}");
        }

        private void BindEvents(QGradebook queryGradebook)
        {
            var events = queryGradebook.GradebookEvents.Where(x => x.Event != null).ToList();
            var isOneClass = events.Count <= 1;

            OneClassPanel.Visible = isOneClass;
            MultiClassPanel.Visible = !isOneClass;

            if (isOneClass)
            {
                ClassTitle.Text = !string.IsNullOrWhiteSpace(queryGradebook.Event?.EventTitle)
                    ? $"<a href=\"/ui/admin/events/classes/outline?event={queryGradebook.Event?.EventIdentifier}\">{queryGradebook.Event?.EventTitle} </a>"
                    : "None";

                if (queryGradebook.Event != null)
                    ClassScheduled.Text = $"Scheduled: {GetLocalTime(queryGradebook.Event.EventScheduledStart)} - {GetLocalTime(queryGradebook.Event.EventScheduledEnd)}";
            }
            else
            {
                var dataSource = events
                    .OrderBy(x => x.EventIdentifier == queryGradebook.EventIdentifier ? 0 : 1)
                    .ThenBy(x => x.Event.EventScheduledStart)
                    .ThenBy(x => x.EventIdentifier)
                    .Select(x => new
                    {
                        EventIdentifier = x.EventIdentifier,
                        EventTitle = x.Event.EventTitle,
                    })
                    .ToList();

                ClassRepeater.DataSource = dataSource;
                ClassRepeater.DataBind();
            }
        }

        private static string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }
    }
}
