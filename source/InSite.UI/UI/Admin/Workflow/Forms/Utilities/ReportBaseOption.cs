using System;

using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    internal abstract class ReportBaseOption : IReportOption
    {
        public abstract Guid ID { get; }

        public abstract string Text { get; }

        public abstract string Category { get; }

        public abstract decimal Score { get; }

        public abstract IReportQuestion Question { get; }

        public virtual int Frequency => Analysis.AnswerFrequency;

        public virtual SubmissionAnalysis.SelectionItem Analysis => _analysis
            ?? (_analysis = Question.Analysis.SelectionAnalysis.GetAnalysisForQuestionAndAnswer(Question.ID, ID));

        public virtual decimal? Relative
        {
            get
            {
                decimal frequency = Frequency;
                decimal total = Question.QuestionType == SurveyQuestionType.CheckList
                    ? Question.FrequencySum
                    : Question.Analysis.SessionCount;

                return frequency > 0 && total > 0
                    ? frequency / total
                    : (decimal?)null;
            }
        }

        public virtual decimal? Valid
        {
            get
            {
                decimal frequency = Frequency;

                return frequency > 0 && Question.FrequencySum > 0
                    ? frequency / Question.FrequencySum
                    : (decimal?)null;
            }
        }

        #region Fields

        private SubmissionAnalysis.SelectionItem _analysis = null;

        #endregion
    }
}