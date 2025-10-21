using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Courses.Reports.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public void LoadData(int count)
        {
            Course2Repeater.DataSource = CourseSearch.SelectRecentCourses(Organization.OrganizationIdentifier, count).Select(x => new
            {
                x.CourseIdentifier,
                x.CourseName,
                LastChangeTimestamp = $"Course changed by {UserSearch.GetFullName(x.ModifiedBy)} {Shift.Common.Humanizer.Humanize(x.Modified)}"
            });
            Course2Repeater.DataBind();
        }
    }
}