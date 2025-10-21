using System;
using System.Data.SqlClient;
using System.Linq;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class Sequence
    {
        private static readonly object Lock = new object();

        public static int Increment(Guid organization, SequenceType type, int startNumber = 1)
            => IncrementMany(organization, type, 1, startNumber).First();

        public static int[] IncrementMany(Guid organization, SequenceType type, int count, int startNumber = 1)
        {
            if (startNumber <= 0)
                startNumber = 1;

            using (var db = new InternalDbContext())
            {
                var sqlParams = new[]
                {
                    new SqlParameter("@OrganizationIdentifier", organization),
                    new SqlParameter("@SequenceType", type.ToString()),
                    new SqlParameter("@SequenceCount", count),
                    new SqlParameter("@StartNumber", startNumber)
                };

                lock (Lock)
                {
                    return db.Database
                        .SqlQuery<int>("settings.IncrementSequence @OrganizationIdentifier, @SequenceType, @SequenceCount, @StartNumber", sqlParams)
                        .OrderBy(x => x)
                        .ToArray();
                }
            }
        }
    }
}
