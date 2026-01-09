using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class StandardSearch
    {
        #region Classes

        private class StandardReadHelper : ReadHelper<Standard>
        {
            public static readonly StandardReadHelper Instance = new StandardReadHelper();

            public T[] Bind<T>(
                InternalDbContext context,
                Expression<Func<Standard, T>> binder,
                Expression<Func<Standard, bool>> filter,
                string modelSort = null,
                string entitySort = null)
            {
                var query = context.Standards.AsQueryable().AsNoTracking();
                var modelQuery = BuildQuery(query, binder, filter, null, modelSort, entitySort, false);

                return modelQuery.ToArray();
            }

            public T[] Bind<T>(
                Expression<Func<Standard, T>> binder,
                StandardFilter filter)
            {
                using (var context = new InternalDbContext())
                {
                    var query = context.Standards.AsQueryable().AsNoTracking();
                    Func<IQueryable<Standard>, IQueryable<T>> bind = q => q.Select(binder);
                    Func<IQueryable<Standard>, IQueryable<Standard>> filterQuery = q => StandardFilterHelper.ApplyFilter(q, filter, context);

                    var modelQuery = BuildQuery(query, bind, filterQuery, q => q, filter.Paging, filter.OrderBy, null, false);

                    return modelQuery.ToArray();
                }
            }

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Standard>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.Standards.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public class DocumentCountInfo
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public class FinderEntity
        {
            public Guid StandardIdentifier { get; set; }
            public int Number { get; set; }
            public string Type { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
        }

        #endregion

        #region Constants

        public const int NewAssessmentPageId = int.MinValue;

        #endregion

        #region Select (entity)

        public static Standard Select(Guid key, params Expression<Func<Standard, object>>[] includes)
        {
            return StandardReadHelper.Instance.SelectFirst(x => x.StandardIdentifier == key, includes);
        }
        public static Standard SelectOccupation(Expression<Func<Standard, bool>> filter)
        {
            return StandardReadHelper.Instance.SelectFirst(filter, null);
        }

        public static Standard SelectFirst(Expression<Func<Standard, bool>> filter,
            params Expression<Func<Standard, object>>[] includes)
        {
            return StandardReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static Standard Select(Guid organizationId, int number, params Expression<Func<Standard, object>>[] includes) =>
            StandardReadHelper.Instance.SelectFirst(x => x.OrganizationIdentifier == organizationId && x.AssetNumber == number, includes);

        public static IReadOnlyList<Standard> Select(
            Expression<Func<Standard, bool>> filter,
            params Expression<Func<Standard, object>>[] includes)
        {
            return StandardReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<Standard> Select(
            Expression<Func<Standard, bool>> filter,
            string sortExpression,
            params Expression<Func<Standard, object>>[] includes)
        {
            return StandardReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        #endregion

        #region Select (model)

        public static StandardModel SelectModel(Guid id) =>
            SelectModelInternal(x => x.StandardIdentifier == id);

        public static StandardModel SelectModel(Guid organizationId, int number) =>
            SelectModelInternal(x => x.OrganizationIdentifier == organizationId && x.AssetNumber == number);

        private static StandardModel SelectModelInternal(Expression<Func<Standard, bool>> filter)
        {
            using (var context = new InternalDbContext())
            {
                var root = context.Standards
                    .Include(x => x.Children)
                    .Include(x => x.ParentContainments)
                    .SingleOrDefault(filter);

                var model = StandardMapper.Map(root);

                SelectModelHelper(root, model, context);

                return model;
            }
        }

        #endregion

        #region Select (scalar)

        public static Guid? GetStandardRootKey(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<Guid?>(
                    "SELECT standards.GetStandardRootKey(@StandardIdentifier)", new SqlParameter("@StandardIdentifier", id)).FirstOrDefault();
            }
        }

        public static int CalculateExpectedStandardDepth(Guid parentId, Guid? updateId)
        {
            using (var db = new InternalDbContext())
                return CalculateExpectedStandardDepth(db, parentId, updateId);
        }

        internal static int CalculateExpectedStandardDepth(InternalDbContext db, Guid parentId, Guid? updateId)
        {
            return db.Database
                .SqlQuery<int>(
                    "SELECT standards.CalculateExpectedStandardDepth(@ParentIdentifier,@UpdateIdentifier)",
                    new SqlParameter("@ParentIdentifier", (object)parentId ?? DBNull.Value),
                    new SqlParameter("@UpdateIdentifier", (object)updateId ?? DBNull.Value))
                .FirstOrDefault();
        }

        public static int SelectNextSequence(Guid parentId)
        {
            using (var db = new InternalDbContext())
            {
                var max = db.Standards
                    .Where(x => x.ParentStandardIdentifier == parentId)
                    .Max(x => (int?)x.Sequence);

                return (max ?? 0) + 1;
            }
        }

        #endregion

        #region Select (statistics)

        public static ICollection<CountModel> Count(OrganizationState organization)
        {
            const string sql = @"
select
    Standard.StandardType         as Type
  , count(*)                      as Count
  , StandardType.StandardTypeIcon as Icon
from
    standards.Standard
    inner join
    standards.StandardType on StandardType.StandardTypeName = Standard.StandardType
    inner join
    accounts.QOrganization AS Organization on Organization.OrganizationIdentifier = Standard.OrganizationIdentifier
where
    Organization.OrganizationIdentifier = @OrganizationIdentifier
group by
    Standard.StandardType
  , StandardType.StandardTypeIcon
order by
    Standard.StandardType;
";

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<CountModel>(
                        sql,
                        new SqlParameter("@OrganizationIdentifier", organization.OrganizationIdentifier)
                    )
                    .ToArray();
            }
        }

        public static string GetPathCode(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var standard = db.StandardHierarchies.FirstOrDefault(x => x.StandardIdentifier == id);
                return standard?.PathCode;
            }
        }

        #endregion

        #region Select (filter)

        public static List<FinderEntity> SelectFinderEntity(StandardFilter filter, string language)
        {
            using (var db = new InternalDbContext())
            {
                var query = StandardFilterHelper.ApplyFilter(db.Standards.AsQueryable().AsNoTracking(), filter, db)
                    .GroupJoin(db.TContents.Where(x => x.ContentLanguage == language && x.ContentLabel == "Title"),
                        a => a.StandardIdentifier,
                        b => b.ContainerIdentifier,
                        (a, b) => new { Standard = a, Contents = b.DefaultIfEmpty() }
                    )
                    .SelectMany(x => x.Contents.Select(y => new FinderEntity
                    {
                        StandardIdentifier = x.Standard.StandardIdentifier,
                        Number = x.Standard.AssetNumber,
                        Type = x.Standard.StandardType,
                        Code = x.Standard.Code,
                        Title = y.ContentText ?? x.Standard.ContentTitle ?? "Unknown",
                    }));

                if (!string.IsNullOrEmpty(filter.OrderBy))
                    query = query.OrderBy(filter.OrderBy);

                return query
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public static int Count(StandardFilter filter)
        {
            using (var db = new InternalDbContext())
                return StandardFilterHelper.ApplyFilter(db.Standards.AsQueryable().AsNoTracking(), filter, db).Count();
        }

        public static SearchResultList SearchStandardsRecent(string sort, Paging paging, Guid organizationId)
        {
            if (string.IsNullOrEmpty(sort))
                sort = "Modified";

            using (var db = new InternalDbContext())
            {
                var items = db.Standards
                    .Where(x => x.OrganizationIdentifier == organizationId && x.StandardType == "Standard")
                    .OrderBy(sort)
                    .ApplyPaging(paging)
                    .Select(asset => new SearchResultItem(
                        asset.StandardIdentifier,
                        asset.ContentName,
                        asset.Icon,
                        asset.ModifiedBy,
                        asset.AssetNumber,
                        asset.StandardType,
                        asset.OrganizationIdentifier,
                        asset.Organization.CompanyName,
                        asset.ContentTitle,
                        asset.Modified
                    )
                );

                foreach (var item in items)
                    if (item.Icon == null)
                        item.Icon = GetStandardTypeIcon(item.SubType);

                return items.ToSearchResult();
            }
        }

        public static string GetStandardTypeIcon(string type)
        {
            switch (type)
            {
                case "Area": return "far fa-cube";
                case "Collection": return "far fa-box-open";
                case "Competency": return "far fa-cube";
                case "Document": return "far fa-file-alt";
                case "Framework": return "far fa-sitemap";
                case "Profile": return "far fa-id-badge";
            }

            return "far fa-question-square";
        }

        #endregion

        #region Select (NOSFilter)

        public static int Count(StandardDocumentFilter filter)
        {
            using (var db = new InternalDbContext())
                return StandardDocumentFilterHelper.ApplyFilter(db, filter).Count();
        }

        public static SearchResultList SelectSearchResults(StandardDocumentFilter filter, string language = "en")
        {
            const string sort = "ContentTitle";

            using (var db = new InternalDbContext())
            {
                return StandardDocumentFilterHelper.ApplyFilter(db, filter)
                    .Select(x => new
                    {
                        x.StandardIdentifier,
                        x.DocumentType,
                        x.ContentTitle,
                        TranslatedTitle = CoreFunctions.GetContentText(x.StandardIdentifier, ContentLabel.Title, language) ?? x.ContentTitle,
                        x.IsTemplate,
                        x.StandardPrivacyScope,
                        x.LevelType,
                        x.DatePosted,
                        x.CreatedBy
                    })
                    .OrderBy(sort)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static Standard GetDocument(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                return db.Standards.AsQueryable().FirstOrDefault(x => x.StandardIdentifier == id);
            }
        }

        public static List<Standard> SelecStandardDocuments(StandardDocumentFilter filter, int rowNumberFrom, int rowNumberThru, string language = "en")
        {
            using (var db = new InternalDbContext())
            {
                return StandardDocumentFilterHelper.ApplyFilter(db, filter).ToList<Standard>();
            }
        }

        public static List<Standard> SelecStandardDocuments(StandardDocumentFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return StandardDocumentFilterHelper.ApplyFilter(db, filter).ToList<Standard>();
            }
        }

        public static int count(StandardDocumentFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return StandardDocumentFilterHelper.ApplyFilter(db, filter).ToList<Standard>().Count();
            }
        }

        public static DocumentCountInfo[] CountDocumentTypes(StandardDocumentFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return StandardDocumentFilterHelper
                    .ApplyFilter(db, filter)
                    .GroupBy(x => x.DocumentType)
                    .Select(x => new DocumentCountInfo
                    {
                        Name = x.Key,
                        Count = x.Count()
                    })
                    .ToArray();
            }
        }

        #endregion

        #region Select (OccupationFilter)

        public static int Count(OccupationFilter filter)
        {
            using (var db = new InternalDbContext())
                return OccupationFilterHelper.ApplyFilter(db.Occupations.AsQueryable().AsNoTracking(), filter).Count();
        }

        public static SearchResultList SelectSearchResults(OccupationFilter filter)
        {
            var sort = "JobTitle";

            using (var db = new InternalDbContext())
            {
                var assets = OccupationFilterHelper.ApplyFilter(db.Occupations.AsQueryable().AsNoTracking(), filter);

                return assets
                    .OrderBy(sort)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        #endregion

        #region Select (Other)

        public static List<Guid> GetUserStandardFrameworks(Guid organizationId, Guid userId)
        {
            const string query = "exec standards.GetUserStandardFrameworks @OrganizationID, @UserID";

            var parameters = new[]
            {
                new SqlParameter("@OrganizationID", organizationId),
                new SqlParameter("@UserID", userId),
            };

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Guid>(query, parameters).ToList();
        }

        public static StandardHierarchy SelectStandardHierarchy(Guid standardIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.StandardHierarchies
                    .Where(x => x.StandardIdentifier == standardIdentifier)
                    .FirstOrDefault();
            }
        }

        public static List<StandardHierarchy> SelectStandardHierarchyList(Guid rootIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.StandardHierarchies
                    .Where(x => x.RootStandardIdentifier == rootIdentifier)
                    .ToList();
            }
        }

        public static IReadOnlyCollection<string> SelectStandardTypes(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                return db.Standards
                    .Where(x => x.OrganizationIdentifier == organizationId)
                    .Select(x => x.StandardType)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public static DocumentCountInfo[] CountDocumentTypes(StandardFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return StandardFilterHelper
                    .ApplyFilter(db.Standards.AsQueryable().AsNoTracking(), filter, db)
                    .Where(x => x.StandardType == StandardType.Document)
                    .GroupBy(x => x.DocumentType)
                    .Select(x => new DocumentCountInfo
                    {
                        Name = x.Key,
                        Count = x.Count()
                    })
                    .ToArray();
            }
        }

        public static List<Standard> Select(StandardFilter filter)
        {
            var sortExpression = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "ContentTitle";

            using (var db = new InternalDbContext())
            {
                return StandardFilterHelper
                    .ApplyFilter(db.Standards.AsQueryable(), filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        #endregion

        #region Binding

        public static T[] Bind<T>(
            Expression<Func<Standard, T>> binder,
            Expression<Func<Standard, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return StandardReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T[] Bind<T>(
            Expression<Func<Standard, T>> binder,
            StandardFilter filter)
        {
            return StandardReadHelper.Instance.Bind(binder, filter);
        }

        public static T BindFirst<T>(
            Expression<Func<Standard, T>> binder,
            Expression<Func<Standard, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return StandardReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static T Bind<T>(
            Guid assetId,
            Expression<Func<Standard, T>> binder) => StandardReadHelper.Instance.BindFirst(binder, (Expression<Func<Standard, bool>>)(x => x.StandardIdentifier == assetId));

        public static T BindByNumber<T>(
            Guid organizationId,
            int number,
            Expression<Func<Standard, T>> binder)
        {
            return StandardReadHelper.Instance.BindFirst(binder, x => x.OrganizationIdentifier == organizationId && x.AssetNumber == number);
        }

        public static int Count(Expression<Func<Standard, bool>> filter)
        {
            return StandardReadHelper.Instance.Count(filter);
        }

        public static bool Exists(Guid id) =>
            StandardReadHelper.Instance.Exists((Expression<Func<Standard, bool>>)(x => x.StandardIdentifier == id));

        public static bool Exists(Expression<Func<Standard, bool>> filter) =>
            StandardReadHelper.Instance.Exists(filter);

        #endregion

        #region Helpers

        private static void SelectModelHelper(Standard asset, StandardModel model, InternalDbContext context)
        {
            foreach (var child in asset.Children.OrderBy(x => x.Sequence))
            {
                var branch = StandardMapper.Map(child);
                branch.Parent = model;

                model.Children.Add(branch);
                branch.Parents.Add(model);

                SelectModelHelper(child, branch, context);
            }

            foreach (var edge in asset.ParentContainments.OrderBy(x => x.Child.Sequence))
            {
                var from = StandardMapper.Map(edge.Parent);
                var to = StandardMapper.Map(edge.Child);

                to.IsShared = true;
                to.Parent = model;

                model.Children.Add(to);
                to.Parents.Add(model);

                SelectModelHelper(edge.Child, to, context);
            }
        }

        public static string[] GetAllTypeNames(Guid organization)
        {
            return GetAllTypes(organization, x => x.ItemName);
        }

        public static TCollectionItem[] GetAllTypeItems(Guid organization)
        {
            return GetAllTypes(organization, x => x);
        }

        private static T[] GetAllTypes<T>(Guid organization, Expression<Func<TCollectionItem, T>> bind)
        {
            var organizations = GetOrganizationsForFilter(organization);

            using (var db = new InternalDbContext(false, true))
            {
                return db.TCollectionItems
                    .Where(x => x.Collection.CollectionName == CollectionName.Skills_Standards_Classification_Level
                             && organizations.Any(o => o == x.OrganizationIdentifier))
                    .OrderBy(x => x.ItemName)
                    .Select(bind)
                    .ToArray();
            }
        }

        private static IEnumerable<Guid> GetOrganizationsForFilter(Guid child)
        {
            var organizations = new List<Guid> { child };

            var parent = OrganizationSearch.Select(child)?.ParentOrganizationIdentifier;
            if (parent == null || parent == Guid.Empty)
                return organizations;

            organizations.Add(parent.Value);

            var grandparent = OrganizationSearch.Select(parent.Value)?.ParentOrganizationIdentifier;
            if (grandparent == null || grandparent == Guid.Empty)
                return organizations;

            organizations.Add(grandparent.Value);

            return organizations;
        }

        #endregion
    }
}
