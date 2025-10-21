using System.Linq;

namespace InSite.Persistence.Integration.BCMail
{
    public class ExamDistributionRequestSearch
    {
        public static ExamDistributionRequest[] GetRequests(string code)
        {
            using (var db = new InternalDbContext())
                return db.ExamDistributionRequests
                    .Where(x => x.JobCode == code)
                    .OrderByDescending(x => x.Requested)
                    .ToArray();
        }

        public static int Count()
        {
            using (var db = new InternalDbContext())
                return db.ExamDistributionRequests.Count();
        }
    }
}
