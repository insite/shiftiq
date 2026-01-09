using System;
using System.Linq;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    public class QStandardValidationSearch : IStandardValidationSearch
    {
        public bool Exists(Guid standardId, Guid userId, Guid? excludeValidationId = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QStandardValidations.AsQueryable()
                    .Where(x => x.StandardIdentifier == standardId && x.UserIdentifier == userId);

                if (excludeValidationId.HasValue)
                    query = query.Where(x => x.StandardValidationIdentifier != excludeValidationId.Value);

                return query.Any();
            }
        }

        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false, false);
        }

        public QStandardValidation GetStandardValidation(Guid standardValidationId)
        {
            using (var db = CreateContext())
                return db.QStandardValidations.FirstOrDefault(x => x.StandardValidationIdentifier == standardValidationId);
        }

        public QStandardValidation GetStandardValidation(Guid standardId, Guid userId)
        {
            using (var db = CreateContext())
                return db.QStandardValidations.FirstOrDefault(x => x.StandardIdentifier == standardId && x.UserIdentifier == userId);
        }

        public QStandardValidationLog GetStandardValidationLog(Guid logId)
        {
            using (var db = CreateContext())
                return db.QStandardValidationLogs.FirstOrDefault(x => x.LogIdentifier == logId);
        }

        public int CountStandardValidationLogs(Guid standardValidationId)
        {
            using (var db = CreateContext())
                return db.QStandardValidationLogs.Count(x => x.StandardValidationIdentifier == standardValidationId);
        }
    }
}
