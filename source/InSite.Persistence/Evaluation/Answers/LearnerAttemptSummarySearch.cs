using System;
using System.Linq;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class LearnerAttemptSummarySearch : ILearnerAttemptSummarySearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public TLearnerAttemptSummary GetSummary(Guid form, Guid learner)
        {
            using (var db = CreateContext())
                return db.TLearnerAttemptSummaries.AsNoTracking()
                    .Where(x => x.FormIdentifier == form && x.LearnerUserIdentifier == learner)
                    .FirstOrDefault();
        }

        public TLearnerAttemptSummary GetFormSummary(Guid form)
        {
            using (var db = CreateContext())
            {
                var entities = db.TLearnerAttemptSummaries.AsNoTracking()
                    .Where(x => x.FormIdentifier == form)
                    .ToArray();

                var result = new TLearnerAttemptSummary
                {
                    FormIdentifier = form
                };

                foreach (var entity in entities)
                {
                    result.AttemptStartedCount += entity.AttemptStartedCount;
                    result.AttemptSubmittedCount += entity.AttemptSubmittedCount;
                    result.AttemptGradedCount += entity.AttemptGradedCount;
                    result.AttemptPassedCount += entity.AttemptPassedCount;
                    result.AttemptFailedCount += entity.AttemptFailedCount;
                    result.AttemptVoidedCount += entity.AttemptVoidedCount;
                    result.AttemptImportedCount += entity.AttemptImportedCount;
                    result.AttemptTotalCount += entity.AttemptTotalCount;
                }

                return result;
            }
        }
    }
}
