using System;
using System.Linq;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class ChangeUser : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request["gradebook"], out var value) ? value : Guid.Empty;

        private Guid StudentIdentifier => Guid.TryParse(Request["student"], out var value) ? value : Guid.Empty;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"id={GradebookIdentifier}&panel=scores" : null;

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
                var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Achievement);
                if (gradebook == null || gradebook.OrganizationIdentifier != Organization.OrganizationIdentifier)
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);

                if (!BindItem(data, StudentIdentifier))
                    HttpResponseHelper.Redirect($"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}");

                SaveButton.Visible = !gradebook.IsLocked;

                CancelButton.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier);
            if (!gradebook.IsLocked)
                ServiceLocator.SendCommand(new NoteGradebookUser(GradebookIdentifier, StudentIdentifier, UserNote.Text, UserAdded.Value));

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        private bool BindItem(GradebookState data, Guid studentIdentifier)
        {
            if (!data.ContainsLearner(studentIdentifier))
                return false;

            var student = ServiceLocator.PersonSearch.GetPerson(studentIdentifier, Organization.Identifier, x => x.User);
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(GradebookIdentifier, x => x.Event, x => x.Achievement, x => x.Framework);
            var title = $"{student.User.FullName} <span class='form-text'>for {data.Name}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            PersonDetail.BindPerson(student, User.TimeZone);
            GradebookDetails.BindGradebook(gradebook, User.TimeZone);

            var users = ServiceLocator.RecordSearch.GetEnrollments(new QEnrollmentFilter { GradebookIdentifier = GradebookIdentifier });
            var user = users.FirstOrDefault(x => x.LearnerIdentifier == studentIdentifier);

            UserNote.Text = user.EnrollmentComment;
            UserAdded.Value = user.EnrollmentStarted;

            return true;
        }
    }
}
