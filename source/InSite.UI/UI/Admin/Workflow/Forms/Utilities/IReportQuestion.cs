using System;
using System.Collections.Generic;

using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    internal interface IReportQuestion
    {
        Guid ID { get; }

        SurveyQuestionType QuestionType { get; }

        string Text { get; }

        SubmissionAnalysis Analysis { get; }

        int FrequencySum { get; }

        bool NumericEnableAnalysis { get; }

        IEnumerable<IReportOption> Options { get; }
    }
}
