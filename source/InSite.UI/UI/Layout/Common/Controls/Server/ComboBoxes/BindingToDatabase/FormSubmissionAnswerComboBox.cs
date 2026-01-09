using System;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FormSubmissionAnswerComboBox : ComboBox
    {
        #region Properties

        public Guid? SurveyQuestionIdentifier
        {
            get { return (Guid?)ViewState[nameof(SurveyQuestionIdentifier)]; }
            set { ViewState[nameof(SurveyQuestionIdentifier)] = value; }
        }

        public int? MaxTextLength
        {
            get { return (int?)ViewState[nameof(MaxTextLength)]; }
            set { ViewState[nameof(MaxTextLength)] = value; }
        }

        public bool ShowEllipsisOnTextCut
        {

            get => (bool?)ViewState[nameof(ShowEllipsisOnTextCut)] ?? false;
            set => ViewState[nameof(ShowEllipsisOnTextCut)] = value;
        }

        #endregion

        #region Selecting data

        protected override ListItemArray CreateDataSource()
        {
            var result = new ListItemArray();

            if (SurveyQuestionIdentifier == null)
                return result;

            var data = ServiceLocator.SurveySearch.GetResponseAnswersText(SurveyQuestionIdentifier.Value);
            foreach (var value in data)
            {
                string text;

                if (MaxTextLength.HasValue && value.Length > MaxTextLength.Value)
                {
                    text = value.Substring(0, MaxTextLength.Value);
                    if (ShowEllipsisOnTextCut)
                        text += "...";
                }
                else
                    text = value;

                result.Add(value, text);
            }

            return result;
        }

        #endregion
    }
}