using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Assessor.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QAttemptFilter>
    {
        private bool _allowView;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var link = (IconLink)e.Row.FindControl("ViewAttemptLink");
            link.Visible = _allowView;

            var attempt = (QAttempt)e.Row.DataItem;

            link.ToolTip = attempt.AttemptGraded == null && attempt.AttemptSubmitted.HasValue
                ? "Grade Attempt"
                : "View Attempt";
        }

        protected override int SelectCount(QAttemptFilter filter)
        {
            return ServiceLocator.AttemptSearch.CountAttempts(filter);
        }

        protected override IListSource SelectData(QAttemptFilter filter)
        {
            _allowView = true;

            return ServiceLocator.AttemptSearch
                .GetAttempts(filter, x => x.AssessorPerson, x => x.LearnerPerson, x => x.Form)
                .ToSearchResult();
        }

        protected override void SetGridVisibility(bool isVisible, bool showInstructions)
        {
            base.SetGridVisibility(isVisible, showInstructions);
        }

        protected string FormatScore()
        {
            // Access to individual performance metrics may be denied.

            if (!_allowView)
                return "<span class='text-danger' title='Access to individual performance metrics is denied'>******</span>";

            var attempt = (QAttempt)Page.GetDataItem();
            if (!attempt.AttemptGraded.HasValue)
            {
                return attempt.AttemptSubmitted.HasValue
                    ? "<span class='badge bg-warning'>Pending</span>"
                    : string.Empty;
            }

            var statusHtml = attempt.AttemptIsPassing
                ? "<span class='badge bg-success'>Pass</span>"
                : "<span class='badge bg-danger'>Fail</span>";

            return $"<div>{attempt.AttemptScore:p0}</div>"
                + $"<div class='form-text'>{attempt.AttemptPoints} / {attempt.FormPoints}</div>"
                + statusHtml;
        }

        protected string FormatTime()
        {
            var html = new StringBuilder();

            var attempt = (QAttempt)Page.GetDataItem();
            if (attempt.AttemptStarted.HasValue)
                html.Append("<div>Started " + attempt.AttemptStarted.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptGraded.HasValue)
                html.Append("<div>Completed " + attempt.AttemptGraded.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptImported.HasValue)
                html.Append("<div class='form-text'>Imported " + attempt.AttemptImported.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptDuration.HasValue)
                html.Append("<div class='form-text'>Time Taken = " + ((double)attempt.AttemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second) + "</div>");

            return html.ToString();
        }

        protected string GetFormAsset()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            var form = attempt.Form;
            if (form == null)
                return string.Empty;

            var assetVersion = attempt.Form.FormAssetVersion;
            if (form.FormFirstPublished.HasValue && attempt.AttemptStarted.HasValue && attempt.AttemptStarted.Value < form.FormFirstPublished.Value)
                assetVersion = 0;

            return $"{form.FormAsset}.{assetVersion}";
        }

    }
}