using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDistributionSearchResults : BaseUserControl
    {
        #region Classes

        private class QuestionInfo : IReportQuestion
        {
            #region Properties

            public Guid QuestionID { get; }

            public SurveyQuestionType QuestionType { get; }

            public string QuestionText { get; set; }

            public string QuestionSubText { get; set; }

            public SubmissionAnalysis Analysis { get; }

            public bool NumericEnableAnalysis { get; set; }

            public bool ListEnableQualifierAfterLastOption { get; set; }

            public OptionInfoCollection Options { get; }

            int IReportQuestion.FrequencySum => Options.FrequencySum;

            IEnumerable<IReportOption> IReportQuestion.Options => Options;

            Guid IReportQuestion.ID => QuestionID;

            string IReportQuestion.Text => QuestionText;

            #endregion

            #region Construction

            public QuestionInfo(SurveyQuestion question, SubmissionAnalysis analysis)
            {
                QuestionID = question.Identifier;
                QuestionType = question.Type;
                QuestionText = question.Content.Title.GetHtml(question.Form.Language);

                NumericEnableAnalysis = question.NumberEnableStatistics;
                ListEnableQualifierAfterLastOption = question.ListEnableOtherText;

                Analysis = analysis;
                Options = new OptionInfoCollection(this);
            }

            public void Load(IEnumerable<SurveyOptionItem> list, string language)
            {
                foreach (var o in list)
                {
                    var optionAnalysis = Analysis.SelectionAnalysis.GetAnalysisForQuestionAndAnswer(QuestionID, o.Identifier);
                    if (optionAnalysis == null)
                        continue;

                    var option = new OptionInfo(
                        o.Identifier,
                        o.Content.Title.GetText(language),
                        o.Category,
                        o.Points);

                    if (string.IsNullOrWhiteSpace(option.Text))
                        continue;

                    Options.Add(option);
                }
            }

            #endregion
        }

        private class OptionInfo : ReportBaseOption
        {
            #region Properties

            public override Guid ID { get; }

            public override string Text { get; }

            public override string Category { get; }

            public override decimal Score { get; }

            public override IReportQuestion Question => _collection.Question;

            #endregion

            #region Fields

            private OptionInfoCollection _collection;

            #endregion

            #region Construction

            public OptionInfo(Guid id, string text, string category, decimal score)
            {
                ID = id;
                Text = text;
                Category = category;
                Score = score;
            }

            #endregion

            #region Methods

            internal void SetCollection(OptionInfoCollection collection)
            {
                if (_collection != null)
                    throw new ApplicationError("Options collection is already assigned.");

                _collection = collection;
            }

            #endregion
        }

        private class OptionInfoCollection : IEnumerable<OptionInfo>
        {
            #region Properties

            public int Count => _items.Count;

            public int FrequencySum => _items.Sum(x => x.Frequency);

            public QuestionInfo Question { get; private set; }

            #endregion

            #region Fields

            private readonly List<OptionInfo> _items = new List<OptionInfo>();

            #endregion

            #region Construction

            public OptionInfoCollection(QuestionInfo question)
            {
                Question = question;
            }

            #endregion

            #region Methods

            public void Add(OptionInfo item)
            {
                item.SetCollection(this);

                _items.Add(item);
            }

            #endregion

            #region IEnumerable

            public IEnumerator<OptionInfo> GetEnumerator() => _items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion

        #region Properties

        public QResponseAnalysisFilter Filter
        {
            get => (QResponseAnalysisFilter)ViewState[nameof(Filter)];
            private set => ViewState[nameof(Filter)] = value;
        }

        public bool Print
        {
            get { return ViewState[nameof(Print)] != null && (bool)ViewState[nameof(Print)]; }
            set { ViewState[nameof(Print)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionRepeater.ItemCreated += QuestionRepeater_ItemCreated;
            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void QuestionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var analysisRepeater = (Repeater)e.Item.FindControl("AnalysisRepeater");
            analysisRepeater.ItemDataBound += AnalysisRepeater_ItemDataBound;
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (QuestionInfo)e.Item.DataItem;
            var analysisInitializers = new List<Action<DynamicControl>>();

            if (Filter.ShowSelections && (dataItem.QuestionType == SurveyQuestionType.RadioList || dataItem.QuestionType == SurveyQuestionType.Selection || dataItem.QuestionType == SurveyQuestionType.CheckList || dataItem.QuestionType == SurveyQuestionType.Likert))
            {
                analysisInitializers.Add(container =>
                {
                    var analysis = (ReportDistributionAnalysisSelection)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/ReportDistributionAnalysisSelection.ascx");
                    analysis.LoadResponses(dataItem);
                });

                if (dataItem.QuestionType == SurveyQuestionType.RadioList && dataItem.Options.Any(x => !string.IsNullOrEmpty(x.Category)))
                {
                    analysisInitializers.Add(container =>
                    {
                        var analysis = (ReportDistributionAnalysisSelection)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/ReportDistributionAnalysisSelection.ascx");
                        analysis.LoadCategories(dataItem);
                    });
                }
            }

            if (Filter.ShowNumbers && dataItem.QuestionType == SurveyQuestionType.Number)
            {
                analysisInitializers.Add(container =>
                {
                    var analysis = (ReportDistributionAnalysisNumber)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/ReportDistributionAnalysisNumber.ascx");
                    analysis.LoadData(dataItem);
                });
            }

            if (Filter.ShowComments)
            {
                if (dataItem.QuestionType == SurveyQuestionType.Text || dataItem.QuestionType == SurveyQuestionType.Date)
                {
                    analysisInitializers.Add(container =>
                    {
                        var analysis = (ReportDistributionAnalysisText)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/ReportDistributionAnalysisText.ascx");
                        analysis.LoadData(dataItem.QuestionText, dataItem.Analysis.TextAnalysis.Where(x => x.QuestionID == dataItem.QuestionID));
                    });
                }
                else if (dataItem.QuestionType == SurveyQuestionType.Comment)
                {
                    analysisInitializers.Add(container =>
                    {
                        var analysis = (ReportDistributionAnalysisText)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/ReportDistributionAnalysisText.ascx");
                        analysis.LoadData(dataItem.QuestionText, dataItem.Analysis.CommentAnalysis.Where(x => x.QuestionID == dataItem.QuestionID));
                    });
                }

                if (dataItem.ListEnableQualifierAfterLastOption)
                {
                    analysisInitializers.Add(container =>
                    {
                        var literal = container.LoadControl<LiteralControl>();
                        literal.Text = "<br />";
                    });

                    analysisInitializers.Add(container =>
                    {
                        var analysis = (ReportDistributionAnalysisText)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/ReportDistributionAnalysisText.ascx");
                        analysis.LoadData(dataItem.QuestionText, dataItem.Analysis.CommentAnalysis.Where(x => x.QuestionID == dataItem.QuestionID));
                    });
                }
            }

            var analysisRepeater = (Repeater)e.Item.FindControl("AnalysisRepeater");
            analysisRepeater.DataSource = analysisInitializers;
            analysisRepeater.DataBind();
        }

        private void AnalysisRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var initializer = (Action<DynamicControl>)e.Item.DataItem;
            var container = (DynamicControl)e.Item.FindControl("AnalysisContainer");

            initializer(container);
        }

        #endregion

        #region Search results

        public void Search(QResponseAnalysisFilter filter, QResponseSessionFilter responseFilter = null)
        {
            QuestionRepeater.DataSource = null;

            MapFResponseAnalysisFilterilter(filter, responseFilter);

            if (filter.SurveyFormIdentifier.HasValue)
            {
                var survey = ServiceLocator.SurveySearch.GetSurveyState(filter.SurveyFormIdentifier.Value);

                if (survey.Form.Tenant != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                {
                    Visible = false;

                    var screenStatus = (Alert)ControlHelper.GetControl(Page, "ScreenStatus");
                    screenStatus.Text = $"Your customer account " +
                        $"({CurrentSessionState.Identity.Organization.Name}) does not have access to form \"{survey.Form.Name}\".";
                    screenStatus.Indicator = AlertType.Error;

                    return;
                }

                Filter = filter;

                if (!Visible)
                    Visible = true;

                var helper = new SubmissionAnalysis(filter, CurrentSessionState.Identity.User.Email);
                var language = survey.Form.Language;
                var controlTypes = GetControlTypes(filter).ToHashSet();
                var questions = new List<QuestionInfo>();

                foreach (var q in survey.Form.Questions)
                {
                    if (!controlTypes.Contains(q.Type))
                        continue;

                    if (q.Type == SurveyQuestionType.Likert)
                    {
                        for (var i = 0; i < q.Options.Lists.Count; i++)
                        {
                            var list = q.Options.Lists[i];

                            var question = new QuestionInfo(q, helper);

                            if (i > 0)
                                question.QuestionText = string.Empty;

                            if (list.Content != null)
                                question.QuestionSubText = list.Content.Title.GetText();

                            question.Load(list.Items, language);

                            if (question != null && (question.Options.Count > 0 || helper.CommentAnalysis.Any(x => x.QuestionID == question.QuestionID)))
                                questions.Add(question);
                        }
                    }
                    else if (q.Type == SurveyQuestionType.RadioList || q.Type == SurveyQuestionType.Selection || q.Type == SurveyQuestionType.CheckList)
                    {
                        var list = q.Options.Lists.FirstOrDefault();
                        if (list == null)
                            continue;

                        var question = new QuestionInfo(q, helper);

                        question.Load(list.Items, language);

                        if (question != null && (question.Options.Count > 0 || helper.CommentAnalysis.Any(x => x.QuestionID == question.QuestionID)))
                            questions.Add(question);
                    }
                    else if (q.Type == SurveyQuestionType.Number)
                    {
                        var numberAnalysis = helper.NumberAnalysis.GetData(q.Identifier);
                        if (numberAnalysis != null)
                            questions.Add(new QuestionInfo(q, helper));
                    }
                    else if (q.Type == SurveyQuestionType.Text || q.Type == SurveyQuestionType.Date)
                    {
                        if (helper.TextAnalysis.Any(x => x.QuestionID == q.Identifier))
                            questions.Add(new QuestionInfo(q, helper));
                    }
                    else if (q.Type == SurveyQuestionType.Comment)
                    {
                        if (helper.CommentAnalysis.Any(x => x.QuestionID == q.Identifier))
                            questions.Add(new QuestionInfo(q, helper));
                    }
                }

                QuestionRepeater.DataSource = questions;
            }

            QuestionRepeater.DataBind();
        }

        private static void MapFResponseAnalysisFilterilter(QResponseAnalysisFilter filter, QResponseSessionFilter responseFilter)
        {
            if (responseFilter != null)
            {
                filter.OrganizationIdentifier = responseFilter.OrganizationIdentifier;
                filter.SurveyQuestionIdentifier = responseFilter.SurveyQuestionIdentifier;
                filter.ResponseAnswerText = responseFilter.ResponseAnswerText;
                filter.RespondentName = responseFilter.RespondentName;
                filter.StartedSince = responseFilter.StartedSince;
                filter.StartedBefore = responseFilter.StartedBefore;
                filter.CompletedSince = responseFilter.CompletedSince;
                filter.CompletedBefore = responseFilter.CompletedBefore;
                filter.IsPlatformAdministrator = responseFilter.IsPlatformAdministrator;
                filter.IsLocked = responseFilter.IsLocked;
                filter.GroupIdentifier = responseFilter.GroupIdentifier;
            }
        }

        public void Clear(QResponseAnalysisFilter filter)
        {
            QuestionRepeater.DataSource = null;
            QuestionRepeater.DataBind();

            Filter = filter;
        }

        #endregion

        #region Helpers

        private static IEnumerable<SurveyQuestionType> GetControlTypes(QResponseAnalysisFilter filter)
        {
            if (filter.ShowSelections)
                yield return SurveyQuestionType.Selection;

            if (filter.ShowSelections || filter.ShowText)
            {
                yield return SurveyQuestionType.CheckList;
                yield return SurveyQuestionType.RadioList;
                yield return SurveyQuestionType.Likert;
            }

            if (filter.ShowText)
            {
                yield return SurveyQuestionType.Comment;
                yield return SurveyQuestionType.Date;
                yield return SurveyQuestionType.Text;
            }

            if (filter.ShowNumbers)
            {
                yield return SurveyQuestionType.Number;
            }
        }

        #endregion
    }
}