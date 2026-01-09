using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Controls;
using InSite.Admin.Assessments.Attempts.Models;
using InSite.Admin.Assessments.Attempts.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    public partial class Report : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/assessments/attempts/reports/search";

        #endregion

        #region Classes

        private class QuestionData : AttemptAnalysis.QuestionEntity, QuestionCompetencySummaryRepeater.IQuestion
        {
            public string QuestionText { get; private set; }

            public static new readonly Expression<Func<QAttemptQuestion, AttemptAnalysis.QuestionEntity>> Binder = LinqExtensions1.Expr<QAttemptQuestion, AttemptAnalysis.QuestionEntity>(x => new QuestionData
            {
                AttemptIdentifier = x.AttemptIdentifier,
                QuestionIdentifier = x.QuestionIdentifier,
                ParentQuestionIdentifier = x.ParentQuestionIdentifier,
                QuestionSequence = x.QuestionSequence,
                QuestionPoints = x.QuestionPoints,
                AnswerPoints = x.AnswerPoints,
                AnswerOptionKey = x.AnswerOptionKey,

                CompetencyAreaIdentifier = x.CompetencyAreaIdentifier,
                CompetencyAreaLabel = x.CompetencyAreaLabel,
                CompetencyAreaCode = x.CompetencyAreaCode,
                CompetencyAreaTitle = x.CompetencyAreaTitle,

                CompetencyItemIdentifier = x.CompetencyItemIdentifier,
                CompetencyItemLabel = x.CompetencyItemLabel,
                CompetencyItemCode = x.CompetencyItemCode,
                CompetencyItemTitle = x.CompetencyItemTitle,

                QuestionText = x.QuestionText
            });
        }

        #endregion

        #region Properties

        private QAttemptFilter Filter
        {
            get => (QAttemptFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"asset={Request.QueryString["asset"]}"
                : null;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var codes = OrganizationSearch.SelectAll()
                .Where(x => Identity.Organizations.Any(y => y.OrganizationIdentifier == x.Identifier))
                .Select(x => x.Code).ToArray();

            SearchOrganizationIdentifier.Filter.IncludeOrganizationCode = codes;
            SearchOrganizationIdentifier.Value = Identity.Organization.Identifier;
            SearchOrganizationIdentifier.AutoPostBack = true;
            SearchOrganizationIdentifier.ValueChanged += (x, y) => BindModelToControls();

            ResultStatisticGacRepeater.ItemDataBound += ResultStatisticRepeater_ItemDataBound;
            ResultStatisticCompetencyRepeater.ItemDataBound += ResultStatisticRepeater_ItemDataBound;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AttemptTab.Visible = true;

            if (IsPostBack)
                return;

            BindModelToControls();

            CloseButton.NavigateUrl = SearchUrl;
        }

        private void BindModelToControls()
        {
            Filter = LoadFilter();

            if (Filter == null)
                HttpResponseHelper.Redirect(SearchUrl);

            if (IsNullOrEmpty(Filter.BankIdentifier) && IsNullOrEmpty(Filter.FormIdentifier) && IsNullOrEmpty(Filter.EventIdentifier))
            {
                NavPanel.Visible = false;

                ScreenStatus.AddMessage(
                    AlertType.Error,
                    "The search criteria filter does not have enough criteria to limit your search results to an acceptable number of records. " +
                    $"Please go to <a href='{SearchUrl}'>Search</a> page to specify the Exam Bank or Exam Event criteria field to run this report.");

                return;
            }

            SetupFilterOutputs();
            LoadData();

            bool IsNullOrEmpty(Guid? value)
            {
                return !value.HasValue || value.Value == Guid.Empty;
            }
        }

        private QAttemptFilter LoadFilter()
        {
            if (Guid.TryParse(Request.QueryString["form"], out Guid form))
                return new QAttemptFilter { FormIdentifier = form };

            var searchAction = SearchUrl.Substring(1);
            var report = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Search
                  && x.ReportDescription == searchAction);

            if (report == null)
                return null;

            var settings = ReportRequest.Deserialize(report.ReportData)?.GetSearch<QAttemptFilter>();

            return settings?.Filter;
        }

        #endregion

        #region Event handlers

        private void ResultStatisticRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var statistic = (ResultStatisticCompetency)e.Item.FindControl("ResultStatisticCompetency");
            statistic.LoadData((AttemptAnalysis)DataBinder.Eval(e.Item.DataItem, "Analysis"));
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            Response.SendFile("attempt_report", "xlsx", AttemptReportExport.GetXlsx(Filter, IncludeAdditionalSheets.Checked));
        }

        #endregion

        #region Methods (data binding)

        private void SetupFilterOutputs()
        {
            const string emptyValue = "<i>(Empty)</i>";

            var filter = Filter;

            var bank = filter.BankIdentifier.HasValue && filter.BankIdentifier != Guid.Empty
                ? ServiceLocator.BankSearch.GetBank(filter.BankIdentifier.Value)
                : null;
            SearchCriteriaExamBank.Text = bank != null ? bank.BankName ?? bank.BankTitle : GetValueString(null);

            var form = filter.FormIdentifier.HasValue
                ? ServiceLocator.BankSearch.GetForm(filter.FormIdentifier.Value)
                : null;
                SearchCriteriaExamForm.Text = form != null ? form.FormName : GetValueString(null);

            var candidate = filter.LearnerUserIdentifier.HasValue
                ? ServiceLocator.ContactSearch.GetPerson(filter.LearnerUserIdentifier.Value, Organization.Identifier)
                : null;
            SearchCriteriaExamCandidate.Text = candidate != null
                ? $"{candidate.UserFullName} <span class='form-text'>{candidate.PersonCode}</span>"
                : GetValueString(null);

            SearchCriteriaCandidateName.Text = GetValueString(filter.LearnerName);

            var exam = filter.EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(filter.EventIdentifier.Value)
                : null;
            SearchCriteriaExamEvent.Text = exam != null
                ? $"{exam.EventType} {exam.EventNumber}-{exam.EventBillingType}: {exam.EventTitle}"
                : GetValueString(null);

            SearchCriteriaEventFormat.Text = GetValueString(filter.EventFormat);
            SearchCriteriaExamEvent.Text = GetValueString(filter.Event);
            SearchCriteriaAttemptTag.Text = GetValueStringArray(filter.AttemptTag);

            SearchCriteriaStartedSince.Text = GetValueDate(filter.AttemptStartedSince);
            SearchCriteriaStartedBefore.Text = GetValueDate(filter.AttemptStartedBefore);
            SearchCriteriaCompletedSince.Text = GetValueDate(filter.AttemptGradedSince);
            SearchCriteriaCompletedBefore.Text = GetValueDate(filter.AttemptGradedBefore);
            SearchCriteriaPilotAttempts.Text = GetValueString(GetGetPilotAttemptsInclusionString(filter.PilotAttemptsInclusion));
            SearchCriteriaCandidateType.Text = GetValueStringArray(filter.CandidateType);
            SearchCriteriaPendingAttempts.Text = filter is AttemptReportFilter reportFilter && reportFilter.IncludePendingAttempts
                ? "Include"
                : "Exclude";

            string GetGetPilotAttemptsInclusionString(InclusionType input)
            {
                if (input == InclusionType.Include)
                    return "Include";
                else if (input == InclusionType.Exclude)
                    return "Exclude Pilot Attempts";
                else if (input == InclusionType.Only)
                    return "Pilot Attempts Only";

                return null;
            }

            string GetValueString(string input) =>
                string.IsNullOrEmpty(input) ? emptyValue : WebUtility.HtmlEncode(input);

            string GetValueStringArray(IEnumerable<string> input) =>
                input == null || !input.Any() ? emptyValue : WebUtility.HtmlEncode(string.Join(", ", input));

            string GetValueDate(DateTimeOffset? input) =>
                !input.HasValue ? emptyValue : input.Value.Format(isHtml: true);
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            var questionId = Guid.TryParse(Request.QueryString["question"], out var tempQuestionId) ? tempQuestionId : (Guid?)null;
            var panel = Request.QueryString["panel"];

            var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch);
            {
                var filter = Filter.Clone();
                filter.FormOrganizationIdentifier = SearchOrganizationIdentifier.Value ?? Guid.Empty;
                filter.OrderBy = null;

                if (filter is AttemptReportFilter reportFilter && reportFilter.IncludePendingAttempts)
                {
                    filter.IsSubmitted = true;
                    filter.IsCompleted = null;
                }
                else
                {
                    filter.IsCompleted = true;
                }

                settings.Filter = filter;
                settings.QuestionEntityBinder = QuestionData.Binder;
            }

            var analysis = AttemptAnalysis.Create(settings);
            if (!analysis.HasData)
            {
                ScreenStatus.AddMessage(AlertType.Information, "No completed attempts were found that match the search criteria.");
                NavPanel.Visible = false;
                return;
            }

            var occupations = StandardSummary.GetData(analysis, Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection);

            TimeSeriesChart.LoadData(analysis);
            ResultStatisticExam.LoadData(analysis, settings.Filter);
            QuestionAnalysisRepeater.LoadData(analysis, settings.Filter, panel == "questionAnalysis" ? questionId : null);
            QuestionCommentsRepeater.LoadData(settings.Filter);
            AttemptGrid.LoadData(settings.Filter);
            CompetencySummary.LoadData(occupations, analysis, settings.Filter);

            ResultStatisticGacRepeater.DataSource = occupations
                .SelectMany(x => x.Frameworks).SelectMany(x => x.Gacs)
                .Where(x => x.ID != Guid.Empty).OrderBy(x => x.Name)
                .Select(gac => new
                {
                    gac.Name,
                    Analysis = analysis.FilterQuestion(q => ((QuestionData)q).CompetencyAreaIdentifier == gac.ID)
                });
            ResultStatisticGacRepeater.DataBind();

            ResultStatisticCompetencyRepeater.DataSource = occupations
                .SelectMany(x => x.Frameworks)
                .SelectMany(x => x.Gacs)
                .Where(x => x.ID != Guid.Empty).OrderBy(x => x.Name)
                .SelectMany(gac => gac.Competencies.Where(comp => comp.ID != Guid.Empty).OrderBy(comp => comp.Name).Select(comp => new
                {
                    GacName = gac.Name,
                    CompetencyName = comp.Name,
                    Analysis = analysis.FilterQuestion(q => ((QuestionData)q).CompetencyItemIdentifier == comp.ID)
                }));
            ResultStatisticCompetencyRepeater.DataBind();

            if (panel == "questionAnalysis")
                QuestionAnalysisTab.IsSelected = true;
            else
                AttemptTab.IsSelected = true;
        }

        #endregion
    }
}