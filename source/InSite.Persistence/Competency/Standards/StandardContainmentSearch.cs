using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class StandardContainmentSearch
    {
        #region Classes

        private class ReadHelper : ReadHelper<StandardContainment>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<StandardContainment>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.StandardContainments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public class ContainmentTreeInfo
        {
            public Guid ParentStandardIdentifier { get; set; }
            public Guid ChildStandardIdentifier { get; set; }
            public int ChildSequence { get; set; }
            public int Sequence { get; set; }
        }

        public class UpstreamRelationshipInfo
        {
            public Guid ParentStandardIdentifier { get; set; }
            public Guid ChildStandardIdentifier { get; set; }
            public int Distance { get; set; }
        }

        #endregion

        #region Bind

        public static StandardContainment SelectFirst(
            Expression<Func<StandardContainment, bool>> filter,
            params Expression<Func<StandardContainment, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<StandardContainment> Select(
            Expression<Func<StandardContainment, bool>> filter,
            params Expression<Func<StandardContainment, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, includes);

        public static IReadOnlyList<StandardContainment> Select(
            Expression<Func<StandardContainment, bool>> filter,
            string sortExpression,
            params Expression<Func<StandardContainment, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T BindFirst<T>(
            Expression<Func<StandardContainment, T>> binder,
            Expression<Func<StandardContainment, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<StandardContainment, T>> binder,
            Expression<Func<StandardContainment, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<StandardContainment, bool>> filter) =>
            ReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<StandardContainment, bool>> filter) =>
            ReadHelper.Instance.Exists(filter);

        #endregion

        #region Select (model)

        public static ContainmentTreeInfo[] SelectTree(IEnumerable<Guid> standardKeys) =>
            SelectTree<ContainmentTreeInfo>(standardKeys);

        public static T[] SelectTree<T>(IEnumerable<Guid> standardKeys)
        {
            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<T>("EXEC standards.SelectStandardContainmentTree @StandardFilter", SqlParameterHelper.IdentifierList("@StandardFilter", standardKeys)).ToArray();
        }

        public static UpstreamRelationshipInfo[] SelectUpstreamRelationships(IEnumerable<Guid> standardKeys) =>
            SelectUpstreamRelationships<UpstreamRelationshipInfo>(standardKeys);

        public static T[] SelectUpstreamRelationships<T>(IEnumerable<Guid> standardKeys)
        {
            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<T>("EXEC standards.SelectUpstreamStandards @AssetFilter", SqlParameterHelper.IdentifierList("@AssetFilter", standardKeys)).ToArray();
        }

        public static T[] SelectOrganizationEdges<T>(Guid organization)
        {
            const string query = @"
SELECT
    StandardContainment.ParentStandardIdentifier
   ,StandardContainment.ChildStandardIdentifier
   ,ToAsset.AssetNumber AS ToAssetNumber
   ,StandardContainment.ChildSequence
   ,0 AS [Sequence]
   ,CAST(1 AS BIT) AS IsRelationship
FROM
    standards.StandardContainment
    INNER JOIN standards.[Standard] AS ToAsset ON ToAsset.StandardIdentifier = StandardContainment.ChildStandardIdentifier
WHERE
    ToAsset.OrganizationIdentifier = @OrganizationIdentifier

UNION

SELECT DISTINCT
    ParentStandardIdentifier
   ,StandardIdentifier
   ,AssetNumber
   ,0
   ,[Standard].[Sequence]
   ,CAST(0 AS BIT) AS IsRelationship
FROM
    standards.[Standard]
WHERE
    ParentStandardIdentifier IS NOT NULL
    AND OrganizationIdentifier = @OrganizationIdentifier

ORDER BY
    ChildSequence
   ,[Sequence]
   ,ToAssetNumber;
";
            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<T>(query.ToString(), new[] { new SqlParameter("@OrganizationIdentifier", organization) }).ToArray();
        }

        #endregion

        #region Select

        public static int SelectMaxSequence(Guid parentStandardIdentifier)
        {
            using (var context = new InternalDbContext())
            {
                return context.StandardContainments
                    .Where(x => x.ParentStandardIdentifier == parentStandardIdentifier)
                    .Max(x => (int?)x.ChildSequence) ?? 0;
            }
        }

        public static StandardContainmentSummary[] SelectByChildStandardIdentifier(Guid childStandardIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.StandardContainmentSummaries.AsNoTracking()
                    .Where(x => x.ChildStandardIdentifier == childStandardIdentifier)
                    .OrderBy(x => x.ParentAssetNumber)
                    .ToArray();
            }
        }

        public static StandardContainmentSummary[] SelectByParentStandardIdentifier(Guid childParentStandardIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.StandardContainmentSummaries.AsNoTracking()
                        .Where(x => x.ParentStandardIdentifier == childParentStandardIdentifier)
                        .OrderBy(x => x.ParentSequence).ThenBy(x => x.ChildSequence)
                        .ToArray();
            }
        }

        public static IReadOnlyList<StandardContainment> SelectCompetencyContainments(Guid parentKey, params Expression<Func<StandardContainment, object>>[] includes)
        {
            return Select(x => x.ParentStandardIdentifier == parentKey && x.Child.StandardType == StandardType.Competency, "ChildSequence", includes);
        }

        public static IReadOnlyList<T> BindCompetencyContainments<T>(Guid parentKey, Expression<Func<StandardContainment, T>> binder)
        {
            return Bind(
                binder,
                x => x.ParentStandardIdentifier == parentKey && x.Child.StandardType == StandardType.Competency, null, "ChildSequence");
        }

        #endregion
    }
}