using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Shift.Common;

namespace InSite.Persistence
{
    public class VCatalogProgramSearch
    {
        public enum SortBy { Title, Newest }

        public const int PageSize = 24;

        private List<VCatalogProgram> _datasource;
        private List<CatalogMenu> _catalogs;
        private List<CourseCatalogItem> _items;

        public int ItemCount { get; private set; }
        public int PageCount { get; private set; }

        public List<CatalogMenu> Catalogs => _catalogs;

        private VCatalogProgramSearch(List<VCatalogProgram> datasource, List<CatalogMenu> catalogs)
        {
            _datasource = datasource;
            _catalogs = catalogs;
        }

        public static VCatalogProgramSearch Create(Guid organizationId, string catalog, Guid[] groupIds, bool viewEntireCatalog)
        {
            var datasource = GetCatalogPrograms(organizationId, catalog, groupIds, viewEntireCatalog);
            var catalogs = GetCatalogs(datasource);

            return new VCatalogProgramSearch(datasource, catalogs);
        }

        public PaginationItem[] CreatePages(int current, int count)
        {
            var list = new List<PaginationItem>();
            for (var i = 0; i < count; i++)
                list.Add(new PaginationItem { PageNumber = i + 1, IsCurrent = current == i + 1 });
            return list.ToArray();
        }

        public CatalogMenu FindCatalog(string catalog, string category, bool skipCategory)
        {
            if (string.IsNullOrEmpty(catalog))
                return null;

            if (category.IsEmpty() && !skipCategory)
            {
                return _catalogs.FirstOrDefault(x => x.CatalogName == catalog
                    && x.Categories.Any(y => StringHelper.Equals(y.CategoryName, category)));
            }

            return _catalogs.FirstOrDefault(x => x.CatalogName == catalog);
        }

        public List<CourseCatalogItem> SearchPrograms(string keyword, CatalogItemFilter filter, SortBy sort, int page)
        {
            if (filter.Organizations.IsEmpty())
                return new List<CourseCatalogItem>();

            var query = ApplyFilter(keyword, filter, sort);

            var listQuery = query
                .GroupBy(x => x.ProgramIdentifier)
                .Select(x => x.First());

            if (sort == SortBy.Newest)
                listQuery = listQuery.OrderByDescending(x => x.ProgramModified).ThenBy(x => x.ProgramName).ThenBy(x => x.ProgramIdentifier);
            else
                listQuery = listQuery.OrderBy(x => x.ProgramName).ThenBy(x => x.ProgramIdentifier);

            var list = listQuery.ToList();

            _items = list
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new CourseCatalogItem(x.ProgramName)
                {
                    Authored = x.ProgramCreated,
                    ItemIdentifier = x.ProgramIdentifier,
                    ItemType = x.CatalogName,
                    ItemSubcategories = GetCategoriesHtml(x.ProgramIdentifier),
                    ItemDescription = x.ProgramDescription,
                    Posted = x.ProgramModified,
                    ThumbnailImageUrl = x.ProgramImage,
                })
                .ToList();

            ItemCount = list.Count;
            PageCount = (int)Math.Ceiling((double)ItemCount / PageSize);

            return _items;
        }

        private IEnumerable<VCatalogProgram> ApplyFilter(string keyword, CatalogItemFilter filter, SortBy sort)
        {
            var query = _datasource.Where(x => filter.Organizations.Any(y => y == x.OrganizationIdentifier));

            if (keyword.HasValue())
            {
                query = query.Where(x => x.ProgramName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                      || (x.ProgramCategory != null &&
                                          x.ProgramCategory.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                      )
                                  );
            }

            if (filter.CatalogSlugs.IsNotEmpty())
            {
                var slugs = filter.CatalogSlugs.Select(y => GetCatalogName(y)).ToList();
                query = query.Where(x => slugs.Contains(x.CatalogName));
            }

            if (filter.CategorySlugs.IsNotEmpty())
            {
                var slugs = filter.CategorySlugs.Select(y => GetCategoryName(y)).ToList();
                query = query.Where(x => slugs.Contains(x.ProgramCategory));
            }

            return query;
        }

