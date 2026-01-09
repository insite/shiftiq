using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CourseComboBox : ComboBox
    {
        public QCourseFilter ListFilter => (QCourseFilter)(ViewState[nameof(ListFilter)]
        ?? (ViewState[nameof(ListFilter)] = new QCourseFilter()));

        protected override ListItemArray CreateDataSource()
        {
            ListFilter.OrderBy = "CourseName";

            var data = CourseSearch.BindCourses(
                x => new ListItem
                {
                    Value = x.CourseIdentifier.ToString(),
                    Text = x.CourseName
                },
                ListFilter);

            return new ListItemArray(data);
        }
    }

    public class CatalogComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;

            var data = CourseSearch.GetCatalogs(organization, null)
                .Select(x => new ListItem
                {
                    Value = x.CatalogIdentifier.ToString(),
                    Text = x.CatalogName
                });

            return new ListItemArray(data);
        }
    }
}