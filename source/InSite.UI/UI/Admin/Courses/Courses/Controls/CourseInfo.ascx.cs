using InSite.Application.Courses.Read;

namespace InSite.Admin.Courses.Courses.Controls
{
    public partial class CourseInfo : System.Web.UI.UserControl
    {
        public void BindCourse(QCourse course)
        {
            CourseLink.HRef = $"/ui/admin/courses/manage?course={course.CourseIdentifier}";
            CourseName.Text = course.CourseName ?? "Untitled Course";
            CourseCode.Text = course.CourseCode ?? "N/A";
            CourseLabel.Text = course.CourseLabel ?? "N/A";
            CourseAsset.Text = course.CourseAsset.ToString();
        }
    }
}