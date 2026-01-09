using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public class QLearnerProgramSummaryStore
    {
        public static void Delete(Guid organization, DateTimeOffset asAt)
        {
            using (var db = new InternalDbContext())
            {
                var range = db.QLearnerProgramSummaries.Where(x => x.OrganizationIdentifier == organization && x.AsAt == asAt);
                db.QLearnerProgramSummaries.RemoveRange(range);
                db.SaveChanges();
            }
        }

        public static void TakeSnapshot(Guid organization)
        {
            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand("reports.RefreshLearnerProgramSummary @OrganizationIdentifier", new SqlParameter("@OrganizationIdentifier", organization));

            DisallowMultipleSnapshotsForTheSameDate(organization);
        }

        private static void DisallowMultipleSnapshotsForTheSameDate(Guid organization)
        {
            var search = new QLearnerProgramSummarySearch();
            var offsets = search.GetSnapshotDates(organization).OrderByDescending(x => x).ToArray();

            var dates = new Dictionary<string, DateTimeOffset>();
            foreach (var offset in offsets)
            {
                var pst = TimeZones.FormatDateOnly(offset, TimeZones.Pacific);
                if (dates.ContainsKey(pst))
                    Delete(organization, offset);
                else
                    dates.Add(pst, offset);
            }
        }
    }
}