        private string GetCategoryName(string slug)
        {
            foreach (var catalog in _catalogs)
            {
                var category = catalog.Categories.SingleOrDefault(x => x.CategorySlug == slug)?.CategoryName;
                if (category != null)
                    return category;
            }
            return null;
        }

        private string GetCatalogName(string slug)
        {
            return _catalogs.SingleOrDefault(x => x.CatalogSlug == slug)?.CatalogName;
        }

        private string GetCategoriesHtml(Guid programId)
        {
            var items = _datasource
                .Where(x => x.ProgramIdentifier == programId)
                .OrderBy(x => x.ProgramCategory)
                .ToList();

            if (items.Count == 0)
                return null;

            var html = new StringBuilder();
            html.Append("<div class='fs-xs mb-1'>");
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var url = $"/ui/portal/learning/programs/catalog?catalog={item.CatalogName}&category={HttpUtility.UrlEncode(item.ProgramCategory)}";
                var link = $"<a class='meta-link' href='{url}'>{item.ProgramCategory}</a>";
                html.Append(link);
                if (i < items.Count - 1)
                    html.Append("; ");
            }
            html.Append("</div>");
            return html.ToString();
        }

        private static List<VCatalogProgram> GetCatalogPrograms(Guid organizationId, string catalog, Guid[] groupIds, bool viewEntireCatalog)
        {
            var organizationIds = GetOrganizationIds(organizationId);

            using (var db = new InternalDbContext(false))
            {
                var query = db.VCatalogPrograms
                    .Where(x => !x.CatalogIsHidden && organizationIds.Any(t => t == x.OrganizationIdentifier));

                if (!string.IsNullOrEmpty(catalog) && !viewEntireCatalog)
                    query = query.Where(x => x.CatalogName == catalog);

                var programsAndGroups = query
                    .GroupJoin(db.TGroupPermissions,
                        program => program.ProgramIdentifier,
                        permission => permission.ObjectIdentifier,
                        (program, permissions) => new
                        {
                            Program = program,
                            GroupIds = permissions.Select(y => y.GroupIdentifier).ToList()
                        }
                    )
                    .ToList();

                return programsAndGroups
                    .Where(x => x.GroupIds.Count == 0 || x.GroupIds.Intersect(groupIds).Any())
                    .Select(x => x.Program)
                    .ToList();
            }
        }

        private static List<Guid> GetOrganizationIds(Guid childOrganizationId)
        {
            var result = new List<Guid>();
            result.Add(childOrganizationId);

            var organization = OrganizationSearch.Select(childOrganizationId);

            if (organization.ParentOrganizationIdentifier.HasValue)
                result.Add(organization.ParentOrganizationIdentifier.Value);

            return result;
        }

        private static List<CatalogMenu> GetCatalogs(List<VCatalogProgram> datasource)
        {
            var catalogs = datasource
                .Where(x => x.CatalogName != null && x.ProgramCategory != null)
                .Select(x => new { x.CatalogIdentifier, x.CatalogName, x.ProgramCategory })
                .Distinct()
                .GroupBy(x => x.CatalogName)
                .OrderBy(x => x.Key)
                .Select(x => new CatalogMenu
                {
                    CatalogName = x.Key,
                    Categories = x.Select(y => new CatalogMenuItem { CategoryName = y.ProgramCategory.Trim() })
                        .OrderBy(z => z.CategoryName)
                        .ToList()
                })
                .ToList();

            foreach (var catalog in catalogs)
            {
                var programs = datasource.Where(x => x.CatalogName == catalog.CatalogName).ToList();

                catalog.CatalogId = programs.Select(x => x.CatalogIdentifier).First();
                catalog.CatalogSize = programs.Select(x => x.ProgramIdentifier).Distinct().Count();

                foreach (var category in catalog.Categories)
                {
                    category.SetMenu(catalog);
                }
            }

            return catalogs;
        }
    }
}
