using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
            var data = new List<ListItem>();
            var questions = SurveyIdentifier.HasValue
                ? ServiceLocator.SurveySearch.GetSurveyQuestions(new QSurveyQuestionFilter
                {
                    SurveyFormIdentifier = SurveyIdentifier.Value,
                    HasResponseAnswer = HasResponseAnswer,
                    ExcludeQuestionsID = ExcludeQuestionsID,
                    ExcludeQuestionsTypes = ExcludeSpecialQuestions
                        ? new[]
                        {
                            SurveyQuestionType.BreakPage
                        }
                        : null,
                    IncludeQuestionsTypes = QuestionType,
                    HasOptions = HasOptions
                })
                : null;

            if (questions != null)
            {
                if (ExcludeHiddenMaskingOptionID.HasValue)
                {
                    var option = ServiceLocator.SurveySearch.GetSurveyOptionItem(ExcludeHiddenMaskingOptionID.Value, x => x.SurveyOptionList.SurveyQuestion);

                    var surveyQuestionSequence = option?.SurveyOptionList?.SurveyQuestion?.SurveyQuestionSequence;

                    if (surveyQuestionSequence.HasValue)
                        questions = questions.Where(x => x.SurveyQuestionSequence > surveyQuestionSequence).ToList();
                }

                foreach (var question in questions)
                {
                    data.Add(new ListItem
                    {
                        Value = question.SurveyQuestionIdentifier.ToString(),
                        Text = GetText(question)
                    });
                }
            }

            return new ListItemArray(data);
        }

        private string GetText(QSurveyQuestion question)
        {
            var title = string.Empty;

            var content = ServiceLocator.ContentSearch.GetBlock(
                question.SurveyQuestionIdentifier,
                ContentContainer.DefaultLanguage,
                new[] { ContentLabel.Title });

            if (content != null)
                title = content.Title.GetSnip();

            if (MaxTextLength.HasValue && title.Length > MaxTextLength.Value)
                title = title.Substring(0, MaxTextLength.Value - 3) + "...";

            var prefix = question.SurveyQuestionCode.IfNullOrEmpty(() => question.SurveyQuestionSequence.ToString()) + ".";

            return title.HasValue()
                ? $"{prefix} {title}"
                : $"Question {prefix} {question.SurveyQuestionType}";
        }

        #endregion
    }
}