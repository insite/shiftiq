namespace InSite.Domain.Integrations.Prometric
{
    public class GetEligibilityOutput
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public EligibilityModel Model { get; set; }
    }
}
