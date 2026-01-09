using System;
using System.Linq;

using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Courses.Courses
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid CourseIdentifier => Guid.TryParse(Request["course"], out var value) ? value : Guid.Empty;

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

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var pages = ServiceLocator.PageSearch.Select(x => x.ObjectType == "Course" && x.ObjectIdentifier == CourseIdentifier);
            foreach (var page in pages)
            {
                var commands = new PageCommandGenerator().
                    DeletePageWithChildren(ServiceLocator.PageSearch.GetPageChildrenIds(page.PageIdentifier));

                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);
            }

            Course2Store.DeleteCourse(CourseIdentifier);

            HttpResponseHelper.Redirect("/ui/admin/courses/search");
        }

        private void LoadData()
        {
            var course = CourseSearch.SelectCourse(CourseIdentifier);

            if (course == null || course.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect($"/ui/admin/courses/search");
                return;
            }

            var title = ServiceLocator.ContentSearch.GetTitleText(CourseIdentifier);

            PageHelper.AutoBindHeader(this, null, title ?? course.CourseName ?? "Untitled Course");

            var modules = CourseSearch.BindModules(
                x => new { ActivityCount = x.Activities.Count() },
                x => x.Unit.CourseIdentifier == CourseIdentifier
            );

            var moduleCount = modules.Length;
            var activityCount = modules.Length > 0 ? modules.Sum(x => x.ActivityCount) : 0;
            var webPageCount = ServiceLocator.PageSearch.Count(x => x.ObjectType == "Course" && x.ObjectIdentifier == CourseIdentifier);

            CourseDetail.BindCourse(course);

            ModuleCount.Text = $"{moduleCount:n0}";
            ActivityCount.Text = $"{activityCount:n0}";
            WebPageCount.Text = $"{webPageCount:n0}";

            CancelButton.NavigateUrl = $"/ui/admin/courses/manage?course={CourseIdentifier}&panel=config";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"course={CourseIdentifier}&panel=config"
                : null;
        }
    }
}