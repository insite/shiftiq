using System;

namespace InSite.Admin.Surveys.Forms.Utilities
{
    internal interface IReportOption
    {
        Guid ID { get; }

        string Text { get; }

        string Category { get; }

        decimal Score { get; }

        decimal? Relative { get; }

        decimal? Valid { get; }

        ResponseAnalysis.SelectionItem Analysis { get; }

        IReportQuestion Question { get; }
    }
}
