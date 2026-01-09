using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDuplicateGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Classes

        [Serializable]
        private class DuplicateRow
        {
            public Guid UserIdentifier { get; internal set; }
            public string FullName { get; internal set; }
            public int ResponseCount { get; internal set; }
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        private DuplicateRow[] Duplicates
        {
            get => (DuplicateRow[])ViewState[nameof(Duplicates)];
            set => ViewState[nameof(Duplicates)] = value;
        }

        private Dictionary<Guid, DateTimeOffset[]> Dates
        {
            get => (Dictionary<Guid, DateTimeOffset[]>)ViewState[nameof(Dates)];
            set => ViewState[nameof(Dates)] = value;
        }

        #endregion

        #region Public methods

        public bool LoadData(SurveyForm survey, IEnumerable<QResponseSession> responses)
        {
            if (!survey.EnableUserConfidentiality)
            {
                Duplicates = responses
                    .Where(x => x.Respondent != null)
                    .GroupBy(x => new { x.Respondent.UserIdentifier, x.Respondent.UserFullName })
                    .Select(x => new DuplicateRow
                    {
                        UserIdentifier = x.Key.UserIdentifier,
                        FullName = x.Key.UserFullName,
                        ResponseCount = x.Count(),
                    })
                    .Where(x => x.ResponseCount > 1)
                    .OrderBy(x => x.FullName)
                    .ToArray();

                Dates = responses
                    .Where(x => x.Respondent != null && x.ResponseSessionStarted.HasValue)
                    .GroupBy(x => x.Respondent.UserIdentifier)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(y => y.ResponseSessionStarted.Value).OrderBy(y => y).ToArray()
                    );
            }
            else
            {
                Duplicates = new DuplicateRow[0];
                Dates = new Dictionary<Guid, DateTimeOffset[]>();
            }

            Search(new NullFilter());

            return HasRows;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = e.Row.DataItem;
            var userKey = (Guid)DataBinder.Eval(row, "UserIdentifier");

            if (Dates != null && Dates.ContainsKey(userKey))
            {
                var dateList = Dates[userKey];
                var textDates = new StringBuilder();
                var prevDate = string.Empty;

                foreach (var date in dateList)
                {
                    var textDate = date.ToString("MMM d");
                    if (prevDate == textDate)
                        continue;

                    if (textDates.Length > 0)
                        textDates.Append("; ");

                    textDates.Append(textDate);

                    prevDate = textDate;
                }

                var datesLiteral = (ITextControl)e.Row.FindControl("Dates");
                datesLiteral.Text = textDates.ToString();
            }
        }

        #endregion

        #region Search results

        protected override int SelectCount(NullFilter filter)
        {
            return Duplicates.Length;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            return Duplicates.ApplyPaging(filter).ToList().ToSearchResult();
        }

        #endregion
    }
}