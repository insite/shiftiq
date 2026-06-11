namespace InSite.Persistence.Integration.BCMail
{
    public static class ExamDistributionRequestStore
    {
        public static void Insert(ExamDistributionRequest request)
        {
            using (var db = new InternalDbContext())
            {
                db.ExamDistributionRequests.Add(request);
                db.SaveChanges();
            }
        }
    }
}
