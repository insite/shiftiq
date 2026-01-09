using System;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Courses
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BindModelToControls();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var courseCount = CourseSearch.CountCourses(new QCourseFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            CourseCount.InnerText = courseCount.ToString();

            var catalogCount = CourseSearch.GetCatalogs(Organization.Identifier, null).Count;
            CatalogCount.InnerText = catalogCount.ToString();

            var categoryCount = TCollectionItemSearch.Count(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CollectionName = CollectionName.Learning_Catalogs_Category_Name
            });
            CategoryCount.InnerText = categoryCount.ToString();

            RecentList.LoadData(10);
        }
    }
}