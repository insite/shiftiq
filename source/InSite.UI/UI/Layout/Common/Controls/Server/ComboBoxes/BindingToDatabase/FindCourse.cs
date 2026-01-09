using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Courses.Read;
using InSite.Persistence;

namespace InSite.Common.Web.UI
{
    public class FindCourse : BaseFindEntity<QCourseFilter>
    {
        public bool? HasGradebook
        {
            get => Filter.HasGradebook;
            set => Filter.HasGradebook = value;
        }

        public QCourseFilter Filter => (QCourseFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QCourseFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier, HasGradebook = false }));

        protected override string GetEntityName() => "Course";

        protected override QCourseFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.CourseName = keyword;

            return filter;
        }

        protected override int Count(QCourseFilter filter)
        {
            return CourseSearch.CountCourses(filter);
        }

        protected override DataItem[] Select(QCourseFilter filter)
        {
            filter.OrderBy = nameof(QCourse.CourseName);

            return CourseSearch.BindCourses(x => x, filter).Select(GetDataItem).ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return CourseSearch.BindCourses(x => x, x => ids.Contains(x.CourseIdentifier)).Select(GetDataItem).ToArray();
        }

        private static DataItem GetDataItem(QCourse x) => new DataItem
        {
            Value = x.CourseIdentifier,
            Text = x.CourseName,
        };
    }
}
