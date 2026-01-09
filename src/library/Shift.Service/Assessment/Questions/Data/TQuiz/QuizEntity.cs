namespace Shift.Service.Assessment;

public partial class QuizEntity
{
    public Guid GradebookIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid QuizIdentifier { get; set; }

    public string QuizData { get; set; } = null!;
    public string QuizName { get; set; } = null!;
    public string QuizType { get; set; } = null!;

    public int AttemptLimit { get; set; }
    public int PassingKph { get; set; }
    public int PassingWpm { get; set; }
    public int TimeLimit { get; set; }

    public decimal? MaximumPoints { get; set; }
    public decimal PassingAccuracy { get; set; }
    public decimal? PassingPoints { get; set; }
    public decimal? PassingScore { get; set; }
}