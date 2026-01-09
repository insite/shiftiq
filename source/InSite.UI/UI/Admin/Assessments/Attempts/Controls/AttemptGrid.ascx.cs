using System;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class AttemptGrid : BaseUserControl
    {
        private IAttemptSearch AttemptSearch => ServiceLocator.AttemptSearch;

        private QAttemptFilter Filter
        {
            get => (QAttemptFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
        }

        public void LoadData(QAttemptFilter filter)
        {
            Filter = filter;

            Grid.PageIndex = 0;
            Grid.VirtualItemCount = AttemptSearch.CountAttempts(filter);
            Grid.DataBind();
        }

        private void Grid_DataBinding(object source, EventArgs e)
        {
            if (Filter == null)
                return;

            Filter.Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            Grid.DataSource = AttemptSearch
                .GetAttempts(Filter, x => x.Form, x => x.LearnerPerson)
                .Where(x => x.LearnerPerson != null)
                .Select(x => new AttemptGridItem
                {
                    AttemptIdentifier = x.AttemptIdentifier,
                    LearnerUserIdentifier = x.LearnerUserIdentifier,
                    LearnerName = x.LearnerPerson.UserFullName,
                    LearnerEmail = x.LearnerPerson.UserEmail,
                    LearnerCode = x.LearnerPerson.PersonCode,
                    FormPassingScore = x.Form.FormPassingScore,
                    FormPoints = x.FormPoints,
                    AttemptIsPassing = x.AttemptIsPassing,
                    AttemptPoints = x.AttemptPoints,
                    AttemptStarted = x.AttemptStarted,
                    AttemptSubmitted = x.AttemptSubmitted,
                    AttemptGraded = x.AttemptGraded,
                    AttemptTag = x.AttemptTag ?? "",
                    AttemptScore = x.AttemptScore ?? 0,
                    SebVersion = AttemptHelper.GetSebVersion(x.UserAgent)
                })
                .OrderBy(x => x.LearnerName)
                .ToArray();
        }

        protected string GetCompletedHtml()
        {
            var item = (AttemptGridItem)Page.GetDataItem();
            return item.AttemptGraded.HasValue
                ? item.AttemptGraded.Value.Format(User.TimeZone, true)
                : item.AttemptSubmitted.HasValue
                    ? "Pending"
                    : "Not Completed";
        }

        protected string GetScoreHtml()
        {
            var item = (AttemptGridItem)Page.GetDataItem();
            return !item.AttemptGraded.HasValue
                ? string.Empty
                : $"{item.AttemptScore:p0} <span class='form-text'>({item.AttemptPoints}/{item.FormPoints})</span>";
        }

        protected string GetGradeHtml()
        {
            var item = (AttemptGridItem)Page.GetDataItem();
            return !item.AttemptGraded.HasValue
                ? string.Empty
                : item.AttemptIsPassing
                    ? $"<span class='badge bg-success'>Pass</span>"
                    : $"<span class='badge bg-danger'>Fail</span>";
        }
    }
}