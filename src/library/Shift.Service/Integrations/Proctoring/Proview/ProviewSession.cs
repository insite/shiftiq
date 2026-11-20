namespace Shift.Service.Integrations.Proctoring.Proview;

public class ProviewSession
{
    public class DataType
    {
        public required string InitScriptUrl { get; set; }
        public required string ProviewToken { get; set; }
        public required string SessionTitle { get; set; }
        public required string ProfileId { get; set; }
        public required string Session { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public required string Email { get; set; }
    }

    public DataType? Data { get; set; }
    public string? RedirectUrl { get; set; }
}