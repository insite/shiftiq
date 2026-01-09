using System;
using System.Collections.Generic;

using InSite.Application.Surveys.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FormOptionItemComboBox : ComboBox
    {
        #region Properties

        public Guid? OptionListIdentifier
        {
            get { return (Guid?)ViewState[nameof(OptionListIdentifier)]; }
            set { ViewState[nameof(OptionListIdentifier)] = value; }
        }

        #endregion

        #region Selecting data

        protected override ListItemArray CreateDataSource()
        {
            var data = new List<ListItem>();
            var items = OptionListIdentifier.HasValue
                ? ServiceLocator.SurveySearch.GetSurveyOptionItems(new QSurveyOptionItemFilter { SurveyOptionListIdentifier = OptionListIdentifier.Value })
                : null;

            if (items != null)
            {
                foreach (var item in items)
                {
                    data.Add(new ListItem
                    {
                        Value = item.SurveyOptionItemIdentifier.ToString(),
                        Text = GetText(item)
                    });
                }
            }

            return new ListItemArray(data);
        }

        private string GetText(QSurveyOptionItem item)
        {
            var title = ServiceLocator.ContentSearch.GetTitleText(item.SurveyOptionItemIdentifier);

            return $"{item.SurveyOptionItemSequence}. {title.IfNullOrEmpty("Option")}";
        }

        #endregion
    }
}