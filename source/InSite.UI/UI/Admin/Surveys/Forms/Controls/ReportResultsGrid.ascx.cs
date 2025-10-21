using System;
using System.ComponentModel;
using System.Web.UI;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Surveys.Forms.Controls
{
    public partial class ReportResultsGrid : SearchResultsGridViewController<QResponseSessionFilter>
    {
        #region Properties

        protected override bool IsFinder => false;

        #endregion

        #region Public methods

        public bool LoadData(Guid surveyId, int? page = null)
        {
            var filter = new QResponseSessionFilter { SurveyFormIdentifier = surveyId };

            if (page.HasValue)
                Search(filter, page.Value);
            else
                Search(filter);

            return HasRows;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;
        }

        #endregion

        #region Event handlers

        private void FilterButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ReportResultsGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterTextBox.ClientID}', true);",
                true);
        }

        #endregion

        #region Search results

        protected override int SelectCount(QResponseSessionFilter filter)
        {
            filter.RespondentName = FilterTextBox.Text;

            return ServiceLocator.SurveySearch.CountResponseSessions(filter);
        }

        protected override IListSource SelectData(QResponseSessionFilter filter)
        {
            filter.RespondentName = FilterTextBox.Text;

            return ServiceLocator.SurveySearch
                .GetResponseSessions(filter)
                .ToSearchResult();
        }

        #endregion
    }
}