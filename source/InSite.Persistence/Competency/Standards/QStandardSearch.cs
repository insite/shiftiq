using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Standards.Read;
using InSite.Persistence.Foundation;

using Shift.Common;

namespace InSite.Persistence
{
    public class QStandardSearch : IStandardSearch
    {
        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false, false);
        }

        public bool Exists(Guid organizationId, int assetNumber)
        {
            using (var db = CreateContext())
                return db.QStandards.Any(x => x.OrganizationIdentifier == organizationId && x.AssetNumber == assetNumber);
        }

        public QStandard GetStandard(Guid standardId)
        {
            using (var db = CreateContext())
                return db.QStandards.FirstOrDefault(x => x.StandardIdentifier == standardId);
        }

        public int CountStandards(QStandardFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        public List<QStandard> GetStandards(QStandardFilter filter, params Expression<Func<QStandard, object>>[] includes)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).ApplyIncludes(includes).ToList();
        }

        private static IQueryable<QStandard> CreateQuery(QStandardFilter filter, InternalDbContext db)
        {
            var query = db.QStandards.AsQueryable();

            if (filter == null)
                return query;

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if(filter.StandardIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.StandardIdentifiers.Contains(x.StandardIdentifier));

            if (filter.AssetNumbers.IsNotEmpty())
                query = query.Where(x => filter.AssetNumbers.Contains(x.AssetNumber));

            return query;
        }

        public QStandardCategory GetStandardCategory(Guid standardId, Guid categoryId)
        {
            using (var db = CreateContext())
                return db.QStandardCategories.FirstOrDefault(x => x.StandardIdentifier == standardId && x.CategoryIdentifier == categoryId);
        }

        public QStandardConnection GetStandardConnection(Guid fromStandardId, Guid toStandardId)
        {
            using (var db = CreateContext())
                return db.QStandardConnections.FirstOrDefault(x => x.FromStandardIdentifier == fromStandardId && x.ToStandardIdentifier == toStandardId);
        }

        public List<QStandardConnection> GetAllStandardConnections()
        {
            using (var db = CreateContext())
                return db.QStandardConnections.ToList();
        }

        public QStandardContainment GetStandardContainment(Guid parentStandardId, Guid childStandardId)
        {
            using (var db = CreateContext())
                return db.QStandardContainments.FirstOrDefault(x => x.ParentStandardIdentifier == parentStandardId && x.ChildStandardIdentifier == childStandardId);
        }

        public List<QStandardContainment> GetAllStandardContainments()
        {
            using (var db = CreateContext())
                return db.QStandardContainments.ToList();
        }

        public QStandardOrganization GetStandardOrganization(Guid standardId, Guid organizationId)
        {
            using (var db = CreateContext())
                return db.QStandardOrganizations.FirstOrDefault(x => x.StandardIdentifier == standardId && x.ConnectedOrganizationIdentifier == organizationId);
        }

        public QStandardAchievement GetStandardAchievement(Guid standardId, Guid achievementId)
        {
            using (var db = CreateContext())
                return db.QStandardAchievements.FirstOrDefault(x => x.StandardIdentifier == standardId && x.AchievementIdentifier == achievementId);
        }

        public QStandardGroup GetStandardGroup(Guid standardId, Guid groupId)
        {
            using (var db = CreateContext())
                return db.QStandardGroups.FirstOrDefault(x => x.StandardIdentifier == standardId && x.GroupIdentifier == groupId);
        }

        public int CountStandardCategories(Guid standardId)
        {
            using (var db = CreateContext())
                return db.QStandardCategories.Count(x => x.StandardIdentifier == standardId);
        }

        public int CountStandardConnections(Guid fromStandardId)
        {
            using (var db = CreateContext())
                return db.QStandardConnections.Count(x => x.FromStandardIdentifier == fromStandardId);
        }

        public int CountStandardContainments(Guid parentStandardId)
        {
            using (var db = CreateContext())
                return db.QStandardContainments.Count(x => x.ParentStandardIdentifier == parentStandardId);
        }

        public int CountStandardOrganizations(Guid standardId)
        {
            using (var db = CreateContext())
                return db.QStandardOrganizations.Count(x => x.StandardIdentifier == standardId);
        }

        public int CountStandardAchievements(Guid standardId)
        {
            using (var db = CreateContext())
                return db.QStandardAchievements.Count(x => x.StandardIdentifier == standardId);
        }

        public int CountStandardGroups(Guid standardId)
        {
            using (var db = CreateContext())
                return db.QStandardGroups.Count(x => x.StandardIdentifier == standardId);
        }
    }
}
