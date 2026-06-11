using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace InSite.Admin.Courses
{
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

            var data = CourseSearch.BindVCourses(x => x, filter);

            SetPublicationStatus(data);

            return data.ToSearchResult();
        }

        private static void SetPublicationStatus(CourseMatch[] data)
        {
            var courseIds = data.Select(x => x.CourseId).Distinct().ToArray();

            // 2025-12-05: Aleksey - when there are more than one page per course then ToDictionary crashes, don't use it pls
            var coursePages = ServiceLocator.PageSearch
                .Select(
                    x => x.ObjectType == "Course"
                        && courseIds.Contains(x.ObjectIdentifier.Value)
                        && x.IsHidden == false
                );

            foreach (var item in data)
            {
                var page = coursePages.FirstOrDefault(x => x.ObjectIdentifier == item.CourseId);
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