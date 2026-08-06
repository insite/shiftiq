using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace InSite.Admin.Courses
{
    public partial class SearchResults : SearchResultsGridViewController<QCourseFilter>
    {
        private Dictionary<Guid, string[]> _courseAccessGroups;

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
            LoadAccessGroups(data);

            return data.ToSearchResult();
        }

        private void LoadAccessGroups(CourseMatch[] data)
        {
            var courseIds = data.Select(x => x.CourseId).Distinct().ToArray();

            var permissions = TGroupPermissionSearch.Bind(
                x => new { x.ObjectIdentifier, x.Group.GroupName },
                x => courseIds.Contains(x.ObjectIdentifier));

            _courseAccessGroups = permissions
                .GroupBy(x => x.ObjectIdentifier)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.GroupName).Distinct().OrderBy(y => y).ToArray());
        }

        protected string GetCatalogueHtml(CourseMatch item)
        {
            var html = new StringBuilder();

            var hasCatalog = item.CatalogName.HasValue();
            if (hasCatalog)
                html.Append($"<div>{HttpUtility.HtmlEncode(item.CatalogName)}</div>");

            var badges = new StringBuilder();

            if (hasCatalog)
            {
                var isVisible = !item.CourseIsHidden && item.CatalogIsHidden != true;
                if (isVisible)
                {
                    badges.Append("<span class='badge bg-success' title='Visible in the course catalogue'>Visible</span>");
                }
                else
                {
                    string reason;
                    if (item.CourseIsHidden && item.CatalogIsHidden == true)
                        reason = "course and catalogue are hidden";
                    else if (item.CourseIsHidden)
                        reason = "course is hidden";
                    else
                        reason = "catalogue is hidden";

                    badges.Append($"<span class='badge bg-danger' title='Hidden from the course catalogue ({reason})'>Hidden</span>");
                }
            }

            var groups = GetAccessGroups(item.CourseId);
            if (groups.Length > 0)
            {
                if (badges.Length > 0)
                    badges.Append(" ");

                var names = HttpUtility.HtmlEncode(string.Join(", ", groups));
                badges.Append($"<span class='badge bg-warning' title='Access restricted to: {names}'><i class='far fa-lock me-1'></i>Restricted</span>");
            }

            if (badges.Length > 0)
                html.Append($"<div class='mt-1'>{badges}</div>");

            return html.ToString();
        }

        protected string GetGroupPermissionsHtml(CourseMatch item)
        {
            var groups = GetAccessGroups(item.CourseId);

            if (groups.Length == 0)
                return "<span class='text-muted'>(public to all)</span>";

            return HttpUtility.HtmlEncode(string.Join(", ", groups));
        }

        private string[] GetAccessGroups(Guid courseId)
        {
            if (_courseAccessGroups == null)
                return Array.Empty<string>();

            var found = _courseAccessGroups.TryGetValue(courseId, out var groups);

            return found ? groups : Array.Empty<string>();
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