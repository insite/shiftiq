using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class QuestionsManager : BaseUserControl
    {
        #region Properties

        public event EventHandler Copied;

        protected Guid? SurveyID
        {
            get => (Guid?)ViewState[nameof(SurveyID)];
            set => ViewState[nameof(SurveyID)] = value;
        }

        private ICollection<Guid> SuppressionMaskOptions => (ICollection<Guid>)(ViewState[nameof(SuppressionMaskOptions)] ??
            (ViewState[nameof(SuppressionMaskOptions)] = new HashSet<Guid>()));

        public int QuestionCount => QuestionRepeater.Items.Count;

        protected bool CanEdit
        {
            get => ViewState[nameof(CanEdit)] as bool? ?? false;
            set => ViewState[nameof(CanEdit)] = value;
        }

        private List<Guid> QuestionIdentifiers
        {
            get => (List<Guid>)ViewState[nameof(QuestionIdentifiers)];
            set => ViewState[nameof(QuestionIdentifiers)] = value;
        }

        protected int MaxOptionCount => 5;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SurveyPage.AutoPostBack = true;
            SurveyPage.ValueChanged += SurveyPage_ValueChanged;

            FilterButton.Click += FilterButton_Click;

            DownloadPdfButton.Click += DownloadPdfButton_Click;

            QuestionRepeater.DataBinding += QuestionRepeater_DataBinding;
            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
            QuestionRepeater.ItemCommand += QuestionRepeater_ItemCommand;

            AddNewQuestion.Click += AddNewQuestion_Click;
            AddNewQuestionBottom.Click += AddNewQuestion_Click;

            CommonScript.ContentKey = typeof(QuestionsManager).FullName;
        }

        #endregion

        #region Event handlers

        private void DownloadPdfButton_Click(object sender, EventArgs e)
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyID.Value);

            var report = (QuestionsReport)LoadControl("~/UI/Admin/Workflow/Forms/Controls/QuestionsReport.ascx");
            report.LoadReport(survey);

            var html = new StringBuilder();
            using (var stringWriter = new StringWriter(html))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    report.RenderControl(htmlWriter);
            }

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                MarginTop = 26.5f,
                MarginBottom = 26.5f,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Workflow/Forms/Content/QuestionsManagerHeader.html"),
                HeaderSpacing = 10,

                FooterUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Workflow/Forms/Content/QuestionsManagerFooter.html")
            };

            var title = survey != null ? survey.Form.Name : "error";
            var fileName = StringHelper.Sanitize(title, '_');

            var data = HtmlConverter.HtmlToPdf(html.ToString(), settings);

            Response.SendFile($"{fileName}", "pdf", data);
        }

        private void AddNewQuestion_Click(object sender, EventArgs e)
        {
            var url = $"/ui/admin/workflow/forms/questions/add?form={SurveyID}";

            var page = SurveyPage.ValueAsInt;
            if (page.HasValue)
                url += $"&page={page.Value}";

            Response.Redirect(url, true);
        }

        #endregion

        #region Data binding

        private void SurveyPage_ValueChanged(object sender, EventArgs e)
        {
            RebindQuestions();
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            RebindQuestions();
        }

        public void LoadData(SurveyForm survey, int? pageNumber, Guid? questionId, bool canEdit)
        {
            SurveyID = survey.Identifier;
            CanEdit = canEdit;
            var responseCount = ServiceLocator.SurveySearch.CountResponseSessions(new QResponseSessionFilter() { SurveyFormIdentifier = survey.Identifier });
            bool isPublished = survey.Status == SurveyFormStatus.Opened && responseCount > 0;

            EnsureChildControls();

            SuppressionMaskOptions.Clear();

            var suppressionMaskCollection = ServiceLocator.SurveySearch.GetSurveyConditions(new QSurveyConditionFilter { SurveyFormIdentifier = SurveyID.Value });
            foreach (var conditionInfo in suppressionMaskCollection)
                SuppressionMaskOptions.Add(conditionInfo.MaskingSurveyOptionItemIdentifier);

            var pages = survey.GetPages();

            BindPages(pages);

            if (pageNumber.HasValue)
                SurveyPage.ValueAsInt = pageNumber;

            BindQuestions(survey, pages);

            if (questionId.HasValue && questionId != Guid.Empty)
            {
                var index = QuestionIdentifiers.IndexOf(questionId.Value);
                if (index >= 0)
                    ScriptManager.RegisterStartupScript(
                        Page,
                        typeof(QuestionsManager),
                        "scroll",
                        $"questionsManager.scrollToQuestion({index});",
                        true);
            }

            ReorderQuestions.NavigateUrl = $"/ui/admin/workflow/forms/questions/reorder?form={SurveyID}";

            AddOptionList.Visible = CanEdit;
            AddNewQuestion.Visible = CanEdit;
            AddNewQuestionBottom.Visible = CanEdit;
            ReorderField.Visible = CanEdit && !isPublished;
        }

        private void QuestionRepeater_DataBinding(object sender, EventArgs e)
        {
            QuestionIdentifiers = new List<Guid>();
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var info = (SurveyQuestion)e.Item.DataItem;

            QuestionIdentifiers.Add(info.Identifier);

            var sequenceLabel = (ITextControl)e.Item.FindControl("SequenceLabel");
            var indicator = info.Code.HasValue() ? info.Code : info.Sequence.ToString();
            var indicatorStyle = info.GetIndicatorStyleName();

            sequenceLabel.Text = $"<span class='badge bg-{indicatorStyle}'>{indicator}</span>";

            var questionType = (ITextControl)e.Item.FindControl("SurveyQuestionType");
            questionType.Text = GetQuestionType(info);

            if (!info.Options.IsEmpty)
            {
                var isLikert = info.Type == SurveyQuestionType.Likert;

                var optionListPanel = (Panel)e.Item.FindControl("OptionListPanel");
                optionListPanel.Visible = true;

                var lists = info.Options.Lists
                    .Select(x => new
                    {
                        x.Sequence,
                        x.Identifier,
                        info.ListEnableBranch,
                        TitleHtml = Markdown.ToHtml(x.Content?.Title?.Text.Default),
                        Items = x.Items.Select(y => new
                        {
                            Letter = GetLetter(y.Sequence),
                            TitleHtml = Markdown.ToHtml(y.Content?.Title?.Text.Default),
                            PointsText = y.Points != 0 ? $"{y.Points:n2} points" : ""
                        })
                        .ToList(),
                        IsLikert = isLikert
                    })
                    .ToList();

                var optionListRepeater = (Repeater)e.Item.FindControl("OptionListRepeater");
                optionListRepeater.ItemDataBound += OptionListRepeater_ItemDataBound;
                optionListRepeater.DataSource = lists;
                optionListRepeater.DataBind();
            }

            var addConditionPanel = e.Item.FindControl("AddConditionPanel");
            addConditionPanel.Visible = CanEdit && !info.Options.IsEmpty && info.Options.Lists.Any(x => !x.IsEmpty);
        }

        private void QuestionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Copy")
            {
                var questionIdentifier = QuestionIdentifiers[e.Item.ItemIndex];
                var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyID.Value);
                var question = survey.Form.FindQuestion(questionIdentifier);

                var clone = question.Clone();
                clone.Identifier = UniqueIdentifier.Create();

                foreach (var list in clone.Options.Lists)
                {
                    list.Identifier = UniqueIdentifier.Create();

                    foreach (var option in list.Items)
                        option.Identifier = UniqueIdentifier.Create();
                }

                var commands = new SurveyCommandGenerator().GetCommands(survey.Form.Identifier, clone);
                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);

                var updatedSurvey = ServiceLocator.SurveySearch.GetSurveyState(SurveyID.Value);

                LoadData(updatedSurvey.Form, SurveyPage.ValueAsInt, question.Identifier, CanEdit);

                Copied?.Invoke(this, new EventArgs());
            }
        }

        private void OptionListRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var items = (IList)DataBinder.Eval(e.Item.DataItem, "Items");

            var optionItemRepeater = (Repeater)e.Item.FindControl("OptionItemRepeater");
            optionItemRepeater.DataSource = items;
            optionItemRepeater.DataBind();

            var showExtraOptionsLink = (HtmlAnchor)e.Item.FindControl("ShowExtraOptions");

            showExtraOptionsLink.Visible = items.Count > MaxOptionCount;
        }

        private void BindPages(IEnumerable<SurveyPage> pages)
        {
            SurveyPage.Items.Clear();
            SurveyPage.Items.Add(new ComboBoxOption());

            foreach (var page in pages)
                SurveyPage.Items.Add(new ComboBoxOption($"Page {page.Sequence}", page.Sequence.ToString()));
        }

        private void RebindQuestions()
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyID.Value);

            BindQuestions(survey.Form, survey.Form.GetPages());
        }

        private void BindQuestions(SurveyForm form, IList<SurveyPage> pages)
        {
            IQueryable<SurveyQuestion> questions = null;

            var pageNumber = SurveyPage.ValueAsInt;
            if (pageNumber.HasValue)
                questions = pages.FirstOrDefault(p => p.Sequence == pageNumber.Value)?.Questions.AsQueryable();

            if (questions == null)
                questions = form.Questions.AsQueryable();

            var questionText = FilterTextBox.Text;
            if (questionText.IsNotEmpty())
                questions = questions
                    .Where(q => q != null && q.Content != null && q.Content.Title.Text.Default != null
                             && q.Content.Title.Text.Default.Contains(questionText, StringComparison.OrdinalIgnoreCase));

            QuestionRepeater.DataSource = questions;
            QuestionRepeater.DataBind();
        }

        #endregion

        #region Helper methods

        protected string GetTitle(Guid id)
        {
            return ServiceLocator.ContentSearch.GetHtml(id, ContentLabel.Title);
        }

        protected string GetHint(Guid id)
        {
            return ServiceLocator.ContentSearch.GetHtml(id, ContentLabel.Hint);
        }

        protected string GetLetter(int sequence)
        {
            return Calculator.ToBase26(sequence);
        }

        public string GetQuestionType(SurveyQuestion question)
        {
            var type = question.Type;
            var html = $"<span class='badge bg-{type.GetContextualClass()}'>{type.GetDescription()}</span>";

            if (question.IsRequired)
                html += $" <span class='badge bg-dark'>Required</span>";

            if (question.IsNested)
                html += $" <span class='badge bg-dark'>Nested</span>";

            if (question.Attribute.HasValue())
                html += $" <span class='badge bg-success'>{question.Attribute}</span>";

            if (question.EnableCreateCase && question.Form.State.WorkflowConfiguration != null)
                html += $" <span class='badge bg-info'>Create Case</span>";

            return html;
        }

        #endregion
    }
}