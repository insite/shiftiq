namespace InSite.Persistence.Integration.BCMail
{
    public interface IBCMailServer
    {
        // Commands
        DistributionJob Create(DistributionRequest distributions, bool isTest);

        // Queries
        DistributionJob[] Status(string[] jobs, bool isTest);
    }
}