namespace InSite.Domain.Integrations.Prometric
{
    public class GetEligibilityInput
    {
        public string SRClientCode { get; set; }
        public string ClientEligibilityCode { get; set; }
        public string SRProgramCode { get; set; } 
        public string SRExamCode { get; set; }
        public string SRAppointmentID { get; set; }
    }
}