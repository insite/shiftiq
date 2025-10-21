using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Courses
{
    public class CourseSearchResult
    {
        public Guid CourseIdentifier { get; set; }
        public string CourseCode { get; set; }
        public string CourseHook { get; set; }
        public string CourseLabel { get; set; }
        public string CourseName { get; set; }
        public string CatalogName { get; set; }

        public int? UnitCount { get; set; }
        public int? ModuleCount { get; set; }
        public int? ActivityCount { get; set; }
    }

    public partial class SearchResults : SearchResultsGridViewController<TCourseFilter>
    {
        protected static string GetLocalDateTime(DateTimeOffset value) => value.Format(User.TimeZone);

        protected string GetHtml(string name) =>
            ContentContainerItem.GetHtml((string)Eval(name + "Text"), (string)Eval(name + "Html"));

        protected override int SelectCount(TCourseFilter filter)
        {
            return CourseSearch.CountCourses(filter);
        }

        protected override IListSource SelectData(TCourseFilter filter)
        {
            filter.OrderBy = "CourseName";

            var data = CourseSearch.BindCourses(
                x => new CourseSearchResult
                {
                    CourseCode = x.CourseCode,
                    CourseHook = x.CourseHook,
                    CourseIdentifier = x.CourseIdentifier,
                    CourseLabel = x.CourseLabel,
                    CourseName = x.CourseName,
                    UnitCount = x.Units.Count,
                    ModuleCount = x.Units.Sum(y => y.Modules.Count),
                    ActivityCount = x.Units.Sum(y => y.Modules.Sum(z => z.Activities.Count)),
                    CatalogName = x.Catalog.CatalogName
                },
                filter);

            return data.ToSearchResult();
        }
    }
}