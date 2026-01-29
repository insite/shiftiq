using System;
using System.ComponentModel;
using System.Linq;
using System.Web;

using InSite.Application.Surveys.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class FormQuestionComboBox : ComboBox
    {
        #region Properties

        public Guid? SurveyIdentifier
        {
            get { return (Guid?)ViewState[nameof(SurveyIdentifier)]; }
            set { ViewState[nameof(SurveyIdentifier)] = value; }
        }

        public Guid? ExcludeHiddenMaskingOptionID
        {
            get { return (Guid?)ViewState[nameof(ExcludeHiddenMaskingOptionID)]; }
            set { ViewState[nameof(ExcludeHiddenMaskingOptionID)] = value; }
        }

        public Guid[] ExcludeQuestionsID
        {
            get { return (Guid[])ViewState[nameof(ExcludeQuestionsID)]; }
            set { ViewState[nameof(ExcludeQuestionsID)] = value; }
        }

        public bool? HasResponseAnswer
        {
            get { return (bool?)ViewState[nameof(HasResponseAnswer)]; }
            set { ViewState[nameof(HasResponseAnswer)] = value; }
        }

        public int? MaxTextLength
        {
            get { return (int?)ViewState[nameof(MaxTextLength)]; }
            set { ViewState[nameof(MaxTextLength)] = value; }
        }

        public bool ExcludeSpecialQuestions
        {
            get => (bool?)ViewState[nameof(ExcludeSpecialQuestions)] ?? false;
            set => ViewState[nameof(ExcludeSpecialQuestions)] = value;
        }

        [TypeConverter(typeof(SurveyQuestionTypeArrayConverter))]
        public SurveyQuestionType[] QuestionType
        {
            get { return (SurveyQuestionType[])ViewState[nameof(QuestionType)]; }
            set { ViewState[nameof(QuestionType)] = value; }
        }

        public bool? HasOptions
        {
            get => (bool?)ViewState[nameof(HasOptions)];
            set => ViewState[nameof(HasOptions)] = value;
        }

        #endregion

        #region Selecting data

        protected override ListItemArray CreateDataSource()
        {
            if (!SurveyIdentifier.HasValue)
                return new ListItemArray();

            var questions = ServiceLocator.SurveySearch.GetSurveyQuestions(new QSurveyQuestionFilter
            {
                SurveyFormIdentifier = SurveyIdentifier.Value,
                HasResponseAnswer = HasResponseAnswer,
                ExcludeQuestionsID = ExcludeQuestionsID,
                ExcludeQuestionsTypes = ExcludeSpecialQuestions
                    ? new[] { SurveyQuestionType.BreakPage }
                    : null,
                IncludeQuestionsTypes = QuestionType,
                HasOptions = HasOptions
            });

            if (questions.IsEmpty())
                return new ListItemArray();

            if (ExcludeHiddenMaskingOptionID.HasValue)
            {
                var option = ServiceLocator.SurveySearch.GetSurveyOptionItem(ExcludeHiddenMaskingOptionID.Value, x => x.SurveyOptionList.SurveyQuestion);

                var surveyQuestionSequence = option?.SurveyOptionList?.SurveyQuestion?.SurveyQuestionSequence;

                if (surveyQuestionSequence.HasValue)
                    questions = questions.Where(x => x.SurveyQuestionSequence > surveyQuestionSequence).ToList();
            }

            var result = new ListItemArray();
            var contents = ServiceLocator.ContentSearch.GetBlocks(
                questions.Select(x => x.SurveyQuestionIdentifier),
                ContentContainer.DefaultLanguage,
                new[] { ContentLabel.Title });
            var maxLength = MaxTextLength ?? 100;

            foreach (var question in questions)
            {
                var title = string.Empty;

                var content = contents.GetOrDefault(question.SurveyQuestionIdentifier);
                if (content != null)
                    title = content.Title.Html.Default.IfNullOrEmpty(content.Title.Text.Default).MaxLength(maxLength, true);

                var prefix = question.SurveyQuestionCode.IfNullOrEmpty(() => question.SurveyQuestionSequence.ToString()) + ".";

                result.Add(new ListItem
                {
                    Value = question.SurveyQuestionIdentifier.ToString(),
                    Text = title.IsNotEmpty() ? $"{prefix} {HttpUtility.HtmlEncode(title)}" : $"Question {prefix} {question.SurveyQuestionType}"
                });
            }

            return result;
        }

        #endregion
    }
}