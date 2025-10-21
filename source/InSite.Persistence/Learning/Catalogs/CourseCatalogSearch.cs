using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Domain.Organizations;

using Shift.Common;

namespace InSite.Persistence
{
    public class CourseCatalogSearch
    {
        private List<VCatalogCourse> _datasource;
        private OrganizationList _organizations { get; set; }

        public List<CatalogMenu> Catalogs { get; set; }
        public List<CourseCatalogItem> Items { get; set; }
        public List<CourseCatalogOrganization> Organizations { get; set; }

        public int ItemCount { get; set; }
        public const int PageSize = 24;
        public int PageCount { get; set; }

        public CourseCatalogSearch(Guid organization, Guid? catalog, Guid[] groups, bool viewEntireCatalog)
        {
            GetOrganizations(organization);

            var identifiers = _organizations.Select(x => x.OrganizationIdentifier).ToList();

            using (var db = new InternalDbContext(false))
            {
                _datasource = db.VCatalogCourses.AsNoTracking()
                    .Where(x => !x.CatalogIsHidden && identifiers.Any(t => t == x.OrganizationIdentifier))
                    .ToList();

                if (catalog.HasValue && !viewEntireCatalog)
                    _datasource = _datasource.Where(x => x.CatalogIdentifier == catalog).ToList();
            }

            _datasource = ApplyPrivacySettings(_datasource.ToArray(), groups).ToList();

            Organizations = _datasource
                .GroupBy(x => x.OrganizationIdentifier)
                .Select(x => new CourseCatalogOrganization
                {
                    OrganizationIdentifier = x.Key,
                    OrganizationName = x.FirstOrDefault()?.OrganizationName,
                    OrganizationSize = x.Select(y => y.CourseIdentifier).Distinct().Count(),
                    IsSelected = true
                })
                .ToList();

            GetCategories(null);
        }

        /// <summary>
        /// Create a list of accessible courses
        /// </summary>
        /// <remarks>
        /// If a course has no permissions then we assume it is accessible.
        /// </remarks>
        /// <param name="courses">List of courses</param>
        /// <param name="groups">Groups requesting access</param>
        private VCatalogCourse[] ApplyPrivacySettings(VCatalogCourse[] courses, Guid[] groups)
        {
            var courseIdentifiers = courses.Select(x => x.CourseIdentifier).Distinct().ToArray();

            var permissions = TGroupPermissionSearch
                .Bind(x => x, x => courseIdentifiers.Any(id => id == x.ObjectIdentifier))
                .ToList();

            var result = new List<VCatalogCourse>();

            foreach (var course in courses)
            {
                var courseHasPermissions = permissions.Any(p => p.ObjectIdentifier == course.CourseIdentifier);

                if (courseHasPermissions)
                {
                    var groupsWithAccess = permissions
                        .Where(p => p.ObjectIdentifier == course.CourseIdentifier)
                        .Select(x => x.GroupIdentifier)
                        .ToArray();

                    if (!groupsWithAccess.Intersect(groups).Any())
                        continue;
                }

                if (!course.CourseIsHidden)
                    result.Add(course);
            }

            return result.ToArray();
        }

        public void Refresh(CatalogItemFilter filter)
        {
            GetCategories(filter);
        }

        private void GetCategories(CatalogItemFilter filter)
        {
            var query = _datasource.AsQueryable();

            if (filter != null && filter.Organizations.IsNotEmpty())
                query = query.Where(x => filter.Organizations.Any(y => y == x.OrganizationIdentifier));

            Catalogs = query
                .Where(x => x.CatalogName != null && x.CourseCategory != null)
                .Select(x => new { x.CatalogIdentifier, x.CatalogName, x.CourseCategory })
                .Distinct()
                .GroupBy(x => x.CatalogName)
                .OrderBy(x => x.Key)
                .Select(x => new CatalogMenu
                {
                    CatalogName = x.Key,
                    Categories = x.Select(y => new CatalogMenuItem { CategoryName = y.CourseCategory.Trim() })
                        .OrderBy(z => z.CategoryName)
                        .ToList()
                })
                .ToList();

            foreach (var catalog in Catalogs)
            {
                var courses = query.Where(x => x.CatalogName == catalog.CatalogName).ToList();

                catalog.CatalogId = courses.Select(x => x.CatalogIdentifier).First();

                catalog.CatalogSize = courses.Select(x => x.CourseIdentifier).Distinct().Count();

                foreach (var category in catalog.Categories)
                {
                    category.SetMenu(catalog);
                }
            }
        }

