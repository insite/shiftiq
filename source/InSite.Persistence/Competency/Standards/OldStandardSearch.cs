using System;
using System.Linq;

using InSite.Application.Standards.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class OldStandardSearch : IOldStandardSearch
    {
        private InternalDbContext CreateContext() => new InternalDbContext(false);

        public VStandard GetStandard(Guid standard)
        {
            using (var db = CreateContext())
                return db.VStandards.SingleOrDefault(x => x.StandardIdentifier == standard);
        }

        public string GetCalculationMethod(Guid standard)
        {
            using (var db = CreateContext())
                return db.VStandards
                    .Where(x => x.StandardIdentifier == standard)
                    .Select(x => x.CompetencyScoreSummarizationMethod)
                    .SingleOrDefault()
                    .NullIfEmpty();
        }

        public VCompetency GetCompetency(Guid standard)
        {
            using (var db = CreateContext())
                return db.VCompetencies.FirstOrDefault(x => x.CompetencyIdentifier == standard);
        }
    }
}