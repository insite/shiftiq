using System;
using System.Web;

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

        public override string Value
        {
            get
            {
                var value = base.Value;
                return value.IsNotEmpty() ? HttpUtility.HtmlDecode(value) : null;
            }
            set
            {
                base.Value = value.IsNotEmpty() ? HttpUtility.HtmlEncode(value) : null;
            }
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
                var text = MaxTextLength.HasValue
                    ? value.MaxLength(MaxTextLength.Value, ShowEllipsisOnTextCut)
                    : value;

                result.Add(HttpUtility.HtmlEncode(value), HttpUtility.HtmlEncode(text));
            }

            return result;
        }

        #endregion

        #region IPostBackDataHandler

        protected override bool LoadPostData(string value)
        {
            if (value.IsNotEmpty())
                value = HttpUtility.HtmlDecode(value);

            return base.LoadPostData(value);
        }

        #endregion
    }
}