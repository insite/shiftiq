namespace Shift.Service.Cases;

public class TCaseStatusEntity
{
    public Guid StatusIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public string CaseType { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int StatusSequence { get; set; }
    public string StatusCategory { get; set; } = string.Empty;
    public string? ReportCategory { get; set; }
    public string? StatusDescription { get; set; }
}
