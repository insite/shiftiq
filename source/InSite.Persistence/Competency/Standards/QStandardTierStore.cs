using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Standards.Read;
using InSite.Domain.Standards;

using Shift.Common;

namespace InSite.Persistence
{
    public class QStandardTierStore : IStandardTierStore
    {
        public static readonly IReadOnlyDictionary<int, string> TierNameMapping = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>
        {
            { 1, "Occupation" },
            { 2, "Level" },
            { 3, "Area" },
            { 4, "Competency" },
            { 5, "Skill" },
            { 6, "Subskill" }
        });

        private class StandardRootAndDepthInfo
        {
            public Guid RootStandardIdentifier { get; set; }
            public int Depth { get; set; }
        }

        public void DeleteAll()
        {
            const string sql = @"
TRUNCATE TABLE [standard].QStandardTier;
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql);
        }

        public void DeleteAll(Guid id)
        {
            const string sql = @"
DELETE [standard].QStandardTier WHERE StandardIdentifier = @ID;
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql, new[]
                {
                    new SqlParameter("ID", id)
                });
        }

        public void Insert(StandardCreated e)
        {
            var entity = new QStandardTier
            {
                RootStandardIdentifier = e.AggregateIdentifier,
                ItemStandardIdentifier = e.AggregateIdentifier,
            };

            SetTierNumber(entity, 1);

            using (var db = new InternalDbContext())
            {
                db.QStandardTiers.Add(entity);
                db.SaveChanges();
            }
        }

        public void Delete(StandardRemoved e)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QStandardTiers.FirstOrDefault(x => x.ItemStandardIdentifier == e.AggregateIdentifier);
                if (entity == null)
                    return;

                db.QStandardTiers.Remove(entity);
                db.SaveChanges();
            }
        }

        public void Update(StandardFieldGuidModified e)
        {
            if (e.Field == StandardField.ParentStandardIdentifier)
                OnParentUpdated(e.AggregateIdentifier);
        }

        public void Update(StandardFieldsModified e)
        {
            if (e.Fields.ContainsKey(StandardField.ParentStandardIdentifier))
                OnParentUpdated(e.AggregateIdentifier);
        }

        private static void OnParentUpdated(Guid standardId)
        {
            const string query = "SELECT * FROM standards.GetStandardRootAndDepth(@StandardID);";

            using (var db = new InternalDbContext())
            {
                var info = db.Database
                    .SqlQuery<StandardRootAndDepthInfo>(query, new SqlParameter("@StandardID", standardId))
                    .FirstOrDefault();

                if (info == null)
                    return;

                var entity = db.QStandardTiers.FirstOrDefault(x => x.ItemStandardIdentifier == standardId);
                if (entity == null)
                {
                    db.QStandardTiers.Add(entity = new QStandardTier
                    {
                        ItemStandardIdentifier = standardId
                    });
                }

                entity.RootStandardIdentifier = info.RootStandardIdentifier;

                SetTierNumber(entity, info.Depth);

                db.SaveChanges();
            }
        }

        private static void SetTierNumber(QStandardTier entity, int tierNumber)
        {
            entity.TierNumber = tierNumber;
            entity.TierName = TierNameMapping.GetOrDefault(tierNumber, "Undefined");
        }
    }
}
