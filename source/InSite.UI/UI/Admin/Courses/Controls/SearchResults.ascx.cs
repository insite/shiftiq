using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Courses
{
    internal class CourseSearchResult
    {
        public Guid CourseIdentifier { get; set; }
        public string CourseCode { get; set; }
        public string CourseHook { get; set; }
        public string CourseLabel { get; set; }
        public string CourseName { get; set; }
        public string CatalogName { get; set; }

        public string PublicationStatus { get; set; }
        public DateTimeOffset? PublicationDate { get; set; }
        public string PublicationAuthor { get; set; }

        public int? UnitCount { get; set; }
        public int? ModuleCount { get; set; }
        public int? ActivityCount { get; set; }
        public int EnrollmentStarted { get; set; }
        public int EnrollmentCompleted { get; set; }

        public Guid? GradebookIdentifier { get; set; }
        public string GradebookTitle { get; set; }
    }

    public partial class SearchResults : SearchResultsGridViewController<QCourseFilter>
    {
        protected static string GetLocalDateTime(DateTimeOffset value) => value.Format(User.TimeZone);

        protected string GetHtml(string name) =>
            ContentContainerItem.GetHtml((string)Eval(name + "Text"), (string)Eval(name + "Html"));

        protected override int SelectCount(QCourseFilter filter)
        {
            return CourseSearch.CountVCourses(filter);
        }

        protected override IListSource SelectData(QCourseFilter filter)
        {
            filter.OrderBy = "CourseName";

            var data = CourseSearch.BindVCourses(
                x => new CourseSearchResult
                {
                    CourseCode = x.CourseCode,
                    CourseHook = x.CourseHook,
                    CourseIdentifier = x.CourseIdentifier,
                    CourseLabel = x.CourseLabel,
                    CourseName = x.CourseName,
                    UnitCount = x.UnitCount,
                    ModuleCount = x.ModuleCount,
                    ActivityCount = x.ActivityCount,
                    CatalogName = x.CatalogName,
                    EnrollmentStarted = x.EnrollmentStarted,
                    EnrollmentCompleted = x.EnrollmentCompleted,
                    GradebookIdentifier = x.GradebookIdentifier,
                    GradebookTitle = x.GradebookTitle,
                },
                filter);

            SetPublicationStatus(data);

            return data.ToSearchResult();
        }

        private static void SetPublicationStatus(CourseSearchResult[] data)
        {
            var courseIds = data.Select(x => x.CourseIdentifier).Distinct().ToArray();

            // 2025-12-05: Aleksey - when there are more than one page per course then ToDictionary crashes, don't use it pls
            var coursePages = ServiceLocator.PageSearch
                .Select(
                    x => x.ObjectType == "Course"
                        && courseIds.Contains(x.ObjectIdentifier.Value)
                        && x.IsHidden == false
                );

            foreach (var item in data)
            {
                var page = coursePages.FirstOrDefault(x => x.ObjectIdentifier == item.CourseIdentifier);
                if (page != null)
                {
                    item.PublicationStatus = "Published";
                    item.PublicationDate = page.AuthorDate;
                    item.PublicationAuthor = page.AuthorName;
                }
                else
                {
                    item.PublicationStatus = "Unpublished";
                }
            }
        }
    }
}