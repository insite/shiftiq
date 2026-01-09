using System;

using InSite.Application.Surveys.Read;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    [Serializable]
    public partial class SubmissionAnalysis
    {
        #region Fields

        public SelectionList SelectionAnalysis { get; private set; }
        public CategoryList CategoryAnalysis { get; private set; }
        public NumberList NumberAnalysis { get; private set; }
        public TextList TextAnalysis { get; private set; }
        public TextList CommentAnalysis { get; private set; }

        public int SessionCount { get; private set; }

        public QResponseAnalysisFilter Filter { get; private set; }

        #endregion

        #region Construction

        public SubmissionAnalysis(QResponseAnalysisFilter filter, string userName)
        {
            Filter = filter;

            SessionCount = ServiceLocator.SurveySearch.CountResponseSessions(filter);

            SelectionAnalysis = new SelectionList();
            CategoryAnalysis = new CategoryList();
            NumberAnalysis = new NumberList();
            TextAnalysis = new TextList();
            CommentAnalysis = new TextList();

            if (Filter.ShowSelections)
            {
                foreach (var item in ServiceLocator.SurveySearch.GetSelectionAnalysis(filter))
                    SelectionAnalysis.Add(new SelectionItem(item));

                foreach (var item in ServiceLocator.SurveySearch.GetChecklistAnalysis(filter))
                    SelectionAnalysis.Add(
                        item.QuestionIdentifier,
                        item.OptionIdentifier,
                        (double)item.OptionPoints);

                foreach (var item in ServiceLocator.SurveySearch.GetCategoryAnalysis(filter))
                    CategoryAnalysis.Add(new CategoryItem(item));
            }

            if (Filter.ShowNumbers)
            {
                foreach (var item in ServiceLocator.SurveySearch.GetIntegerAnalysis(filter))
                    NumberAnalysis.Add(new NumberItem(item));
            }

            if (Filter.ShowText)
            {
                foreach (var item in ServiceLocator.SurveySearch.GetTextAnalysis(filter))
                    TextAnalysis.Add(new TextItem
                    {
                        ResponseSessionID = item.ResponseSessionIdentifier,
                        QuestionID = item.QuestionIdentifier,
                        AnswerText = item.AnswerText,
                    });
            }

            if (Filter.ShowComments)
            {
                foreach (var item in ServiceLocator.SurveySearch.GetCommentAnalysis(filter))
                    CommentAnalysis.Add(new TextItem
                    {
                        ResponseSessionID = item.ResponseSessionIdentifier,
                        QuestionID = item.QuestionIdentifier,
                        AnswerText = item.AnswerComment,
                    });
            }
        }

        #endregion
    }
}