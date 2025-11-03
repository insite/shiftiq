using System;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Programs
{
    public partial class DeleteUser : AdminBasePage, IHasParentLinkParameters
    {
        private Guid ProgramIdentifier => Guid.TryParse(Request["program"], out var id) ? id : Guid.Empty;
        private Guid UserIdentifier => Guid.TryParse(Request["user"], out var id) ? id : Guid.Empty;

        private string ReturnUrl
        {
            get => (string)ViewState[nameof(ReturnUrl)];
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var returnUrl = Request.QueryString["return"];
            ReturnUrl = returnUrl.IfNullOrEmpty(Outline.GetNavigateUrl(ProgramIdentifier, panel: "users"));

            var program = ProgramSearch.GetProgram(ProgramIdentifier);
            var programUser = ProgramSearch1.GetProgramUsers(new VProgramEnrollmentFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                ProgramIdentifier = ProgramIdentifier,
                UserIdentifier = UserIdentifier
            })
                .FirstOrDefault();

            if (programUser == null || program?.OrganizationIdentifier != Organization.Identifier)
            {
                Search.Redirect();
                return;
            }

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, Organization.Identifier, x => x.User);
            if (person == null)
                return;

            PersonDetail.BindPerson(person, User.TimeZone);

            ProgramName.Text = $"<a href=\"{ReturnUrl}\">{program.ProgramName}</a>";

            PageHelper.AutoBindHeader(Page, null, program.ProgramName);

            CancelButton.NavigateUrl = ReturnUrl;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var tasks = TaskStore.DeleteEnrollments(Organization.Identifier, ProgramIdentifier, UserIdentifier);

            if(tasks != null && tasks.Length > 0)
            {
                EnsureCourseEnrollmentDeletion(UserIdentifier, tasks);
                EnsureLogbookEnrollmentDeletion(UserIdentifier, tasks);
            }

            ProgramStore.DeleteEnrollment(ProgramIdentifier, UserIdentifier);

            HttpResponseHelper.Redirect(ReturnUrl);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramIdentifier}&panel=users"
                : GetParentLinkParameters(parent, null);
        }

        private static void EnsureLogbookEnrollmentDeletion(Guid userIdentifier, TTask[] tasks)
        {
            foreach (var task in tasks.Where(x => x.ObjectType == "Logbook"))
                ProgramHelper.EnsureLogbookEnrollmentDeletion(userIdentifier, task.ObjectIdentifier, Organization.Identifier);
        }

        private static void EnsureCourseEnrollmentDeletion(Guid userIdentifier, TTask[] tasks)
        {
            foreach (var task in tasks.Where(x => x.ObjectType == "Course"))
                ProgramHelper.EnsureCourseEnrollmentDeletion(userIdentifier, task.ObjectIdentifier, Organization.Identifier);
        }
    }
}