using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Courses
{
    public partial class SearchCriteria : SearchCriteriaController<TCourseFilter>
    {
        public override TCourseFilter Filter
        {
            get
            {
                var filter = new TCourseFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    CatalogIdentifier = CatalogIdentifier.ValueAsGuid,
                    CourseName = CourseName.Text,
                    CourseAsset = CourseAsset.ValueAsInt,
                    CourseLabel = CourseLabel.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                CatalogIdentifier.ValueAsGuid = value.CatalogIdentifier;
                CourseName.Text = value.CourseName;
                CourseAsset.ValueAsInt = value.CourseAsset;
                CourseLabel.Text = value.CourseLabel;
            }
        }

        public override void Clear()
        {
            CatalogIdentifier.ClearSelection();
            CourseName.Text = null;
            CourseAsset.ValueAsInt = null;
            CourseLabel.Text = null;
        }
    }
}