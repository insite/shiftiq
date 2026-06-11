namespace InSite.Domain.Integrations.Bambora
{
    public class BamboraErrorResponse
    {
        public int code { get; set; }
        public int category { get; set; }
        public string message { get; set; }
        public BamboraDetail[] details { get; set; }
    }
}
