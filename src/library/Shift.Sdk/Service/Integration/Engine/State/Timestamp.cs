namespace InSite.Domain.Integrations.Engine
{
    public class Timestamp
    {
        public System.DateTimeOffset When { get; set; }

        public string What { get; set; }
        public string Who { get; set; }
        public string Why { get; set; }
        
        public string How { get; set; }
    }
}
