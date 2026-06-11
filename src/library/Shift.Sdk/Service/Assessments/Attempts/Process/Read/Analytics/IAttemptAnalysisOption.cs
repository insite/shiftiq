namespace InSite.Application.Attempts.Read
{
    public interface IAttemptAnalysisOption
    {
        int AttemptCount { get; }

        int AnswerCount { get; }

        decimal AnswerRate { get; }

        int AverageAttemptScorePercent { get; }

        double ItemTotalCorrelation { get; }

        double ItemRestCoefficient { get; }
    }
}
