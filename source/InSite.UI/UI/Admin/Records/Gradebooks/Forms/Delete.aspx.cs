using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Exceptions;

using InSite.Application.Courses.Write;
using InSite.Application.Credentials.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCheck.AutoPostBack = true;
            DeleteCheckLearners.AutoPostBack = true;

            DeleteCheck.CheckedChanged += (x, y) => UpdateDeleteButtonState();
            DeleteCheckLearners.CheckedChanged += (x, y) => UpdateDeleteButtonState();

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event, x => x.Achievement, x => x.Framework);
            if (gradebook == null || gradebook.OrganizationIdentifier != CurrentSessionState.Identity.Organization.Identifier || gradebook.IsLocked)
            {
                HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/search");
                return;
            }

            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            var progressCount = ServiceLocator.RecordSearch.CountGradebookScores(new QProgressFilter { GradebookIdentifier = GradebookIdentifier });

            PageHelper.AutoBindHeader(this, null, gradebook.GradebookTitle);

            GradebookDetails.BindGradebook(gradebook, User.TimeZone);

            ScoreItemsCount.Text = $"{data.AllItemCount:n0}";
            StudentsCount.Text = $"{data.Enrollments.Count:n0}";
            ScoresCount.Text = $"{progressCount:n0}";

            var hasStudents = progressCount == 0 && progressCount == 0;
            var hasItems = data.RootItems.Count > 0;

            NoVoid.Visible = !hasStudents;

            DeleteCheckLearners.Visible = data.Enrollments.Count > 0;

            var backUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=gradebook";

            CancelButton.NavigateUrl = backUrl;

            UpdateDeleteButtonState();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (DeleteGradebook())
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");
            }
            catch (UnhandledCommandException uncomex)
            {
                if (uncomex.InnerException is LockedGradebookException lockex)
                    Status.AddMessage(AlertType.Error, "The gradebook is locked");
            }
        }

        private void UpdateDeleteButtonState()
        {
            if (DeleteCheckLearners.Visible)
                DeleteButton.Enabled = DeleteCheck.Checked && DeleteCheckLearners.Checked;
            else
                DeleteButton.Enabled = DeleteCheck.Checked;
        }

        private bool DeleteGradebook()
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier);
            if (gradebook == null)
                return true;

            if (gradebook.IsLocked)
            {
                Status.AddMessage(AlertType.Error, "The gradebook is locked");
                return false;
            }

            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            foreach (var student in data.Enrollments)
            {
                var studentCredentials = GetStudentCredentials(data, student.Learner);

                if (studentCredentials != null)
                {
                    foreach (var credential in studentCredentials)
                        ServiceLocator.SendCommand(new DeleteCredential(credential.CredentialIdentifier));
                }

                ServiceLocator.SendCommand(new DeleteEnrollment(GradebookIdentifier, student.Learner));
            }

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter
            {
                GradebookIdentifier = gradebook.GradebookIdentifier
            });

            foreach (var progress in progresses)
                ServiceLocator.SendCommand(new DeleteProgress(progress.ProgressIdentifier));

            ServiceLocator.SendCommand(new DeleteGradebook(GradebookIdentifier));

            var courses = CourseSearch.BindCourses(x => x, x => x.GradebookIdentifier == GradebookIdentifier);
            foreach (var course in courses)
                ServiceLocator.SendCommand(new ConnectCourseGradebook(course.CourseIdentifier, null));

            return true;
        }

        private List<VCredential> GetStudentCredentials(GradebookState data, Guid studentIdentifier)
        {
            var achievements = data.GetItemsWithAchievements().Select(x => x.Achievement.Achievement).ToList();

            if (achievements.Count == 0)
                return null;

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(new VCredentialFilter { UserIdentifier = studentIdentifier });

            return credentials.Where(x => achievements.Any(y => y == x.AchievementIdentifier)).ToList();
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookIdentifier}&panel=gradebook"
                : null;
        }
    }
}