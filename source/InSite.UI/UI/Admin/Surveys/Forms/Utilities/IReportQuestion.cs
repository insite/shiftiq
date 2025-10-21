using System;
using System.Collections.Generic;

using Shift.Constant;

namespace InSite.Admin.Surveys.Forms.Utilities
{
    internal interface IReportQuestion
    {
        Guid ID { get; }

        SurveyQuestionType QuestionType { get; }

        string Text { get; }

        ResponseAnalysis Analysis { get; }

        int FrequencySum { get; }

        bool NumericEnableAnalysis { get; }

        IEnumerable<IReportOption> Options { get; }
    }
}
