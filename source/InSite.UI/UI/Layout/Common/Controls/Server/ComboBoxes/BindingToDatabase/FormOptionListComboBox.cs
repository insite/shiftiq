using System;
using System.Collections.Generic;

using InSite.Application.Surveys.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FormOptionListComboBox : ComboBox
    {
        #region Properties

        public Guid? QuestionIdentifier
        {
            get { return (Guid?)ViewState[nameof(QuestionIdentifier)]; }
            set { ViewState[nameof(QuestionIdentifier)] = value; }
        }

        #endregion

        #region Selecting data

        protected override ListItemArray CreateDataSource()
        {
            var data = new List<ListItem>();
            var lists = QuestionIdentifier.HasValue
                ? ServiceLocator.SurveySearch.GetSurveyOptionLists(new QSurveyOptionListFilter { SurveyQuestionIdentifier = QuestionIdentifier.Value })
                : null;

            if (lists != null)
            {
                foreach (var list in lists)
                {
                    data.Add(new ListItem
                    {
                        Value = list.SurveyOptionListIdentifier.ToString(),
                        Text = GetText(list)
                    });
                }
            }

            return new ListItemArray(data);
        }

        private string GetText(QSurveyOptionList list)
        {
            var title = ServiceLocator.ContentSearch.GetTitleText(list.SurveyOptionListIdentifier);

            return $"{list.SurveyOptionListSequence}. {title.IfNullOrEmpty("Option List")}";
        }

        #endregion
    }
}