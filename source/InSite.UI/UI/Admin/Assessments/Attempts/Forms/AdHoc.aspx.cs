using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Controls;
using InSite.Admin.Assessments.Attempts.Models;
using InSite.Admin.Assessments.Attempts.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Common.Events;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    public partial class AdHoc : SearchPage<QAttemptFilter>
    {
        #region Classes

        public class AttemptData : AttemptAnalysis.AttemptEntity
        {
            public Guid? RegistrationIdentifier { get; private set; }
            public string AttemptTag { get; private set; }
            public int? AttemptDuration { get; private set; }
            public string AttemptGrade { get; private set; }

            public static new readonly Expression<Func<QAttempt, AttemptAnalysis.AttemptEntity>> Binder = LinqExtensions1.Expr<QAttempt, AttemptAnalysis.AttemptEntity>(x => new AttemptData
            {
                AttemptIdentifier = x.AttemptIdentifier,
                FormIdentifier = x.FormIdentifier,
                LearnerUserIdentifier = x.LearnerUserIdentifier,
                AttemptStarted = x.AttemptStarted,
                AttemptGraded = x.AttemptGraded,
                AttemptNumber = x.AttemptNumber,
                AttemptScore = x.AttemptScore,
                AttemptPoints = x.AttemptPoints,
                AttemptIsPassing = x.AttemptIsPassing,

                RegistrationIdentifier = x.RegistrationIdentifier,
                AttemptTag = x.AttemptTag,
                AttemptDuration = (int?)x.AttemptDuration,
                AttemptGrade = x.AttemptGrade
            });
        }

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

        public override string EntityName => "Attempt";

        protected override string SearchResultTitle => "Attempts";

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.DataStateChanged += SearchResults_DataStateChanged;

            ResultStatisticGacRepeater.ItemDataBound += ResultStatisticRepeater_ItemDataBound;
            ResultStatisticCompetencyRepeater.ItemDataBound += ResultStatisticRepeater_ItemDataBound;

            PageHelper.AutoBindHeader(this);
        }

        #endregion

        #region Event handlers

        private void ResultStatisticRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var statistic = (ResultStatisticCompetency)e.Item.FindControl("ResultStatisticCompetency");
            statistic.LoadData((AttemptAnalysis)DataBinder.Eval(e.Item.DataItem, "Analysis"));
        }

        private void SearchResults_DataStateChanged(object sender, BooleanValueArgs args)
        {
            SearchResultsTab.Visible = args.Value;
            TimeSeriesTab.Visible = args.Value;
            BellCurveTab.Visible = args.Value;
            QuestionAnalysisTab.Visible = args.Value;
            CommentTab.Visible = args.Value;
            StandardsTab.Visible = args.Value;
            RatioButton.Visible = args.Value;
        }

        protected override void OnSearching(QAttemptFilter filter)
        {
            var internalFilter = (AdHocAttemptFilter)filter;

            AttemptAnalysis analysis;
            if (filter.FrameworkIdentifier.HasValue && filter.FrameworkIdentifier.Value != Guid.Empty)
            {
                var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch);
                {
                    settings.Filter = filter.Clone();
                    settings.Filter.OrderBy = null;

                    settings.AttemptEntityBinder = AttemptData.Binder;
                    settings.QuestionEntityBinder = QuestionData.Binder;

                    settings.IncludeOnlyFirstAttempt = internalFilter.IncludeOnlyFirstAttempt;
                }

                analysis = AttemptAnalysis.Create(settings);
            }
            else
            {
                analysis = AttemptAnalysis.Empty();
            }

            LoadSearchResults(analysis);

            base.OnSearching(filter);

            var occupations = StandardSummary.GetData(analysis, Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection);

            TimeSeriesChart.LoadData(analysis);
            ResultStatisticExam.LoadData(analysis, filter);
            var questionCount = QuestionAnalysisRepeater.LoadData(analysis, filter, null);
            var commentCount = QuestionCommentsRepeater.LoadData(filter);
            CompetencySummary.LoadData(occupations, analysis, filter);

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

            QuestionAnalysisTab.Title = Global.Translate("Question Analysis") + $"<span class='badge rounded-pill bg-info ms-2'>{questionCount}</span>";
            CommentTab.Title = Global.Translate("Comments") + $"<span class='badge rounded-pill bg-info ms-2'>{commentCount}</span>";
        }

        private void LoadSearchResults(AttemptAnalysis analysis)
        {
            var forms = new Dictionary<Guid, QBankForm>();
            var candidates = new Dictionary<Guid, VPerson>();
            var registrations = new Dictionary<Guid, QRegistration>();

            foreach (AttemptData a in analysis.Attempts)
            {
                if (!forms.ContainsKey(a.FormIdentifier))
                    forms.Add(a.FormIdentifier, null);

                if (!candidates.ContainsKey(a.LearnerUserIdentifier))
                    candidates.Add(a.LearnerUserIdentifier, null);

                if (a.RegistrationIdentifier.HasValue && !registrations.ContainsKey(a.RegistrationIdentifier.Value))
                    registrations.Add(a.RegistrationIdentifier.Value, null);
            }

            {
                var entities = ServiceLocator.BankSearch.GetForms(forms.Keys, x => x.VBank);
                foreach (var entity in entities)
                    forms[entity.FormIdentifier] = entity;
            }

            {
                var entities = ServiceLocator.ContactSearch.GetPersons(candidates.Keys, Organization.Identifier);
                foreach (var group in entities.GroupBy(x => x.UserIdentifier))
                {
                    var entity = group.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier);

                    if (entity == null)
                    {
                        entity = group.FirstOrDefault();
                        entity.PersonCode = null;
                        entity.SocialInsuranceNumber = null;
                    }

                    candidates[entity.UserIdentifier] = entity;
                }
            }

            {
                var entities = ServiceLocator.RegistrationSearch.GetRegistrations(new QRegistrationFilter 
                { 
                    RegistrationIdentifiers = registrations.Keys.ToArray(), 
                    HasEvent = true 
                });
                foreach (var entity in entities)
                    registrations[entity.RegistrationIdentifier] = entity;
            }

            SearchResults.DataItems = analysis.Attempts
                .Cast<AttemptData>().Select(x =>
                {
                    var form = forms.ContainsKey(x.FormIdentifier) ? forms[x.FormIdentifier] : null;
                    var candidate = candidates.ContainsKey(x.LearnerUserIdentifier) ? candidates[x.LearnerUserIdentifier] : null;
                    var registration = x.RegistrationIdentifier.HasValue && registrations.ContainsKey(x.RegistrationIdentifier.Value) ? registrations[x.RegistrationIdentifier.Value] : null;

                    return new AdHocAttemptDataItem(x, form, candidate, registration);
                })
                .ToArray();
        }

        #endregion

        #region Methods (export)

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("ProfileTitle", "Profile", null, 45),
                new DownloadColumn("FrameworkTitle", "Framework", null, 45),
                new DownloadColumn("FormName", "Form", null, 45),
                new DownloadColumn("FormAssetVersion", "Form Version", null, 15),
                new DownloadColumn("FormAsset", "Form Asset", null, 15),
                new DownloadColumn("CandidateName", "Candidate Name", null, 30),
                new DownloadColumn("CandidateCode", "Candidate Code", null, 15),
                new DownloadColumn("EventFormat", "Event Format", null, 15),
                new DownloadColumn("AttemptTag", "Attempt Tag", null, 30),
                new DownloadColumn("AttemptStartedText", "Attempt Started", null, 20, HorizontalAlignment.Right),
                new DownloadColumn("AttemptCompletedText", "Attempt Completed", null, 20, HorizontalAlignment.Right),
                new DownloadColumn("AttemptDuration", "Attempt Duration", null, 20, HorizontalAlignment.Right),
                new DownloadColumn("AttemptScore", "Attempt Score", null, 15, HorizontalAlignment.Right),
                new DownloadColumn("AttemptGrade", "Attempt Grade", null, 15)
            };
        }

        #endregion
    }
}