        private void GetOrganizations(Guid id)
        {
            _organizations = new OrganizationList();

            var organization = OrganizationSearch.Select(id);
            _organizations.Add(organization);

            if (organization.ParentOrganizationIdentifier.HasValue)
            {
                var parent = OrganizationSearch.Select(organization.ParentOrganizationIdentifier.Value);
                if (parent != null)
                    _organizations.Add(parent);
            }
        }

        public List<CourseCatalogItem> ApplyFilter(string keyword, CatalogItemFilter filter, string sort, int page)
        {
            var query = _datasource.AsQueryable();

            if (keyword.HasValue())
                query = query.Where(x => x.CourseName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                      || (x.CourseCategory != null &&
                                          x.CourseCategory.Contains(keyword, StringComparison.OrdinalIgnoreCase)));

            // At least one organization MUST be specified!
            if (filter.Organizations.IsEmpty())
                query = query.Where(x => false);
            else
                query = query.Where(x => filter.Organizations.Any(y => y == x.OrganizationIdentifier));

            if (filter.CatalogSlugs.IsNotEmpty())
                query = query.Where(x => filter.CatalogSlugs.Any(y => GetCatalogName(y) == x.CatalogName));

            if (filter.CategorySlugs.IsNotEmpty())
                query = query.Where(x => filter.CategorySlugs.Any(y => GetCategoryName(y) == x.CourseCategory));

            if (sort == "newest")
                query = query.OrderByDescending(x => x.CourseModified).ThenByDescending(x => x.CourseCreated).ThenBy(x => x.CourseName);
            else
                query = query.OrderBy(x => x.CourseName);

            var source = query.ToList();

            var list = new List<CourseCatalogItem>();

            foreach (var x in source)
            {
                if (list.Any(i => i.ItemIdentifier == x.CourseIdentifier))
                    continue;

                var item = new CourseCatalogItem(x.CourseName)
                {
                    Authored = x.CourseCreated,
                    ItemIdentifier = x.CourseIdentifier,
                    ItemType = x.CatalogName,
                    ItemPopularity = 0,
                    ItemSubcategories = GetCategoriesHtml(x.CourseIdentifier),
                    Posted = x.CourseModified,
                    ThumbnailImageUrl = x.CourseImage,
                    CourseFlagColor = x.CourseFlagColor?.ToLower(),
                    CourseFlagText = x.CourseFlagText,

                };
                list.Add(item);
            }

            ItemCount = list.Count;

            PageCount = (int)Math.Ceiling((double)list.Count / PageSize);

            Items = list.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            return Items;
        }

        private string GetCategoryName(string slug)
        {
            foreach (var catalog in Catalogs)
            {
                var category = catalog.Categories.SingleOrDefault(x => x.CategorySlug == slug)?.CategoryName;
                if (category != null)
                    return category;
            }
            return null;
        }

        private string GetCatalogName(string slug)
        {
            return Catalogs.SingleOrDefault(x => x.CatalogSlug == slug)?.CatalogName;
        }

        public PaginationItem[] CreatePages(int current, int count)
        {
            var list = new List<PaginationItem>();
            for (var i = 0; i < count; i++)
                list.Add(new PaginationItem { PageNumber = i + 1, IsCurrent = current == i + 1 });
            return list.ToArray();
        }

        public CatalogMenu FindCatalog(Guid? catalogId, string category, bool skipCategory)
        {
            if (catalogId == null)
                return null;

            if (category.IsEmpty() && !skipCategory)
                return Catalogs.FirstOrDefault(x => x.CatalogId == catalogId
                    && x.Categories.Any(y => StringHelper.Equals(y.CategoryName, category)));

            return Catalogs.FirstOrDefault(x => x.CatalogId == catalogId);
        }

        private string GetCategoriesHtml(Guid course)
        {
            var items = _datasource
                .Where(x => x.CourseIdentifier == course)
                .OrderBy(x => x.CourseCategory)
                .ToList();

            if (items.Count == 0)
                return null;

            var html = new StringBuilder();
            html.Append("<div class='fs-xs mb-1'>");
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var url = $"/ui/portal/learning/catalog?catalog={item.CatalogName}&category={HttpUtility.UrlEncode(item.CourseCategory)}";
                var link = $"<a class='meta-link' href='{url}'>{item.CourseCategory}</a>";
                html.Append(link);
                if (i < items.Count - 1)
                    html.Append("; ");
            }
            html.Append("</div>");
            return html.ToString();
        }

        public Guid? GetCatalogId(string catalogName)
        {
            return _datasource.Where(x => x.CatalogName == catalogName).FirstOrDefault()?.CatalogIdentifier;
        }
    }
}
