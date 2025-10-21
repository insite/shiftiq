namespace InSite.Persistence.Integration.BCMail
{
    public class DistributionRequestItem
    {
        public string Title { get; set; }
        public string Json { get; set; }

        public DistributionRequestItem(string title, string json)
        {
            Title = title;
            Json = json;
        }
    }
}