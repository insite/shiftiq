namespace InSite.Domain.Integrations.Prometric
{
    public class GeeResponse
    {
        public bool OK => Status == 200;
        public int Status { get; set; }
        public string Content { get; set; }

        public GeeResponse(int status, string content)
        {
            Status = status;
            Content = content;
        }
    }
}
