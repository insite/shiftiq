using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TCollectionItemSearch
    {
        public static int GetNextSequence(Guid collectionId, Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var result = db.TCollectionItems.Where(x => x.CollectionIdentifier == collectionId && x.OrganizationIdentifier == organizationId).Max(x => (int?)x.ItemSequence);
                return (result ?? 0) + 1;
            }
        }

        public static TCollectionItem Select(Guid id)
        {
            using (var db = new InternalDbContext())
                return db
                    .TCollectionItems
                    .Include(x => x.Courses)
                    .Include(x => x.Programs)
                    .FirstOrDefault(x => x.ItemIdentifier == id);
        }

        public static List<TCollectionItem> Select(TCollectionItemFilter filter, params Expression<Func<TCollectionItem, object>>[] includes)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.TCollectionItems.AsQueryable().AsNoTracking()
                    .ApplyIncludes(includes)
                    .Filter(filter)
                    .OrderBy(filter.OrderBy.IfNullOrEmpty(nameof(TCollectionItem.ItemNumber)))
                    .ApplyPaging(filter.Paging)
                    .ToList();
            }
        }

        public static bool Exists(TCollectionItemFilter filter)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.TCollectionItems.AsQueryable().Filter(filter).Any();
            }
        }

        public static int Count(TCollectionItemFilter filter)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.TCollectionItems.AsQueryable().Filter(filter).Count();
            }
        }

        public static int? GetReferenceCount(TCollectionItem item)
        {
            if (item != null)
            {
                var collection = TCollectionSearch.Select(item.CollectionIdentifier);
                if (collection != null)
                    return GetReferenceCount(collection, item);
            }

            return null;
        }

        public static int? GetReferenceCount(TCollection collection, TCollectionItem item)
        {
            if (!collection.CollectionReferences.HasValue())
                return null;

            var references = TCollectionSearch.ParseCollectionReferences(collection.CollectionReferences);

            return GetReferenceCount(item, references);
        }

        public static int? GetReferenceCount(TCollectionItem item, IEnumerable<TCollectionSearch.ReferenceInfo> references)
        {
            var referenceCount = 0;

            using (var db = new InternalDbContext())
            {
                foreach (var reference in references)
                {
                    if (reference == null || !reference.IsValid)
                        continue;

                    var parameters = new List<SqlParameter>();
                    var query = $"SELECT COUNT(*) FROM [{reference.SchemaName}].[{reference.TableName}] WHERE [{reference.ColumnName}] = ";

                    if (reference.IsCharType)
                    {
                        parameters.Add(new SqlParameter("@ItemName", item.ItemName));
                        query += "@ItemName";
                    }
                    else if (reference.IsGuidType)
                    {
                        parameters.Add(new SqlParameter("@ItemID", item.ItemIdentifier));
                        query += "@ItemID";
                    }

                    var organization = reference.HasOrganizationColumn ? OrganizationSearch.Select(item.OrganizationIdentifier.Value) : null;
                    if (organization != null)
                    {
                        parameters.Add(new SqlParameter("@OrganizationIdentifier", organization.Identifier));
                        query += " AND [OrganizationIdentifier] = @OrganizationIdentifier";
                    }

                    try
                    {
                        referenceCount += db.Database.SqlQuery<int>(query, parameters.ToArray()).Single();
                    }
                    catch (EntityCommandExecutionException efex)
                    {
                        if (efex.InnerException is SqlException sqlex)
                            throw ApplicationError.Create(
                                "An error occurred while processing the reference '{0}.{1}.{2}': {3}",
                                reference.SchemaName, reference.TableName, reference.ColumnName, sqlex.Message);

                        throw efex;
                    }
                }
            }

            return referenceCount;
        }
    }
}