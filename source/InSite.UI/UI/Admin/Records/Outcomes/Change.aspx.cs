using System;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Outcomes
{
    public partial class Change : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private string ReturnUrl => GetParentUrl(null);

        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid CompetencyIdentifier => Guid.TryParse(Request["competency"], out var value) ? value : Guid.Empty;

        private Guid UserIdentifier => Guid.TryParse(Request["user"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var validation = ServiceLocator.RecordSearch.GetValidation(
                    GradebookIdentifier,
                    UserIdentifier,
                    CompetencyIdentifier,
                    x => x.Gradebook.Event,
                    x => x.Gradebook.Achievement,
                    x => x.Gradebook.Framework,
                    x => x.Student,
                    x => x.Standard
                );

                if (validation == null)
                    HttpResponseHelper.Redirect(ReturnUrl);

                PageHelper.AutoBindHeader(
                    this, 
                    qualifier: $"{validation.Gradebook.GradebookTitle} <span class='form-text'>{validation.Student.UserFullName}</span>");

                SetInputValues(validation);

                CancelButton.NavigateUrl = ReturnUrl;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new ChangeGradebookValidation(GradebookIdentifier, UserIdentifier, CompetencyIdentifier, ValidationPoints.ValueAsDecimal));

            HttpResponseHelper.Redirect(ReturnUrl);
        }

        private void SetInputValues(QGradebookCompetencyValidation validation)
        {
            var gradebook = validation.Gradebook;

            GradebookTitle.Text = gradebook.GradebookTitle;
            GradebookCreated.Text = gradebook.GradebookCreated.Format(User.TimeZone);
            LockedField.Visible = gradebook.IsLocked;

            ClassField.Visible = gradebook.Event != null;
            if (gradebook.Event != null)
            {
                ClassTitle.Text = $"<a href=\"/ui/admin/events/classes/outline?event={gradebook.Event.EventIdentifier}\">{gradebook.Event.EventTitle} </a>";
                ClassScheduled.Text = $"{GetLocalTime(gradebook.Event.EventScheduledStart)} - {GetLocalTime(gradebook.Event.EventScheduledEnd)}";

                var instructors = ServiceLocator.EventSearch
                    .GetAttendees(new QEventAttendeeFilter
                    {
                        GradebookIdentifier = gradebook.GradebookIdentifier
                    }, x => x.Person)
                    .Where(x => x.AttendeeRole == "Instructor")
                    .ToList();

                ClassInstructors.Text = instructors.Count > 0
                    ? string.Join(", ", instructors.Select(x => $"<a href=\"/ui/admin/contacts/people/edit?contact={x.UserIdentifier}\">{x.UserFullName}</a>"))
                    : "";

                ClassInstructorsField.Visible = instructors.Count > 0;
            }

            AchievementField.Visible = gradebook.Achievement != null;
            if (gradebook.Achievement != null)
                AchievementTitle.Text = $"<a href=\"/ui/admin/records/achievements/outline?id={gradebook.Achievement.AchievementIdentifier}\">{gradebook.Achievement.AchievementTitle} </a>";

            FrameworkField.Visible = gradebook.Framework != null;
            if (gradebook.Framework != null)
                FrameworkTitle.Text = $"<a href=\"/ui/admin/standards/edit?id={gradebook.FrameworkIdentifier}\">{gradebook.Framework.FrameworkTitle} </a>";

            StudentFullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={validation.UserIdentifier}\">{validation.Student.UserFullName}</a>";

            CompetencyTitle.Text = validation.Standard.StandardTitle;

            ValidationPoints.ValueAsDecimal = validation.ValidationPoints;

            SaveButton.Visible = !gradebook.IsLocked;
        }

        private string GetLocalTime(DateTimeOffset? date)
        {
            return date != null ? date.FormatDateOnly(User.TimeZone) : "";
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);
    }
}
