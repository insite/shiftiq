using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Courses
{
    public partial class SearchCriteria : SearchCriteriaController<QCourseFilter>
    {
        public override QCourseFilter Filter
        {
            get
            {
                var filter = new QCourseFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    CatalogIdentifier = CatalogIdentifier.ValueAsGuid,
                    CourseName = CourseName.Text,
                    CourseAsset = CourseAsset.ValueAsInt,
                    CourseLabel = CourseLabel.Text,
                    HasWebPage = HasWebPage.ValueAsBoolean,
                    WebPageAuthoredSince = WebPageAuthoredSince.Value,
                    WebPageAuthoredBefore = WebPageAuthoredBefore.Value,
                    GradebookTitle = GradebookTitle.Text,
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
                HasWebPage.ValueAsBoolean = value.HasWebPage;
                WebPageAuthoredSince.Value = value.WebPageAuthoredSince;
                WebPageAuthoredBefore.Value = value.WebPageAuthoredBefore;
                GradebookTitle.Text = value.GradebookTitle;
            }
        }

        public override void Clear()
        {
            CatalogIdentifier.ClearSelection();
            CourseName.Text = null;
            CourseAsset.ValueAsInt = null;
            CourseLabel.Text = null;
            HasWebPage.ValueAsBoolean = null;
            WebPageAuthoredSince.Value = null;
            WebPageAuthoredBefore.Value = null;
            GradebookTitle.Text = null;
        }
    }
}