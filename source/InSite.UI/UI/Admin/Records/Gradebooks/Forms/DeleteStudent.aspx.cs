using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
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
    public partial class DeleteStudent : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid StudentIdentifier => Guid.TryParse(Request["student"], out var value) ? value : Guid.Empty;


        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"id={GradebookIdentifier}&panel=scores" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Achievement);
                if (gradebook == null || gradebook.OrganizationIdentifier != Organization.OrganizationIdentifier)
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

                if (!BindItem(data, StudentIdentifier))
                    HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}");

                CancelButton.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}";
            }

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

            if (!data.IsLocked)
            {
                var commands = CreateDeleteCommands(data);

                ServiceLocator.SendCommands(commands);

                Course2Store.ClearCache(Organization.Identifier);
            }

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        private List<ICommand> CreateDeleteCommands(GradebookState data)
        {
            var commands = new List<ICommand>();

            var studentCredentials = GetStudentCredentials(data);
            if (studentCredentials != null)
            {
                foreach (var credential in studentCredentials)
                    commands.Add(new DeleteCredential(credential.CredentialIdentifier));
            }

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter
            {
                GradebookIdentifier = GradebookIdentifier,
                StudentUserIdentifier = StudentIdentifier
            });

            foreach (var progress in progresses)
                commands.Add(new DeleteProgress(progress.ProgressIdentifier));

            commands.Add(new DeleteEnrollment(GradebookIdentifier, StudentIdentifier));

            return commands;
        }

        private bool BindItem(GradebookState data, Guid studentIdentifier)
        {
            if (!data.ContainsLearner(studentIdentifier))
                return false;

            var person = ServiceLocator.PersonSearch.GetPerson(studentIdentifier, Organization.Identifier);
            var user = ServiceLocator.UserSearch.GetUser(studentIdentifier);
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event, x => x.Achievement, x => x.Framework);
            var title = $"{user.FullName} <span class='form-text'>for {data.Name}</span>";

            PageHelper.AutoBindHeader(this, null, title);
            
            PersonDetail.BindPerson(person, user, User.TimeZone);

            GradebookLink.HRef = $"/ui/admin/records/gradebooks/outline?id={gradebook.GradebookIdentifier}";
            GradebookTitle.Text = gradebook.GradebookTitle;

            var scores = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { GradebookIdentifier = GradebookIdentifier, StudentUserIdentifier = studentIdentifier });

            ScoresCount.Text = $"{scores.Count:n0}";

            var studentCredentials = GetStudentCredentials(data);

            CredentialCount.Text = $"{(studentCredentials?.Count ?? 0):n0}";

            DeleteButton.Visible = !data.IsLocked;

            if (data.IsLocked)
            {
                AlertPanel.Visible = false;
                Status.AddMessage(AlertType.Warning, "The gradebook is locked and therefore student cannot be deleted");
            }

            return true;
        }

        private List<VCredential> GetStudentCredentials(GradebookState data)
        {
            var achievements = data.GetItemsWithAchievements().Select(x => x.Achievement.Achievement).ToList();

            var toRemove = new List<Guid>();

            foreach (var achievementId in achievements)
            {
                var gradeItems = ServiceLocator.RecordSearch.GetGradeItems(new QGradeItemFilter { AchievementIdentifier = achievementId })
                    .ToArray()
                    .Select(g => g.GradebookIdentifier)
                    .Distinct()
                    .Count();

                if (gradeItems > 1)
                    toRemove.Add(achievementId);
            }

            achievements.RemoveAll(a => toRemove.Contains(a));

            if (achievements.Count == 0)
                return null;

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(new VCredentialFilter { UserIdentifier = StudentIdentifier });

            return credentials.Where(x => achievements.Any(y => y == x.AchievementIdentifier)).ToList();
        }
    }
}