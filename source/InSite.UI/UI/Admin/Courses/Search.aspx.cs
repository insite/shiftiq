using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Courses
{
    public partial class Search : SearchPage<QCourseFilter>
    {
        public override string EntityName => "Course";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Course", "/ui/admin/courses/create", null, null));
        }
    }
}