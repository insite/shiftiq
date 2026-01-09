using System;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    internal interface IReportOption
    {
        Guid ID { get; }

        string Text { get; }

        string Category { get; }

        decimal Score { get; }

        decimal? Relative { get; }

        decimal? Valid { get; }

        SubmissionAnalysis.SelectionItem Analysis { get; }

        IReportQuestion Question { get; }
    }
}
