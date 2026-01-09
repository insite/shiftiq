using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    public partial class Ratio : AdminBasePage
    {
        #region Constants

        private const string ParentUrl = "/ui/admin/assessments/attempts/ad-hoc";

        #endregion

        #region Properties

        private QAttemptFilter Filter
        {
            get => (QAttemptFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        private GroupByColumn[] GroupByColumns
        {
            get => (GroupByColumn[])Session[_groupByColumnsKey];
            set => Session[_groupByColumnsKey] = value;
        }

        #endregion

        #region Fields

        private static readonly string _groupByColumnsKey = typeof(Ratio).FullName + "." + nameof(GroupByColumns);

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CompetencyRatio.Alert += (s, a) => ScreenStatus.AddMessage(a);

            RefreshButton.Click += RefreshButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Filter = LoadFilter();

            if (Filter == null)
                HttpResponseHelper.Redirect(ParentUrl);

            if (!Filter.FrameworkIdentifier.HasValue || Filter.FrameworkIdentifier.Value == Guid.Empty)
            {
                NavPanel.Visible = false;

                ScreenStatus.AddMessage(
                    AlertType.Error,
                    "The search criteria filter does not have enough criteria to limit your search results to an acceptable number of records. " +
                    "Please go to <a href='/ui/admin/assessments/attempts/ad-hoc'>Ad-Hoc</a> page to specify the Exam Framework criteria field to run this report.");

                return;
            }

            PageHelper.AutoBindHeader(this);

            SetCriteriaValues();
            SetGroupBySelector();
            LoadData();
        }

        private QAttemptFilter LoadFilter()
        {
            var parentAction = ParentUrl.Substring(1);
            var report = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == parentAction);

            if (report == null)
                return null;

            return ReportRequest.Deserialize(report.ReportData)?.GetSearch<QAttemptFilter>()?.Filter;
        }

        private void LoadData()
        {
            var filter = Filter;

            filter.CandidateOrganizationIdentifiers = Identity.Organizations.Select(x => x.OrganizationIdentifier).ToArray();
            filter.IsCompleted = true;
            filter.OrderBy = null;

            var questions = ServiceLocator.AttemptSearch.BindAttemptQuestions(
                x => new Controls.QuestionCompetencyRatioRepeater.QuestionDataItem
                {
                    CompetencyAreaIdentifier = x.CompetencyAreaIdentifier,
                    CompetencyAreaCode = x.CompetencyAreaCode,
                    CompetencyAreaTitle = x.CompetencyAreaTitle,

                    AttemptIdentifier = x.Attempt.AttemptIdentifier,
                    AttemptTag = x.Attempt.AttemptTag,
                    AttemptStarted = x.Attempt.AttemptStarted,
                    AttemptGraded = x.Attempt.AttemptGraded,
                    AttemptIsPassing = x.Attempt.AttemptIsPassing,
                    EventFormat = x.Attempt.Registration.Event.EventFormat,

                    FormIdentifier = x.Attempt.FormIdentifier,
                    FormAsset = x.Attempt.Form.FormAsset,
                    FormName = x.Attempt.Form.FormName,
                    FormTitle = x.Attempt.Form.FormTitle,

                    LearnerUserIdentifier = x.Attempt.LearnerUserIdentifier,

                    QuestionIdentifier = x.QuestionIdentifier,
                    AnswerPoints = x.AnswerPoints
                },
                filter);

            if (filter is AdHocAttemptFilter internalFilter && internalFilter.IncludeOnlyFirstAttempt)
            {
                var includeAttempts = questions
                    .GroupBy(x => new { x.AttemptIdentifier, x.FormIdentifier, x.LearnerUserIdentifier })
                    .Select(g => g.OrderBy(y => y.AttemptStarted).ThenBy(y => y.AttemptGraded).First().AttemptIdentifier)
                    .ToHashSet();

                questions = questions.Where(x => includeAttempts.Contains(x.AttemptIdentifier)).ToArray();
            }

            var areaIds = questions
                .Where(x => x.CompetencyAreaIdentifier.HasValue)
                .Select(x => x.CompetencyAreaIdentifier.Value)
                .Distinct()
                .ToArray();

            var areaMapping =
                StandardSearch.Bind(
                    x => new
                    {
                        AreaIdentifier = x.StandardIdentifier,

                        OccupationIdentifier = (Guid?)x.Parent.Parent.StandardIdentifier,
                        OccupationTitle = x.Parent.Parent.ContentTitle ?? x.Parent.Parent.ContentName,

                        FrameworkIdentifier = (Guid?)x.Parent.StandardIdentifier,
                        FrameworkTitle = x.Parent.ContentTitle ?? x.Parent.ContentName,
                    },
                    x => areaIds.Contains(x.StandardIdentifier))
                .ToDictionary(x => x.AreaIdentifier);

            foreach (var q in questions)
            {
                if (!q.CompetencyAreaIdentifier.HasValue || !areaMapping.TryGetValue(q.CompetencyAreaIdentifier.Value, out var area))
                    continue;

                q.OccupationIdentifier = area.OccupationIdentifier;
                q.OccupationTitle = area.OccupationTitle;

                q.FrameworkIdentifier = area.FrameworkIdentifier;
                q.FrameworkTitle = area.FrameworkTitle;
            }

            var hasData = questions.Length > 0;

            RatioTab.Visible = hasData;

            if (hasData)
            {
                RatioTab.IsSelected = true;
                RatioTab.SetTitle("Ratio", questions.Select(x => x.AttemptIdentifier).Distinct().Count());
            }

            CompetencyRatio.LoadData(questions, GroupByColumns);
        }

        #endregion

        #region Event handlers

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            GetGroupBySelector();
            LoadData();
        }

        #endregion

        #region Methods (data binding)

        private void SetCriteriaValues()
        {
            const string emptyValue = "<i>(Empty)</i>";

            var filter = Filter;
            var organizationId = CurrentSessionState.Identity.Organization.Identifier;

            var occupation = filter.OccupationIdentifier.HasValue
                ? ServiceLocator.BankSearch.GetBankOccupation(organizationId, filter.OccupationIdentifier.Value)
                : null;
            var framework = filter.FrameworkIdentifier.HasValue
                ? ServiceLocator.BankSearch.GetBankFramework(organizationId, filter.FrameworkIdentifier.Value)
                : null;
            var bank = filter.BankIdentifier.HasValue
                ? ServiceLocator.BankSearch.GetBank(filter.BankIdentifier.Value)
                : null;
            var form = filter.FormIdentifier.HasValue
                ? ServiceLocator.BankSearch.GetForm(filter.FormIdentifier.Value)
                : null;
            var learner = filter.LearnerUserIdentifier.HasValue
                ? FindPerson.GetItemText(organizationId, filter.LearnerUserIdentifier.Value)
                : null;
            var @event = filter.EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(filter.EventIdentifier.Value)
                : null;

            SearchCriteriaExamBankOccupation.Text = GetValueString(occupation?.OccupationTitle);
            SearchCriteriaExamBankFramework.Text = GetValueString(framework?.FrameworkTitle);
            SearchCriteriaExamBank.Text = GetValueString(bank == null ? null : bank.BankName ?? bank.BankTitle);
            SearchCriteriaExamForm.Text = GetValueString(form == null ? null : form.FormName ?? form.FormTitle);
            SearchCriteriaExamCandidate.Text = GetValueString(learner);
            SearchCriteriaExamEvent.Text = GetValueString(@event == null ? null : $"{@event.EventType} {@event.EventNumber}-{@event.EventBillingType}: {@event.EventTitle}");
            SearchCriteriaEventFormat.Text = GetValueString(filter.EventFormat);

            SearchCriteriaAttemptTag.Text = GetValueStringArray(filter.AttemptTag);
            SearchCriteriaAttemptStartedSince.Text = GetValueDate(filter.AttemptStartedSince);
            SearchCriteriaAttemptStartedBefore.Text = GetValueDate(filter.AttemptStartedBefore);
            SearchCriteriaAttemptCompletedSince.Text = GetValueDate(filter.AttemptGradedSince);
            SearchCriteriaAttemptCompletedBefore.Text = GetValueDate(filter.AttemptGradedBefore);

            if (filter is AdHocAttemptFilter internalFilter)
            {
                SearchCriteriaIncludeAttemptMode.Text = GetValueString(internalFilter.IncludeOnlyFirstAttempt ? "First Only" : "All");
            }
            else
            {
                SearchCriteriaIncludeAttemptMode.Text = GetValueString(null);
            }

            string GetValueString(string input) =>
                string.IsNullOrEmpty(input) ? emptyValue : WebUtility.HtmlEncode(input);

            string GetValueStringArray(IEnumerable<string> input) =>
                input == null || !input.Any() ? emptyValue : WebUtility.HtmlEncode(string.Join(", ", input));

            string GetValueDate(DateTimeOffset? input) =>
                !input.HasValue ? emptyValue : input.Value.Format(isHtml: true);
        }

        private void SetGroupBySelector()
        {
            if (GroupByColumns.IsNotEmpty())
                GroupBySelector.Values = GroupByColumns.Select(x => x.GetName());
            else
                GroupBySelector.SelectAll();
        }

        private void GetGroupBySelector()
        {
            GroupByColumns = GroupBySelector.Values.Select(x => x.ToEnum<GroupByColumn>()).ToArray();
        }

        #endregion
    }
}